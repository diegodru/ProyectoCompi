namespace Core.Expressions
{
  public abstract class Expression
  {
    protected readonly Type type;

    public Token Token { get; }
    
    public Expression(Token token, Type type)
    {
      Token = token;
      this.type = type;
    }

    public abstract string Generate();
  }
}
