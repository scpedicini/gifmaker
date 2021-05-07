using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace GifCreator
{
    public partial class WndMain : Form
    {
        Image PreviewImage;

        
        Gif ActiveGif;

        object gifLock = new object();

        int PreviewFrameIndex;

        public WndMain()
        {
            InitializeComponent();

            ActiveGif = new Gif();
            ListImages.ListViewItemSorter = new ListViewIndexComparer();
            ListImages.InsertionMark.Color = Color.Green;

            ListImages.AllowDrop = true; 
            ListImages.DragDrop += ListImages_DragDrop;
            ListImages.DragEnter += ListImages_DragEnter;
            ListImages.DragOver += ListImages_DragOver;
            ListImages.ItemDrag += ListImages_ItemDrag;
            ListImages.DragLeave += ListImages_DragLeave;


            TimerPreview.Start();
            
            // configure track bar


            //ImageSystem.DownloadAsBitmap(@"http://flyers.arcade-museum.com/flyers_video/atari/11007601.jpg");
        }

       

        Bitmap TryGetDib(DataObject dataObj)
        {
            if (!dataObj.GetDataPresent(DataFormats.Dib)) return null;
            Debug.WriteLine("Dib Data Present in DataObject");

            Bitmap bmp = null;
            // first attempt use DIB (Data Independent Bitmap)
            try
            {
                if (dataObj.GetDataPresent(DataFormats.Dib))
                {
                    var bitmapStream = new BitmapFromDibStream((Stream)dataObj.GetData(DataFormats.Dib));
                    bmp = new Bitmap(bitmapStream);
                }
            }
            catch 
            {

            }

            if (bmp != null) Debug.WriteLine("Got Bitmap from DataObject.Dib");

            return bmp;
        }

        Bitmap TryGetText(DataObject dataObj)
        {
            if (!dataObj.GetDataPresent(DataFormats.Text)) return null;
            Debug.WriteLine("Text Data Present in DataObject");

            Bitmap bmp = null;
            try
            {
                var text = dataObj.GetText();
                // can we get the file from a text url
                if (text.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    bmp = ImageSystem.DownloadAsBitmap(text);
                }
            }
            catch
            {

            }

            if (bmp != null) Debug.WriteLine("Got Bitmap from DataObject.Text");

            return bmp;
        }

        List<KeyValuePair<string, Bitmap>> TryGetFileDrop(DataObject dataObj)
        {
            var listfiles = new List<KeyValuePair<string, Bitmap>>();

            if (dataObj.GetDataPresent(DataFormats.FileDrop))
            {
                Debug.WriteLine("FileDrop Data Present in DataObject");

                Bitmap bmp = null;
                // check for a file drop
                try
                {
                    foreach (var file in ((string[])dataObj.GetData(DataFormats.FileDrop)))
                    {
                        bmp = new Bitmap(file);
                        if (bmp != null) Debug.WriteLine("Got Bitmap from DataObject.FileDrop");
                        listfiles.Add(new KeyValuePair<string, Bitmap>(file, bmp));
                    }

                    // attempt to acquire each file
                    // bmp = new Bitmap(((string[])dataObj.GetData(DataFormats.FileDrop)).ElementAt(0));
                }
                catch
                {

                }
            }

            if (CheckSortDate.Checked)
            {
                listfiles = listfiles.Select(l => new { k = l.Key, v = l.Value, d = new FileInfo(l.Key).CreationTime })
                    .OrderBy(o => o.d).Select(o => new KeyValuePair<string, Bitmap>(o.k, o.v)).ToList();
            }

            return listfiles;
        }

        Bitmap TryGetHtml(DataObject dataObj)
        {
            Bitmap bmp = null;

            if (!dataObj.GetDataPresent(DataFormats.Html)) return null;

            Debug.WriteLine("HTML Present in DataObject");
            try
            {
                // locate img tag
                // <img style="margin-top: 138px;" src="http://icongal.com/gallery/image/9098/transparent.png" id="irc_mi" height="256" width="256">
                // src may also include base 64 data
                var doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml((string)dataObj.GetData(DataFormats.Html));

                var images = doc.DocumentNode.Descendants("img");
                if (images.Count() > 0)
                {
                    var firstImage = images.ElementAt(0);

                    var src = firstImage.Attributes["src"].Value;

                    var base64ImageRegexPat = @"^\s*data:image/.*?;base64,";
                    // data:image/png;base64,

                    var match = Regex.Match(src, base64ImageRegexPat, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        var base64String = src.Remove(0, match.Length);
                        var ms = new MemoryStream(Convert.FromBase64String(base64String));
                        bmp = new Bitmap(ms);
                    }
                    else
                    {
                        // regular style download
                        bmp = ImageSystem.DownloadAsBitmap(src);
                    }
                }

            }
            catch
            {
                bmp = null;
            }

            if (bmp != null) Debug.WriteLine("Got Bitmap from DataObject.Html");

            return bmp;
        }

        void AddImage(DataObject dataObj)
        {
            if (dataObj == null) return;

            var imgName = dataObj.ContainsText() ? dataObj.GetText() : "image file";

            Bitmap framePicture = null;


            var tmpList = new List<KeyValuePair<string, Bitmap>>();

            framePicture = TryGetHtml(dataObj);

            if (framePicture != null)
            {
                // extracted from the HTML img tag
                tmpList.Add(new KeyValuePair<string, Bitmap>(imgName, framePicture));
            }
            else
            {
                // attempt to acquire from the text itself
                framePicture = TryGetText(dataObj);
                if (framePicture != null) tmpList.Add(new KeyValuePair<string, Bitmap>(imgName, framePicture));
            }


            if (tmpList.Count == 0)
            {
                // attempt to acquire from a file drop
                tmpList = TryGetFileDrop(dataObj);
            }

            if (tmpList.Count == 0)
            {
                // attempt to acquire from a DIB
                if ((framePicture = TryGetDib(dataObj)) != null)
                    tmpList.Add(new KeyValuePair<string, Bitmap>(imgName, framePicture));
            }


            if (tmpList != null && tmpList.Count > 0)
            {
                foreach (var kvp in tmpList)
                {
                    imgName = kvp.Key;
                    framePicture = kvp.Value;

                    // break into list of possible frames if gif file
                    List<Bitmap> bmpList = new List<Bitmap>();

                    try
                    {
                        var fd = new FrameDimension(framePicture.FrameDimensionsList[0]);
                        var frameCount = framePicture.GetFrameCount(fd);
                        if (frameCount > 1)
                        {
                            for (int i = 0; i < frameCount; i++)
                            {
                                // represents a GIF file
                                framePicture.SelectActiveFrame(fd, i);
                                Bitmap singleAnimFrame = new Bitmap(framePicture);
                                bmpList.Add(singleAnimFrame);
                            }
                        }
                        else
                        {
                            // single image
                            bmpList.Add(framePicture);
                        }
                    }
                    catch
                    {
                        // single image
                        bmpList.Add(framePicture);
                    }



                    foreach (var bmp in bmpList)
                    {
                        var gifimg = ActiveGif.AddFrame(imgName, bmp);
                        var listItem = ListImages.Items.Add(imgName);
                        listItem.Tag = gifimg;
                    }
                    // if we want to look specifically at the image via the list, how can we do that?
                }
            }
            else
            {
                // unable to acquire image, possibly look through the HTML DataFormat and extract an <img> tag if possible.
                Debug.WriteLine("Image was not able to be acquired.");
            }
        }


        private void TrackDelay_ValueChanged(object sender, EventArgs e)
        {
            // update the timer

            ActiveGif.FrameDelay = TrackDelay.Value * 25;
            TimerPreview.Interval = ActiveGif.FrameDelay;

        }

        private void TimerPreview_Tick(object sender, EventArgs e)
        {
            // display a picture 
            if (ActiveGif.GifImages.Count == 0) return;

            try
            {
                if (PreviewFrameIndex >= ActiveGif.GifImages.Count) PreviewFrameIndex = 0;

                PreviewImage = ActiveGif.GifImages.ElementAt(PreviewFrameIndex).Frame;

                // attempt to draw the image onto the picture box
                PicturePreview.Invalidate();
                PreviewFrameIndex = (PreviewFrameIndex + 1) % ActiveGif.GifImages.Count;
            }
            catch
            {
                PreviewFrameIndex = 0;
            }
        }

        private void PicturePreview_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (ActiveGif.GifImages.Count == 0)
                {
                    e.Graphics.Clear(BackColor);
                }
                else if (PreviewImage != null)
                {
                    e.Graphics.DrawImage(PreviewImage, PicturePreview.DisplayRectangle);
                }
             
            }
            catch
            {

            }
        }

        private void BtnToClipboard_Click(object sender, EventArgs e)
        {
            // export to temporary file name
            if (ActiveGif.GifImages.Count > 0)
            {
                UseWaitCursor = true;
                var tmpFile = string.Concat(System.IO.Path.GetTempFileName(), ".gif");

                try
                {
                    ActiveGif.Export(tmpFile, true);
                    Clipboard.SetText(tmpFile);
                }
                catch
                {
                    MessageBox.Show(this, "An unknown error occurred while trying to export the gif file.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                UseWaitCursor = false;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ActiveGif.GifImages.Clear();
            PicturePreview.Invalidate();


            ListImages.Items.Clear();
        }


        private void WndMain_KeyDown(object sender, KeyEventArgs e)
        {
            DataObject data = Clipboard.GetDataObject() != null ? Clipboard.GetDataObject() as DataObject : null;
            if (e.Control && e.KeyCode == Keys.V && data != null)
            {
                AddImage(data);
            }
        }


        /** ListImages Events **/


        private void ListImages_DragLeave(object sender, EventArgs e)
        {
            ListImages.InsertionMark.Index = -1;
        }

        private void ListImages_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListImages.DoDragDrop(e.Item, DragDropEffects.Move); ;
        }


        private void ListImages_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint =
                ListImages.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            int targetIndex = ListImages.InsertionMark.NearestIndex(targetPoint);

            // Confirm that the mouse pointer is not over the dragged item.
            if (targetIndex > -1)
            {
                // Determine whether the mouse pointer is to the left or
                // the right of the midpoint of the closest item and set
                // the InsertionMark.AppearsAfterItem property accordingly.
                Rectangle itemBounds = ListImages.GetItemRect(targetIndex);
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    ListImages.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    ListImages.InsertionMark.AppearsAfterItem = false;
                }
            }

            // Set the location of the insertion mark. If the mouse is
            // over the dragged item, the targetIndex value is -1 and
            // the insertion mark disappears.
            ListImages.InsertionMark.Index = targetIndex;
        }

        void ListImages_DragDrop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("ListImages_DragDrop");

            var dataObj = e.Data as DataObject;

            if (dataObj.GetDataPresent(typeof(ListViewItem)))
            {
                // trying to re-sort items
                // Retrieve the index of the insertion mark;
                int targetIndex = ListImages.InsertionMark.Index;

                // If the insertion mark is not visible, exit the method.
                if (targetIndex == -1)
                {
                    return;
                }

                // If the insertion mark is to the right of the item with
                // the corresponding index, increment the target index.
                if (ListImages.InsertionMark.AppearsAfterItem)
                {
                    targetIndex++;
                }

                // Retrieve the dragged item.
                ListViewItem draggedItem =
                    (ListViewItem)e.Data.GetData(typeof(ListViewItem));

                // Insert a copy of the dragged item at the target index.
                // A copy must be inserted before the original item is removed
                // to preserve item index values.
                ListImages.Items.Insert(
                    targetIndex, (ListViewItem)draggedItem.Clone());

                // Remove the original copy of the dragged item.
                ListImages.Items.Remove(draggedItem);

                lock (gifLock)
                {
                    ActiveGif.Clear();
                    ActiveGif.GifImages = ListImages.Items.Cast<ListViewItem>().Select(l => l.Tag as GifImage).ToList();
                }

                
            }
            else
            {
                AddImage(dataObj);
            }


        }

        void ListImages_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;

            //throw new NotImplementedException();
            Debug.WriteLine("ListImages_DragEnter");

            //e.Effect = DragDropEffects.Copy;
            return;


            DataObject obj = (DataObject)e.Data;

            // IE9 and Firefox both have a DeviceIndependentBitmap file format that is available when dragging an non-hyperlink image. 
            // This seems to be a safer alternative, though Chrome does not seem to support it. It is also not so useful with hyperlink images.
            var dib = e.Data.GetData(DataFormats.Dib) as MemoryStream;
            Bitmap dibMap = null;
            if (dib != null)
            {
                var bFromStream = new BitmapFromDibStream(dib);
                dibMap = new Bitmap(bFromStream);
            }

            var bmp = obj.GetData(DataFormats.Bitmap);

            var html = obj.GetData(DataFormats.Html);

            //TryGetHtml(obj);
            /*
                Sample Return:
                Version:0.9
                StartHTML:00000147
                EndHTML:00000874
                StartFragment:00000181
                EndFragment:00000838
                SourceURL:https://en.wikipedia.org/wiki/Arimaspi
                <html>
                    <body>
                        <!--StartFragment-->
                        <a href="https://en.wikipedia.org/wiki/File:Satyr_griffin_Arimaspus_Louvre_CA491.jpg" class="image">
                            <img alt="" src="https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Satyr_griffin_Arimaspus_Louvre_CA491.jpg/280px-Satyr_griffin_Arimaspus_Louvre_CA491.jpg" class="thumbimage" srcset="//upload.wikimedia.org/wikipedia/commons/thumb/0/00/Satyr_griffin_Arimaspus_Louvre_CA491.jpg/420px-Satyr_griffin_Arimaspus_Louvre_CA491.jpg 1.5x, //upload.wikimedia.org/wikipedia/commons/thumb/0/00/Satyr_griffin_Arimaspus_Louvre_CA491.jpg/560px-Satyr_griffin_Arimaspus_Louvre_CA491.jpg 2x" data-file-width="2000" data-file-height="1820" height="255" width="280">
                        </a>
                        <!--EndFragment-->
                    </body>
                </html>
              
                check for img tag and base64 data array 
            */
            var text = obj.GetData(DataFormats.Text);

            // string[] array of objects
            // note that this will download a transparent file as a bmp file with a black background 
            var files = obj.GetData(DataFormats.FileDrop); // ((System.Windows.Forms.DataObject)e.Data).ContainsFileDropList(), ((System.Windows.Forms.DataObject)e.Data).GetFileDropList()


            e.Effect = DragDropEffects.Copy;

            //if (e.Data.GetDataPresent(DataFormats.Text))
            //{
            //    e.Effect = DragDropEffects.Copy;
            //}
            //else
            //{
            //    e.Effect = DragDropEffects.None;
            //}
        }


        bool deletionMode = false;
        private void ListImages_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete && !deletionMode)
            {
                deletionMode = true;

                foreach (ListViewItem item in ListImages.SelectedItems)
                {
                    var gifImageTag = item.Tag as GifImage;
                    if (gifImageTag != null) { ActiveGif.GifImages.Remove(gifImageTag); }
                    ListImages.Items.Remove(item);
                }

                PicturePreview.Invalidate();
                deletionMode = false;
            }

        }


        private class ListViewIndexComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
            }
        }


        private void BtnTest_Click(object sender, EventArgs e)
        {
            //var url = @"http://icons.iconarchive.com/icons/icons-land/metro-raster-sport/128/Soccer-Ball-icon.png";
            //var url2 = @"http://icons.iconarchive.com/icons/hopstarter/scrap/128/Aqua-Ball-Red-icon.png";
            //var url3 = @"http://images.hookbag.ca/300fbd88db2f858a/image.jpg";

            //var img1 = DownloadImage(url);
            //var img2 = DownloadImage(url2);
            //var img3 = DownloadImage(url3);

            //ActiveGif.AddImage(url);
            //ActiveGif.AddImage(url2);
            //ActiveGif.AddImage(url3);

            //ActiveGif.MakeImageSizesUniform();

            //ActiveGif.Export(@"C:\Temp\GifCreator\test.gif");

        }

        private void WndMain_DoubleClick(object sender, EventArgs e)
        {
            // test area

            Debug.WriteLine("WndMain_DoubleClick");
        }
    }



}
