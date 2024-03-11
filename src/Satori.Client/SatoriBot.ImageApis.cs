using Satori.Protocol.Models;

namespace Satori.Client;

public partial class SatoriBot
{
    public Task<Image> GetImageAsync(string imageHash)
    {
        var bytes = GetFileAsync("/v1/assets/" + imageHash);
        return Task.FromResult(new Image
        {
            Hash = imageHash,
            Binrary = bytes.Result
        });
    }

}