using Satori.Client;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

Console.Write("Input Token: ");
var token = Console.ReadLine()?.Trim() ?? "";

Console.Write("Input Platform: ");
var platform = Console.ReadLine()?.Trim() ?? "";

Console.Write("Input SelfId: ");
var selfId = Console.ReadLine()?.Trim() ?? "";

using var client = new SatoriClient("http://localhost:5500", token);

client.Logging += (_, log) => { Console.WriteLine($"[{log.LogTime}] [{log.LogLevel}] {log.Message}"); };

var bot = client.Bot(platform, selfId);

await client.StartAsync();

var http = new HttpClient();

bot.EventReceived += async (_, e) =>
{
    Console.WriteLine($"----------------------------------------------------");
    var name = e.User!.Name;
    name += e.Member != null ? "|" + e.Member!.Nick : "";
    var guild = e.Guild != null ? e.Guild!.Name : "0";
    Console.WriteLine($"[{guild}] - [{name}] : {e.Message!.Content}");
    Console.WriteLine($"----------------------------------------------------");
    var content = e.Message!.Content;
    if (content == null) return;

    // check images/{guild}/ is not esxist, create it
    var dir = "images/" + guild;
    if (!Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }

    // <img src="(.*)"/>
    var match = Regex.Match(content, @"<img src="".*/v1/assets/(.*)""/>");
    if (match.Success)
    {
        var hash = match.Groups[1].Value;
        var response = await bot.GetImageAsync(hash);
        if (response.Binrary == null)
        {
            Console.WriteLine($"Image {hash} not found");
            return;
        }

        // get image file header and get the file type
        var header = response.Binrary.Take(8).ToArray();
        var fileType = ".png";
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
        {
            fileType = ".png";
        }
        else if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            fileType = ".jpg";
        }
        else if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38)
        {
            fileType = ".gif";
        }
        else if (header[0] == 0x42 && header[1] == 0x4D)
        {
            fileType = ".bmp";
        }
        else
        {
            fileType = ".png";
        }

        var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(response.Binrary);
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var path = dir + "/" + hashString + fileType;
        await using var fileStream = new FileStream(path, FileMode.Create);
        fileStream.Write(response.Binrary);



        Console.WriteLine($"Downloaded {hash} to {path}");

        //await bot.SendImageAsync(e.Guild!.Id, e.Channel!.Id, bytes);
    }
    else
    {
        //await bot.SendMessageAsync(e.Guild!.Id, e.Channel!.Id, content);
    }
};

await Task.Delay(-1);