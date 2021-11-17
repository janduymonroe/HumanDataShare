using HumanDataShare.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace HumanDataShare.Controllers
{
    [ApiController]
    public class HumanDataShareController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public HumanDataShareController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        [HttpPost]
        [Route("upload")]
        public ActionResult PostImage([FromForm] ImageUpload objImage)
        {
            try
            {
                if (objImage.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\upload\\" + objImage.files.FileName))
                    {
                        objImage.files.CopyTo(fileStream);
                        fileStream.Flush();
                        return Ok("\\upload\\" + objImage.files.FileName);
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost(), Route("write-message-on-image")]
        public async Task<ActionResult<string>> EncodigImage([FromBody]JsonObject data)
        {
            try
            {
                Bitmap encImage;
                string fileName = data["fileName"].ToString();
                Bitmap bitmapImg = new Bitmap(_environment.WebRootPath + data["filePath"].ToString());
                encImage = Steganography.EmbedText(data["message"].ToString(), bitmapImg);

                encImage.Save(_environment.WebRootPath + "\\upload\\" + $"{fileName}.bmp", ImageFormat.Bmp);

                return Ok($"{fileName}.bmp");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
           
        }

        [HttpGet(), Route("get-image")]
        public async Task<ActionResult> GetNewImage([FromQuery] string fileName)
        {

            try
            {
                var filepath = _environment.WebRootPath + "\\upload\\" + fileName + ".bmp";

                var fs = new FileStream(filepath, FileMode.Open);

                return File(fs, "application/octet-stream", $"{fileName}.bmp");
            }
            catch (Exception)
            {

                return NotFound();
            }

        }

        [HttpGet(), Route("decode-message-from-image")]
        public async Task<ActionResult> DecodeMessage([FromQuery] string fileName)
        {

            var filePath = _environment.WebRootPath + "\\upload\\" + fileName + ".bmp";

            Bitmap encImage = new Bitmap(filePath);

            string decMsg = Steganography.ExtractText(encImage);

            return Ok(decMsg);

        }
    }
}
