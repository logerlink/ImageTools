using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Diagnostics;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using ZXing.QrCode.Internal;
using System.Net;

namespace ImageTools
{
    public class ImageHelper
    {
        #region 文件相关  图片也是文件
        /// <summary>
        /// 将文件转成字节数组
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static byte[] File2Bytes(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] tempArr = new byte[fs.Length];
            fs.Read(tempArr, 0, tempArr.Length);
            return tempArr;
        }

        /// <summary>
        /// 将字节数组转换成文件
        /// </summary>
        /// <param name="filePath">输出文件的路径</param>
        /// <param name="bytes">字节数组内容</param>
        public static void Bytes2File(string filePath, byte[] bytes)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        /// <param name="dFile">目标文件路径</param>
        public static void FileCopyTo(string filePath, string dFile)
        {
            if (File.Exists(filePath)) File.Copy(filePath,dFile);
        }
        /// <summary>
        /// 重命名 (剪切/粘贴)到
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        /// <param name="dFile">目标文件路径</param>
        public static void FileChangeTo(string filePath, string dFile)
        {
            if(File.Exists(filePath)) File.Move(filePath, dFile);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        public static void FileDelete(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        /// <summary>
        /// 查看文件信息
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        public static FileInfo FileGetInfo(string filePath)
        {
            if (File.Exists(filePath)) return new FileInfo(filePath);
            return null;
        }

        /// <summary>
        /// 根据文件头判断上传的文件类型
        /// </summary>
        /// <param name="filePath">filePath是文件的完整路径 </param>
        /// <returns>返回true或false</returns>
        public static bool IsPicture(string filePath)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                string fileClass;
                byte buffer;
                buffer = reader.ReadByte();
                fileClass = buffer.ToString();
                buffer = reader.ReadByte();
                fileClass += buffer.ToString();
                reader.Close();
                fs.Close();
                if (fileClass == "255216" || fileClass == "7173" || fileClass == "13780" || fileClass == "6677")
                //255216是jpg;7173是gif;6677是BMP,13780是PNG;7790是exe,8297是rar 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public enum FileExtension
        {
            JPG = 255216,
            GIF = 7173,
            BMP = 6677,
            PNG = 13780,
            COM = 7790,
            EXE = 7790,
            DLL = 7790,
            RAR = 8297,
            ZIP = 8075,
            XML = 6063,
            HTML = 6033,
            ASPX = 239187,
            CS = 117115,
            JS = 119105,
            TXT = 210187,
            SQL = 255254,
            BAT = 64101,
            BTSEED = 10056,
            RDP = 255254,
            PSD = 5666,
            PDF = 3780,
            CHM = 7384,
            LOG = 70105,
            REG = 8269,
            HLP = 6395,
            DOC = 208207,
            XLS = 208207,
            DOCX = 208207,
            XLSX = 208207,
        }
        #endregion

        #region 网络下载图片
        /// <summary>
        /// 网络下载图片 get请求
        /// </summary>
        /// <param name="url">图片url</param>
        /// <param name="dPath">存储文件夹路径</param>
        public static string DownloadImageByUrl(string url, string dPath = "")
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            System.IO.Stream stream = null;
            try
            {
                var dFile = "";
                if (string.IsNullOrWhiteSpace(dPath)) dPath = AppDomain.CurrentDomain.BaseDirectory + "Image\\";
                if (!Directory.Exists(dPath)) Directory.CreateDirectory(dPath);
                var imgNameMatches = new Regex("\\w+\\.\\w+").Matches(rsp.ResponseUri.OriginalString);
                if (imgNameMatches.Count > 0) dFile = dPath + imgNameMatches.LastOrDefault().Value;
                else
                {
                    dFile = dPath + Guid.NewGuid().ToString() + ".png";
                }
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                Image.FromStream(stream).Save(dFile);
                return dFile;
            }
            finally
            {
                // 释放资源
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }
        #endregion

        #region base64与图片
        /// <summary>
        /// 64转图片
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="filePath">存储图片的位置</param>
        /// <returns></returns>
        public static void Base64ToImage(string base64,string filePath)
        {
            var reg = new Regex("data:.*?base64,");
            base64 = reg.Replace(base64,"");//将base64头部信息替换
            byte[] imageBytes = Convert.FromBase64String(base64);
            //将字节数组转换成文件
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(imageBytes, 0, imageBytes.Length);
        }
        /// <summary>
        /// 图片转base64
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <param name="isAddPrefix">是否自动加上图片的前缀</param>
        /// <returns></returns>
        public static string ImageToBase64(string filePath,bool isAddPrefix = true)
        {
            //见文件转成字节数组
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] tempArr = new byte[fs.Length];
            fs.Read(tempArr, 0, tempArr.Length);

            var base64 = Convert.ToBase64String(tempArr);
            if (isAddPrefix)
            {
                var sufName = filePath.Split(".").LastOrDefault();
                return $"data:image/{sufName};base64," + base64;
            }
            return base64;
        }
        #endregion

        #region 缩略图 无压缩  

        /// <summary>
        /// 图片缩略图   处理不了gif
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">缩略保存图片地址</param>
        /// <param name="scale">缩小倍数</param>
        /// <param name="quality">调整质量 100为原图，越小图片质量越差</param>
        public static bool ResizeImage(string sFile, string dFile, int scale = 2, int quality = 90)
        {
            //参考：https://www.cnblogs.com/rinack/p/3500239.html
            var sufName = sFile.Split(".").LastOrDefault();
            sufName = sufName.Replace("jpg", "jpeg");
            if (sufName.ToUpper() == "GIF") throw new Exception("该方法不支持gif图片");
            long imageQuality = quality;
            Bitmap sourceImage = new Bitmap(sFile);
            ImageCodecInfo myImageCodecInfo = GetImageCoderInfo("image/" + sufName);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            float xWidth = sourceImage.Width;
            float yWidth = sourceImage.Height;
            Bitmap newImage = new Bitmap((int)(xWidth / scale), (int)(yWidth / scale));
            Graphics g = Graphics.FromImage(newImage);

            g.DrawImage(sourceImage, 0, 0, newImage.Width, newImage.Height);
            g.Dispose();
            newImage.Save(dFile, myImageCodecInfo, myEncoderParameters);
            return true;
        }

        /// <summary>
        /// 获取图片编码类型信息
        /// </summary>
        /// <param name="coderType">编码类型</param>
        /// <returns>ImageCodecInfo</returns>
        private static ImageCodecInfo GetImageCoderInfo(string coderType)
        {
            ImageCodecInfo[] iciS = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo retIci = null;
            foreach (ImageCodecInfo ici in iciS)
            {
                if (ici.MimeType.Equals(coderType))
                {
                    retIci = ici;
                    break;
                }
                    
            }
            return retIci;
        }

        #region 设置Gif图片宽高（大小）
        /// <summary> 
        /// 设置GIF大小  只处理gif/tiff
        /// </summary> 
        /// <param name="sFile">图片路径</param> 
        /// <param name="dFile">缩略保存图片地址</param> 
        /// <param name="scale">缩小倍数</param> 
        public static void ResizeGif(string sFile, string dFile, int scale = 2)
        {
            var sufName = sFile.Split(".").LastOrDefault();
            sufName = sufName.Replace("jpg", "jpeg");
            var upperSufName = sufName.ToUpper();
            if (upperSufName != "GIF" && upperSufName != "TIFF") throw new Exception("该方法仅支持gif/tiff图片");
            Image res = Image.FromFile(sFile);
            int width = res.Width / scale;      //若未提供宽高则表示原始宽高/2
            int height = res.Height / scale;
            Image gif = new Bitmap(width, height);
            Image frame = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(gif);
            Rectangle rg = new Rectangle(0, 0, width, height);
            Graphics gFrame = Graphics.FromImage(frame);
            foreach (Guid gd in res.FrameDimensionsList)
            {
                FrameDimension fd = new FrameDimension(gd);
                //因为是缩小GIF文件所以这里要设置为Time，如果是TIFF这里要设置为PAGE，因为GIF以时间分割，TIFF为页分割 
                FrameDimension f = FrameDimension.Time;
                if (upperSufName == "TIFF") f = FrameDimension.Page;
                int count = res.GetFrameCount(fd);
                ImageCodecInfo codecInfo = GetImageCoderInfo("image/" + sufName);
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.SaveFlag;
                EncoderParameters eps = null;

                for (int i = 0; i < count; i++)
                {
                    res.SelectActiveFrame(f, i);
                    if (0 == i)
                    {
                        g.DrawImage(res, rg);
                        eps = new EncoderParameters(1);
                        //第一帧需要设置为MultiFrame 
                        eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
                        BindProperty(res, gif);
                        gif.Save(dFile, codecInfo, eps);
                    }
                    else
                    {
                        gFrame.DrawImage(res, rg);
                        eps = new EncoderParameters(1);
                        //如果是GIF这里设置为FrameDimensionTime，如果为TIFF则设置为FrameDimensionPage 
                        eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionTime);
                        BindProperty(res, frame);
                        gif.SaveAdd(frame, eps);
                    }
                }
                eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
                gif.SaveAdd(eps);
            }
        }

