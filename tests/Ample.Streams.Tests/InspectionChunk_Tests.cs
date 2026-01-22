namespace Ample.Streams.Tests;

public class InspectionChunk_Tests
{
    [Fact]
    public void Ctor_DataNull_Throws()
    {
        Should.Throw<ArgumentException>(() => new InspectionChunk("TestSession", Direction.ClientToServer, null!));
    }

    [Fact]
    public void Ctor_Data0_Throws()
    {
        Should.Throw<ArgumentException>(() => new InspectionChunk("TestSession", Direction.ClientToServer, []));
    }

    [Fact]
    public void SetLength_Negative_Throws()
    {
        var data = new byte[2048];
        var sut = new InspectionChunk("TestSession", Direction.ServerToClient, data);
        Should.Throw<ArgumentException>(() => sut.Length = -10);
    }

    [Fact]
    public void SetLength_TooLarge_Throws()
    {
        var data = new byte[2048];
        var sut = new InspectionChunk("TestSession", Direction.ServerToClient, data);
        Should.Throw<ArgumentException>(() => sut.Length = data.Length + 10);
    }

    [Fact]
    public void SetLength_Zero_Sets()
    {
        var data = new byte[2048];
        var sut = new InspectionChunk("TestSession", Direction.ServerToClient, data);
        sut.Length = 0;
        sut.Length.ShouldBe(0);
    }

    [Fact]
    public void SetLength_MaxLength_Sets()
    {
        var data = new byte[2048];
        var sut = new InspectionChunk("TestSession", Direction.ServerToClient, data);
        sut.Length = sut.Data.Length;
        sut.Length.ShouldBe(data.Length);
    }
}
