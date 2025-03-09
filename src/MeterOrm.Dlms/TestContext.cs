using MeterOrm.Core;

namespace MeterOrm.Dlms;

public class TestContext
{
    private readonly TestQueryProvider _provider;

    public IQueryable<Measurement> Measurements { get; }

    public TestContext()
    {
        _provider = new TestQueryProvider();
        Measurements = new ProtocolQueryable<Measurement>(_provider);
    }
}


public class Measurement
{
    public string LogicalName { get; set; }
    public double Value { get; set; }
}
