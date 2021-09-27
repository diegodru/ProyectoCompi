using System;
using System.Collections.Generic;
namespace Core.Expressions
{
  public class Method : Environment
  {
    private readonly Dictionary<string, Symbol> _table;

    public Method(Id id)
    {
      Identifier = id;
    }
    public Id Identifier { get; }
  }
}

