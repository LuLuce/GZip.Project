using System.IO;
using System.IO.Compression;

namespace GZipTest.Action
{
    public class CompressAction : IBlockHandlingAction
    {
        public void Act(Block block)
        {      
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream compressStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    compressStream.Write(block.Data, 0, block.OrigBlockSize);
                }
                block.Data = stream.ToArray();
            }
        }
    }
}
