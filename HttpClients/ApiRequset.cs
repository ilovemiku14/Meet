using RestSharp;

namespace Meet.HttpClients;

internal class ApiRequest
{
    public ApiRequest(Method method)
    {
        this.method = method;
    }

    public ApiRequest()
    {
    }

    public ApiRequest(string route, Method method, object parameters, string contentType)
    {
        this.route = route;
        this.method = method;
        Parameters = parameters;
        ContentType = contentType;
    }

    //路由地址
    public string route { get; set; }

    //请求方式post，get，put，delete
    public Method method { get; set; }

    //参数
    public object Parameters { get; set; }

    //请求头默认为json
    public string ContentType { get; set; } = "application/json";
}