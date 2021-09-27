namespace Core.Expressions
{
  public class Id : TypedExpression
  {
    public Id(Token token, Type type) : base(token, type)
    {
      IsAttribute = false;
    }

    public Id(Token token, Type type, bool isAttribute) : base(token, type)
    {
      IsAttribute = isAttribute;
      nombreVariable = "this";
    }

    public Id(Token token, Type type, string nombre) : base(token, type)
    {
      IsAttribute = true;
      nombreVariable = nombre;
    }

    public bool IsAttribute { get; }
    public string nombreVariable { get; }

    public override dynamic Evaluate()
    {
      return EnvironmentManager.GetSymbolForEvaluation(Token.Lexeme).Value;
    }
    
    public override string Generate()
    {
      if(IsAttribute)
        return $"{nombreVariable}.{Token.Lexeme}";
      return Token.Lexeme;
    }

    public override Type GetExpressionType()
    {
      return type;
    }
  }
}
