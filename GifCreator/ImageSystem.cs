using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GifCreator
{
    public static class ImageSystem
    {

        public static Bitmap OldDownloadAsBitmap(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ApplicationException(string.Format("Url: {0} is not a valid http format", url));
            Bitmap bmp;
            try
            {
                HttpWebRequest hr = WebRequest.CreateHttp(url);
                using (var stream = hr.GetResponse().GetResponseStream())
                using (var img = Image.FromStream(stream))
                {
                    bmp = new Bitmap(img);
                    img.Dispose();
                    stream.Dispose();
                }

                return bmp;
            }
            catch (WebException)
            {
                throw new ApplicationException(string.Format("Unable to download: {0}", url));
            }
        }

        /// <summary>
        /// Have to use this method in order to support animated GIF files.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap DownloadAsBitmap(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate( url , UriKind.Absolute, out uriResult) && 
                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
                throw new ApplicationException(string.Format("Url: {0} is not a valid http format", url));
            Bitmap bmp;
            url = uriResult.AbsoluteUri;

            try
            {
                HttpWebRequest hr = WebRequest.CreateHttp(url);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)";

                using (var stream = hr.GetResponse().GetResponseStream())
                {
                    bmp = Image.FromStream(stream) as Bitmap;
                }

                return bmp;
            }
            catch (WebException)
            {
                throw new ApplicationException(string.Format("Unable to download: {0}", url));
            }
        }
   

    }
}
