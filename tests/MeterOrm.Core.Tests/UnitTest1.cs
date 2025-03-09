using MeterOrm.Dlms;
using Xunit.Abstractions;

namespace MeterOrm.Core.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        var context = new TestContext();
        var result = context.Measurements
            .Where(m => m.LogicalName == "1.1.1.1.1.1" && m.Value > 10)
            .ToList();

        foreach (var item in result)
        {
            _testOutputHelper.WriteLine($"Measurement: {item.LogicalName} = {item.Value}");
        }
        
        Assert.True(true);
    }
    
    [Fact]
    public void Test2()
    {
        var context = new TestContext();
        var result = context.Measurements
            .Where(m => m.LogicalName == "1.1.1.1.1.1" && m.Value > 10)
            .Select(m => new { m.Value, m.LogicalName }) // Выбираем только 2 поля
            .ToList();

        foreach (var item in result)
        {
            _testOutputHelper.WriteLine($"Measurement: {item.LogicalName} = {item.Value}");
        }


        Assert.True(true);
    }
}