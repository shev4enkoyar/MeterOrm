using MeterOrm.Core;

namespace MeterOrm.Dlms;

public class DlmsContext : MeterContext
{
    public DlmsContext(ITransport transport) : base(transport) { }

    public async Task<byte[]> SendRawRequestAsync(byte[] request, CancellationToken cancellationToken = default)
    {
        return await Transport?.SendRequestAsync(request, cancellationToken)!;
    }
}
