using System;
namespace NoFrillSMF.Chunks
{
    internal static class ChunkMappings
    {
        public static IChunk ChunkFactory(string typeString)
        {
            switch (typeString)
            {
                case "MThd":
                    return new HeaderChunk();
                case "MTrk":
                    return new TrackChunk();
                default:
                    return null;
            }
        }
    }
}
