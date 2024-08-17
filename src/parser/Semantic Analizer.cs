
using System.Reflection.Metadata;

public class SemanticAnalizer
{
	public List<CompilingError> errors;
	Interpreter interpreter;
	ElementalProgram ElementalProgram;
	static readonly string[] ranges = { "\"Melee\"", "\"Range\"", "\"Siege\"" };
	public SemanticAnalizer(ElementalProgram elementalProgram)
	{
		interpreter=new Interpreter(elementalProgram);
		errors = new List<CompilingError>();
		ElementalProgram = elementalProgram;
	}
	#region Expressions
	public void Visit_Atom(Atom expression)
	{
	}
	public void Visit_Binary(BinaryExpression expression)
	{
		interpreter.Evaluate(expression);
	}
	public void Visit_Group(Group expression)
	{
		Analizer(expression.expression);
	}
	public void Visit_Unary(Unary expression)
	{
		Analizer(expression.rigth_expression);
	}
	public void Visit_Get(GetExpression expression)
	{
		try
		{
			GwentClass gwentClass = ReturnClass(expression.callee);
			if (gwentClass.Get(expression.name.Value) == null)
			{
				errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the class not contains the property " + expression.name.Value));
			}
		}
		catch
		{
			errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the initial identifier doesnt exist in the current context"));
		}
	}
	public void Visit_Set(SetExpression expression)
	{
		Analizer(expression.getExpression);
		Analizer(expression.value);
	}
	public void Visit_Metod(MetodExpression expression)
	{
		try
		{
			GwentClass gwentClass = ReturnClass(expression.callee);
			if (gwentClass.FindMetod(expression.name.Value) == null)
			{
				errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the class not contains the metod" + expression.name.Value));
			}
		}
		catch
		{
			errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the initial identifier doesnt exist in the current context"));
		}
	}
	public void Visit_CallEffect(CallEffect expression)
	{
		Analizer(expression.Selector);
		if (ElementalProgram.Effects.Keys.Contains(expression.effect_name))
		{
			Effect actual_effect = ElementalProgram.Effects[expression.effect_name];
			HashSet<Token> Params = new HashSet<Token>(actual_effect.Params);
			HashSet<Token> Args = new HashSet<Token>(expression.arguments.Keys);
			if (!Params.SetEquals(Args))
			{
				if (Args.IsSubsetOf(Params))
				{
					foreach (Token token in Params)
					{
						if (!Args.Contains(token))
						{
							errors.Add(new CompilingError(token.Location, ErrorCode.Expected, "expected " + token.Value + " param in arguments"));
						}
					}

				}
				else
				{
					foreach (Token token in Args)
					{
						if (!Params.Contains(token))
						{
							errors.Add(new CompilingError(token.Location, ErrorCode.Invalid, "the argument " + token.Value + " does not exist as param"));
						}
					}
				}
			}
			else
			{
				Scope last=StartScope();
				foreach (KeyValuePair<Token, Expression> keyValuePair in expression.arguments)
        		{
            		interpreter.Define(keyValuePair.Key.Value,interpreter.Evaluate(keyValuePair.Value));
				}
				
				Analizer(actual_effect);
				interpreter.Scope=last;
			}
		}
		else
		{
			errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The effect " + expression.effect_name + " does not exist in the current context"));
		}
	}
	public void Visit_Variable(Variable expression)
	{
		if(interpreter.Scope.GetValue(expression.name)==null)
		{
			errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The variable " + expression.name + " does not exist in the current context"));
		}
	}
	public void Visit_Assign(AssignExpression expression)
	{
		try
		{
			interpreter.Define(expression.variable.name,interpreter.Evaluate(expression.Right));
		}
		catch
		{
			Analizer(expression.Right);
		}
	}
	public void Visit_Selector(Selector expression)
	{
		Analizer(expression.predicateStmt);
	}
	public void Visit_Predicate(PredicateStmt expression)
	{
		switch(expression.paramsPredicate)
		{
			case ParamsPredicate.unit:
			{
				interpreter.Define("unit",new Card(new CodeLocation()));
				break;
			}
			case ParamsPredicate.boost:
			{
				interpreter.Define("boost",new Card(new CodeLocation()));
				break;
			}
			case ParamsPredicate.weather:
			{
				interpreter.Define("weather",new Card(new CodeLocation()));
				break;
			}
			case ParamsPredicate.card:
			{
				interpreter.Define("card",new Card(new CodeLocation()));
				break;
			}
		}
		object value=interpreter.Evaluate(expression.condition);
		if(value.GetType()!=typeof(bool))
		{
			errors.Add(new CompilingError(expression.condition.Location, ErrorCode.Expected, "expected bool Data type in condition"));
		}
	}

