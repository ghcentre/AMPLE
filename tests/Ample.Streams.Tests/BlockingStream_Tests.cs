using Ample.Streams.Tests.TestInfrastructure;

namespace Ample.Streams.Tests;

public class BlockingStream_Tests
{
    [Fact]
    public void Read_NoMore_Reads()
    {
        var stream = new BlockingStream(100);
        var data = new byte[50];

        int read = stream.Read(data, 0, 50);

        read.ShouldBe(50);
    }

    [Fact]
    public void Read_More_ReturnsCapacity()
    {
        var stream = new BlockingStream(100);
        var data = new byte[150];

        int read = stream.Read(data, 0, 150);

        read.ShouldBe(100);
    }

    [Fact]
    public void Read_MoreAgain_TimesOut()
    {
        var stream = new BlockingStream(100);
        var data = new byte[150];
        var evt = new ManualResetEvent(false);
        int read = 0;

        new Thread(
            () =>
            {
                read = stream.Read(data, 0, 150);
                read = stream.Read(data, 0, 150);
                evt.Set();
            })
            .Start();
        var returned = evt.WaitOne(TimeSpan.FromMilliseconds(500));

        returned.ShouldBeFalse();
    }
}
