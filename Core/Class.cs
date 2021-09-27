using System;
using System.Collections.Generic;
namespace Core.Expressions
{
  public class Class
  {
    private readonly Dictionary<string, Symbol> _table;

    public Class(Id id)
    {
      Identifier = id;
    }
    public Id Identifier { get; }

    public void AddMethod(string lexeme, Id id, BinaryOperator arguments)
    {
      if (!_table.TryAdd(lexeme, new Symbol(SymbolType.Method, id, arguments)))
      {
        throw new ApplicationException($"El metodo {lexeme} ya esta definido");
      }
    }

    public void AddVariable(string lexeme, Id id)
    {
      if (!_table.TryAdd(lexeme, new Symbol(SymbolType.Variable, id, null)))
      {
        throw new ApplicationException($"Variable {lexeme} already defined in current context");
      }
    }

    public Symbol Get(string lexeme)
    {
      if (_table.TryGetValue(lexeme, out var found))
      {
        return found;
      }

      return null;
    }

    public void UpdateVariable(string lexeme, dynamic value)
    {
      var variable = Get(lexeme);
      variable.Value = value;
      _table[lexeme] = variable;
    }

  }
}
