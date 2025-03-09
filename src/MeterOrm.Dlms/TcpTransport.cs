using System.Net.Sockets;
using MeterOrm.Core;

namespace MeterOrm.Dlms;

public class TcpTransport : ITransport
{
    private readonly string _host;
    private readonly int _port;
    private TcpClient? _client;
    private NetworkStream? _stream;

    public bool IsConnected => _client?.Connected ?? false;

    public TcpTransport(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port, cancellationToken);
        _stream = _client.GetStream();
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _stream?.Close();
        _client?.Close();
        _stream = null;
        _client = null;
    }

    public async Task<byte[]> SendRequestAsync(byte[] request, CancellationToken cancellationToken = default)
    {
        if (_stream == null || !IsConnected)
            throw new InvalidOperationException("Соединение не установлено");

        await _stream.WriteAsync(request, cancellationToken);
        await _stream.FlushAsync(cancellationToken);

        var buffer = new byte[1024];
        int bytesRead = await _stream.ReadAsync(buffer, cancellationToken);
        return buffer.Take(bytesRead).ToArray();
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }
}
