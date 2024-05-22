namespace Meet.HttpClients;

internal class ApiResponse
{
    public ApiResponse(int resultCode, string msg, object resultData)
    {
        ResultCode = resultCode;
        Msg = msg;
        ResultData = resultData;
    }

    public ApiResponse()
    {
    }

    public int ResultCode { get; set; }
    public string Msg { get; set; }
    public object ResultData { get; set; }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}