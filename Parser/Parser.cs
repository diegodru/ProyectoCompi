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
      if(this.lookAhead.TokenType == TokenType.ThisKeyword)
      {
        Match(TokenType.ThisKeyword);
        Match(TokenType.Period);
      }
      if (this.lookAhead.TokenType == TokenType.CloseBrace || this.lookAhead.TokenType == TokenType.EOF || this.lookAhead.TokenType == TokenType.ReturnKeyword)
      {//{}
        return null;
      }
      return new SequenceStatement(Stmt(), Stmts());
    }

    private Symbol ID()
    {
      Symbol symbol;
      if(this.lookAhead.TokenType == TokenType.ThisKeyword)
      {
        Match(TokenType.ThisKeyword);
        Console.WriteLine(".");
        Match(TokenType.Period);
        Class clase = (EnvironmentManager.TopContext() as Method).Class;
        if((symbol = clase.Get(this.lookAhead.Lexeme)) == null)
        {
          throw new ApplicationException($"no existe el simbolo {this.lookAhead.Lexeme} en la clase {clase.Identifier.Generate()}");
        }
      }
      else
        symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
      return symbol;
    }

    private Statement Stmt()
    {
      try{
        Expression expression;
        Statement statement1, statement2;
        switch (this.lookAhead.TokenType)
        {
          case TokenType.ConsoleKeyword:
            {
              Match(TokenType.ConsoleKeyword);
              Match(TokenType.Period);
              if(this.lookAhead.TokenType == TokenType.WriteLineKeyword 
                  || this.lookAhead.TokenType == TokenType.ReadLineKeyword
                  )
              {
                var accion = this.lookAhead.TokenType;
                Move();
                Match(TokenType.LeftParens);
                var exp = Eq();
                Match(TokenType.RightParens);
                Match(TokenType.SemiColon);
                return new ConsoleStatement(accion, exp as TypedExpression);
              }
              else
                throw new ApplicationException("Console solo puede leer y escribir");
            }
          case TokenType.Identifier:
            {
              var symbol = ID();
              Match(TokenType.Identifier);
              if (this.lookAhead.TokenType == TokenType.Period)
              {
                Match(TokenType.Period);
                string methodname = this.lookAhead.Lexeme;
                Class clase = EnvironmentManager.GetClass(symbol.Id.GetExpressionType().Lexeme).Value;
                Symbol methodsymbol = clase.Get(methodname);
                if(methodsymbol == null || methodsymbol.SymbolType != SymbolType.Method)
                {
                  throw new ApplicationException($"No existe el metodo '{methodname}' en la clase '{clase.Identifier.Generate()}'");
                }
                Match(TokenType.Identifier);
                Match(TokenType.LeftParens);
                var args = OptArguments();
                Match(TokenType.RightParens);
                Match(TokenType.SemiColon);
                return new CallStatement(symbol, methodsymbol, args as ArgumentExpression);
              }
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
              var symbol = ID();
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
                symbol = ID();
                Match(TokenType.Identifier);
                last = new IncDecStatement(symbol.Id, type, true);
              }
              else
              {
                symbol = ID();
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
              var symbol = ID();
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
            return new ParenthesesExpression(expression as TypedExpression);
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
        case TokenType.NewKeyword:
          {
            Match(TokenType.NewKeyword);
            switch(this.lookAhead.TokenType)
            {
              case TokenType.DateTimeKeyword:
                {
                  Move();
                  Match(TokenType.LeftParens);
                  var args = OptArguments();
                  Match(TokenType.RightParens);
                  return new DateTimeConstant(null, args as ArgumentExpression);
                }
              default:
                var token = this.lookAhead;
                Match(TokenType.Identifier);
                if(!EnvironmentManager.ClassExists(token.Lexeme))
                {
                  throw new ApplicationException($"La clase {token.Lexeme} no existe");
                }
                Match(TokenType.LeftParens);
            }
          }
        default:
          var symbol = ID();
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
          if(!EnvironmentManager.ClassExists(TypeToken.Lexeme))
          {
            throw new ApplicationException($"No existe el tipo {TypeToken.Lexeme}");
          }
          id = new Id(token, new Type(TypeToken.Lexeme, TokenType.ClassType));
          break;
      }
      (EnvironmentManager.TopContext() as Method).AddVariable(token.Lexeme, id);
      //EnvironmentManager.AddVariable(token.Lexeme, id);
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
        return new ArgumentExpression(expression as TypedExpression, null);
      }
      Match(TokenType.Comma);
      expression = new ArgumentExpression(expression as TypedExpression, Arguments() as TypedExpression);
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
      if(this.lookAhead.TokenType == TokenType.ReturnKeyword || this.lookAhead.TokenType == TokenType.CloseBrace)
      {
        return null;
      }
      if(this.lookAhead.TokenType  == TokenType.ThisKeyword)
        {
          Match(TokenType.ThisKeyword);
          Match(TokenType.Period);
        }
      return new SequenceStatement(new SequenceStatement(Decls(), Stmts()), MethodBlock());
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
        {
          if((decl as MethodStatement)?.lol() == typeof(MethodStatement))
          {
            return new SequenceStatement(decl, ClassBlock(Class));
          }
          return ClassBlock(Class);
        }
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
          this.lookAhead.TokenType == TokenType.VoidKeyword ||
          this.lookAhead.TokenType == TokenType.DateTimeKeyword ||
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
        return null;
      }
      Token token = this.lookAhead;
      Match(TokenType.Identifier);
      if(this.lookAhead.TokenType == TokenType.LeftParens)
      {
        Id id2;
        switch (TypeToken.TokenType)
        {
          case TokenType.FloatKeyword:
            id2 = new Id(token, Type.Float);
            break;
          case TokenType.StringKeyword:
            id2 = new Id(token, Type.String);
            break;
          case TokenType.BoolKeyword:
            id2 = new Id(token, Type.Bool);
            break;
          case TokenType.IntKeyword:
            id2 = new Id(token, Type.Int);;
            break;
          case TokenType.VoidKeyword:
            id2 = new Id(token, Type.Void);
            break;
          case TokenType.DateTimeKeyword:
            id2 = new Id(token, Type.DateTime);
            break;
          //case TokenType.ClassKeyword: ////////////////////////TODO
            //throw new NotImplementedException(); //<---- clase
          default: //<-------- TokenType.Identifer
            id2 = new Id(token, new Type(TypeToken.Lexeme, TokenType.ClassType));
            break;
        }
        var method = EnvironmentManager.TopContext().AddMethod(id2.Generate(), id2);
        EnvironmentManager.PushContext(method as Core.Environment);
        Match(TokenType.LeftParens);
        var parametros = OptParams();
        Match(TokenType.RightParens);

        Match(TokenType.OpenBrace);
        Console.WriteLine(this.lookAhead.TokenType);
        var methodblock = MethodBlock();
        ReturnExpression returnexpression = null;
        if (this.lookAhead.TokenType == TokenType.ReturnKeyword)
        {
          Match(TokenType.ReturnKeyword);
          if(id2.GetExpressionType() == Type.Void)
          {
            returnexpression =  new ReturnExpression(null, id2.GetExpressionType(), null);
          }
          else
          {
            returnexpression =  new ReturnExpression(null, id2.GetExpressionType(), Eq() as TypedExpression);
          }
          Match(TokenType.SemiColon);
        }
        Match(TokenType.CloseBrace);
        EnvironmentManager.PopContext();
        return new MethodStatement(id2, parametros, methodblock, returnexpression);
      }
      Match(TokenType.SemiColon);
      bool isAttribute = EnvironmentManager.TopContext().GetType() == typeof(Class);
      Id id;
      switch (TypeToken.TokenType)
      {
        case TokenType.FloatKeyword:
          id = new Id(token, Type.Float, isAttribute);
          break;
        case TokenType.StringKeyword:
          id = new Id(token, Type.String, isAttribute);
          break;
        case TokenType.BoolKeyword:
          id = new Id(token, Type.Bool, isAttribute);
          break;
        case TokenType.IntKeyword:
          id = new Id(token, Type.Int, isAttribute);;
          break;
        case TokenType.DateTimeKeyword:
          id = new Id(token, Type.DateTime, isAttribute);
          break;
        default: //<-------- TokenType.Identifer
          id = new Id(token, new Type(TypeToken.Lexeme, TokenType.ClassType), isAttribute);
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
