using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AsliHalwaiWebApp.Helpers
{
    public static class FileUploadHelper
    {
        public static bool UploadPhoto(
            IFormFile file,
            string folderPath,
            string fileName,
            IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                return false;

            try
            {
                string uploadFolder =
                    Path.Combine(env.WebRootPath, folderPath);

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                string fullPath =
                    Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
