using Core.Expressions;
using System;

namespace Core.Statements
{
  public class ForStatement : Statement
  {
    public ForStatement(TypedExpression expression, Statement loop, AssignationStatement assignation, TypedExpression check, AssignationStatement lastAssignation)
    {
      Expression = expression;
      Loop = loop;
      FirstAssignation = assignation;
      Check = check;
      LastAssignation = lastAssignation;
    }
    public ForStatement(TypedExpression expression, Statement loop, TypedExpression check, AssignationStatement lastAssignation)
    {
      Expression = expression;
      Loop = loop;
      Check = check;
      LastAssignation = lastAssignation;
    }

    public TypedExpression Expression { get; }
    public TypedExpression Check { get; }
    public Statement Loop { get; }
    public AssignationStatement FirstAssignation { get; }
    public Statement LastAssignation { get; }

    public override string Generate()
    {
      var code = $"for({FirstAssignation?.Generate()}; {Expression?.Generate()}; {LastAssignation?.Generate()}){{\n";
      code += $"{Loop?.Generate()}\n}}\n";
      return code;
    }

    public override void Interpret()
    {
      FirstAssignation?.Interpret();
      if(Expression == null)
      {
        while(true)
        {
          Loop.Interpret();
          LastAssignation?.Interpret();
        }
      }
      else
      {
        while(Expression.Evaluate())
        {
          Loop.Interpret();
          LastAssignation?.Interpret();
        }
      }
    }
    
    public override void ValidateSemantic()
    {
      if(Expression != null)
      {
        if(Expression.GetExpressionType() != Type.Bool)
        {
          throw new ApplicationException("el for solo puede evaluar bools");
        }
      }
    }

  }
}
