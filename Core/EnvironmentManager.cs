using Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
  public static class EnvironmentManager {
    private static List<Environment> _contexts = new List<Environment>();
    private static List<Environment> _interpretContexts = new List<Environment>();
    private static int _currentIndex = -1;

    public static Environment PushContext(Environment env)
    {
      _contexts.Add(env);
      _interpretContexts.Add(env);
      return env;
    }

    public static Environment PushContext()
    {
      var env = new Environment();
      _contexts.Add(env);
      _interpretContexts.Add(env);
      return env;
    }

    public static Environment PopContext()
    {
      var lastContext = _contexts.Last();
      _contexts.Remove(lastContext);
      return lastContext;
    }

    public static Environment TopContext()
    {
      var lastContext = _contexts.Last();
      return lastContext;
    }

    public static bool ClassExists(string lexeme)
    {
      foreach (var context in _interpretContexts)
      {
        var symbol = context.Get(lexeme);
        if (symbol != null && symbol.SymbolType == SymbolType.Class)
        {
          return true;
        }
      }
      return false;
    }

    public static Symbol GetClass(string lexeme)
    {
      for(int i = _contexts.Count - 1; i >= 0; i--)
      {
        var context = _contexts[i];
        var symbol = context.Get(lexeme);
        if (symbol != null && symbol.SymbolType == SymbolType.Class)
        {
          return symbol;
        }
      }
      throw new ApplicationException($"Symbol '{lexeme}' doesn't exist in current context");
    }

    public static Symbol GetSymbol(string lexeme)
    {
      for(int i = _contexts.Count - 1; i >= 0; i--)
      {
        var context = _contexts[i];
        var symbol = context.Get(lexeme);
        if (symbol != null)
        {
          return symbol;
        }
      }
      throw new ApplicationException($"Symbol '{lexeme}' doesn't exist in current context");
    }

    public static Symbol GetSymbolForEvaluation(string lexeme)
    {
      foreach (var context in _interpretContexts)
      {
        var symbol = context.Get(lexeme);
        if (symbol != null)
        {
          return symbol;
        }
      }
      throw new ApplicationException($"Symbol {lexeme} doesn't exist in current context");
    }
    public static Class AddClass(string lexeme, Id id){
      return _contexts.Last().AddClass(lexeme, id);
    }

    public static Method AddMethod(string lexeme, Id id, BinaryOperator arguments)
    {
      return _contexts.Last().AddMethod(lexeme, id);
    }

    public static void AddVariable(string lexeme, Id id) => _contexts.Last().AddVariable(lexeme, id);

    public static void UpdateVariable(string lexeme, dynamic value)
    {
      for (int i = _contexts.Count - 1; i >= 0; i--)
      {
        var context = _contexts[i];
        var symbol = context.Get(lexeme);
        if (symbol != null)
        {
          context.UpdateVariable(lexeme, value);
        }
      }
    }

  }
  public class Environment
  {
    private readonly Dictionary<string, Symbol> _table;

    public Environment()
    {
      _table = new Dictionary<string, Symbol>();
    }

    public virtual void AddVariable(string lexeme, Id id)
    {
      if (!_table.TryAdd(lexeme, new Symbol(SymbolType.Variable, id, null)))
      {
        throw new ApplicationException($"Variable {lexeme} already defined in current context");
      }
    }

    public virtual void UpdateVariable(string lexeme, dynamic value)
    {
      var variable = Get(lexeme);
      variable.Value = value;
      _table[lexeme] = variable;
    }

    public virtual Method AddMethod(string lexeme, Id id)
    {
      Method method = new Method(id, this as Class);
      if (!_table.TryAdd(lexeme, new Symbol(SymbolType.Method, id, method)))
      {
        throw new ApplicationException($"El metodo {lexeme} ya esta definido");
      }
      return method;
    }


    public virtual Class AddClass(string lexeme, Id id)
    {
      Class NewClass = new Class(id);
      if (!_table.TryAdd(lexeme, new Symbol(SymbolType.Class, id, NewClass)))
      {
        throw new ApplicationException($"La Clase {lexeme} ya esta definida");
      }
      return NewClass;
    }

    public virtual Symbol Get(string lexeme)
    {
      if (_table.TryGetValue(lexeme, out var found))
      {
        return found;
      }

      return null;
    }
  }
}
