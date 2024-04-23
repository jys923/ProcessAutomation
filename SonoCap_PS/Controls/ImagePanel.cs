
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageQM
{



    public partial class ImagePanel : UserControl
    {


        [DllImport("user32")]
        private static extern IntPtr GetDCEx(IntPtr windowHandle, IntPtr clipRegionHandle, int flag);

        protected int viewRectWidth; // view window width and height

        protected int viewRectHeight; // view window width and height

        protected Bitmap m_Image = null;
        public Bitmap Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                m_Image = value;

                setScrollbarValues(this.Width, this.Height);

                GetImageViewPort(ref m_rectViewport);

                GetSourceImageRegion(ref m_rectSrcImg);

                Invalidate();
            }
        }

        protected double zoomStep = 0.2f;
        public double ZoomStep
        {
            get { return this.zoomStep; }
            set
            {
                if (value <= 0.1f)
                    value = 0.1f;

                this.zoomStep = value;
            }
        }

        protected double zoom = 1.0f;
        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (value < 0.1f) value = 0.1f;

                zoom = value;
                //if (zoom < 1.0f) zoom = 1.0f;
                if (zoom < 0.61f) zoom = 0.6f;

                setScrollbarValues(this.Width, this.Height);

                GetImageViewPort(ref m_rectViewport);

                GetSourceImageRegion(ref m_rectSrcImg);

                Invalidate();
            }
        }

        /// <param name="zoomStep">The zoom step.</param>
        public void Enlarge(double zoomStep)
        {
            this.Zoom += zoomStep;
        }

        public void Enlarge() { this.Enlarge(this.zoomStep); }

        /// <param name="zoomStep">The zoom step.</param>
        public void Reduce(double zoomStep)
        {
            this.Zoom -= zoomStep;
        }

        public void Reduce() { this.Reduce(this.zoomStep); }


        protected InterpolationMode interMode = InterpolationMode.HighQualityBilinear;
        public InterpolationMode InterpolationMode
        {
            get { return interMode; }
            set { interMode = value; }
        }


        protected DateTime m_dtUpdateTime = DateTime.Now; //new DateTime();
        public DateTime UpdateTime
        {
            get
            {
                return m_dtUpdateTime;
            }
        }


        protected Rectangle m_rectViewport = Rectangle.Empty;
        protected Rectangle m_rectSrcImg = Rectangle.Empty;



        public ImagePanel()
        {
            InitializeComponent();

            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                          ControlStyles.DoubleBuffer, true);

            this.MouseUp += new MouseEventHandler(this.ImageViewer_MouseUp);
            this.MouseWheel += new MouseEventHandler(this.ImageViewer_MouseWheel);

            //            this.GetContainerControl();

            /*
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                throw new Exception(); //No longer fails in design-time.
            }
            */
        }


        public int Create(int w, int h, int ch)
        {
            m_Image = new Bitmap(w, h, PixelFormat.Format24bppRgb);

            return 1;
        }


        public int Create(Bitmap bmp)
        {
            Image = new Bitmap(bmp);

            return 1;
        }


        public void Destroy()
        {
            if (null != m_Image)
            {
                m_Image.Dispose();
                m_Image = null;
            }

        }


        public int convertByteArrToBitmap(byte[] raw_img, Bitmap m_bmp)
        {
            try
            {
                BitmapData bmpData = m_bmp.LockBits(new Rectangle(0, 0,
                                                                m_bmp.Width,
                                                                m_bmp.Height),
                                                  ImageLockMode.ReadWrite,
                                                  m_bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(raw_img, 0, pNative, raw_img.Length);
                m_bmp.UnlockBits(bmpData);

                return 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }

        public int Update(byte[] data, DateTime time)
        {
            if (null == m_Image) return -1;

            m_dtUpdateTime = time;

            convertByteArrToBitmap(data, m_Image);

            this.Invalidate();

            return 1;
        }


        protected override void OnLoad(EventArgs e)
        {
            setScrollbarValues(this.Width, this.Height);

            base.OnLoad(e);
        }


        protected override void OnResize(EventArgs e)
        {
            setScrollbarValues(this.Width, this.Height);

            base.OnResize(e);
        }


        public int GetImageViewPort(ref Rectangle rectViewport)
        {
            if (null == this.Image) return -1;

            int width = this.Width;
            int height = this.Height;
            Size scaledImgSize = new Size((int)(this.m_Image.Width * this.zoom), (int)(this.m_Image.Height * this.zoom));

            if (false == hScrollBar1.Visible)
            {
                height = Height;
            }
            else
            {
                height = Height - hScrollBar1.Height - 1;
            }

            if (false == vScrollBar1.Visible)
            {
                width = Width;
            }
            else
            {
                width = Width - vScrollBar1.Width - 1;
            }

            // 1. 클라이언트 영역 계산
            Rectangle innerRectangle = new Rectangle(0, 0, width, height);


            Point offset;
            //if (this.AutoCenter)
            if (true)
            {
                int x = 0;
                int y = 0;
                x = !hScrollBar1.Visible ? (innerRectangle.Width - scaledImgSize.Width) / 2 : 0;
                y = !vScrollBar1.Visible ? (innerRectangle.Height - scaledImgSize.Height) / 2 : 0;

                offset = new Point(x, y);

                if (hScrollBar1.Visible)
                {
                    width = Math.Min(scaledImgSize.Width - Math.Abs(this.hScrollBar1.Value), innerRectangle.Width);
                }
                else
                {
                    width = Math.Min(scaledImgSize.Width, innerRectangle.Width);
                }

                if (vScrollBar1.Visible)
                {
                    height = Math.Min(scaledImgSize.Height - Math.Abs(this.vScrollBar1.Value), innerRectangle.Height);
                }
                else
                {
                    height = Math.Min(scaledImgSize.Height, innerRectangle.Height);
                }

            }
            /*
            else
            {
                offset = Point.Empty;
                width = innerRectangle.Width;
                height = innerRectangle.Height;
            }
            */

            // 3. 최종적으로, 이미지가 그려질 클라이언트 윈도우의 사각형 영역을 구함.
            rectViewport.X = offset.X + innerRectangle.Left;
            rectViewport.Y = offset.Y + innerRectangle.Top;
            rectViewport.Width = width;
            rectViewport.Height = height;

            return 1;
        }

        // 실제로 이미지가 그려지는 영역을 계산
        public virtual Rectangle GetImageViewPort()
        {
            if (null == this.Image) return Rectangle.Empty;

            Rectangle vp = new Rectangle();
            int ret = GetImageViewPort(ref vp);
            if (ret < 0) return Rectangle.Empty;

            return vp;
        }


        public int GetSourceImageRegion(ref Rectangle rectSrcReg)
        {
            if (null == this.Image) return -1;

            int sourceLeft;
            int sourceTop;
            int sourceWidth;
            int sourceHeight;

            Rectangle viewPort;
            viewPort = this.GetImageViewPort();

            // 만일, 확대인경우, 원래 영상보다 더 적은 영역을 DC 영역에서 확대해서 그려야 함.
            // 그래서, 나누어주는 것임.
            sourceLeft = (int)(this.hScrollBar1.Value / this.Zoom);
            sourceTop = (int)(this.vScrollBar1.Value / this.Zoom);

            // 만일, 2배 확대된 이미지라면, 확대된 만큼, 이미지 영역의 2배 축소된 영역만 전시하면
            // 2배 확대된 효과를 볼 수 있음.
            sourceWidth = (int)(viewPort.Width / this.Zoom);
            sourceHeight = (int)(viewPort.Height / this.Zoom);

            rectSrcReg.X = sourceLeft;
            rectSrcReg.Y = sourceTop;
            rectSrcReg.Width = sourceWidth;
            rectSrcReg.Height = sourceHeight;

            return 1;
        }


        // 실제로 그려질 이미지 영역을 계산함.
        public virtual Rectangle GetSourceImageRegion()
        {
            if (null == this.Image) return Rectangle.Empty;

            Rectangle region = new Rectangle();
            int ret = GetSourceImageRegion(ref region);
            if (ret < 0) return Rectangle.Empty;

            return region;
        }


        private void setScrollbarValues(int DispW, int DispH)
        {
            if (null == m_Image) return;

            viewRectWidth = DispW;
            viewRectHeight = DispH;

            Size scaledImgSize = new Size((int)(this.m_Image.Width * this.zoom), (int)(this.m_Image.Height * this.zoom));

            // If the zoomed m_Image is wider than view window, show the HScrollBar and adjust the view window
            if (viewRectWidth > scaledImgSize.Width)
            {
                hScrollBar1.Visible = false;
                viewRectHeight = DispH;
            }
            else
            {
                hScrollBar1.Visible = true;
                viewRectHeight = DispH - hScrollBar1.Height;
            }

            // If the zoomed m_Image is taller than view window, show the VScrollBar and adjust the view window
            if (viewRectHeight > scaledImgSize.Height)
            {
                vScrollBar1.Visible = false;
                viewRectWidth = DispW;
            }
            else
            {
                vScrollBar1.Visible = true;
                viewRectWidth = DispW - vScrollBar1.Width;
            }

            // Set up scrollbars
            hScrollBar1.Location = new Point(0, DispH - hScrollBar1.Height);
            hScrollBar1.Width = viewRectWidth;

            vScrollBar1.Location = new Point(DispW - vScrollBar1.Width, 0);
            vScrollBar1.Height = viewRectHeight;



            // Set the Maximum, Minimum, LargeChange and SmallChange properties.
            this.vScrollBar1.Minimum = 0;
            this.hScrollBar1.Minimum = 0;
            //this.hScrollBar1.Value = 0;
            //this.vScrollBar1.Value = 0;


            // If the offset does not make the Maximum less than zero, set its value. 
            if ((scaledImgSize.Width - viewRectWidth) > 0)
            {
                this.hScrollBar1.Maximum = scaledImgSize.Width - viewRectWidth;
            }

            // If the VScrollBar is visible, adjust the Maximum of the 
            // HSCrollBar to account for the width of the VScrollBar.  
            if (this.vScrollBar1.Visible)
            {
                this.hScrollBar1.Maximum += this.vScrollBar1.Width;
            }
            else
            {
                this.hScrollBar1.Value = 0;
            }
            this.hScrollBar1.LargeChange = this.hScrollBar1.Maximum / 10;
            this.hScrollBar1.SmallChange = this.hScrollBar1.Maximum / 20;
            this.hScrollBar1.Maximum += this.hScrollBar1.LargeChange;

            // If the offset does not make the Maximum less than zero, set its value.    
            if ((scaledImgSize.Height - viewRectHeight) > 0)
            {
                this.vScrollBar1.Maximum = scaledImgSize.Height - viewRectHeight;
            }

            if (this.hScrollBar1.Visible)
            {
                this.vScrollBar1.Maximum += this.hScrollBar1.Height;
            }
            else
            {
                this.vScrollBar1.Value = 0;
            }
            this.vScrollBar1.LargeChange = this.vScrollBar1.Maximum / 10;
            this.vScrollBar1.SmallChange = this.vScrollBar1.Maximum / 20;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.vScrollBar1.Maximum += this.vScrollBar1.LargeChange;

        }

        public int getScrollbarValues(int mode, ref int min, ref int max, ref int LargeChange, ref int SmallChange)
        {
            if (0 == mode)
            {
                min = 0;
                max = this.hScrollBar1.Maximum;
                LargeChange = this.hScrollBar1.LargeChange;
                SmallChange = this.hScrollBar1.SmallChange;
            }
            else if (1 == mode)
            {
                min = 0;
                max = this.vScrollBar1.Maximum;
                LargeChange = this.vScrollBar1.LargeChange;
                SmallChange = this.vScrollBar1.SmallChange;

            }
            else
            {
                return -1;
            }

            return 1;
        }

        private void ScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //if (m_rectViewport == Rectangle.Empty)
            GetImageViewPort(ref m_rectViewport);

            //if (m_rectSrcImg == Rectangle.Empty)
            GetSourceImageRegion(ref m_rectSrcImg);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            /*
            IntPtr hWnd = this.Handle;
            IntPtr hRgn = IntPtr.Zero;
            IntPtr hdc = GetDCEx(hWnd, hRgn, 1027);
            using (Graphics grfx = Graphics.FromHdc(hdc))
            {
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                Pen pen = new Pen(Color.Red, 5);
                grfx.DrawRectangle(pen, rect);
            }
            */


            if (m_Image != null)
            {
                using (BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
                {
                    bufferedGraphics.Graphics.Clear(this.BackColor);
                    bufferedGraphics.Graphics.DrawImage(m_Image, m_rectViewport, m_rectSrcImg, GraphicsUnit.Pixel);
                    bufferedGraphics.Render(e.Graphics);
                }
            }
        }

        private void ImageViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MouseEventHandler(this.ImageViewer_MouseUp), new object[] { sender, e });
                return;
            }

            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None)
            {
                this.Enlarge();
            }
            else if (e.Button == MouseButtons.Right || (e.Button == MouseButtons.Left && Control.ModifierKeys != Keys.None))
            {
                this.Reduce();
            }
        }

        private void ImageViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MouseEventHandler(this.ImageViewer_MouseWheel), new object[] { sender, e });
                return;
            }

            if (e.Delta < 0)
            {
                this.Reduce();
            }
            else
            {
                this.Enlarge();
            }
        }



    }
}
