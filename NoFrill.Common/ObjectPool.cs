using System;
using System.Collections.Concurrent;

namespace NoFrill.Common
{
    public class ObjectPool<T> where T : IPoolable, new()
    {
        private ConcurrentStack<T> _objects;
        private Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentStack<T>();
            _objectGenerator = objectGenerator;
            // _objects.Push(new T());
        }

        public T Request()
        {
            T item;

            if (_objects.TryPop(out item))
                return item;

            item = _objectGenerator();
            // item.Pool = this;
            return item;
        }

        public void Return(T item)
        {
            item.Clear();

            _objects.Push(item);
        }
    }
}