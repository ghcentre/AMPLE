using Ample.Streams.Tests.TestInfrastructure;

namespace Ample.Streams.Tests;

public class BlockingStream_Tests
{
    [Fact]
    public void Read_LessThanCapacity_ReadsRequested()
    {
        var sut = new BlockingStream(100);
        var data = new byte[50];

        int read = sut.Read(data, 0, 50);

        read.ShouldBe(50);
    }

    [Fact]
    public void Read_OnceMoreThanCapacity_ReadsCapacity()
    {
        var sut = new BlockingStream(100);
        var data = new byte[150];

        int read = sut.Read(data, 0, 150);

        read.ShouldBe(100);
    }

    [Fact]
    public void Read_TwoTimes_ReadsCapacity()
    {
        var sut = new BlockingStream(100);
        var data = new byte[75];

        int read1 = sut.Read(data, 0, 75);
        int read2 = sut.Read(data, 0, 75);

        read1.ShouldBe(75);
        read2.ShouldBe(25);
    }


    [Fact]
    public void Read_TwoTimesMoreThanCapacity_TimesOut()
    {
        var sut = new BlockingStream(50);
        var data = new byte[150];
        var evt = new ManualResetEvent(false);
        int read = 0;

        Task.Run(
            () =>
            {
                read = sut.Read(data, 0, 150);
                read.ShouldBe(50);
                read = sut.Read(data, 0, 150);
                evt.Set();
            },
            TestContext.Current.CancellationToken);

        var returned = evt.WaitOne(TimeSpan.FromMilliseconds(200));

        returned.ShouldBeFalse();
    }
}
