using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost("UploadFile")]
        public async Task<ActionResult<object>> UploadFile(IFormFile file)
        {
            var size = await SaveFile(file);
            return Ok(new
            {
                Size = size
            });
        }

        [HttpPost,Route("UploadFiles")]
        public async Task<ActionResult<List<ResponseModel>>> UploadFiles(List<IFormFile> files)
        {
            var response = await SaveFiles(files);
            if (response is null)
            {
                return BadRequest();
            }

            return Ok(response);
        }


        private async Task<List<ResponseModel>> SaveFiles(List<IFormFile> files)
        {
            var responseModels = new List<ResponseModel>();

            if (files != null)
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var size = await SaveFile(files[i]);
                    responseModels.Add(new ResponseModel
                    {
                        FileSequenceId = i,
                        Size = size,
                    });
                }
                return responseModels;
            }

            return null;
        }

        private async Task<long> SaveFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);
                await using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);
                return stream.Length;
            }

            return 0;
        }


        public class ResponseModel
        {
            public int FileSequenceId { get; set; }
            public long Size { get; set; }
        }
    }
}
