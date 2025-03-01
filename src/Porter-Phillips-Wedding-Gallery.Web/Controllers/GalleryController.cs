using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Porter_Phillips_Wedding_Gallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly string _imageFolder = "wwwroot/wedding-images/";
        private readonly string _thumbnailFolder = "wwwroot/wedding-images/thumbnails/";
        private const int ImagesPerPage = 25;
        private const int ThumbnailWidth = 300; // Adjust thumbnail size for better performance

        [HttpGet]
        [Route("api/gallery")]
        public IActionResult GetImages(int page = 1)
        {
            try
            {
                if (!Directory.Exists(_imageFolder))
                    return NotFound("Image folder not found.");

                if (!Directory.Exists(_thumbnailFolder))
                    Directory.CreateDirectory(_thumbnailFolder);

                var files = Directory.GetFiles(_imageFolder).OrderBy(x => x)
                                    .Select(Path.GetFileName)
                                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                var paginatedImages = files.Skip((page - 1) * ImagesPerPage).Take(ImagesPerPage).ToList();

                var imagesWithThumbnails = paginatedImages.Select(image =>
                {
                    string fullPath = Path.Combine(_imageFolder, image);
                    string thumbnailPath = Path.Combine(_thumbnailFolder, image);

                    if (!System.IO.File.Exists(thumbnailPath))
                        GenerateThumbnail(fullPath, thumbnailPath, ThumbnailWidth);

                    return new { full = $"/wedding-images/{image}", thumbnail = $"/wedding-images/thumbnails/{image}" };
                }).ToList();

                return Json(imagesWithThumbnails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving images: {ex.Message}");
            }
        }

        private void GenerateThumbnail(string imagePath, string thumbnailPath, int width)
        {
            using (var image = Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(width, 0)
                }));

                using (var outputStream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    image.Save(outputStream, new JpegEncoder());
                }
            }
        }
    }
}
