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
    public async Task Forward_TokenCancelled_Throws()
    {
        var sut = new StreamForwarder();
        var client = new MemoryStream(AllocateAndFillBuffer(2048));
        var server = new MemoryStream(AllocateAndFillBuffer(2048));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => sut.ForwardBidirectionalAsync(
                "TestSession",
                client,
                server,
                new byte[2048],
                new byte[2048],
                Inspector.Default,
                cts.Token));

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
    [InlineData(123457)]
    public async Task Forward_ServerStreamEnds_CopiesClientToServerNoMoreThanBuffer(int bufferSize)
    {
        var sut = new StreamForwarder();
        var clientBuffer = AllocateAndFillBuffer(bufferSize);
        var client = new MemoryStream(clientBuffer);
        var server = new MemoryStream();

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            client,
            server,
            new byte[16384],
            new byte[16384],
            Inspector.Default,
            TestContext.Current.CancellationToken);

        var serverBuffer = server.ToArray();

        sut.ClientToServerBytesTransferred.ShouldBe(Math.Min(bufferSize, 16384));
        serverBuffer.ShouldBe(clientBuffer.Take(16384));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(100)]
    [InlineData(16384)]
    [InlineData(32768)]
    [InlineData(135169)]
    [InlineData(123457)]
    public async Task Forward_ClientStreamEnds_CopiesServerToClientNoMoreThanBuffer(int bufferSize)
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
            TestContext.Current.CancellationToken);

        var clientBuffer = client.ToArray();

        sut.ServerToClientBytesTransferred.ShouldBe(Math.Min(bufferSize, 16384));
        clientBuffer.ShouldBe(serverBuffer.Take(16384));
    }

    [Theory]
    [InlineData(10, 12)]
    [InlineData(20, 17)]
    [InlineData(100, 200)]
    [InlineData(200, 100)]
    [InlineData(16384, 16384)]
    [InlineData(16384, 16100)]
    [InlineData(16100, 16384)]
    [InlineData(16384, 32768)]
    [InlineData(32768, 16384)]
    [InlineData(135169, 130000)]
    [InlineData(123457, 800121)]
    public async Task Forward_ClientAndServerHasData_CopiesLessThanBuffer(int clientBufferSize, int serverBufferSize)
    {
        var sut = new StreamForwarder();
        var client = new NetworkStreamMock(clientBufferSize, reportEndOfStream: true);
        var server = new NetworkStreamMock(serverBufferSize, reportEndOfStream: true);

        await sut.ForwardBidirectionalAsync(
            "TestSession",
            client,
            server,
            new byte[16384],
            new byte[16384],
            Inspector.Default,
            TestContext.Current.CancellationToken);

        sut.ClientToServerBytesTransferred.ShouldBeLessThanOrEqualTo(clientBufferSize);
        sut.ServerToClientBytesTransferred.ShouldBeLessThanOrEqualTo(serverBufferSize);
    }

    [Theory]
    [InlineData(32 * 1024, 123)]
    [InlineData(112, 64 * 1024)]
    [InlineData(112 * 1024 + 19, 64 * 1024 + 21)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "<Pending>")]
    public async Task Forward_TokenCanceled_Returns(int serverSize, int clientSize)
    {
        // arrange
        var sessionId = "TestSession";
        var clientStream = new NetworkStreamMock(clientSize);
        var serverStream = new NetworkStreamMock(serverSize);
        var clientBuffer = new byte[16384];
        var serverBuffer = new byte[16384];
        var cts = new CancellationTokenSource(800);
        var sut = new StreamForwarder();

        var task = sut.ForwardBidirectionalAsync(sessionId, clientStream, serverStream, clientBuffer, serverBuffer, Inspector.Default, cts.Token);

        task.GetAwaiter().GetResult();

        task.IsCompleted.ShouldBeTrue();
        sut.ClientToServerBytesTransferred.ShouldBeLessThanOrEqualTo(clientSize);
        sut.ServerToClientBytesTransferred.ShouldBeLessThanOrEqualTo(serverSize);
    }

    [Theory]
    [InlineData(1234)]
    [InlineData(12340)]
    public async Task Inspector_ClientHasData_CalledInspectClientToServer(int buffersize)
    {
        var clientBuffer = AllocateAndFillBuffer(buffersize);
        var clientStream = new MemoryStream(clientBuffer);
        var serverStream = new NetworkStreamMock(0);
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
        var clientStream = new NetworkStreamMock(0);
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