        /// <summary> 
        /// 将源图片文件里每一帧的属性设置到新的图片对象里 
        /// </summary> 
        /// <param name="a">源图片帧</param> 
        /// <param name="b">新的图片帧</param> 
        private static void BindProperty(Image a, Image b)
        {
            //这个东西就是每一帧所拥有的属性，可以用GetPropertyItem方法取得这里用为完全复制原有属性所以直接赋值了 
            //顺便说一下这个属性里包含每帧间隔的秒数和透明背景调色板等设置，这里具体那个值对应那个属性大家自己在msdn搜索GetPropertyItem方法说明就有了 
            for (int i = 0; i < a.PropertyItems.Length; i++)
            {
                b.SetPropertyItem(a.PropertyItems[i]);
            }
        }
        #endregion

        #endregion

        #region 压缩

        #region 压缩图片
        /// <summary>
        /// 有损压缩图片   处理不了gif
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小  默认300k</param>
        /// <param name="isFirst">是否是第一次调用</param>
        public static bool CompressImage(string sFile, string dFile, int flag = 60, int size = 300, bool isFirst=true)
        {
            try
            {
                var sufName = sFile.Split(".").LastOrDefault();
                sufName = sufName.Replace("jpg", "jpeg");
                if (sFile.EndsWith("gif") || dFile.EndsWith("gif"))
                {
                    throw new Exception("无法处理gif类型图片");
                }
                //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
                FileInfo firstFileInfo = new FileInfo(sFile);
                if (isFirst == true && firstFileInfo.Length < size * 1024)
                {
                    File.Copy(sFile, dFile);
                    return true;
                }
                using (Image iSource = Image.FromFile(sFile))
                {
                    ImageFormat tFormat = iSource.RawFormat;

                    Bitmap ob = new Bitmap(iSource.Width, iSource.Height);

                    Graphics g = Graphics.FromImage(ob);
                    g.Clear(Color.White);
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(iSource, new Rectangle(0, 0, iSource.Width, iSource.Height), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
                    g.Dispose();

                    //设置压缩质量
                    EncoderParameters ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, new long[1] { flag });

                    try
                    {
                        //找到系统中可用的图片编码器信息
                        ImageCodecInfo myImageCodecInfo = GetImageCoderInfo("image/" + sufName);
                        //如果编码器存在的，可以压缩
                        if (myImageCodecInfo != null)
                        {
                            ob.Save(dFile, myImageCodecInfo, ep);
                            FileInfo fi = new FileInfo(dFile);
                            if (fi.Length > 1024 * size && flag >20)
                            {
                                flag -= 20;
                                CompressImage(sFile, dFile, flag, size, false);
                            }
                        }
                        else
                        {
                            ob.Save(dFile, tFormat);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        iSource?.Dispose();
                        ob?.Dispose();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }
        #endregion

        #region 压缩gif
        //借助 gifSicle
        /// <summary>
        /// 有损压缩gif图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="lossy">压缩质量 0-150 越大质量越差</param>
        /// <param name="scale">缩小比例 0.1-1 越大图片宽高也越大 scale为1 则为原图</param>
        /// <param name="optimize">压缩方式 O1 O2 O3</param>
        /// <param name="gap">抽帧步长  越大抽帧越少 动图越流畅，为0时不抽帧</param>
        /// <returns>是否成功,失败信息</returns>
        public static (bool isOk, string message) CompressGif(string sFile, string dFile, int lossy = 150, double scale = 0.5, string optimize = "O2", int gap = 10, string gifSiclePath = "")
        {

            try
            {
                if (!(sFile.EndsWith("gif") && dFile.EndsWith("gif")))
                {
                    throw new Exception("无法处理非gif类型图片");
                }
                var optimizeList = new List<string>() { "O2", "O1", "O3" };
                if (!optimizeList.Contains(optimize))
                {
                    throw new Exception("optimize 错误，仅支持：" + string.Join(optimize, ","));
                }

                if (string.IsNullOrWhiteSpace(gifSiclePath))
                {
                    gifSiclePath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\gifsicle\\gifsicle.exe";
                }
                if (!File.Exists(gifSiclePath)) throw new Exception("未找到gifsicle软件，无法进行压缩。未知文件：" + gifSiclePath);


                #region 抽帧延时
                var deleteFrame = "";
                var delayStr = "";
                if (gap > 0)
                {
                    string cmdInfo = $"{gifSiclePath} --info {sFile}";
                    var info = RunCmd(cmdInfo);
                    var numReg = new Regex("#(\\d+)");
                    var numMatches = numReg.Matches(info);
                    var numMatch = numMatches[numMatches.Count - 1];
                    if (numMatch.Success && numMatch.Groups.Count > 0)
                    {
                        var numStr = numMatch.Groups[numMatch.Groups.Count - 1].Value;
                        var isParse = int.TryParse(numStr, out int num);

                        if (isParse && num > 0)
                        {
                            double delay = 0;
                            deleteFrame = " --delete ";
                            for (int i = gap; i < num;)
                            {
                                //+image #46 864x486
                                //delay 10.03s
                                var reg = new Regex($"image #{i} [\\s\\S.*]+?([\\d+\\.?]+\\d+)s");
                                var match = reg.Match(info);
                                if (match.Success && match.Groups.Count > 0)
                                {
                                    var isParseDouble = double.TryParse(match.Groups[1].Value, out double delayNow);
                                    if (isParseDouble)
                                    {
                                        delay += delayNow;
                                        delay = Math.Round(delay, 2);
                                        delayStr = $"-d {delay * 100}";
                                    }
                                }
                                deleteFrame += $"#{i} ";
                                i += gap;
                            }
                        }
                    }
                }
                #endregion

                string cmd = $"{gifSiclePath} -{optimize} {sFile} {deleteFrame} {delayStr} --scale {scale} --lossy={lossy} -o {dFile}";
                //gifsicle.exe -O2 C:\Users\loger\Desktop\4777.gif  --delete #3 #7 #11 #15 #19 #23 #27 #31 #35 #39 #43 #47 #51 #55 #59 #63 #67 #71 #75 #79 #83 #87 #91 #95 #99 #103 #107 #111 #115  -d 87 --scale 0.5 --lossy=150 -o C:\Users\loger\Desktop\4777-222.gif
                var message = RunCmd(cmd);
                //图片是否通过命令生成
                return (File.Exists(dFile), message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

        }
        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string RunCmd(string cmd)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe", "/c" + cmd)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            var msg = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return msg;
        }
        #endregion
        #endregion

        #region 给图片添加水印
        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">加水印厚图片地址</param>
        /// <param name="text">文字</param>
        /// <param name="size">字体大小-30</param>
        /// <param name="c">颜色</param>
        /// <param name="rotate">水印倾斜，往下倾斜45°，往上倾斜315°</param>
        /// <param name="locationX">水印x开始位置 默认为-1 当前图片宽度-150</param>
        /// <param name="locationY">水印y开始位置 默认为-1 当前图片高度-50</param>
        /// <returns>成功,失败 </returns>
        public static bool AddWaterMarkText(string sFile, string dFile, string text, int size, Color c ,int rotate = 0, int locationX = -1, int locationY = -1)
        {
            //https://www.cnblogs.com/simadi/p/15355480.html
            //https://www.cnblogs.com/li-learning/p/WaterMark.html 旋转
            using (Image image = Image.FromFile(sFile))
            {
                try
                {
                    Bitmap bitmap = new Bitmap(image);
                    int width = bitmap.Width, height = bitmap.Height;
                    Graphics g = Graphics.FromImage(bitmap);
                    g.DrawImage(bitmap, 0, 0);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.ResetTransform();
                    g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);

                    Font crFont = new Font("微软雅黑", size, FontStyle.Bold);
                    SizeF crSize = new SizeF();
                    crSize = g.MeasureString(text, crFont);//文字大小
                    //背景位置(去掉了. 如果想用可以自己调一调 位置.)
                    //graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 255, 255, 255)), (width - crSize.Width) / 2, (height - crSize.Height) / 2, crSize.Width, crSize.Height);
                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(120, c.R, c.G, c.B));
                    //将原点移动 到图片中点
                    var space = GetStrLength(text,size);
                    if (locationX == -1) locationX = width - space;
                    if (locationY == -1) locationY = height - 50;
                    g.TranslateTransform(locationX, locationY);
                    g.RotateTransform(rotate);
                    g.DrawString(text, crFont, semiTransBrush, new PointF(0, 0));
                    //保存文件
                    bitmap.Save(dFile, image.RawFormat);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加图片水印  不支持图片倾斜
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">生成的图片</param>
        /// <param name="waterFile">水印图片</param>
        /// <param name="scale">水印图片的缩放 默认1</param>
        /// <param name="opacity">透明度 0-1</param>
        /// <param name="locationX">水印x开始位置 默认为-1 当前图片宽度 - 水印宽度 - 10</param>
        /// <param name="locationY">水印y开始位置 默认为-1 当前图片高度 - 水印高度 - 10</param>
        /// <returns></returns>
        public static bool AddWaterMarkImg(string sFile,string dFile, string waterFile,double scale = 1, double opacity = 0.8,int locationX = -1, int locationY = -1)
        {
            //https://www.cnblogs.com/shiliang199508/p/9238868.html
            Bitmap sourceImage = new Bitmap(sFile);
            Bitmap waterImage = new Bitmap(waterFile);

            Bitmap bitmap = new Bitmap(sourceImage, sourceImage.Width, sourceImage.Height);

            Graphics g = Graphics.FromImage(bitmap);

            float waterWidth = (float)(waterImage.Width * scale);
            float waterHeight = (float)(waterImage.Height * scale);

            //下面定义一个矩形区域      
            float rectWidth = waterWidth;
            float rectHeight = waterHeight;

            if (locationX == -1) locationX = (int)(sourceImage.Width - rectWidth );
            if (locationY == -1) locationY = (int)(sourceImage.Height - rectHeight);
            //声明矩形域
            RectangleF textArea = new RectangleF(locationX, locationY, rectWidth, rectHeight);
            Bitmap w_bitmap = ChangeOpacity(waterImage, scale,opacity);

            g.DrawImage(w_bitmap, textArea);
            //保存图片
            bitmap.Save(dFile);
            g.Dispose();
            return true;

        }
        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="opacityvalue">透明度</param>
        /// <returns></returns>
        public static Bitmap ChangeOpacity(Image waterImg, double scale,double opacityvalue)
        {

            float[][] nArray ={ new float[] {1, 0, 0, 0, 0},

                                new float[] {0, 1, 0, 0, 0},

                                new float[] {0, 0, 1, 0, 0},

                                new float[] {0, 0, 0, (float)opacityvalue, 0},

                                new float[] {0, 0, 0, 0, 1}};

            ColorMatrix matrix = new ColorMatrix(nArray);

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var waterWidth = (int)(waterImg.Width * scale);
            var waterHeight = (int)(waterImg.Height * scale);

            //缩放水印图片
            Bitmap waterBitMap = new Bitmap(waterWidth, waterHeight);
            Graphics waterG = Graphics.FromImage(waterBitMap);
            waterG.DrawImage(waterImg, 0,0,waterWidth,waterHeight);
            

            Bitmap resultImage = new Bitmap(waterWidth, waterHeight);
            Graphics g = Graphics.FromImage(resultImage);
            g.DrawImage(waterBitMap, new Rectangle(0, 0, waterWidth, waterHeight), 0, 0, waterWidth, waterHeight, GraphicsUnit.Pixel, attributes);

            return resultImage;
        }
        #endregion

        #region 给gif添加水印
        //借助ffmpeg
        /// <summary>
        /// 借助ffmpeg给gif添加文字水印  不支持倾斜
        /// </summary>
        /// <param name="sFile">原图</param>
        /// <param name="dFile">结果图</param>
        /// <param name="text">水印文字</param>
        /// <param name="c">颜色</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="locationX">X开始位置 默认：W-100 W为原图宽度，H为原图高度  W-100 即距离右边100 H-30  即距离底部30</param>
        /// <param name="locationY">Y开始位置 默认：H-30</param>
        /// <param name="ffmpegPath">ffmpeg.exe 的位置</param>
        /// <param name="fontfile">字体位置  通常需要将 C:\Windows\Fonts 中的某个字体加到我们项目中 
        /// 原路径：C:\Windows\Fonts\xxx.ttf  要改成 C\:/Windows/Fonts/xxx.ttf
        /// </param>
        /// <returns></returns>
        public static (bool isOk,string message) AddWaterMarkTextGif(string sFile, string dFile, string text, Color c, int fontsize=16, string locationX = "-1", string locationY = "-1", string ffmpegPath = "",string fontfile = "")
        {
            //ffmpeg -i star.gif -y -vf "drawtext=fontfile=simsun.ttc:text='蕾蕾':x=W-100:y=H-10:fontsize=24:fontcolor=yellow:shadowy=2" drawtext.gif
            if(string.IsNullOrWhiteSpace(text)) throw new Exception("请提供水印文字");
            if (!File.Exists(sFile)) throw new Exception("未找到源文件："+sFile);
            if (string.IsNullOrWhiteSpace(ffmpegPath)) ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            if (string.IsNullOrWhiteSpace(fontfile) || !File.Exists(fontfile)) 
            {
                fontfile = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\simsun.ttc";
                if (!File.Exists(fontfile)) throw new Exception("未找到 fontfile 文件：" + fontfile);
            }
            fontfile = fontfile.Replace("\\", "/").Replace(":", "\\:");//不能是c:\xxx\xx这种形式  要改成这样子xxx/xx
            if (!File.Exists(ffmpegPath)) throw new Exception("未找到 ffmpeg 文件："+ ffmpegPath);
            if (locationX == "-1")
            {
                var space = GetStrLength(text,fontsize);   //字体 16
                locationX = "W-"+ space;    //"W-100"
            }
            if (locationY == "-1") locationY = "H-30";
            var cmd = $"{ffmpegPath} -i {sFile} -y -vf \"drawtext=fontfile='{fontfile}':text='{text}':x={locationX}:y={locationY}:fontsize={fontsize}:fontcolor={c.Name}:shadowy=2\" {dFile}";
            var msg = RunCmd(cmd);
            return (msg.Contains("Error"), msg);
        }
        
        /// <summary>
        /// 借助ffmpeg给gif添加图片水印  仅支持90°倾斜
        /// </summary>
        /// <param name="sFile">原图</param>
        /// <param name="dFile">结果图</param>
        /// <param name="waterFile">水印图 原路径：D:\xxx\xx.png  要改成 D\:/xxx/xx.png</param>
        /// <param name="scaleX">水印图缩放尺寸宽度 默认-1 表原尺寸</param>
        /// <param name="scaleY">水印图缩放尺寸高度 默认-1 表原尺寸，自适应的时候只需指定其中一个 ，另一个保持为-1不变即可，如scalex=500,scaleY=-1</param>
        /// <param name="opacity">水印透明度</param>
        /// <param name="locationX">水印图左上角坐标的X位置 默认-1 即W-w 刚好留有水印的区域</param>
        /// <param name="locationY">水印图左上角坐标的Y位置 默认-1 即H-h 刚好留有水印的区域</param>
        /// <param name="ffmpegPath">ffmpeg.exe 的位置</param>
        /// <param name="transpose">
        /// 旋转角度 只能是90°    rotate也是旋转但是效果不好
        /// 4 默认不旋转 0 逆时针旋转90并垂直反转，1 顺时针旋转90  2 逆时针旋转90  3 顺时针旋转90并垂直翻转
        /// </param>
        /// <returns></returns>
        public static (bool isOk, string message) AddWaterMarkImgGif(string sFile, string dFile, string waterFile, double scaleX = -1,double scaleY = -1, double opacity = 0.8, string locationX = "-1", string locationY = "-1", string ffmpegPath = "",int transpose = 4)
        {
            //ffmpeg -i star.gif -y -vf "movie=disk.png,scale=-1:-1,lut=a=val*0.6[watermark];[in][watermark] overlay=W-w:H-h,split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse[out] " out3.gif
            if (!File.Exists(waterFile)) throw new Exception("未找到源文件：" + waterFile);
            if (!File.Exists(sFile)) throw new Exception("未找到源文件：" + sFile);
            if (string.IsNullOrWhiteSpace(ffmpegPath)) ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            if (!File.Exists(ffmpegPath)) throw new Exception("未找到 ffmpeg 文件：" + ffmpegPath);
            if (locationX == "-1") locationX = "W-w";
            if (locationY == "-1") locationY = "H-h";
            waterFile = waterFile.Replace("\\", "/").Replace(":", "\\:");//不能是c:\xxx\xx这种形式  要改成这样子xxx/xx
            var cmd = $"{ffmpegPath} -i {sFile} -y -vf \"movie='{waterFile}',scale={scaleX}:{scaleY},transpose={transpose},lut=a=val*{opacity}[watermark];[in][watermark] overlay={locationX}:{locationY},split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse[out]\" {dFile}";
            var msg = RunCmd(cmd);
            return (msg.Contains("Error"), msg);
        }
        #endregion

        #region 全屏水印
        /// <summary>
        /// 借助ffmpeg给gif添加文字水印  支持倾斜、全屏
        /// </summary>
        /// <param name="sFile">原图</param>
        /// <param name="dFile">结果图</param>
        /// <param name="isFull">水印是否全屏</param>
        /// <param name="text">水印文字</param>
        /// <param name="c">水印颜色</param>
        /// <param name="fontsize">水印字体大小 默认为16</param>
        /// <param name="ffmpegPath">ffmpeg.exe 的位置  默认为空，自动寻找Tools/ffmpeg/ffmpeg.exe</param>
        /// <param name="opacity">整个水印的透明度  默认为0.8</param>
        /// <param name="rotate">全屏下水印内文字的倾斜度</param>
        /// <param name="locationX">非全屏下，整个水印的位置X，默认为W-w;全屏下，默认为0</param>
        /// <param name="locationY">非全屏下，整个水印的位置Y，默认为H-h;全屏下，默认为0</param>
        /// <param name="waterWidth">非全屏下，整个水印宽度，默认0，自动根据水印长度来确定宽度；全屏状态下水印宽度等于原图宽度</param>
        /// <param name="waterHeight">非全屏下，整个水印高度，默认0，0时自动为30。全屏状态下水印宽度等于原图高度</param>
        /// <param name="scaleX">非全屏下，水印图缩放尺寸高度 默认-1 表原尺寸；全屏下，默认为-1</param>
        /// <param name="scaleY">非全屏下，水印图缩放尺寸高度 默认-1 表原尺寸；全屏下，默认为-1；自适应的时候只需指定其中一个 ，另一个保持为-1不变即可，如scalex=500,scaleY=-1</param>
        /// <param name="lineHeight">行高  调节水印疏密</param>
        /// <param name="space">水印前后</param>
        /// <returns></returns>
        public static (bool isOk, string message) AddWaterMarkTextGif(string sFile, string dFile, bool isFull, string text, Color c, int fontsize = 16, string ffmpegPath = "", double opacity = 0.8, int rotate = 0, string locationX = "-1", string locationY = "-1", int waterWidth = 0, int waterHeight = 0, double scaleX = -1, double scaleY = -1,int lineHeight = 10,int space =0)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new Exception("请提供水印文字");
            if (!File.Exists(sFile)) throw new Exception("未找到源文件：" + sFile);
            if (string.IsNullOrWhiteSpace(ffmpegPath)) ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            if (!File.Exists(ffmpegPath)) throw new Exception("未找到 ffmpeg 文件：" + ffmpegPath);
            Image gifImg = Image.FromFile(sFile);
            var dir = AppDomain.CurrentDomain.BaseDirectory + $"Image";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            //临时水印位置
            var waterFile = dir+$"\\{ Guid.NewGuid().ToString()}.png";
            if (isFull)
            {
                waterWidth = gifImg.Width;
                waterHeight = gifImg.Height;
                DrawTextPng(waterWidth, waterHeight, isFull, waterFile, text, new Font("微软雅黑", fontsize), c, rotate,lineHeight: lineHeight, space:space);
                locationX = "0";
                locationY = "0";
                scaleX = -1;
                scaleY = -1;
            }
            else
            {
                if (waterWidth <= 0) waterWidth = GetStrLength(text, fontsize);
                if (waterHeight <= 0) waterHeight = 50;
                DrawTextPng(waterWidth, waterHeight, isFull, waterFile, text, new Font("微软雅黑", fontsize), c, rotate, lineHeight: lineHeight, space: space);
                if (locationX == "-1") locationX = "W-w";
                if (locationY == "-1") locationY = "H-h";
            }
            if (!File.Exists(waterFile)) throw new Exception("生成水印失败");
            //waterFile 要处理一下
            var waterFileCmd = waterFile.Replace("\\", "/").Replace(":", "\\:");

            var cmd = $"{ffmpegPath} -i {sFile} -y -vf \"movie='{waterFileCmd}',scale={scaleX}:{scaleY},lut=a=val*{opacity}[watermark];[in][watermark] overlay={locationX}:{locationY},split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse[out]\" {dFile}";
            var msg = RunCmd(cmd);
            File.Delete(waterFile);
            return (msg.Contains("Error"), msg);
        }

        /// <summary>
        /// 图片添加文字  全屏 倾斜
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="isFull">是否全屏</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="text">文字内容</param>
        /// <param name="font">字体</param>
        /// <param name="color">字体颜色</param>
        /// <param name="rotate">全屏下，文字倾斜</param>
        /// <param name="x">非全屏下，文字添加位置的起始坐标X</param>
        /// <param name="y">非全屏下，文字添加位置的起始坐标Y</param>
        /// <param name="lineHeight">行高  调节水印疏密</param>
        /// <param name="space">水印前后</param>
        public static void DrawTextPng(int width, int height, bool isFull, string savePath, string text, Font font, Color color, int rotate = 0, int x = 0, int y = 0, int lineHeight = 10, int space = 0)
        {
            space = space + GetStrLength(text, (int)font.Size);
            if (!isFull)
            {
                var radian = (2 * Math.PI / 360 * rotate);
                height = (int)(Math.Cos(radian) * space);   //计算对边
                width = Math.Abs((int)(Math.Sin(radian) * space));    //计算直角边
            }
            Bitmap bitmap = new Bitmap(width, height);
            //图片的宽度与高度
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            g.DrawImage(bitmap, 0, 0);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
            //字体
            if (font == null) font = new Font("微软雅黑", 22, FontStyle.Bold);
            //颜色
            SolidBrush semiTransBrush = new SolidBrush(color);
            
            if (!isFull)
            {
                g.TranslateTransform(0, height);
                g.RotateTransform(rotate);
                if (x == 0) x += 20;
                if (y == 0) y -= 10;
                g.DrawString(text, font, semiTransBrush, x, y);
            }
            else
            {
                //将原点移动 到图片中点
                //g.TranslateTransform(0, height);
                //以原点为中心 转 -45度
                g.RotateTransform(rotate);
                var loopCount = (int)Math.Ceiling((decimal)width / (space + 20) / 2) + 4;   // +20 文字前后的间距   取一半再多取4个
                var icount = (int)Math.Ceiling((decimal)height / space / 2) * 10 + 50;   // 多少行字 *10 为了扩大行间距     取一半再多取20个，为了再多循环50/5次
                for (int i = -icount; i <= icount;)
                {
                    for (int j = -loopCount; j < loopCount; j++)    //列
                    {
                        g.DrawString(text, font, semiTransBrush, i * 10 + space * j, i * 20);
                    }
                    i += lineHeight;
                }
            }
            //保存文件
            bitmap.Save(savePath, ImageFormat.Png);
        }

        /// <summary>
        /// 普通图片绘制全屏水印  倾斜
        /// </summary>
        /// <param name="sFile">原图</param>
        /// <param name="dFile">结果图</param>
        /// <param name="text">水印内容</param>
        /// <param name="font">水印字体大小</param>
        /// <param name="color">水印颜色</param>
        /// <param name="rotate">水印文字的倾斜</param>
        /// <param name="lineHeight">行高  调节水印疏密</param>
        /// <param name="space">水印前后</param>
        public static void DrawFullText(string sFile, string dFile, string text, Color color, Font font = null, int rotate = 0, int lineHeight = 10, int space = 0)
        {
            Bitmap bitmap = new Bitmap(sFile);
            //图片的宽度与高度
            Graphics g = Graphics.FromImage(bitmap);
            //字体
            if (font == null) font = new Font("微软雅黑", 22, FontStyle.Bold);
            //颜色
            SolidBrush semiTransBrush = new SolidBrush(color);

            //将原点移动 到图片中点
            //g.TranslateTransform(0, height);
            //以原点为中心 转 -45度
            g.RotateTransform(rotate);
            space = space + GetStrLength(text, (int)font.Size);
            var loopCount = (int)Math.Ceiling((decimal)bitmap.Width / (space + 20) / 2) + 4;   // +20 文字前后的间距   取一半再多取4个
            var icount = (int)Math.Ceiling((decimal)bitmap.Height / space / 2) * 10 + 50;   // 多少行字 *10 为了扩大行间距     取一半再多取20个，为了再多循环50/5次
            for (int i = -icount; i <= icount;)
            {
                for (int j = -loopCount; j < loopCount; j++)    //列
                {
                    g.DrawString(text, font, semiTransBrush, i * 10 + space * j, i * 20);
                }
                i += lineHeight;
            }

            //保存文件
            bitmap.Save(dFile);
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <returns></returns>
        public static int GetStrLength(string inputString, int fontSize)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen * (int)(fontSize * 0.75) + fontSize; // 取四分之三
        }
        #endregion

        #region 二维码、条形码
        /// <summary>
        /// 文字转条形码
        /// </summary>
        /// <param name="text">二维码内容</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="dFile">结果图</param>
        /// <param name="codeFormat"></param>
        public static void CreateBarcode(string text, int width, int height, string dFile, string codeFormat = "Code128")
        {
            BarcodeWriter bw = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions
            {
                Height = height,
                Width = width,
                PureBarcode = true,
            };
            bw.Options = encodingOptions;
            //使用ITF 格式，不能被现在常用的支付宝、微信扫出来
            //如果想生成可识别的可以使用 CODE_128 格式
            if (codeFormat == "CodeBar")
            {
                bw.Format = BarcodeFormat.CODABAR;
            }
            else if (codeFormat == "Code128")
            {
                bw.Format = BarcodeFormat.CODE_128;
            }
            else if (codeFormat == "Code39")
            {
                bw.Format = BarcodeFormat.CODE_39;
            }
            else
            {
                bw.Format = BarcodeFormat.CODE_128;
            }
            Bitmap bm = bw.Write(text);
            bm.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            bm.Dispose();
        }

        /// <summary>
        /// 生成二维码 并保存到filePath中
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="dFile">结果图</param>
        /// <returns></returns>
        public static void CreateQRCode(string text, int width, int height, string dFile)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                DisableECI = true,//设置内容编码
                CharacterSet = "UTF-8", 
                Width = width, //设置二维码的宽度和高度
                Height = height,
                Margin = 1//设置二维码的边距,单位不是固定像素
            };

            writer.Options = options;
            Bitmap bm = writer.Write(text);
            bm.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            bm.Dispose();
        }

        /// <summary>
        /// 生成中间带有图片的二维码图片  
        /// 参考：http://www.cnblogs.com/Interkey/p/qrcode.html
        /// </summary>
        /// <param name="text">要生成二维码包含的信息</param>
        /// <param name="sFile">要生成到二维码中间的图片</param>
        /// <param name="width">生成的二维码宽度</param>
        /// <param name="height">生成的二维码高度</param>
        /// <param name="dFile">结果图</param>
        /// <returns></returns>
        public static void CreateQRCode(string text, string sFile, int width, int height, string dFile)
        {
            if (string.IsNullOrEmpty(text)) throw new Exception("请提供内容");
            if (!File.Exists(sFile))
            {
                CreateQRCode(text,width,height,dFile);
                return;
            }

            ////本文地址：
            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            //生成二维码
            BitMatrix bm = mutiWriter.encode(text, BarcodeFormat.QR_CODE, width, height, hint);
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            Bitmap bitmap = barcodeWriter.Write(bm);

            //获取二维码实际尺寸（去掉二维码两边空白后的实际尺寸）
            int[] rectangle = bm.getEnclosingRectangle();

            var middleImg = new Bitmap(sFile);
            //计算插入图片的大小和位置
            int middleImgW = Math.Min((int)(rectangle[2] / 3.5), middleImg.Width);
            int middleImgH = Math.Min((int)(rectangle[3] / 3.5), middleImg.Height);
            int middleImgL = (bitmap.Width - middleImgW) / 2;
            int middleImgT = (bitmap.Height - middleImgH) / 2;

            //将img转换成bmp格式，否则后面无法创建 Graphics对象
            Bitmap bmpimg = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(bitmap, 0, 0);
            }

            //在二维码中插入图片
            Graphics myGraphic = Graphics.FromImage(bmpimg);
            //白底
            myGraphic.FillRectangle(Brushes.White, middleImgL, middleImgT, middleImgW, middleImgH);
            myGraphic.DrawImage(middleImg, middleImgL, middleImgT, middleImgW, middleImgH);

            bmpimg.Save(dFile);
        }
        /// <summary>
        /// 解析二维码、条形码的内容
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static string GetCodeText(string sFile)
        {
            BarcodeReader reader = new BarcodeReader();
            reader.Options.CharacterSet = "UTF-8";
            Bitmap map = new Bitmap(sFile);
            Result result = reader.Decode(map);
            return result == null ? "" : result.Text;
        }
        #endregion
    }
}
