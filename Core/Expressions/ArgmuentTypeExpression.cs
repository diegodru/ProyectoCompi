using System;
namespace Core.Expressions
{
  public class ArgumentTypeExpression : TypedBinaryOperator
  {
    public ArgumentTypeExpression(TypedExpression first, TypedExpression second)
      : base(null, first, second, null)
    {
      First = first;
      Second = second;
    }

    public ArgumentTypeExpression(TypedExpression first)
      : base(null, first, null, null)
    {
      First = first;
    }
    
    public TypedExpression First { get; }
    public TypedExpression Second { get; }

    public override Type GetExpressionType()
    {
        throw new System.NotImplementedException();
    }


    public override string Generate()
    {
      if (left != null)
      {
        return  $"{left?.Generate()}, {right.Generate()}";
      }
      return left?.Generate();
    }

    public override dynamic Evaluate()
            {
                throw new NotImplementedException();
            }

  }
}
