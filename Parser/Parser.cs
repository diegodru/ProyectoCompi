using Core;
using Core.Expressions;
using Core.Interfaces;
using Core.Statements;
using System;
using Type = Core.Type;

namespace test
{
  public class ElParser : IParser
  {
    private readonly IScanner scanner;
    private Token lookAhead;

    public ElParser(IScanner scanner)
    {
      this.scanner = scanner;
      this.Move();
    }

    public string Parse()
    {
      return Program().Generate();
    }

    private Statement Program()
    {
      var block = Block();
      try{
        block.ValidateSemantic();
      }catch(Exception e)
      {
        Console.Error.WriteLine(e.Message);
      }
      return block;
    }

    private Statement Block()
    {
      EnvironmentManager.PushContext();
      var stmts = new SequenceStatement(Decls(), Stmts());
      EnvironmentManager.PopContext();
      return stmts;
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
      try{
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
                var assStmt = AssignStmt(symbol.Id);
                Match(TokenType.SemiColon);
                return assStmt;
              }
              else if (this.lookAhead.TokenType == TokenType.Increment || this.lookAhead.TokenType == TokenType.Decrement)
              {
                var type = this.lookAhead.TokenType;
                Move();
                var incdec = new IncDecStatement(symbol.Id, type);
                Match(TokenType.SemiColon);
                return incdec;
              }
              throw new ApplicationException("No se han implementado los metodos");
            }
          case TokenType.IfKeyword:
            {
              Match(TokenType.IfKeyword);
              Match(TokenType.LeftParens);
              expression = Eq();
              Match(TokenType.RightParens);
              if(this.lookAhead.TokenType == TokenType.OpenBrace){
                Match(TokenType.OpenBrace);
                statement1 = Block();
                Match(TokenType.CloseBrace);
              }
              else
                statement1 = Stmt();
              if (this.lookAhead.TokenType != TokenType.ElseKeyword)
              {
                return new IfStatement(expression as TypedExpression, statement1);
              }
              Match(TokenType.ElseKeyword);
              statement2 = Stmt();
              return new ElseStatement(expression as TypedExpression, statement1, statement2);
            }
          case TokenType.ForKeyword:
            {
              Match(TokenType.ForKeyword);
              Match(TokenType.LeftParens);
              var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
              Match(TokenType.Identifier);
              var firstAssignation = AssignStmt(symbol.Id);
              Match(TokenType.SemiColon);
              var check = Eq();
              Match(TokenType.SemiColon);
              symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
              Match(TokenType.Identifier);
              Statement last;
              if(this.lookAhead.TokenType == TokenType.Assignation)
                last = AssignStmt(symbol.Id);
              else //if (this.lookAhead.TokenType == TokenType.Increment || this.lookAhead.TokenType == TokenType.Decrement)
              {
                var type = this.lookAhead.TokenType;
                Move();
                last = new IncDecStatement(symbol.Id, type);
              }
              Match(TokenType.RightParens);
              Statement loop;
              if(this.lookAhead.TokenType == TokenType.OpenBrace){
                Match(TokenType.OpenBrace);
                  loop = Block();
                Match(TokenType.CloseBrace);
              }
              else
                loop = Stmt();
              return new ForStatement(firstAssignation as AssignationStatement, check as TypedExpression, last, loop); 
            }
          default:
            return Block();
        }
      }catch(Exception e){
        Console.Error.WriteLine(e.Message);
        throw new ApplicationException("No se han implementado los metodos");
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
      while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Division || this.lookAhead.TokenType == TokenType.Modulus)
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
        case TokenType.BoolConstant:
          constant = new Constant(lookAhead, Type.Bool);
          Match(TokenType.BoolConstant);
          return constant;
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
      return new AssignationStatement(id, expression as TypedExpression);
    }

    private Statement Decls()
    {
      if (this.lookAhead.TokenType == TokenType.IntKeyword ||
          this.lookAhead.TokenType == TokenType.FloatKeyword ||
          this.lookAhead.TokenType == TokenType.BoolKeyword ||
          this.lookAhead.TokenType == TokenType.StringKeyword)
        //<------------ Class
      {
        return new SequenceStatement(Decl(), Decls());
      }
      return null;
    }

    private Statement Decl()
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
      return new DeclarationStatement(id);
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
