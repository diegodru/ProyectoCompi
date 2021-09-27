using Core;
using Core.Expressions;
using Core.Interfaces;
using Core.Statements;
using System;
using Type = Core.Type;

namespace test
{
  public class Parser2 : IParser
  {
    private readonly IScanner scanner;
    private Token lookAhead;

    public Parser2(IScanner scanner)
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
      try{
        var block = Block();
        block.ValidateSemantic();
        return block;
      }catch(Exception e)
      {
        Console.Error.WriteLine(e.Message);
        throw new ApplicationException();
      }
    }

    private Statement Block()
    {
      EnvironmentManager.PushContext();
      EnvironmentManager.AddClass("test", new Id(new Token
            {
            Lexeme = "test",
            }, new Type("test", TokenType.ClassType))
          );
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
                Console.WriteLine(symbol.Id.Generate());
                var assStmt = AssignStmt(symbol.Id);
                Match(TokenType.SemiColon);
                return assStmt;
              }
              else if (this.lookAhead.TokenType == TokenType.Increment || this.lookAhead.TokenType == TokenType.Decrement)
              {
                var type = this.lookAhead.TokenType;
                Move();
                var incdec = new IncDecStatement(symbol.Id, type, false);
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
              while(this.lookAhead.TokenType == TokenType.AND || this.lookAhead.TokenType == TokenType.OR)
              {
                var token = this.lookAhead;
                Move();
                expression = new LogicalOperator(token, expression as TypedExpression, Eq() as TypedExpression);
              }
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
          case TokenType.WhileKeyword:
            {
              Match(TokenType.WhileKeyword);
              Match(TokenType.LeftParens);
              var relationalExpression = Rel();
              while(this.lookAhead.TokenType == TokenType.AND || this.lookAhead.TokenType == TokenType.OR)
              {
                var token = this.lookAhead;
                Move();
                relationalExpression = new LogicalOperator(token, relationalExpression as TypedExpression, Eq() as TypedExpression);
              }
              Match(TokenType.RightParens);
              Statement loop;
              if(this.lookAhead.TokenType == TokenType.OpenBrace)
              {
                Match(TokenType.OpenBrace);
                loop = Block();
                Match(TokenType.CloseBrace);
              }else
              {
                loop = Stmt();
              }
              return new WhileStatement(relationalExpression as TypedExpression, loop);

            }
          case TokenType.ForKeyword:
            {
              Match(TokenType.ForKeyword);
              Match(TokenType.LeftParens);
              Console.WriteLine(this.lookAhead.Lexeme);
              var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
              Match(TokenType.Identifier);
              var firstAssignation = AssignStmt(symbol.Id);
              Match(TokenType.SemiColon);
              var check = Eq();
              while(this.lookAhead.TokenType == TokenType.AND || this.lookAhead.TokenType == TokenType.OR)
              {
                var token = this.lookAhead;
                Move();
                expression = new LogicalOperator(token, check as TypedExpression, Eq() as TypedExpression);
              }
              Match(TokenType.SemiColon);
              Statement last;
              if (this.lookAhead.TokenType == TokenType.Increment || this.lookAhead.TokenType == TokenType.Decrement)
              {
                var type = this.lookAhead.TokenType;
                Move();
                symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                Match(TokenType.Identifier);
                last = new IncDecStatement(symbol.Id, type, true);
              }
              else
              {
                symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                Match(TokenType.Identifier);
                if(this.lookAhead.TokenType == TokenType.Assignation)
                {
                  last = AssignStmt(symbol.Id);
                }
                else if (this.lookAhead.TokenType == TokenType.Increment || this.lookAhead.TokenType == TokenType.Decrement)
                {
                  var type = this.lookAhead.TokenType;
                  Move();
                  last = new IncDecStatement(symbol.Id, type, false);
                }
                else
                  last = new IncDecStatement(null, TokenType.RightParens, false);  // <----------- CallStmt
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
          case TokenType.Increment:
          case TokenType.Decrement:
            {
              var token = this.lookAhead.TokenType;
              Move();
              var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
              Match(TokenType.Identifier);
              Match(TokenType.SemiColon);
              return new IncDecStatement(symbol.Id, token, true);
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
      var expression = Logical();
      if (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
      {
        var token = lookAhead;
        Move();
        expression = new RelationalExpression(token, expression as TypedExpression, Rel() as TypedExpression);
      }

      return expression;
    }

    private Expression Logical()
    {
      var expression = Rel();
      if (this.lookAhead.TokenType == TokenType.AND ||
          this.lookAhead.TokenType == TokenType.OR)
      {
        var token = lookAhead;
        Move();
        expression = new LogicalOperator(token, expression as TypedExpression, Rel() as TypedExpression);
      }
      return expression;
    }

    private Expression Rel()
    {
      var expression = Expr();
      if (this.lookAhead.TokenType == TokenType.LessThan
          || this.lookAhead.TokenType == TokenType.GreaterThan
          || this.lookAhead.TokenType == TokenType.LessThanOrEqual
          || this.lookAhead.TokenType == TokenType.GreaterThanOrEqual
          || this.lookAhead.TokenType == TokenType.Equal
          || this.lookAhead.TokenType == TokenType.NotEqual
          )
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

    private Statement Param()
    {
      Token TypeToken = this.lookAhead;
      Move();
      Token token = this.lookAhead;
      Match(TokenType.Identifier);
      Id id;
      switch (TypeToken.TokenType)
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
        case TokenType.IntKeyword:
          id = new Id(token, Type.Int);;
          break;
        default: //<-------- TokenType.Identifer
          id = new Id(token, new Type(TypeToken.Lexeme, TokenType.ClassType));
          break;
      }
      EnvironmentManager.AddVariable(token.Lexeme, id);
      return new DeclarationStatement(id, true);
    }

    private Statement Params()
    {
      var param = Param();
      if(this.lookAhead.TokenType == TokenType.Comma)
      {
        Match(TokenType.Comma);
        return new ParameterStatement(param, Params());
      }
      return param;
    }

    private Statement OptParams()
    {
      if (this.lookAhead.TokenType != TokenType.RightParens)
      {
        return Params();
      }
      return null;
    }

    private Expression OptArguments()
    {
      if (this.lookAhead.TokenType != TokenType.RightParens)
      {
        return Arguments();
      }
      return null;
    }

    private Expression Arguments()
    {
      var expression = Eq();
      if (this.lookAhead.TokenType != TokenType.Comma)
      {
        return expression;
      }
      Match(TokenType.Comma);
      expression = new ArgumentExpression(lookAhead, expression as TypedExpression, Arguments() as TypedExpression);
      return expression;
    }

    private Statement AttributeAssignStmt(Id id)
    {
      Match(TokenType.Assignation);
      var expression = Eq();
      return new AssignationStatement(id, expression as TypedExpression, true);
    }

    private Statement AssignStmt(Id id)
    {
      Match(TokenType.Assignation);
      var expression = Eq();
      return new AssignationStatement(id, expression as TypedExpression, false);
    }

    private Statement MethodBlock()
    {
      switch(this.lookAhead.TokenType)
      {
        case TokenType.ThisKeyword:
          {
            Match(TokenType.ThisKeyword);
            Match(TokenType.Period);
            var token = this.lookAhead;
            Match(TokenType.Identifier);
            var symbol = EnvironmentManager.GetSymbol(token.Lexeme);
            var assStmt = AttributeAssignStmt(symbol.Id);
            Match(TokenType.SemiColon);
            return new SequenceStatement(assStmt, MethodBlock());
          }
      }
      return null;
    }

    private Statement ConstructorStmt(Class Class)
    {
        //Match(TokenType.Identifier);

        Match(TokenType.LeftParens);
        EnvironmentManager.PushContext(Class.Constructor);
        var parametros = OptParams();
        Match(TokenType.RightParens);

        Match(TokenType.OpenBrace);
        var methodblock = MethodBlock();
        EnvironmentManager.PopContext();
        Match(TokenType.CloseBrace);

        return new ConstructorStatement(parametros, methodblock);
    }

    private Statement ClassBlock(Class Class)
    {
      if(this.lookAhead.TokenType != TokenType.CloseBrace)
      {
        Statement decl = Decl();
        if(decl != null)
          return ClassBlock(Class);
        var constructor = ConstructorStmt(Class);
        return new SequenceStatement(constructor, ClassBlock(Class));
      }
      return null;
    }

    private Statement ClassDecl()
    {
      Match(TokenType.ClassKeyword);
      var token = this.lookAhead;
      Match(TokenType.Identifier);
      Match(TokenType.OpenBrace);
      Id NewClassId = new Id(token, new Type(token.Lexeme, TokenType.ClassType));
      var NewClass = EnvironmentManager.AddClass(token.Lexeme, NewClassId);
      EnvironmentManager.PushContext(NewClass as Core.Environment);
      var classblock = new ClassStatement(NewClassId, ClassBlock(NewClass));
      EnvironmentManager.PopContext();
      Match(TokenType.CloseBrace);
      return classblock;
    }

    private Statement Decls()
    {
      if (this.lookAhead.TokenType == TokenType.IntKeyword ||
          this.lookAhead.TokenType == TokenType.FloatKeyword ||
          this.lookAhead.TokenType == TokenType.BoolKeyword ||
          this.lookAhead.TokenType == TokenType.StringKeyword ||
          this.lookAhead.TokenType == TokenType.ClassKeyword ||
          EnvironmentManager.ClassExists(this.lookAhead.Lexeme)
          )
      {
        Statement decl;
        if(this.lookAhead.TokenType == TokenType.ClassKeyword)
        {
          decl = ClassDecl();
        }
        else
        {
          decl = Decl();
        }
        return new SequenceStatement(decl, Decls());
      }
      return null;
    }

    private Statement Decl()
    {
      Token TypeToken = this.lookAhead;
      Move();
      if(this.lookAhead.TokenType == TokenType.LeftParens)
      {
        Console.WriteLine("tttt");
        return null;
      }
      Token token = this.lookAhead;
      Match(TokenType.Identifier);
      Match(TokenType.SemiColon);
      Id id;
      switch (TypeToken.TokenType)
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
        case TokenType.IntKeyword:
          id = new Id(token, Type.Int);;
          break;
        //case TokenType.ClassKeyword: ////////////////////////TODO
          //throw new NotImplementedException(); //<---- clase
        default: //<-------- TokenType.Identifer
          id = new Id(token, new Type(TypeToken.Lexeme, TokenType.ClassType));
          break;
      }
      EnvironmentManager.AddVariable(token.Lexeme, id);
      return new DeclarationStatement(id, false);
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
