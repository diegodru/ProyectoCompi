namespace Core
{
  public enum TokenType
  {
    Asterisk,
    Plus,
    Minus,
    LeftParens,
    RightParens,
    SemiColon,
    Equals,
    LessThan,
    LessThanOrEqual,
    Identifier, //<--- este si papa
    IntConstant,
    FloatConstant,
    Assignation,
    StringConstant,
    EOF,
    OpenBrace,
    CloseBrace,
    Comma,
    BasicType,
    IntKeyword,
    FloatKeyword,
    BoolKeyword,
    IfKeyword,
    ElseKeyword,
  }
}
