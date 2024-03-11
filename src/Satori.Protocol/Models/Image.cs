namespace Satori.Protocol.Models;

/// <summary>
/// 图像
/// </summary>
public class Image
{
    /// <summary>
    /// 图像 ID
    /// </summary>
    public string Hash { get; set; } = "";

    /// <summary>
    /// 图像文件内容
    /// </summary>
    public byte[]? Binrary { get; set; }
}