using System.Collections;
using System.Linq.Expressions;

namespace MeterOrm.Core;

public abstract class ProtocolQueryProvider : IQueryProvider
{
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new ProtocolQueryable<TElement>(this, expression);
    }

    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments()[0];
        var queryableType = typeof(ProtocolQueryable<>).MakeGenericType(elementType);
        return (IQueryable)Activator.CreateInstance(queryableType, this, expression)!;
    }

    public object Execute(Expression expression)
    {
        return Execute<object>(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        var translator = CreateExpressionVisitor();
        string query = translator.Translate(expression);

        Console.WriteLine($"[DEBUG] Сформированный запрос: {query}");

        // Выполняем запрос
        var rawResult = ExecuteQuery(query);

        // Определяем, был ли вызван Select()
        if (expression is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
        {
            return (TResult)ApplySelectProjection(methodCall, rawResult);
        }

        // Если TResult - это коллекция, преобразуем
        if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = typeof(TResult).GetGenericArguments()[0];
            return (TResult)ConvertList(rawResult, elementType);
        }

        return (TResult)rawResult;
    }


    protected abstract ProtocolExpressionVisitor CreateExpressionVisitor();
    protected abstract object ExecuteQuery(string query);

    private object ConvertList(object rawData, Type targetType)
    {
        var listType = typeof(List<>).MakeGenericType(targetType);
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var item in (IEnumerable)rawData)
        {
            list.Add(Convert.ChangeType(item, targetType));
        }

        return list;
    }

    private object ApplySelectProjection(MethodCallExpression selectExpression, object rawData)
    {
        var lambda = (LambdaExpression)((UnaryExpression)selectExpression.Arguments[1]).Operand;
        var compiledLambda = lambda.Compile();

        var listType = typeof(List<>).MakeGenericType(lambda.ReturnType);
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var item in (IEnumerable)rawData)
        {
            var projectedItem = compiledLambda.DynamicInvoke(item);
            list.Add(projectedItem);
        }

        return list;
    }
}