	#endregion
	#region Declarations
	public void Visit_Card(Card expression)
	{
		if (expression.CheckPorperties(errors))
		{
			List<string>contexts=new List<string>();
			if (expression.Ranges != null)
			{
				foreach (string Range in expression.Ranges)
				{
					if (!ranges.Contains(Range))
					{
						errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, String.Format("{0} Range Does not exists", Range)));
					}
					if (contexts.Contains(Range))
					{
						errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, String.Format("{0} Range already in use", Range)));
					}
					else
					{
						contexts.Add(Range);
					}
				}
			}
			if (expression.Effect != null)
			{
				Analizer(expression.Effect);
			}
		}
	}
	public void Visit_Effect(Effect expression)
	{
		foreach (Stmt stmt in expression.body)
		{
			Analizer(stmt);
		}
	}
	public void Visit_Program(ElementalProgram program)
	{
		foreach (Effect effect in program.Effects.Values)
		{
			Analizer(effect);
		}
		foreach (Card card in program.Cards.Values)
		{
			Analizer(card);
		}
	}
	public void Visit_DeclarationExpression(ExpressionStmt declaration)
	{
		Analizer(declaration.expression);
	}
	public void Visit_WhileDeclaration(WhileStmt declaration)
	{
		Scope last=StartScope();
		object temp=interpreter.Evaluate(declaration.condition);
		if(temp.GetType()!=typeof(bool))
		{
			errors.Add(new CompilingError(declaration.condition.Location, ErrorCode.Expected, "expected bool Data type in condition"));
		}
		foreach (Stmt stmt in declaration.body)
		{
			Analizer(stmt);
		}
		interpreter.Scope=last;
	}
	public void Visit_ForDeclaration(ForStmt declaration)
	{
		Scope last=StartScope();
		interpreter.Define(declaration.identifier.Value,new Card(new CodeLocation()));
		object? temp=interpreter.Scope.GetValue(declaration.ienumerable.Value);
		if(temp==null || temp.GetType()!=typeof(PackOfCards))
		{
			errors.Add(new CompilingError(declaration.ienumerable.Location, ErrorCode.Expected, "expected IEnumerable Data type in for declaration"));
		}
		foreach (Stmt stmt in declaration.body)
		{
			Analizer(stmt);
		}
		interpreter.Scope=last;
	}
	public void Visit_DeclarationEmpty(EmptyStmt declaration)
	{
	}
	#endregion
	#region Internal Metods
	GwentClass ReturnClass(Expression Expression)
	{
		if (Expression.GetType() == typeof(GetExpression))
		{
			GetExpression getExpression = (GetExpression)Expression;
			GwentClass gwentClass = ReturnClass(getExpression.callee);
			object? temp = gwentClass.Get(getExpression.name.Value);
			if (temp != null)
			{
				gwentClass = (GwentClass)temp;
			}
			else
			{
				errors.Add(new CompilingError(getExpression.name.Location, ErrorCode.Invalid, "the class not contains the property" + getExpression.name.Value));
			}
			return gwentClass;
		}
		if (Expression.GetType() == typeof(MetodExpression))
		{
			MetodExpression metodExpression = (MetodExpression)Expression;
			GwentClass gwentClass = ReturnClass(metodExpression.callee);
			Funtion? funtion = gwentClass.FindMetod(metodExpression.name.Value);
			if (funtion != null)
			{
				object temp = funtion.Delegate.Method.ReturnType;
				gwentClass = (GwentClass)temp;
			}
			else
			{
				errors.Add(new CompilingError(metodExpression.name.Location, ErrorCode.Invalid, "the class not contains the metod" + metodExpression.name.Value));
			}
			return gwentClass;
		}
		else if (Expression.GetType() == typeof(Variable))
		{
			Variable variable = (Variable)Expression;
			return (GwentClass)interpreter.Scope.Variables[variable.name];
		}
		else
		{
			errors.Add(new CompilingError(Expression.Location, ErrorCode.Invalid, "only the class contains properties"));
			throw new Exception();
		}
	}
	void Analizer(ASTNode aSTNode)
	{
		aSTNode.CheckSemantic(this);
	}
	Scope StartScope()
	{
		Scope last = interpreter.Scope;
        interpreter.Scope = last.CreateChild();
		return last;
	}
	public Interpreter Semantic_Analizer()
	{
		Analizer(ElementalProgram);
		return interpreter;
	}
	public void Print_errors()
	{
		if (errors.Count == 0)
		{
			return;
		}
		foreach (CompilingError error in errors)
		{
			Console.WriteLine(error.Code + "=> " + error.Argument + " at line " + error.location.line + " and column " + error.location.column);
		}
	}
	#endregion
}