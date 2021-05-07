namespace GifCreator
{
    partial class WndMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ListImages = new System.Windows.Forms.ListView();
            this.ColUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BtnToClipboard = new System.Windows.Forms.Button();
            this.GroupPreview = new System.Windows.Forms.GroupBox();
            this.PicturePreview = new System.Windows.Forms.PictureBox();
            this.TrackDelay = new System.Windows.Forms.TrackBar();
            this.TimerPreview = new System.Windows.Forms.Timer(this.components);
            this.BtnClear = new System.Windows.Forms.Button();
            this.CheckSortDate = new System.Windows.Forms.CheckBox();
            this.GroupPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // ListImages
            // 
            this.ListImages.AllowDrop = true;
            this.ListImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ListImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColUrl});
            this.ListImages.FullRowSelect = true;
            this.ListImages.GridLines = true;
            this.ListImages.HideSelection = false;
            this.ListImages.Location = new System.Drawing.Point(28, 17);
            this.ListImages.Name = "ListImages";
            this.ListImages.Size = new System.Drawing.Size(320, 390);
            this.ListImages.TabIndex = 0;
            this.ListImages.UseCompatibleStateImageBehavior = false;
            this.ListImages.View = System.Windows.Forms.View.Details;
            this.ListImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListImages_KeyDown);
            // 
            // ColUrl
            // 
            this.ColUrl.Text = "Frame URL";
            this.ColUrl.Width = 316;
            // 
            // BtnToClipboard
            // 
            this.BtnToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnToClipboard.Location = new System.Drawing.Point(28, 418);
            this.BtnToClipboard.Name = "BtnToClipboard";
            this.BtnToClipboard.Size = new System.Drawing.Size(97, 42);
            this.BtnToClipboard.TabIndex = 2;
            this.BtnToClipboard.Text = "Export to Clipboard";
            this.BtnToClipboard.UseVisualStyleBackColor = true;
            this.BtnToClipboard.Click += new System.EventHandler(this.BtnToClipboard_Click);
            // 
            // GroupPreview
            // 
            this.GroupPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupPreview.Controls.Add(this.PicturePreview);
            this.GroupPreview.Location = new System.Drawing.Point(379, 12);
            this.GroupPreview.Name = "GroupPreview";
            this.GroupPreview.Size = new System.Drawing.Size(471, 395);
            this.GroupPreview.TabIndex = 3;
            this.GroupPreview.TabStop = false;
            this.GroupPreview.Text = "Preview";
            // 
            // PicturePreview
            // 
            this.PicturePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PicturePreview.Location = new System.Drawing.Point(19, 22);
            this.PicturePreview.Name = "PicturePreview";
            this.PicturePreview.Size = new System.Drawing.Size(430, 353);
            this.PicturePreview.TabIndex = 0;
            this.PicturePreview.TabStop = false;
            this.PicturePreview.Paint += new System.Windows.Forms.PaintEventHandler(this.PicturePreview_Paint);
            // 
            // TrackDelay
            // 
            this.TrackDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackDelay.Location = new System.Drawing.Point(379, 424);
            this.TrackDelay.Maximum = 160;
            this.TrackDelay.Minimum = 1;
            this.TrackDelay.Name = "TrackDelay";
            this.TrackDelay.Size = new System.Drawing.Size(471, 45);
            this.TrackDelay.TabIndex = 4;
            this.TrackDelay.Value = 1;
            this.TrackDelay.ValueChanged += new System.EventHandler(this.TrackDelay_ValueChanged);
            // 
            // TimerPreview
            // 
            this.TimerPreview.Tick += new System.EventHandler(this.TimerPreview_Tick);
            // 
            // BtnClear
            // 
            this.BtnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnClear.Location = new System.Drawing.Point(131, 418);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(97, 42);
            this.BtnClear.TabIndex = 5;
            this.BtnClear.Text = "Clear";
            this.BtnClear.UseVisualStyleBackColor = true;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // CheckSortDate
            // 
            this.CheckSortDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckSortDate.AutoSize = true;
            this.CheckSortDate.Location = new System.Drawing.Point(235, 418);
            this.CheckSortDate.Name = "CheckSortDate";
            this.CheckSortDate.Size = new System.Drawing.Size(113, 17);
            this.CheckSortDate.TabIndex = 6;
            this.CheckSortDate.Text = "Autosort by Date";
            this.CheckSortDate.UseVisualStyleBackColor = true;
            // 
            // WndMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 472);
            this.Controls.Add(this.CheckSortDate);
            this.Controls.Add(this.BtnClear);
            this.Controls.Add(this.TrackDelay);
            this.Controls.Add(this.GroupPreview);
            this.Controls.Add(this.BtnToClipboard);
            this.Controls.Add(this.ListImages);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "WndMain";
            this.Text = "Gif Creator";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WndMain_KeyDown);
            this.GroupPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicturePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ListImages;
        private System.Windows.Forms.Button BtnToClipboard;
        private System.Windows.Forms.GroupBox GroupPreview;
        private System.Windows.Forms.PictureBox PicturePreview;
        private System.Windows.Forms.TrackBar TrackDelay;
        private System.Windows.Forms.Timer TimerPreview;
        private System.Windows.Forms.ColumnHeader ColUrl;
        private System.Windows.Forms.Button BtnClear;
        private System.Windows.Forms.CheckBox CheckSortDate;
    }
}

