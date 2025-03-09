using MeterOrm.Core;
using System.IO.Ports;

namespace MeterOrm.Dlms;

public class SerialTransport : ITransport
{
    private readonly string _portName;
    private readonly int _baudRate;
    private readonly Parity _parity;
    private readonly int _dataBits;
    private readonly StopBits _stopBits;
    private SerialPort? _serialPort;

    public bool IsConnected => _serialPort?.IsOpen ?? false;

    public SerialTransport(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        _portName = portName;
        _baudRate = baudRate;
        _parity = parity;
        _dataBits = dataBits;
        _stopBits = stopBits;
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _serialPort = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits)
        {
            ReadTimeout = 5000,
            WriteTimeout = 5000
        };

        _serialPort.Open();
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _serialPort?.Close();
        _serialPort = null;
        return Task.CompletedTask;
    }

    public async Task<byte[]> SendRequestAsync(byte[] request, CancellationToken cancellationToken = default)
    {
        if (_serialPort == null || !_serialPort.IsOpen)
            throw new InvalidOperationException("Соединение не установлено");

        _serialPort.Write(request, 0, request.Length);

        // Читаем ответ
        var buffer = new List<byte>();
        var readBuffer = new byte[1024];

        try
        {
            int bytesRead;
            do
            {
                bytesRead = await Task.Run(() => _serialPort.Read(readBuffer, 0, readBuffer.Length), cancellationToken);
                buffer.AddRange(readBuffer.Take(bytesRead));
            }
            while (bytesRead > 0);
        }
        catch (TimeoutException)
        {
            // Таймаут чтения, возвращаем то, что успели прочитать
        }

        return buffer.ToArray();
    }

    public void Dispose()
    {
        _serialPort?.Dispose();
    }
}
