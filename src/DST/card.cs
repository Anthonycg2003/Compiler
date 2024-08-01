using System.Data;
using System.Reflection.Metadata.Ecma335;
public class Card:Stmt,GwentClass
{
    public Card(CodeLocation location,string Name,string Faction,Token Type,List<string>? Ranges,int? Power,CallEffect? funtion):base(location)
    {
        this.Name=Name;
        Effect=funtion;
        this.Power=Power;
        this.Ranges=Ranges;
        this.Faction=Faction;
        this.Type=Type;
        Properties=new Dictionary<string, object?>
        {
            {"Name",Name},{"Faction",Faction},{"Type",Type.Value},{"Power",Power},{"Ranges",Ranges}
        };
        Metods=new Dictionary<string, Funtion>();
    }
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}
    public string name{get{return "card";}}
    public string Name { get; set; }
    public string Faction { get; set; }
    public Token Type { get; set; }
    public List<string>? Ranges{get;set;}
    public int? Power { get; set; }
    public CallEffect? Effect;
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_Card(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Card(this);
    }
    public bool CheckPorperties(List<CompilingError> errors)
    {
            if(this.Type.Value=="\"Plata\""||this.Type.Value=="\"Oro\"")
            {
                if(this.Power==null||this.Ranges==null)
                {
                    errors.Add(new CompilingError(Type.Location, ErrorCode.Invalid,"gold and silver cards should have range and power properties"));
                    return false;
                }
                return true;  
            }
            else if(this.Type.Value=="\"Lider\""||this.Type.Value=="\"Aumento\""||this.Type.Value=="\"Clima\"")
            {
                return true;
            }
            else
            {
                errors.Add(new CompilingError(Type.Location, ErrorCode.Invalid, String.Format("{0} Type Does not exists",Type)));
                return true;
            }
        
    }
}
public class CallEffect:Stmt//llamada a funciones
{
    public string effect_name;
    public Dictionary<Token,Expression> arguments;
    public CallEffect(CodeLocation location,Dictionary<Token,Expression> arguments,string calleer) : base(location)
    {
        effect_name=calleer;
        this.arguments=arguments;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_CallEffect(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_CallEffect(this);
    }
}
