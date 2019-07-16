using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace NoFrillSMF.Tests
{
    public class SmfReadWriteTests
    {
        private byte[] GetHash(byte[] data)
        {
            using (SHA1 sha = SHA1.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        private byte[] GetHash(Stream data)
        {
            using (SHA1 sha = SHA1.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        [Theory]
        [InlineData("midifiles/C3_Sound.mid")]
        //[InlineData("midifiles/notes.mid")]
        public void ReadWriteVerifyTest(string midiFile)
        {
            // Arrange
            byte[] data = File.ReadAllBytes(midiFile);
            MidiFile file = new MidiFile();

            byte[] shaHash = GetHash(data);
            byte[] outHash;

            // Act
            using (MemoryStream msIn = new MemoryStream(data))
            using (MemoryStream msOut = new MemoryStream())
            {

                file.ReadData(msIn);
                file.WriteData(msOut);

                msOut.Position = 0;
                outHash = GetHash(msOut);
            }
            // Assert
            Assert.True(shaHash.SequenceEqual(outHash));

        }
    }
}
