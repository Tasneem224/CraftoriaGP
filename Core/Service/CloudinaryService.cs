using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deleteParams); // sync version
            return result.Result; // "ok" لو تم الحذف
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var ext = System.IO.Path.GetExtension(file.FileName).ToLower();

            RawUploadParams uploadParams;

            if (ext == ".pdf")
            {
                uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true,
                    Folder = "portfolio",       // اختياري
                    AccessMode = "public"       // 👈 مهم جدًا
                };
            }
            else
            {
                uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true,
                    Folder = "images",
                    AccessMode = "public"       // 👈 مهم جدًا
                };
            }

            var result = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return result.SecureUrl.ToString();
        }

    }

}
