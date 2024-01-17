using System.Collections.Concurrent;
using System;
using System.Collections.Generic;

namespace NoFrill.Common
{
    public class ObjectPool
    {
        private static void ThrowPoolTypeNotDefined()
        {
            throw new ArgumentException("Pool object type has not been defined, Please use AddType<T>() to define all pooled types.");
        }

        private static void ThrowNullRef()
        {
            throw new NullReferenceException("Pool object type has not been defined, Please use AddType<T>() to define all pooled types.");
        }

        protected Dictionary<Type, Stack<IPoolable>> pool = new Dictionary<Type, Stack<IPoolable>>();

        public void AddType<T>() where T : class, IPoolable, new()
        {
            pool[typeof(T)] = new Stack<IPoolable>();
        }

        public T Rent<T>() where T : class, IPoolable, new()
        {
            if (pool.TryGetValue(typeof(T), out var typePool))
            {
                if (typePool.Count > 0)
                {
                    T typeFinal = typePool.Pop() as T;
                    return typeFinal;
                }
            }
            else
            {
                ThrowPoolTypeNotDefined();
            }
            return new T();
        }

        public void Return<T>(T obj) where T : class, IPoolable, new()
        {
            if (obj == null) ThrowNullRef();

            if (pool.TryGetValue(obj.GetType(), out var typePool))
            {
                obj.Clear();
                typePool.Push(obj);
            }
            else
            {
                ThrowPoolTypeNotDefined();
            }
        }
    }
}