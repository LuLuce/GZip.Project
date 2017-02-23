using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GZipTest.Action;
using System.IO;

namespace GZipTest.StreamIO
{
    public class DefaultStreamReader : IStreamReader
    {
        private readonly Stream stream;

        private readonly int blockSize;

        private int counter;

        public DefaultStreamReader(Stream stream, int blockSize)
        {
            this.stream = stream;
            this.blockSize = blockSize;
        }

        public byte[] GetNextBlockBytes()
        {
            byte[] buffer = new byte[blockSize];
            int bytesReaded = this.stream.Read(buffer, 0, blockSize);

            if (bytesReaded == 0) return null;

            if (bytesReaded == this.blockSize) return buffer;

            byte[] result = new byte[bytesReaded];
            Array.Copy(buffer, result, result.Length);

            return result;
        }

        public Block GetNextBlock()
        {
            var buffer = this.GetNextBlockBytes();
            Block block = new Block
            {
                Id = this.counter++,
                OrigBlockSize = buffer.Length,
                Data = buffer
            };
            return block;
        }
    }
}
