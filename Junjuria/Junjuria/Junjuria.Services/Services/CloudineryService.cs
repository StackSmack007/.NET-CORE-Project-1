namespace Junjuria.Services.Services
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class CloudineryService : ICloudineryService
    {
        private IConfigurationRoot configuration;
        public CloudineryService()
        {

            configuration = new ConfigurationBuilder()
                                                     .SetBasePath(Directory.GetCurrentDirectory())
                                                     .AddJsonFile("services-settings.json", optional: false, reloadOnChange: true)
                                                     .Build();

            //configuration = new ConfigurationBuilder()
            //                             .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Junjuria.Services/Settings/"))
            //                             .AddJsonFile("services-settings.json", optional: false, reloadOnChange: true)
            //                             .Build();
        }

        public string RelocateImgToCloudinary(string name, string imgPath, string info, bool isUrl = true)
        {
            info = info ?? "no_info";
            var cloud = configuration["CloudinerySettings:CloudName"];
            var apiKey = configuration["CloudinerySettings:APIKey"];
            var apiSecret = configuration["CloudinerySettings:APISecret"];

            var myAccount = new Account(cloud, apiKey, apiSecret);
            var cloudinary = new Cloudinary(myAccount);
            ImageUploadParams parameters = new ImageUploadParams()
            {
                PublicId = info,
            };
            ImageUploadResult uploadResult;
            if (isUrl)
            {
                using (var stream = GetStreamFromUrl(imgPath))
                {
                    if (stream is null) return "image not found";
                    parameters.File = new FileDescription(name, stream);
                    uploadResult = cloudinary.Upload(parameters);
                }
            }
            else
            {
                parameters.File = new FileDescription(name, imgPath);
                uploadResult = cloudinary.Upload(parameters);
            }
            var result = uploadResult.SecureUri.AbsoluteUri;
            return result;
        }

        private MemoryStream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;
            MemoryStream ms;

            ms = null;
            try
            {
                using (var wc = new System.Net.WebClient())
                {
                    imageData = wc.DownloadData(url);
                }
                ms = new MemoryStream(imageData);
            }
            catch
            { }
            return ms;
        }
    }
}
