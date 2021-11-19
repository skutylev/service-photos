using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Service.PhotoPackages.ServiceLayer.Utils
{
    internal class PhotoConverterService : IPhotoConverterService
    {
        private readonly Size _size = new Size(400, 400);

        public byte[] ConvertToThumbnail(byte[] photoBytes)
        {
            if (photoBytes == null) throw new ArgumentNullException(nameof(photoBytes));

            using var input = new MemoryStream(photoBytes);
            using var inputStream = new SKManagedStream(input);
            using var original = SKBitmap.Decode(inputStream);
            int width, height;
            if (original.Width > original.Height)
            {
                width = _size.Width;
                height = original.Height * _size.Width / original.Width;
            }
            else
            {
                width = original.Width * _size.Height / original.Height;
                height = _size.Height;
            }

            using var resized = original
                .Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            
            if (resized == null) throw new ArgumentNullException(nameof(photoBytes));

            using var image = SKImage.FromBitmap(resized);
            return image.Encode(SKEncodedImageFormat.Jpeg, 75).ToArray();
        }
    }
}