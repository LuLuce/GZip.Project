using System;
using System.IO;
using GZipTest.Action;

namespace GZipTest.StreamIO
{
    public class BlockStreamReader : IStreamReader
    {
        private readonly Stream stream;

        public BlockStreamReader(Stream stream)
        {
            this.stream = stream;
        }

        public Header GetHeader()
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            int blockSize = BitConverter.ToInt32(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            uint blocksCount = BitConverter.ToUInt32(bytes, 0);

            int hashType = stream.ReadByte();

            stream.Read(bytes, 0, bytes.Length);

            return new Header
            {
                blockSize = blockSize,
                blockCount = blocksCount,
            };
        }

        public Block GetNextBlock()
        {
            byte[] bytes = new byte[4];

            stream.Read(bytes, 0, bytes.Length);
            int blockSize = BitConverter.ToInt32(bytes, 0);

            byte[] buffer = new byte[blockSize];
            stream.Read(buffer, 0, blockSize);

            int blockId = BitConverter.ToInt32(buffer, 0);
            int originBlockSize = BitConverter.ToInt32(buffer, 4);

            byte[] data = new byte[blockSize - 4];
            Array.Copy(buffer, 4, data, 0, data.Length);

            Block block = new Block
            {
                Id = blockId,
                OrigBlockSize = originBlockSize,
                Data = data
            };
            return block;
        }
    }
}
