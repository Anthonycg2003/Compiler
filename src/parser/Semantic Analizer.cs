
using System.Reflection.Metadata;

public class SemanticAnalizer
{
    public List<CompilingError>errors;
    Interpreter interpreter;
    ElementalProgram ElementalProgram;
    public Stack<List<string>> contexts;
    static readonly string[] ranges={"\"Melee\"","\"Range\"","\"Siege\""};
    public SemanticAnalizer(ElementalProgram elementalProgram)
    {
        interpreter=new Interpreter(elementalProgram);
        errors=new List<CompilingError>();
        contexts=new Stack<List<string>>();
        ElementalProgram=elementalProgram;
    }
    #region Expressions
    public void Visit_Atom(Atom expression)
    {
    }
    public void Visit_Binary(BinaryExpression expression)
    {
        Analizer(expression.Left);
        Analizer(expression.Right);
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
       /*try
        {
            interpreter.Evaluate(expression);
        }
        catch(InvalidCastException)
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," only can access property in classes"));
        }
        catch(Exception except)
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," not defined "+except.Message));
        }*/
    }
    public void Visit_Set(SetExpression expression)
    {
    }
    public void Visit_Metod(MetodExpression expression)
    {
        /*try
        {
            object Class=interpreter.Evaluate(expression.calleer);
            Funtion funtion=((GwentClass)Class).FindMetod(expression.name.Value);
            if(funtion.Arity!=expression.arguments.Count)
            {
                errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," the call to metod "+funtion.name+" should have "+funtion.Arity+" arguments"));
            }
        }
        catch(InvalidCastException)
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," only can access metods in classes"));
        }
        catch(Exception except)
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid," not defined "+except.Message));
        }*/
    }
    public void Visit_CallEffect(CallEffect expression)
    {
        if(ElementalProgram.Effects.Keys.Contains(expression.effect_name))
        {
            Effect actual_effect=ElementalProgram.Effects[expression.effect_name];
            HashSet<Token> Params=new HashSet<Token>(actual_effect.Params);
            HashSet<Token> Args=new HashSet<Token>(expression.arguments.Keys);
            if(!Params.SetEquals(Args))
            {
                if(Args.IsSubsetOf(Params))
                {
                    foreach(Token token in Params)
                    {
                        if(!Args.Contains(token))
                        {
                            errors.Add(new CompilingError(token.Location,ErrorCode.Expected,"expected "+token.Value+" param in arguments"));
                        }
                    }
                    
                }
                else
                {
                    foreach(Token token in Args)
                    {
                        if(!Params.Contains(token))
                        {
                            errors.Add(new CompilingError(token.Location,ErrorCode.Invalid,"the argument "+token.Value+" does not exist as param"));
                        }
                    }
                }
            }
        }
        else
        {
            errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"The effect "+expression.effect_name+" does not exist in the current context"));
        }
    }
    public void Visit_Variable(Variable expression)
    {
        for(int i=contexts.Count-1;i>=0;i--)
        {
            if(contexts.ElementAt(i).Contains(expression.name))
            {
                interpreter.locals[expression]=contexts.Count-1-i;
                return;
            }
        }
        errors.Add(new CompilingError(expression.Location,ErrorCode.Invalid,"The variable "+expression.name+" does not exist in the current context"));
    }
    public void Visit_Assign(AssignExpression expression)
    {
        Analizer(expression.Right);
        for(int i=contexts.Count-1;i>=0;i--)
        {
            if(contexts.ElementAt(i).Contains(expression.variable.name))
            {
                interpreter.locals[expression]=contexts.Count-1-i;
                return;
            }
        }
        contexts.Peek().Add(expression.variable.name);
    }
    
    #endregion
    #region Declarations
    public void Visit_Card(Card expression)
    {
        StartScope();
        if(expression.CheckPorperties(errors))
        {
            if(expression.Ranges!=null)
            {
                foreach (string Range in expression.Ranges)
                {
                    if (!ranges.Contains(Range))
                    {
                        errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, String.Format("{0} Range Does not exists", Range)));
                    }
                    if (contexts.Peek().Contains(Range))
                    {
                        errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, String.Format("{0} Range already in use", Range)));
                    }
                    else
                    {
                        contexts.Peek().Add(Range);
                    }
                }
            }
            if(expression.Effect!=null)
            {
                Analizer(expression.Effect);
            }
        }
        EndScope();
    }
    public void Visit_Effect(Effect expression)
    {
        contexts.Peek().Add(expression.Name);
        StartScope();
        foreach(Token Param in expression.Params)
        {
            contexts.Peek().Add(Param.Value);
        }
        foreach(Stmt stmt in expression.body)
        {
            Analizer(stmt);
        }
        EndScope();
    }
    public void Visit_Program(ElementalProgram program)
    {
        StartScope();
        foreach (Effect effect in program.Effects.Values)
        {
            Analizer(effect);
        }
        foreach (Card card in program.Cards.Values)
        {
            Analizer(card);
        }
        EndScope();
    }
    public void Visit_DeclarationExpression(ExpressionStmt declaration)
    {
        Analizer(declaration.expression);
    }
    public void Visit_WhileDeclaration(WhileStmt declaration)
    {
        StartScope();
        Analizer(declaration.condition);
        foreach(Stmt stmt in declaration.body)
        {
            Analizer(stmt);
        }
        EndScope();
    }
    public void Visit_DeclarationEmpty(EmptyStmt declaration)
    {
    }
    #endregion
    void Analizer(ASTNode aSTNode)
    {
        aSTNode.CheckSemantic(this);
    }
    void StartScope()
    {
        contexts.Push(new List<string>());
    }
    void EndScope()
    {
        contexts.Pop();
    }
    public Interpreter Semantic_Analizer()
    {
        Analizer(ElementalProgram);
        return interpreter;
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