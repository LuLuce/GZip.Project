using GZipTest.Action;

namespace GZipTest.StreamIO
{
    public interface IStreamReader
    {
        Block GetNextBlock();
    }
}
