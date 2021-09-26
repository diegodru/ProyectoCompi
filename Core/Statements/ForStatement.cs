using Core.Expressions;
using System;

namespace Core.Statements
{
  public class ForStatement : Statement
  {
    public ForStatement(AssignationStatement assignation, TypedExpression check, Statement lastAssignation, Statement loop)
    {
      Loop = loop;
      FirstAssignation = assignation;
      Check = check;
      LastAssignation = lastAssignation;
    }
    //public ForStatement(Statement loop, TypedExpression check, AssignationStatement lastAssignation)
    //{
      //Loop = loop;
      //Check = check;
      //LastAssignation = lastAssignation;
    //}

    public TypedExpression Check { get; }
    public Statement Loop { get; }
    public AssignationStatement FirstAssignation { get; }
    public Statement LastAssignation { get; }

    public override string Generate()
    {
      var code = $"for({FirstAssignation?.Generate()}; {Check?.Generate()}; {LastAssignation?.Generate()}){{";
      code += $"{Loop?.Generate()}}}";
      return code;
    }

    public override void Interpret()
    {
      FirstAssignation?.Interpret();
      if(Check == null)
      {
        while(true)
        {
          Loop.Interpret();
          LastAssignation?.Interpret();
        }
      }
      else
      {
        while(Check.Evaluate())
        {
          Loop.Interpret();
          LastAssignation?.Interpret();
        }
      }
    }
    
    public override void ValidateSemantic()
    {
      if(Check != null)
      {
        if(Check.GetExpressionType() != Type.Bool)
        {
          throw new ApplicationException("el for solo puede evaluar bools");
        }
      }
    }

  }
}
