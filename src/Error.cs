public class CompilingError
{
    public CompilingError(CodeLocation location, ErrorCode code, string argument)
    {
        this.Code = code;
        this.Argument = argument;
        this.location=location;
    }

    public ErrorCode Code { get; }

    public string Argument { get; }
    public CodeLocation location{get;}
}
public enum ErrorCode
{
    None,
    Expected,
    Invalid,
    Unknown,
}
