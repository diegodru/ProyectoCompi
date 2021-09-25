namespace Core.Expressions
{
  public abstract class TypedExpression : Expression
  {
    public TypedExpression(Token token, Type type) : base (token, type) { }
    
    public abstract dynamic Evaluate();

    public abstract Type GetExpressionType();
  }
}
