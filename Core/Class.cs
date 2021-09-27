using System;
using System.Collections.Generic;
namespace Core.Expressions
{
  public class Class : Environment
  {
    public Class(Id id)
    {
      Identifier = id;
      Constructor = new Method(id, this);
    }
    public Id Identifier { get; }

    public Method Constructor { get; set; }
  }
}
