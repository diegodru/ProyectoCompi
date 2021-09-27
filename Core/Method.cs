using System;
using System.Collections.Generic;
namespace Core.Expressions
{
  public class Method : Environment
  {
    public List<Id> atributos;
    public Method(Id id, Class clase)
    {
      Identifier = id;
      Class = clase;
    }
    public Id Identifier { get; }

    public Class Class { get; set; }
  }
}

