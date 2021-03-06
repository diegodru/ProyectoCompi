using System;
namespace Core.Statements
{
    public class SequenceStatement : Statement
    {
        public Statement FirstStatement { get; private set; }

        public Statement NextStatement { get; private set; }

        public SequenceStatement(Statement firstStatement, Statement nextStatement)
        {
            FirstStatement = firstStatement;
            NextStatement = nextStatement;
        }

        public override void ValidateSemantic()
        {
            FirstStatement?.ValidateSemantic();
            NextStatement?.ValidateSemantic();
        }

        public override void Interpret()
        {
            FirstStatement?.Interpret();
            NextStatement?.Interpret();
        }

        public override string Generate()
        {
          string newLine = "", newLine2 = "";
          if(NextStatement != null)
            newLine = "\n";
          else if(FirstStatement != null)
            newLine2 = "\n";
          var code = FirstStatement?.Generate() + newLine;
          code += NextStatement?.Generate() + newLine2;
          return code;
        }
    }
}
