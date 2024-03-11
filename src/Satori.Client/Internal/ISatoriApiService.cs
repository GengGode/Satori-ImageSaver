namespace Satori.Client.Internal;

internal interface ISatoriApiService
{
    Task<TData> SendAsync<TData>(string endpoint, string platform, string selfId, object? body);
    Task<byte[]> GetFileAsync(string endpoint, string platform, string selfId);
}