namespace MeterOrm.Core;

public interface ITransport : IDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    Task<byte[]> SendRequestAsync(byte[] request, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}
