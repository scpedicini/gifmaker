using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifCreator
{
    public class Gif
    {
        public List<GifImage> GifImages { get; set; }

        /// <summary>
        /// In milliseconds
        /// </summary>
        public int FrameDelay { get; set; }

        public Gif()
        {
            // default
            FrameDelay = 200;
            GifImages = new List<GifImage>();
        }

        public Gif(int frameDelay)
        {
            FrameDelay = frameDelay;
        }

        //public bool AddImage(string url)
        //{
        //    if (!GifImages.Any(img => img.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        GifImages.Add(GifImage.DownloadFrame(url));
        //        return true;
        //    }

        //    return false;
        //}

        public GifImage AddFrame(string name, Bitmap bmp)
        {
            var gifimg = new GifImage(name, bmp);
            GifImages.Add(gifimg);
            return gifimg;
        }

        public void RemoveImage(string url)
        {
            var removedImages = GifImages.Where(img => img.Url.Equals(url, StringComparison.OrdinalIgnoreCase));

            foreach (var img in removedImages)
            {
                GifImages.Remove(img);
            }
        }

        public void MakeImageSizesUniform()
        {
            var uniformSize = GetUniformSize();

            GifImages.ForEach(img =>
            {
                Bitmap bmp = new Bitmap(img.Frame, uniformSize);
                img.Frame = bmp;
            });
        }

        public Size GetUniformSize()
        {
            var avgWidth = (int)(((float)GifImages.Sum(img => img.Frame.Width)) / GifImages.Count);
            var avgHeight = (int)(((float)GifImages.Sum(img => img.Frame.Height)) / GifImages.Count);
            return new Size(avgWidth, avgHeight);
        }

        public void Export(string fileName, bool uniformSize)
        {
            if (GifImages.Count == 0) return;

            var size = GetUniformSize();

            using (MagickImageCollection collection = new MagickImageCollection())
            {
                // Add first image and set the animation delay to 100ms
                for (int i = 0; i < GifImages.Count; i++)
                {
                    var magicImage = new MagickImage(uniformSize ? new Bitmap(GifImages[i].Frame, size) : GifImages[i].Frame);
                    collection.Add(magicImage);
                    collection[i].AnimationDelay = Convert.ToInt32(FrameDelay * 0.1);
                }

                // Optionally reduce colors
                //QuantizeSettings settings = new QuantizeSettings();
                //settings.Colors = 256;
                //collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                collection.Optimize();

                // Save gif
                collection.Write(fileName);
            }

        }

    }

    public class GifImage
    {
        public Bitmap Frame { get; set; }
        public string Url { get; set; }

        public GifImage(string url, Bitmap frame)
        {
            Frame = frame;
            Url = url;
        }

        //public static GifImage DownloadFrame(string url)
        //{

        //}
    }
}
