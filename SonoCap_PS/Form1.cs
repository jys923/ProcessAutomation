using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


using VILib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SonoCap_PS
{
    public partial class Form1 : Form
    {
        private Bitmap m_OrgBmp = null;
        private Bitmap m_bmpRes = null;
        private Bitmap m_DispMap = null;

        private byte[] bmp_data = null;
        private byte[] res_data = null;

        private int w = 0;
        private int h = 0;
        private int chs = 4;
        private uint size = 0;

        
        private VILibWrapper VI = new VILibWrapper();

        int img_idx = 0;
        bool m_bFirst = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void IDC_VI_START_BTN_Click(object sender, EventArgs e)
        {
            // DLL 로딩시, 1번 호출
            int ret = VI.Load(0);
        }

        private void IDC_VI_END_BTN_Click(object sender, EventArgs e)
        {
            // DLL 종료시, 1번 호출
            int ret = VI.Unload(0);

            this.Close();

        }

        private void IDD_OPEN_BTN_Click(object sender, EventArgs e)
        {
            try
            {
                if( 0 == img_idx ) 
                {
                    img_idx = 1;
                    m_OrgBmp = new Bitmap(@"img\\4.bmp");
                }
                else
                {
                    img_idx = 0;
                    m_OrgBmp = new Bitmap(@"img\\5.bmp");
                }

                if (null == m_OrgBmp)
                {
                    MessageBox.Show("비트맵 객체 생성에 실패함.");
                    return;
                }

                m_DispMap = (Bitmap)m_OrgBmp.Clone();

                if (false == m_bFirst)
                {
                    m_bFirst = true;    

                    m_bmpRes = new Bitmap(m_OrgBmp.Width, m_OrgBmp.Height, PixelFormat.Format32bppArgb);

                    w = m_OrgBmp.Width;
                    h = m_OrgBmp.Height;
                    chs = 4;
                    size = (uint)(w * h * chs);

                    bmp_data = new byte[size];
                    res_data = new byte[size];
                }

                imagePanel1.Zoom = 1.0;
                imagePanel1.Create(m_DispMap);

                Array.Clear(res_data, 0, (int)size);
                convertByteArrToBitmap(res_data, m_bmpRes);
                imagePanel2.Zoom = 1.0;
                imagePanel2.Image = m_bmpRes;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            int ret = 0;

            this.textBox1.Clear();
            this.textBox1.Invalidate();

            Array.Clear(bmp_data, 0, (int)size);
            Array.Clear(res_data, 0, (int)size);

            convertBitmapToByteArr(bmp_data, m_OrgBmp);
        
            int opt = 0;
            string sModel =  "SC-GP5";
            VIRes vIResults = new VIRes();
            ret = VI.DoInspection(sModel, bmp_data, w, h, chs, res_data, ref vIResults, opt);

            StringBuilder sb = new StringBuilder();
         
            for (int i = 0; i < vIResults.nCnt; i++)
            {
                Trace.Write(i + " : ");
                Trace.Write("cen_x : " + vIResults.list[i].cen_x);
                Trace.Write(", cen_y : " + vIResults.list[i].cen_y);
                Trace.WriteLine(" , Lum : " + vIResults.list[i].nval);

                if (vIResults.list[i].nState > 0)
                {

                    String s;
                    int index = i+1;
                    s = index.ToString() + " : " + "cen_x : " + vIResults.list[i].cen_x.ToString() + ", cen_y : " + vIResults.list[i].cen_y.ToString()
                        + " , Lum : " + vIResults.list[i].nval.ToString();

                    sb.AppendLine(s);
                }
            }

            textBox1.Text = sb.ToString();



            Trace.WriteLine("Detect false regions");

            /*
            int[] false_region = Enumerable.Repeat(-1, vIResults.nCnt).ToArray();
            for (int i = 0; i < vIResults.nCnt; i++)
            {
                if (1 == false_region[i]) continue;

                for (int j = 0; j < vIResults.nCnt; j++)
                {
                    if (i == j) continue;
                    if (1 == false_region[j]) continue;

                    int diff_x = Math.Abs(vIResults.list[i].cen_x - vIResults.list[j].cen_x);
                    int diff_y = Math.Abs(vIResults.list[i].cen_y - vIResults.list[j].cen_y);

                    if( (diff_x <20) && (diff_y<20) )
                    {
                        false_region[i] = 1;
                        false_region[j] = 1;

                        Trace.Write(i);
                        Trace.Write(", " + j);
                        Trace.Write(" : " + diff_x);
                        Trace.WriteLine(" , " + diff_y);
                    }
                }
            }
            */

            convertByteArrToBitmap(bmp_data, m_DispMap);
            imagePanel1.Image = m_DispMap;

            convertByteArrToBitmap(res_data, m_bmpRes);
            imagePanel2.Zoom = 1.0;
            imagePanel2.Image = m_bmpRes;

  
        }


 
        // create mask map
        private void button3_Click(object sender, EventArgs e)
        {
            //string spath = Application.StartupPath + @"\SYS\mask.bmp";
            //ISP.MakeMaskImg(spath, 0, 0, 0, 0);

            string sModel = "SC-GP5";
            string spath = Application.StartupPath + @"\SYS\";
            VI.MakeMaskImg(sModel, spath, 780, 780, 390, 390, 140);
        }



        public int convertByteArrToBitmap(byte[] raw_img, Bitmap m_bmp)
        {
            BitmapData bmpData = null;
            try
            {
                bmpData = m_bmp.LockBits(new Rectangle(0, 0,
                                                    m_bmp.Width,
                                                    m_bmp.Height),
                                                    ImageLockMode.WriteOnly,
                                                    m_bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(raw_img, 0, pNative, raw_img.Length);
                m_bmp.UnlockBits(bmpData);

                return 1;
            }
            catch (Exception ex)
            {
                m_bmp.UnlockBits(bmpData);
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }

        public int convertBitmapToByteArr(byte[] result, Bitmap bitmap)
        {
            BitmapData bmpData = null;
            try
            {
                bmpData = bitmap.LockBits(new Rectangle(0, 0,
                                                    bitmap.Width,
                                                    bitmap.Height),
                                                    ImageLockMode.ReadOnly,
                                                    bitmap.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                int numbytes = bmpData.Stride * bitmap.Height;
                Marshal.Copy(pNative, result, 0, numbytes);
                bitmap.UnlockBits(bmpData);

                return 1;
            }
            catch (Exception ex)
            {
                bitmap.UnlockBits(bmpData);
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }


    }
}
