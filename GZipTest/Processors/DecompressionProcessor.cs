using GZipTest.Action;
using GZipTest.StreamIO;
using System;
using System.IO;

namespace GZipTest.Processors
{
    internal class DecompressionProcessor : BaseProcessor
    {
        public DecompressionProcessor(Stream inputStream, Stream outputStream)
            : base(inputStream, outputStream, OperationType.Decompress)
        { }

        protected override bool Init()
        {
            this.streamReader = new BlockStreamReader(this.inputStream);

            Header header;
            try
            {
                header = ((BlockStreamReader)this.streamReader).GetHeader();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            this.blockSize = header.blockSize;

            this.streamWriter = new DefaultStreamWriter(this.outputStream, this.blockSize);
            return true;
        }
    }
}
