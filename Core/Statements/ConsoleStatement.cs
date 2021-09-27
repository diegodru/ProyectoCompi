using Core.Expressions;

namespace Core.Statements
{
  public class ConsoleStatement : Statement
  {
    public ConsoleStatement(TokenType tokentype, TypedExpression expression)
    {
      Accion = tokentype;
      Expression = expression;
    }

    public TokenType Accion { get; }
    public TypedExpression Expression { get; }

    public override string Generate()
    {
      return $"console.log({Expression.Generate()})";
    }

    public override void Interpret()
            {
                throw new System.NotImplementedException();
            }

    public override void ValidateSemantic()
            {
                
            }

  }
}
