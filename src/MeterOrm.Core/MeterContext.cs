namespace MeterOrm.Core;

public abstract class MeterContext : IDisposable
{
    protected readonly ITransport? Transport;

    protected MeterContext(ITransport transport)
    {
        Transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    // Метод обработки push-уведомлений (можно переопределить в наследниках)
    public virtual void HandlePushNotification(byte[] data)
    {
        // Базовая обработка (например, логирование)
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Transport?.Dispose();
        }
    }
}