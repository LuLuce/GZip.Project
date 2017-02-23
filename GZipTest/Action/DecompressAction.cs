using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Action
{
    public class DecompressAction : IBlockHandlingAction
    {
        public void Act(Block block)
        {
            byte[] data = new byte[block.OrigBlockSize];

            using (MemoryStream stream = new MemoryStream(block.Data))
            {
                using (GZipStream compressStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    try
                    {
                        compressStream.Read(data, 0, block.OrigBlockSize);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            block.Data = data;
        }
    }
 }
