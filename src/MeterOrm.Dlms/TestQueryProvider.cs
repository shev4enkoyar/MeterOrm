using MeterOrm.Core;

namespace MeterOrm.Dlms;

public class TestQueryProvider : ProtocolQueryProvider
{
    protected override ProtocolExpressionVisitor CreateExpressionVisitor()
    {
        return new SimpleExpressionVisitor();
    }

    protected override object ExecuteQuery(string query)
    {
        Console.WriteLine($"[TEST] Выполняем запрос: {query}");
        return new List<Measurement> { new() { LogicalName = "1.1.1.1.1.1", Value = 220.5 } };
    }

    private class SimpleExpressionVisitor : ProtocolExpressionVisitor { }
}
