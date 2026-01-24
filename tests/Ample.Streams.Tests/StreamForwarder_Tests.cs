using Ample.Streams.Tests.TestInfrastructure;

namespace Ample.Streams.Tests;

public class StreamForwarder_Tests
{
    private static byte[] AllocateAndFillBuffer(int size)
    {
        var buffer = new byte[size];
        for (int i = 0; i < size; i++)
        {
            buffer[i] = (byte)(i % 256);
        }
        return buffer;
    }

    [Fact]
    public async Task Ctor_BufferZero_Throws()
    {
        var sut = new StreamForwarder();
        await Should.ThrowAsync<ArgumentException>(
            () => sut.ForwardBidirectionalAsync(
                "TestSession",
                new MemoryStream(),
                new MemoryStream(),
                [],
                new byte[2048],
                Inspector.Default,
                CancellationToken.None));
        await Should.ThrowAsync<ArgumentException>(
            () => sut.ForwardBidirectionalAsync(
                "TestSession",
                new MemoryStream(),
                new MemoryStream(),
                new byte[2048],
                [],
                Inspector.Default,
                CancellationToken.None));
    }

    [Fact]
    public async Task Forward_TokenCancelled_ReturnsImmediately()
    {
        var sut = new StreamForwarder();
        var client = new MemoryStream(AllocateAndFillBuffer(2048));
        var server = new MemoryStream(AllocateAndFillBuffer(2048));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            client,
            server,
            new byte[2048],
            new byte[2048],
            Inspector.Default,
            cts.Token);

        client.Position.ShouldBe(0);
        server.Position.ShouldBe(0);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(100)]
    [InlineData(16384)]
    [InlineData(32768)]
    [InlineData(135169)]
    public async Task Forward_NormalFlow_CopiesClientToServer(int bufferSize)
    {
        var sut = new StreamForwarder();
        var clientBuffer = AllocateAndFillBuffer(bufferSize);
        var client = new MemoryStream(clientBuffer);
        var server = new MemoryStream();
        var cts = new CancellationTokenSource();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            client,
            server,
            new byte[16384],
            new byte[16384],
            Inspector.Default,
            cts.Token);
        
        var serverBuffer = server.ToArray();

        sut.ClientToServerBytesTransferred.ShouldBe(bufferSize);
        serverBuffer.ShouldBe(clientBuffer);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(100)]
    [InlineData(16384)]
    [InlineData(32768)]
    [InlineData(135169)]
    public async Task Forward_NormalFlow_CopiesServerToClient(int bufferSize)
    {
        var sut = new StreamForwarder();
        var serverBuffer = AllocateAndFillBuffer(bufferSize);
        var client = new MemoryStream();
        var server = new MemoryStream(serverBuffer);
        var cts = new CancellationTokenSource();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            client,
            server,
            new byte[16384],
            new byte[16384],
            Inspector.Default,
            cts.Token);
        
        var clientBuffer = client.ToArray();

        sut.ServerToClientBytesTransferred.ShouldBe(bufferSize);
        clientBuffer.ShouldBe(serverBuffer);
    }

    [Theory]
    [InlineData(32 * 1024, 123)]
    [InlineData(112, 64 * 1024)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "<Pending>")]
    public async Task Forward_TokenCancelled_Returns(int serverSize, int clientSize)
    {
        // arrange
        var sessionId = "TestSession";
        var clientStream = new BlockingStream(clientSize);
        var serverStream = new BlockingStream(serverSize);
        var clientBuffer = new byte[16384];
        var serverBuffer = new byte[16384];
        var cts = new CancellationTokenSource(500);
        var sut = new StreamForwarder();

        var task = sut.ForwardBidirectionalAsync(sessionId, clientStream, serverStream, clientBuffer, serverBuffer, Inspector.Default, cts.Token);
        await Task.Delay(1000);

        task.GetAwaiter().GetResult();

        task.IsCompleted.ShouldBeTrue();
        sut.ClientToServerBytesTransferred.ShouldBe(clientSize);
        sut.ServerToClientBytesTransferred.ShouldBe(serverSize);
    }

    [Theory]
    [InlineData(1234)]
    [InlineData(12340)]
    public async Task Inspector_ClientHasData_CalledInspectClientToServer(int buffersize)
    {
        var clientBuffer = AllocateAndFillBuffer(buffersize);
        var clientStream = new MemoryStream(clientBuffer);
        var serverStream = new MemoryStream();
        long collectedInInspector = 0;
        var inspector = new ActionInspector(
            async (chunk) =>
            {
                chunk.Direction.ShouldBe(Direction.ClientToServer);
                chunk.Data.Length.ShouldBe(2048);
                chunk.Length.ShouldBeLessThanOrEqualTo(2048);
                collectedInInspector += chunk.Length;
                return InspectionResult.Send;
            });
        var sut = new StreamForwarder();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            clientStream,
            serverStream,
            new byte[2048],
            new byte[2048],
            inspector,
            CancellationToken.None);

        sut.ClientToServerBytesTransferred.ShouldBe(buffersize);
        collectedInInspector.ShouldBe(buffersize);
    }

    [Theory]
    [InlineData(1234)]
    [InlineData(12340)]
    public async Task Inspector_ServerHasData_CalledInspectServerToClient(int buffersize)
    {
        var serverBuffer = AllocateAndFillBuffer(buffersize);
        var serverStream = new MemoryStream(serverBuffer);
        var clientStream = new MemoryStream();
        long collectedInInspector = 0;
        var inspector = new ActionInspector(
            async (chunk) =>
            {
                chunk.Direction.ShouldBe(Direction.ServerToClient);
                chunk.Data.Length.ShouldBe(2048);
                chunk.Length.ShouldBeLessThanOrEqualTo(2048);
                collectedInInspector += chunk.Length;
                return InspectionResult.Send;
            });
        var sut = new StreamForwarder();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            clientStream,
            serverStream,
            new byte[2048],
            new byte[2048],
            inspector,
            CancellationToken.None);

        sut.ServerToClientBytesTransferred.ShouldBe(buffersize);
        collectedInInspector.ShouldBe(buffersize);
    }
}
