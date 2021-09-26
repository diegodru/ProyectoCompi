namespace Core
{
  public enum TokenType
  {
    Asterisk,
    Plus,
    Minus,
    Division,
    Modulus,
    LeftParens,
    RightParens,
    SemiColon,
    Equal,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    NotEqual,
    Identifier, //<--- este si papa
    IntConstant,
    FloatConstant,
    StringConstant,
    BoolConstant,
    Assignation,
    Increment,
    Decrement,
    EOF,
    OpenBrace,
    CloseBrace,
    Comma,
    BasicType,
    IntKeyword,
    FloatKeyword,
    BoolKeyword,
    StringKeyword,
    IfKeyword,
    ElseKeyword,
    ForKeyword,
  }
}
