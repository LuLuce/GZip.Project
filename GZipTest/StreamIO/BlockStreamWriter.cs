﻿using System;
using GZipTest.Action;
using System.IO;

namespace GZipTest.StreamIO
{
    public class BlockStreamWriter : IStreamWriter
    {
        private readonly Stream stream;

        public BlockStreamWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void WriteHeader(Header header)
        {
            var buffer = this.ToByteArray(header);
            this.stream.Write(buffer, 0, buffer.Length);
        }

        public void WriteBlock(Block block)
        {
            var buffer = this.ToByteArray(block);
            this.stream.Write(buffer, 0, buffer.Length);
        }

        private byte[] ToByteArray(Header header)
        {
            byte[] blockSizeBytes = BitConverter.GetBytes(header.blockSize);
            byte[] blocksCountBytes = BitConverter.GetBytes(header.blockCount);

            byte[] result = new byte[8];
            Array.Copy(blockSizeBytes, result, 4);
            Array.Copy(blocksCountBytes, 0, result, 4, 4);

            return result;
        }

        private byte[] ToByteArray(Block block)
        {
            byte[] idBytes = BitConverter.GetBytes(block.Id);
            byte[] originSize = BitConverter.GetBytes(block.OrigBlockSize);

            int blockSize = idBytes.Length + originSize.Length 
                + block.Data.Length;
            byte[] blockSizeBytes = BitConverter.GetBytes(blockSize);
            byte[] result = new byte[blockSize + 4];

            Array.Copy(blockSizeBytes, result, blockSizeBytes.Length);
            Array.Copy(idBytes, 0, result, blockSizeBytes.Length, idBytes.Length);
            Array.Copy(originSize, 0, result, blockSizeBytes.Length + idBytes.Length, originSize.Length);
            Array.Copy(block.Data, 0, result, blockSizeBytes.Length + idBytes.Length + originSize.Length, block.Data.Length);

            return result;           
        }
    }
}
