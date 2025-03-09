using System.Linq.Expressions;
using System.Text;

namespace MeterOrm.Core;

public abstract class ProtocolExpressionVisitor : ExpressionVisitor
{
    protected readonly StringBuilder _queryBuilder = new();
    private readonly List<string> _selectedFields = new();

    public string Translate(Expression expression)
    {
        _queryBuilder.Clear();
        _selectedFields.Clear();
        Visit(expression);

        // Если нет Select(), выбираем всё (*)
        string selectPart = _selectedFields.Count > 0
            ? string.Join(", ", _selectedFields)
            : "*";

        string wherePart = _queryBuilder.Length > 0 ? $" WHERE {_queryBuilder}" : "";

        return $"SELECT {selectPart}{wherePart}";
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == "Select" && node.Arguments.Count == 2)
        {
            var lambda = (LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand;
            Visit(lambda.Body); // Посещаем выражение `Select`
            Visit(node.Arguments[0]); // Посещаем базовое выражение (чтобы обработать `Where`)
            return node;
        }

        if (node.Method.Name == "Where" && node.Arguments.Count == 2)
        {
            var lambda = (LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand;
            Visit(lambda.Body); // Посещаем выражение условия `Where`
            return node;
        }

        return base.VisitMethodCall(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (_queryBuilder.Length > 0)  // Если работаем с `Where`
        {
            _queryBuilder.Append(node.Member.Name);
        }
        else  // Если работаем с `Select`
        {
            _selectedFields.Add(node.Member.Name);
        }

        return base.VisitMember(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        bool needBrackets = node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse;

        if (needBrackets) _queryBuilder.Append("(");
        Visit(node.Left);
        _queryBuilder.Append($" {GetOperator(node.NodeType)} ");
        Visit(node.Right);
        if (needBrackets) _queryBuilder.Append(")");

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        _queryBuilder.Append(node.Value);
        return base.VisitConstant(node);
    }

    private string GetOperator(ExpressionType type) =>
        type switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThan => "<",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new NotSupportedException($"Оператор {type} не поддерживается")
        };
}
