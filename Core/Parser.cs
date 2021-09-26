using Core;
using Core.Expressions;
using Core.Interfaces;
using Core.Statements;
using System;
using Type = Core.Type;

namespace Parser
{
  public class Parser : IParser
  {
    private readonly IScanner scanner;
    private Token lookAhead;

    public Parser(IScanner scanner)
    {
      this.scanner = scanner;
      this.Move();
    }

    public string Parse()
    {
      return Program();
    }

    private string Program()
    {
      var block = Block();
      block.ValidateSemantic();
      var code = block.Generate();
      return code;
    }

    private Statement Block()
    {
      EnvironmentManager.PushContext();
      Decls();
      var statements = Stmts();
      EnvironmentManager.PopContext();
      return statements;
    }

    private Statement Stmts()
    {
      if (this.lookAhead.TokenType == TokenType.CloseBrace || this.lookAhead.TokenType == TokenType.EOF)
      {//{}
        return null;
      }
      return new SequenceStatement(Stmt(), Stmts());
    }

    private Statement Stmt()
    {
      Expression expression;
      Statement statement1, statement2;
      switch (this.lookAhead.TokenType)
      {
        case TokenType.Identifier:
          {
            var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
            Match(TokenType.Identifier);
            if (this.lookAhead.TokenType == TokenType.Assignation)
            {
              return AssignStmt(symbol.Id);
            }
            throw new ApplicationException("No se han implementado los metodos");
          }
        case TokenType.IfKeyword:
          {
            Match(TokenType.IfKeyword);
            Match(TokenType.LeftParens);
            expression = Eq();
            Match(TokenType.RightParens);
            statement1 = Stmt();
            if (this.lookAhead.TokenType != TokenType.ElseKeyword)
            {
              return new IfStatement(expression as TypedExpression, statement1);
            }
            Match(TokenType.ElseKeyword);
            statement2 = Stmt();
            return new ElseStatement(expression as TypedExpression, statement1, statement2);
          }
        default:
          return Block();
      }
    }

    private Expression Eq()
    {
      var expression = Rel();
      while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
      {
        var token = lookAhead;
        Move();
        expression = new RelationalExpression(token, expression as TypedExpression, Rel() as TypedExpression);
      }

      return expression;
    }

    private Expression Rel()
    {
      var expression = Expr();
      if (this.lookAhead.TokenType == TokenType.LessThan
          || this.lookAhead.TokenType == TokenType.GreaterThan
          || this.lookAhead.TokenType == TokenType.LessThanOrEqual
          || this.lookAhead.TokenType == TokenType.GreaterThanOrEqual)
      {
        var token = lookAhead;
        Move();
        expression = new RelationalExpression(token, expression as TypedExpression, Expr() as TypedExpression);
      }
      return expression;
    }

    private Expression Expr()
    {
      var expression = Term();
      while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
      {
        var token = lookAhead;
        Move();
        expression = new ArithmeticOperator(token, expression as TypedExpression, Term() as TypedExpression);
      }
      return expression;
    }

    private Expression Term()
    {
      var expression = Factor();
      while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Division)
      {
        var token = lookAhead;
        Move();
        expression = new ArithmeticOperator(token, expression as TypedExpression, Factor() as TypedExpression);
      }
      return expression;
    }

    private Expression Factor()
    {
      switch (this.lookAhead.TokenType)
      {
        case TokenType.LeftParens:
          {
            Match(TokenType.LeftParens);
            var expression = Eq();
            Match(TokenType.RightParens);
            return expression;
          }
        case TokenType.IntConstant:
          var constant = new Constant(lookAhead, Type.Int);
          Match(TokenType.IntConstant);
          return constant;
        case TokenType.FloatConstant:
          constant = new Constant(lookAhead, Type.Float);
          Match(TokenType.FloatConstant);
          return constant;
        case TokenType.StringConstant:
          constant = new Constant(lookAhead, Type.String);
          Match(TokenType.StringConstant);
          return constant;
        //case TokenType.BoolConstant:
          //constant = new Constant(lookAhead, Type.Bool);
          //Match(TokenType.BoolConstant); //<------------ Implmentar //TODO
          //return constant;
          //case NewKeyword
        default:
          var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
          Match(TokenType.Identifier);
          return symbol.Id;
      }
    }

    //private Statement CallStmt(Symbol symbol)
    //{
      //Match(TokenType.LeftParens);
      //var @params = OptParams();
      //Match(TokenType.RightParens);
      //Match(TokenType.SemiColon);
      //return new CallStatement(symbol.Id, @params, symbol.Attributes);
    //}

    private Expression OptParams()
    {
      if (this.lookAhead.TokenType != TokenType.RightParens)
      {
        return Params();
      }
      return null;
    }

    private Expression Params()
    {
      var expression = Eq();
      if (this.lookAhead.TokenType != TokenType.Comma)
      {
        return expression;
      }
      Match(TokenType.Comma);
      expression = new ArgumentExpression(lookAhead, expression as TypedExpression, Params() as TypedExpression);
      return expression;
    }

    private Statement AssignStmt(Id id)
    {
      Match(TokenType.Assignation);
      var expression = Eq();
      Match(TokenType.SemiColon); 
      return new AssignationStatement(id, expression as TypedExpression);
    }

    private void Decls()
    {
      if (this.lookAhead.TokenType == TokenType.IntKeyword ||
          this.lookAhead.TokenType == TokenType.FloatKeyword ||
          this.lookAhead.TokenType == TokenType.BoolKeyword ||
          this.lookAhead.TokenType == TokenType.StringKeyword)
        //<------------ Class
      {
        Decl();
        Decls();
      }
    }

    private void Decl()
    {
      TokenType tokenType = this.lookAhead.TokenType;
      Match(tokenType);
      Token token = lookAhead;
      Match(TokenType.Identifier);
      Match(TokenType.SemiColon);
      Id id;
      switch (tokenType)
      {
        case TokenType.FloatKeyword:
          id = new Id(token, Type.Float);;
          break;
        case TokenType.StringKeyword:
          id = new Id(token, Type.String);
          break;
        case TokenType.BoolKeyword:
          id = new Id(token, Type.Bool);
          break;
        default:
          id = new Id(token, Type.Int);;
          break;
      }
      EnvironmentManager.AddVariable(token.Lexeme, id);
    }

    private void Move()
    {
      this.lookAhead = this.scanner.GetNextToken();
    }

    private void Match(TokenType tokenType)
    {
      if (this.lookAhead.TokenType != tokenType)
      {
        throw new ApplicationException($"Syntax error! expected token {tokenType} but found {this.lookAhead.TokenType}. Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}");
      }
      this.Move();
    }
  }
}
