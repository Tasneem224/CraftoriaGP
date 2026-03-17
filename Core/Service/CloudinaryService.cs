using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing; 
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {

            var cloudinaryUrl = config["Cloudinary:CloudinaryUrl"];
            //var cloudinaryUrl = builder.Configuration["Cloudinary:CloudinaryUrl"];
            //var cloudUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            _cloudinary = new Cloudinary(cloudinaryUrl);
            _cloudinary.Api.Secure = true;
        }
        public async Task<string> UploadFromUrlAsync(string url, string publicId)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(url), // كلاوديناري يدعم الروابط مباشرة
                PublicId = $"etsy_products/{publicId}",
                Overwrite = true,
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString(); // سيعيد رابط الصورة الجديد من Cloudinary
        }
        public string DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deleteParams); // sync version
            return result.Result; // "ok" لو تم الحذف
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var ext = Path.GetExtension(file.FileName).ToLower();
            bool isPdf = ext == ".pdf";

            string folderName = isPdf ? "portfolio" : "images";

            Stream streamToUpload = isPdf
                ? file.OpenReadStream()
                : await CompressImageAsync(file);

            using (streamToUpload)
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, streamToUpload),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true,
                    Folder = folderName,
                    AccessMode = "public"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                return result.SecureUrl.ToString();
            }
        }

        // دالة مساعدة (Helper Method) لضغط الصور فقط
        private async Task<Stream> CompressImageAsync(IFormFile file)
        {
            var outStream = new MemoryStream();
            using var image = await Image.LoadAsync(file.OpenReadStream());
            int maxWidth = 1080;
            if (image.Width > maxWidth)
            {
                int newHeight = (int)((double)image.Height / image.Width * maxWidth);
                image.Mutate(x => x.Resize(maxWidth, newHeight));
            }

            var encoder = new JpegEncoder { Quality = 75 };
            await image.SaveAsJpegAsync(outStream, encoder);

            outStream.Position = 0;
            return outStream;
        }
    }

}
