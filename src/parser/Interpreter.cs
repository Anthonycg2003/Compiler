
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public class Interpreter:IVisitorExpression,IVisitorDeclaration
{
    public Scope Scope;
    public List<CompilingError>errors;
    public Dictionary<Expression,int> locals;
    ElementalProgram Program;
    public Interpreter(ElementalProgram elementalProgram)
    {
        Program=elementalProgram;
        Scope=new Scope();
        errors=new List<CompilingError>();
        locals=new Dictionary<Expression, int>();
    }
    #region Expressions
    public Object Visit_Atom(Atom expression)
    {
        return expression.Value;  
    }
    public object Visit_Binary(BinaryExpression expression)
    {
        object left=Evaluate(expression.Left);
        object right=Evaluate(expression.Right);
        switch(expression.Operator.Type)
        {
            case TokenType.MINUS:
            {
                try
                {
                    return (double)left - (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.PLUS:
            {
                try
                {
                    return (double)left + (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.STAR:
            {
                try
                {
                    return (double)left * (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.SLASH:
            {
                try
                {
                    return (double)left / (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.GREATER:
            {
                try
                {
                    return (double)left > (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.GREATER_EQUAL:
            {
                try
                {
                    return (double)left >= (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.LESS:
            {
                try
                {
                    return (double)left < (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.LESS_EQUAL:
            {
                try
                {
                    return (double)left <= (double)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.EQUAL_EQUAL:
            {
                try
                {
                    return left.Equals(right);
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be of the same type"));
                    break;
                } 
            }
            case TokenType.Caret:
            {
                try
                {
                    return Math.Pow((double)left,(double)right);
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be numbers"));
                    break;
                } 
            }
            case TokenType.AND:
            {
                try
                {
                    return (bool)left && (bool)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be booleans"));
                    break;
                } 
            }
            case TokenType.OR:
            {
                try
                {
                    return (bool)left || (bool)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be booleans"));
                    break;
                } 
            }
            case TokenType.ARROBA:
            {
                try
                {
                    return (string)left + (string)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be strings"));
                    break;
                } 
            }
            case TokenType.ARROBA_ARROBA:
            {
                try
                {
                    return (string)left +" "+ (string)right;
                }
                catch
                {
                    errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"Both operands must be strings"));
                    break;
                } 
            }
        }
        return null;
    }
    public object Visit_Group(Group expression)
    {
       return Evaluate(expression.expression);
    }
    public object Visit_Unary(Unary expression)
    {
        object left=Evaluate(expression.rigth_expression);
        return -(double)left;
    }
    public object Visit_Get(GetExpression expression)
    {
        object Class=Evaluate(expression.callee);
        return ((GwentClass)Class).Get(expression.name.Value);
    }
    public object Visit_Set(SetExpression expression)
    {
        object Class=Evaluate(expression.callee);
        object value=Evaluate(expression.value);
        try
        {
            ((GwentClass)Class).Properties[expression.name.Value]=value;
        }
        catch(InvalidCastException)
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," only should set values in classes "));
        }
        catch
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"the property "+expression.name+" is not defined"));
        }
        return value;
    }
    public object? Visit_Metod(MetodExpression expression)
    {
        object Class=Evaluate(expression.calleer);
        Funtion funtion=((GwentClass)Class).FindMetod(expression.name.Value);
        try
        {
            object[] arguments=new object[funtion.Arity];
            int count=0;
            foreach(Expression expr in expression.arguments)
            {
                arguments[count]=Evaluate(expr);
                count++;
            }
            return funtion.Delegate.Method.Invoke(null,arguments);
        }
        catch
        {
            ParameterInfo[] ParamsType=funtion.Delegate.Method.GetParameters();
            string s="";
            foreach(ParameterInfo parameterInfo in ParamsType)
            {
                s+=parameterInfo.DefaultValue+" ";
            }
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"the metod "+funtion.name+" should have a argument type "+s));
            return null;
        }
    }
    public void Visit_CallEffect(CallEffect expression)
    {
        Scope last=this.Scope;
        this.Scope=last.CreateChild();
        Effect calleer=Program.Effects[expression.effect_name];
        foreach(KeyValuePair<Token,Expression> keyValuePair in expression.arguments)
        {
            Scope.Variables[keyValuePair.Key.Value]=Evaluate(keyValuePair.Value);
        }
        foreach(Stmt stmt in calleer.body)
        {
            Execute(stmt);
        }
        this.Scope=last;
    }
    public object Visit_Variable(Variable expression)
    {
        try
        {
            return Scope.GetValue(expression.name);
        }
        catch
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"The variable "+expression.name+" does not exist in the current context"));
            return null;
        }
    }
    public object Visit_Assign(AssignExpression expression)
    {
        object value=Evaluate(expression.Right);
        Scope.Variables[expression.variable.name]=value;
        return value;
    }
    
    #endregion
    #region Declarations
    public void Visit_Card(Card expression)
    {
    }
    public void Visit_Effect(Effect expression)
    {
    }
    public void Visit_Program(ElementalProgram program)
    {
    }
    public void Visit_DeclarationExpression(ExpressionStmt declaration)
    {
        Evaluate(declaration.expression);
    }
    public void Visit_WhileDeclaration(WhileStmt declaration)
    {
        Scope last=this.Scope;
        this.Scope=last.CreateChild();
        object value_condition=Evaluate(declaration.condition);
        if(value_condition.GetType()==typeof(bool))
        {
            while((bool)value_condition)
            {
                ExecuteBlock(declaration.body);
            } 
        }
        else
        {
            errors.Add(new CompilingError(declaration.condition.Location,ErrorCode.Invalid,"The condition of while statement must be bool"));
        }
        this.Scope=last;
    }
    public void Visit_DeclarationEmpty(EmptyStmt declaration)
    {
    }
    #endregion
    public object Evaluate(Expression expression)
    {
        object s=expression.Accept(this);
        return s;
    }
    void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
    public void ExecuteBlock(List<Stmt> body)
    {
        foreach(Stmt stmt in body)
        {
            Execute(stmt);
        }
    }
    public void Interpret(ElementalProgram elementalProgram)
    {
        foreach(Card card in elementalProgram.Cards.Values)
        {
            if(card.Effect!=null)
            {
                Execute(card.Effect);
            }
            
        }
    }
    public void Print_errors()
    {
        if(errors.Count==0)
        {
            return;
        }
        foreach(CompilingError error in errors)
        {
            Console.WriteLine(error.Code+"=> "+error.Argument+" at line "+error.location.line+" and column "+error.location.column);
        }
    }
}
