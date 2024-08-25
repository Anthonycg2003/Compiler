
using System.Reflection.Metadata;

public class SemanticAnalizer
{
	public List<CompilingError> errors;
	ElementalProgram ElementalProgram;
	Stack<Dictionary<string,DataType>> DataScope=new Stack<Dictionary<string, DataType>>();
	static readonly string[] ranges = { "\"Melee\"", "\"Ranged\"", "\"Siege\"" };
	static readonly Dictionary<DataType,Dictionary<string,DataType>> DatatypeProperties=new Dictionary<DataType,Dictionary<string,DataType>>
	{
		{DataType.Card,new Dictionary<string,DataType>()
			{
				{"Power",DataType.Number},{"Name",DataType.Number},{"Faction",DataType.String},{"Owner",DataType.Player},
			}
		},
		{DataType.Context,new Dictionary<string,DataType>()
			{
				{"TriggerPlayer",DataType.Player},{"Deck",DataType.PackOfCards},{"Hand",DataType.PackOfCards},{"Board",DataType.PackOfCards},
			}
		}
	};

	static readonly Dictionary<DataType,Dictionary<string,Metod>> DatatypeMetods=new Dictionary<DataType,Dictionary<string,Metod>>
	{
		{DataType.Context,new Dictionary<string,Metod>()
			{
				{"DeckOfPlayer",new Metod(1,DataType.PackOfCards,DataType.Player)},{"HandOfPLayer",new Metod(1,DataType.PackOfCards,DataType.Player)}
			}
		},
		{DataType.PackOfCards,new Dictionary<string,Metod>()
			{
				{"Push",new Metod(1,DataType.Null,DataType.Card)},{"SendBottom",new Metod(1,DataType.Null,DataType.Card)}
            ,{"Pop",new Metod(0,DataType.Card)},{"Remove",new Metod(1,DataType.Null,DataType.Card)}
            ,{"Shuffle",new Metod(0,DataType.Null)},{"Add",new Metod(1,DataType.Null,DataType.Card)}
			}
		},
		
	};
	public SemanticAnalizer(ElementalProgram elementalProgram)
	{
		StartScope();
		errors = new List<CompilingError>();
		ElementalProgram = elementalProgram;
		DataScope.Peek().Add("context",DataType.Context);
		DataScope.Peek().Add("targets",DataType.PackOfCards);
		DataScope.Peek().Add("player",DataType.Player);
		DataScope.Peek().Add("opponent",DataType.Player);
	}
	#region Expressions
	public void Visit_Atom(Atom expression)
	{
	}
	public void Visit_Binary(BinaryExpression expression)
	{
		Analizer(expression.Left);
		Analizer(expression.Right);
		switch(expression.Operator.Type)
        {
            case TokenType.PLUS:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.MINUS:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.STAR:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.SLASH:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.Caret:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.GREATER:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.GREATER_EQUAL:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.LESS:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.LESS_EQUAL:
            if(!CheckDataType(expression.Left,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Number))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.AND:
            if(!CheckDataType(expression.Left,DataType.Bool))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Bool))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.OR:
            if(!CheckDataType(expression.Left,DataType.Bool))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.Bool))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.ARROBA:
            if(!CheckDataType(expression.Left,DataType.String))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.String))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.ARROBA_ARROBA:
            if(!CheckDataType(expression.Left,DataType.String))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "The left operand must be number"));
			}
			if(!CheckDataType(expression.Right,DataType.String))
			{
				errors.Add(new CompilingError(expression.Right.Location, ErrorCode.Invalid, "The rigth operand must be number"));
			}
            break;
            case TokenType.EQUAL_EQUAL:
			DataType DataLeft=expression.Left.dataType;
			DataType DataRight=expression.Right.dataType;
            if(!(DataLeft==DataRight))
			{
				errors.Add(new CompilingError(expression.Left.Location, ErrorCode.Invalid, "Both operands must be same type"));
			}
			break;
    	}
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
			DataType gwentClass = ReturnClass(expression.callee);
			try
			{
				gwentClass = DatatypeProperties[gwentClass][expression.name.Value];
				expression.dataType=gwentClass;
			}
			catch
			{
				errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the class "+ gwentClass.ToString()+ " not contains the property" + expression.name.Value));
			}
		}
		catch
		{
		}
	}
	public void Visit_Set(SetExpression expression)
	{
		Analizer(expression.getExpression);
		Analizer(expression.value);
	}
	public void Visit_Metod(MetodExpression expression)
	{
		foreach(Variable variable in expression.arguments)
		{
			Analizer(variable);
		}
		try
		{
			DataType gwentClass = ReturnClass(expression.callee);
			Metod? metod=null;
			try
			{
				metod=DatatypeMetods[gwentClass][expression.name.Value];
				gwentClass = metod.ReturnType;
				expression.dataType=gwentClass;
			}
			catch
			{
				errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the class "+ gwentClass.ToString()+ " not contains the metod" + expression.name.Value));
			}
			if (metod != null)
			{
				if(metod.Arity==expression.arguments.Count)
				{
					for(int i=0;i<metod.Arity;i++)
					{
						if(expression.arguments[i].dataType!=metod.Params[i])
						{
							errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the metod "+ expression.name.Value+ " not contains in the argument number " + i+1+" a type "+expression.arguments[i].dataType.ToString()));
						}
					}
				}
				else
				{
					errors.Add(new CompilingError(expression.name.Location, ErrorCode.Invalid, "the metod "+ expression.name.Value+ " not contains " + metod.Arity+" arguments"));
				}
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
			HashSet<Token> Params = new HashSet<Token>(actual_effect.Params.Keys);
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
				StartScope();
				foreach (KeyValuePair<Token, Expression> keyValuePair in expression.arguments)
        		{
					Analizer(keyValuePair.Value);
					Token token1;
					foreach(Token token in actual_effect.Params.Keys)
					{
						if(token.Value==keyValuePair.Key.Value)
						{
							token1=token;
							if(actual_effect.Params[token1]==DataType.Null)
							{
								break;
							}
							if(keyValuePair.Value.dataType!=actual_effect.Params[token1])
							{
								errors.Add(new CompilingError(keyValuePair.Key.Location, ErrorCode.Invalid, "the argument " + keyValuePair.Key.Value + " must be has type "+actual_effect.Params[token1].ToString()));
							}
							break;
						}
					}
            		
				}
				EndScope();
			}
		}
		else
		{
			errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The effect " + expression.effect_name + " does not exist in the current context"));
		}
	}
	public void Visit_Variable(Variable expression)
	{
		for(int i=DataScope.Count-1;i>=0;i--)
		{
			if(DataScope.ElementAt(i).ContainsKey(expression.name))
			{
				expression.dataType=DataScope.ElementAt(i)[expression.name];
				return;
			}
		}
		errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The variable " + expression.name + " does not exist in the current context"));
	}
	public void Visit_Assign(AssignExpression expression)
	{
		Analizer(expression.Right);
		DataType ValueDatatype=expression.Right.dataType;
		for(int i=DataScope.Count-1;i>=0;i--)
		{
			if(DataScope.ElementAt(i).ContainsKey(expression.variable.name))
			{
				if(!(DataScope.ElementAt(i)[expression.variable.name]==ValueDatatype))
				{
					errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The variable " + expression.variable.name + " does not assign a type "+ValueDatatype.ToString()+" because has initialized type "+DataScope.ElementAt(i)[expression.variable.name].ToString()));
					return;
				}
			}
		}
		DataScope.Peek()[expression.variable.name]=ValueDatatype;
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
				DataScope.Peek()["unit"]=DataType.Card;
				break;
			}
			case ParamsPredicate.boost:
			{
				DataScope.Peek()["boost"]=DataType.Card;
				break;
			}
			case ParamsPredicate.weather:
			{
				DataScope.Peek()["weather"]=DataType.Card;
				break;
			}
			case ParamsPredicate.card:
			{
				DataScope.Peek()["card"]=DataType.Card;
				break;
			}
		}
		Analizer(expression.condition);
		DataType value=expression.condition.dataType;
		if(value!=DataType.Bool)
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
			if (expression.Effects != null)
			{
				foreach(CallEffect callEffect in expression.Effects)
				{
					Analizer(callEffect);
				}
			}
		}
	}
	public void Visit_Effect(Effect expression)
	{
		StartScope();
		foreach(KeyValuePair<Token,DataType> param in expression.Params)
		{
			DataScope.Peek().Add(param.Key.Value,param.Value);
		}
		foreach (Stmt stmt in expression.body)
		{
			Analizer(stmt);
		}
		EndScope();
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
		StartScope();
		DataType temp=declaration.condition.dataType;
		if(temp!=DataType.Bool)
		{
			errors.Add(new CompilingError(declaration.condition.Location, ErrorCode.Expected, "expected bool Data type in condition"));
		}
		foreach (Stmt stmt in declaration.body)
		{
			Analizer(stmt);
		}
		EndScope();
	}
	public void Visit_ForDeclaration(ForStmt declaration)
	{
		StartScope();
		DataScope.Peek().Add(declaration.identifier.Value,DataType.Card);
		DataType? temp=Seach(declaration.ienumerable.Value);
		if(temp==null || temp!=DataType.PackOfCards)
		{
			errors.Add(new CompilingError(declaration.ienumerable.Location, ErrorCode.Expected, "expected IEnumerable Data type in for declaration"));
		}
		foreach (Stmt stmt in declaration.body)
		{
			Analizer(stmt);
		}
		EndScope();
	}
	public void Visit_DeclarationEmpty(EmptyStmt declaration)
	{
	}
	#endregion
	#region Internal Metods
	DataType ReturnClass(Expression Expression)
	{
		if (Expression.GetType() == typeof(GetExpression))
		{
			GetExpression getExpression = (GetExpression)Expression;
			DataType gwentClass = ReturnClass(getExpression.callee);
			try
			{
				gwentClass = DatatypeProperties[gwentClass][getExpression.name.Value];
			}
			catch
			{
				errors.Add(new CompilingError(getExpression.name.Location, ErrorCode.Invalid, "the class "+ gwentClass.ToString()+ " not contains the property" + getExpression.name.Value));
			}
			return gwentClass;
		}
		if (Expression.GetType() == typeof(MetodExpression))
		{
			MetodExpression metodExpression = (MetodExpression)Expression;
			DataType gwentClass = ReturnClass(metodExpression.callee);
			Metod? metod=null;
			try
			{
				metod=DatatypeMetods[gwentClass][metodExpression.name.Value];
				gwentClass = metod.ReturnType;
			}
			catch
			{
				errors.Add(new CompilingError(metodExpression.name.Location, ErrorCode.Invalid, "the class "+ gwentClass.ToString()+ " not contains the metod" + metodExpression.name.Value));
			}
			if (metod != null)
			{
				if(metod.Arity==metodExpression.arguments.Count)
				{
					for(int i=0;i<metod.Arity;i++)
					{
						if(metodExpression.arguments[i].dataType!=metod.Params[i])
						{
							errors.Add(new CompilingError(metodExpression.name.Location, ErrorCode.Invalid, "the metod "+ metodExpression.name.Value+ " not contains in the " + i+1+" argument a type "+metod.Params[i].ToString()));
						}
					}
				}
				else
				{
					errors.Add(new CompilingError(metodExpression.name.Location, ErrorCode.Invalid, "the metod "+ metodExpression.name.Value+ " not contains " + metod.Arity+" arguments"));
				}
			}
			return gwentClass;
		}
		else if (Expression.GetType() == typeof(Variable))
		{
			Variable variable = (Variable)Expression;
			string name=variable.name;
			DataType? dataType=Seach(name);
			if(dataType!=null)
			{
				return (DataType)dataType;
			}
			errors.Add(new CompilingError(variable.Location, ErrorCode.Invalid, "the variable "+ variable.name+" not exist in the current context"));
			throw new Exception();
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
	bool CheckDataType(Expression expression,params DataType[] dataTypes)
	{
		foreach(DataType dataType in dataTypes)
		{
			if(expression.dataType==dataType)
			{
				return true;
			}
		}
		return false;
	}
	DataType? Seach(string identifier)
	{
		for(int i=DataScope.Count-1;i>=0;i--)
		{
			if(DataScope.ElementAt(i).ContainsKey(identifier))
			{
				return DataScope.ElementAt(i)[identifier];
			}
		}
		return null;
	}
	void StartScope()
	{
		DataScope.Push(new Dictionary<string, DataType>());
	}
	void EndScope()
	{
		DataScope.Pop();
	}
	public void Semantic_Analizer()
	{
		Analizer(ElementalProgram);
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