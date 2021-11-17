using Microsoft.AspNetCore.Http;

namespace HumanDataShare.Model
{
    public class ImageUpload
    {
        public IFormFile files { get; set; }
    }
}
