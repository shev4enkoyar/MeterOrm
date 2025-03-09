using System.Collections;
using System.Linq.Expressions;

namespace MeterOrm.Core;

public class ProtocolQueryable<T> : IQueryable<T>
{
    private readonly Expression _expression;
    private readonly IQueryProvider _provider;

    public ProtocolQueryable(IQueryProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = Expression.Constant(this);
    }

    public ProtocolQueryable(IQueryProvider provider, Expression expression)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public Type ElementType => typeof(T);
    public Expression Expression => _expression;
    public IQueryProvider Provider => _provider;

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
