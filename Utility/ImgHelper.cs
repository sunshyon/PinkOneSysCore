using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class ImgHelper
    {
        #region 图片操作
        private static int defaultMaxWith = 1180;
        /// <summary>
        /// 图片裁剪
        /// </summary>
        public static Bitmap ImageCut(Image oriImg)
        {
            int newW = oriImg.Width < defaultMaxWith ? oriImg.Width : defaultMaxWith;
            int newH = int.Parse(Math.Round(oriImg.Height * (double)newW / oriImg.Width).ToString());

            Bitmap b = new Bitmap(newW, newH);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.DrawImage(oriImg, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, oriImg.Width, oriImg.Height), GraphicsUnit.Pixel);
            g.Dispose();
            return b;
        }
        /// <summary>
        /// 图片压缩
        /// flag取值1到100，越小压缩比越大，推荐50
        /// </summary>
        public static bool ImageCompress(Image oriImg, string outPath, int flag=60)
        {
            //图片过大先裁剪
            if(oriImg.Width> defaultMaxWith)
                oriImg = ImageCut(oriImg);
            //图片稍小
            if (oriImg.Width * oriImg.Height < 600 * 400)
                flag = 80;
            if (oriImg.Width * oriImg.Height < 400 * 200)
                flag = 100;
            ImageFormat tFormat = oriImg.RawFormat;
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo jpegICIinfo = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICIinfo = arrayICI[x];
                    break;
                }
            }
            if (jpegICIinfo != null)
                oriImg.Save(outPath, jpegICIinfo, ep);
            else
                oriImg.Save(outPath, tFormat);
            //oriImg.Dispose();
            return true;
        }
        #endregion

        #region 图形验证码
        public static byte[] CreateImgStream(out string codeStr)
        {
            codeStr = CreateCodeStr();
            MemoryStream ms = new MemoryStream();
            if (string.Equals(codeStr, string.Empty))
                return null;
            Bitmap image = new Bitmap((int)Math.Ceiling(codeStr.Length * 18.5), 30);
            Graphics g = Graphics.FromImage(image);

            Random random = new Random();
            g.Clear(Color.White);
            //生成背景干扰点
            for(int i = 0; i < 25; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);
                g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
            }
            //生成验证字符
            Font font = new Font("Arial", 19, (FontStyle.Bold | FontStyle.Italic));
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            g.DrawString(codeStr, font, brush, 2, 1);
            //生成前景干扰点
            for (int i = 0; i < 100; i++)
            {
                int x1 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                image.SetPixel(x1, y1, Color.FromArgb(random.Next()));
            }
            //图片边框
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            //保存
            image.Save(ms, ImageFormat.Gif);

            return ms.ToArray();
        }
        private static string CreateCodeStr(int len=5)
        {
            int number;
            char code;
            string CheckCode = string.Empty;
            Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                {
                    code = (char)('0' + (char)(number % 10));
                }
                else
                {
                    code = (char)('A' + (char)(number % 26));
                }
                CheckCode += code.ToString();
            }
            CheckCode = CheckCode.Replace("0", "F");
            CheckCode = CheckCode.Replace("O", "F");
            return CheckCode;
        }
        #endregion
    }
}
