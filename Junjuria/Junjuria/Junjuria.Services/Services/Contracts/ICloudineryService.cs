namespace Junjuria.Services.Services.Contracts
{
    public interface ICloudineryService
    {
        string RelocateImgToCloudinary(string name, string imgPath, string info, bool isUrl = true);
    }
}
