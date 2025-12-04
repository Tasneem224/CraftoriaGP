using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ICloudinaryService
    {
        public Task<string> UploadAsync(IFormFile file);
        public string DeleteAsync(string publicId);
    }
}
