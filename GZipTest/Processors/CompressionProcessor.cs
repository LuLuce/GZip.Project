using GZipTest.Action;
using GZipTest.StreamIO;
using System;
using System.IO;

namespace GZipTest.Processors
{
    internal class CompressionProcessor: BaseProcessor
    {
        public CompressionProcessor(Stream inputStream, Stream outputStream, int blockSize)
            :base(inputStream, outputStream, OperationType.Compress, blockSize) { }

        protected override bool Init()
        {
            uint blockCount = (uint)Math.Ceiling(this.inputStream.Length / (this.blockSize + 0.0));

            this.streamReader = new DefaultStreamReader(this.inputStream, this.blockSize);

            Header header = new Header
            {
                blockSize = this.blockSize,
                blockCount = blockCount

            };

            this.streamWriter = new BlockStreamWriter(this.outputStream);
            (this.streamWriter as BlockStreamWriter).WriteHeader(header);
            return true;
        }
    }
}
