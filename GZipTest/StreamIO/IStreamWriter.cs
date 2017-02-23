using GZipTest.Action;

namespace GZipTest.StreamIO
{
    public interface IStreamWriter
    {
        void WriteBlock(Block block);
    }
}
