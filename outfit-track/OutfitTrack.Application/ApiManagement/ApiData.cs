using System.Collections.Concurrent;

namespace OutfitTrack.Application.ApiManagement;

public static class ApiData
{
    public static ConcurrentDictionary<string, ApiDataRequest> ListApiDataRequest { get; private set; } = new ConcurrentDictionary<string, ApiDataRequest>();

    public static Guid CreateApiDataRequest()
    {
        var newApiDataRequest = new ApiDataRequest();
        AddItem(newApiDataRequest);
        return newApiDataRequest.GuidApiDataRequest;
    }

    public static void AddItem(ApiDataRequest item)
    {
        ListApiDataRequest.TryAdd(item.GuidApiDataRequest.ToString(), item);
    }

    public static ApiDataRequest GetApiDataRequest(Guid apiDataRequestId)
    {
        return ListApiDataRequest[apiDataRequestId.ToString()];
    }

    public static void RemoveApiDataRequest(Guid guidBeesApiDataRequest)
    {
        ListApiDataRequest.TryRemove(guidBeesApiDataRequest.ToString(), out var item);
    }
}

public class ApiDataRequest
{
    public ApiDataRequest()
    {
        GuidApiDataRequest = Guid.NewGuid();
    }

    public Guid GuidApiDataRequest { get; private set; }
}