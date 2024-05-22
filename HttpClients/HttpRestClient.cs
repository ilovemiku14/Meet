using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Meet.HttpClients;

internal class HttpRestClient
{
    private readonly RestClient _client;

    public HttpRestClient()
    {
        _client = new RestClient();
    }

    public ApiResponse Excute(ApiRequest apiRequest)
    {
        var request = new RestRequest(apiRequest.method);
        request.AddHeader("Content-Type", "application/json"); // 设置 Content-Type 为 application/json
        if (apiRequest.Parameters != null)
            request.AddParameter("param", JsonConvert.SerializeObject(apiRequest.Parameters), ParameterType.RequestBody);
        _client.BaseUrl = new Uri("http://localhost:5000/" + apiRequest.route);
        var response = _client.Execute(request);
        if (response.StatusCode == HttpStatusCode.OK)
            return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
        // return new ApiResponse(-99, "诶呀！发生了错误！", new object());
        return new ApiResponse(-99, "诶呀！发生了错误！", response);
    }
}