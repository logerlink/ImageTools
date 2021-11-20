using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageTools.Test
{
    public class Tests
    {
        public string CityPic { get; set; }
        public string StarPic { get; set; }
        public string WaterPic { get; set; }
        public string SharkPic { get; set; }
        public string CarPic { get; set; }
        public string DiskPic { get; set; }
        [SetUp]
        public void Setup()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory + "Image\\";
            CityPic = dir + "city.gif";
            StarPic = dir + "star.gif";
            WaterPic = dir + "water.jpg";
            SharkPic = dir + "shark.png";
            CarPic = dir + "car.jpg";
            DiskPic = dir + "disk.png";
        }

        #region 字节与文件
        [Test]
        public void TestFile2Bytes()
        {
            //获取字节数组
            var bytes = ImageHelper.File2Bytes(CityPic);
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test]
        public void TestBytes2File()
        {
            var bytes = ImageHelper.File2Bytes(CityPic);
            //将city的字节数组写到cityTemp文件中
            var tempPath = CityPic.Replace("city", "cityTemp");
            ImageHelper.Bytes2File(tempPath, bytes);
            Assert.IsTrue(File.Exists(tempPath));
        }
        #endregion

        #region 文件增删改查
        [Test]
        public void TestFileInfo()
        {
            //将city复制到 city_copy
            var tempPath = CityPic.Replace("city", "city_copy");
            ImageHelper.FileCopyTo(CityPic, tempPath);
            Assert.IsTrue(File.Exists(tempPath));

            //将city_copy 重命名为 city_rename
            var renamePath = tempPath.Replace("copy", "rename");
            ImageHelper.FileChangeTo(tempPath, renamePath);
            Assert.IsTrue(File.Exists(renamePath));
            //删除 city_rename
            ImageHelper.FileDelete(renamePath);
            Assert.IsFalse(File.Exists(renamePath));
            //获取city的信息
            var fileInfo = ImageHelper.FileGetInfo(CityPic);
            Console.WriteLine(fileInfo);
            Assert.IsNotNull(fileInfo);
        }
        #endregion

        #region 网络下载图片
        [Test]
        public void TestDownloadImg()
        {
            var baidu = "https://www.baidu.com/img/flexible/logo/pc/result.png";
            var dFile = ImageHelper.DownloadImageByUrl(baidu);
            Console.WriteLine(dFile);
            Assert.IsTrue(File.Exists(dFile));
        }
        #endregion

        #region base64与图片
        /// <summary>
        /// 图片转base64 支持图片和gif
        /// </summary>
        [Test]
        public void TestImageToBase64()
        {
            var base64Str = ImageHelper.ImageToBase64(WaterPic);
            Assert.IsNotEmpty(base64Str);
        }
        /// <summary>
        /// base64转图片或gif
        /// </summary>
        [Test]
        public void TestBase64ToImage()
        {
            var base64Str = "data:image/jpg;base64,....";
            //将 TestImageToBase64 生成的base64字符串  转成图片
            var waterBase64 = WaterPic.Replace("water","water_base64");
            ImageHelper.Base64ToImage(base64Str, waterBase64);
            Assert.IsTrue(File.Exists(waterBase64));
        }

        #endregion

        #region 缩略图 无压缩
        /// <summary>
        /// 生成普通图片的缩略图
        /// </summary>
        [Test]
        public void TestResizeImage()
        {
            var waterThum = WaterPic.Replace("water", "water_thum");
            ImageHelper.ResizeImage(WaterPic, waterThum);
            Assert.IsTrue(File.Exists(waterThum));
        }
        /// <summary>
        /// 生成gif的缩略图
        /// </summary>
        [Test]
        public void TestResizeGif()
        {
            var starThum = StarPic.Replace("star", "star_thum");
            ImageHelper.ResizeGif(StarPic, starThum);
            Assert.IsTrue(File.Exists(starThum));
        }

        #endregion

        #region 压缩
        /// <summary>
        /// 压缩图片
        /// </summary>
        [Test]
        public void TestCompressImage()
        {
            // 会增大  有原来的1.9M 增大到 2.7M
            var sharkCompress = SharkPic.Replace("shark", "shark_compress");
            ImageHelper.CompressImage(SharkPic, sharkCompress);
            Assert.IsTrue(File.Exists(sharkCompress));

            // 压缩成功  3.6M 压缩到800K
            var carCompress = CarPic.Replace("car", "car_compress");
            ImageHelper.CompressImage(CarPic, carCompress);
            Assert.IsTrue(File.Exists(carCompress));
        }
        /// <summary>
        /// 借助gifsicle 压缩gif
        /// </summary>
        [Test]
        public void TestCompressGif()
        {
            // 压缩成功 9M 压缩到1M   图片大小缩小一半  画质差
            var cityCompress = CityPic.Replace("city", "city_compress");
            var result = ImageHelper.CompressGif(CityPic, cityCompress);
            Assert.IsTrue(result.isOk);

            // 压缩成功 9M 压缩到3M   图片大小不变  画质较好
            var cityScaleCompress = CityPic.Replace("city", "city_compress_scale");
            result = ImageHelper.CompressGif(CityPic, cityScaleCompress,scale:1,gap:0);
            Assert.IsTrue(result.isOk);
        }
        [Test]
        public void TestCompressGifgg()
        {
            var sfile = @"C:\Users\loger\Desktop\ImageTools\ImageTools.Test\bin\Debug\netcoreapp3.1\Image\star_fullMarkGifChinese.gif";
            var dFile = sfile.Replace("star_fullMarkGifChinese", "star_fullMarkGifChinese_compress");
            var result = ImageHelper.CompressGif(sfile, dFile,scale:0.7,gap:0);
            Assert.IsTrue(result.isOk);
        }

        #endregion

        #region 图片添加水印
        /// <summary>
        /// 给图片添加文字水印
        /// </summary>
        [Test]
        public void TestWatermarkText()
        {
            var waterMarkText = WaterPic.Replace("water", "water_markText");
            //添加文字水印：logerlink，字号12，右下角，字体白色，字体倾斜-45°
            ImageHelper.AddWaterMarkText(WaterPic, waterMarkText, "logerlink",12, System.Drawing.Color.White,-45);
            Assert.IsTrue(File.Exists(waterMarkText));
            
        }
        /// <summary>
        /// 给图片添加图片水印
        /// </summary>
        [Test]
        public void TestWatermarkImg()
        {
            var waterMarkImg = WaterPic.Replace("water", "water_markImg");
            //添加图片水印，水印大小缩放80%，水印透明度：80%
            ImageHelper.AddWaterMarkImg(WaterPic, waterMarkImg, DiskPic,0.8,0.8);
            Assert.IsTrue(File.Exists(waterMarkImg));
        }

        #endregion

        #region gif添加水印
        /// <summary>
        /// 借助ffmpeg给gif添加文字水印
        /// </summary>
        [Test]
        public void TestAddWaterMarkTextGif()
        {
            var pic = StarPic.Replace("star", "star_markGif");
            //添加文字水印
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "logerlink@outlook.com",Color.AliceBlue);
            pic = StarPic.Replace("star", "star_markGifChinese");
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "这是中文水印，仅供参考", Color.AliceBlue);
            pic = StarPic.Replace("star", "star_markGifCenter");
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            var fontfilePath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\simsun.ttc";
            fontfilePath = fontfilePath.Replace("\\", "/").Replace(":", "\\:");
            //添加文字水印  指定ffmpeg和fontfile 
            //"W/2" "H/2" 中心位置
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "logerlink@outlook.com", Color.AliceBlue, 30,"W/2","H/2", ffmpegPath, fontfilePath);
        }
        /// <summary>
        /// 借助ffmpeg给gif添加图片水印
        /// </summary>
        [Test]
        public void TestAddWaterMarkImgGif()
        {
            var pic = StarPic.Replace("star", "star_markImgGif");
            //添加图片水印
            ImageHelper.AddWaterMarkImgGif(StarPic, pic, DiskPic,0);
            pic = StarPic.Replace("star", "star_markImgGifCenter");
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            //添加图片水印  指定ffmpeg   "W/2" "H/2" 中心位置  顺时针旋转90°
            ImageHelper.AddWaterMarkImgGif(StarPic, pic, DiskPic,200,-1,0.5,"W/2","H/2",ffmpegPath,1);
        }
        #endregion

        #region 全屏水印
        /// <summary>
        /// 测试生成Gif全屏水印
        /// </summary>
        [Test]
        public void TestFullMarkGif()
        {
            var pic = StarPic.Replace("star", "star_fullMarkGif");
            //ImageHelper.AddWaterMarkTextGif(StarPic, pic, true, "logerlink@outlook.com", Color.AliceBlue);
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            //pic = StarPic.Replace("star", "star_fullMarkGifChinese");
            //ImageHelper.AddWaterMarkTextGif(StarPic, pic, true, "这是中文水印，仅供参考", Color.AliceBlue, 20, ffmpegPath, rotate: -45);

            pic = StarPic.Replace("star", "star_fullMarkGifChineseNot");
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, false, "这是中文非全屏水印，仅供参考", Color.AliceBlue, 20, ffmpegPath,rotate:-45);
        }

        /// <summary>
        /// 测试生成图片全屏水印
        /// </summary>
        [Test]
        public void TestFullMarkImage()
        {
            var pic = WaterPic.Replace("water", "water_fullMark");
            ImageHelper.DrawFullText(WaterPic, pic, "logerlink@outlook.com", Color.AliceBlue,space:50);
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            pic = WaterPic.Replace("water", "water_fullMarkChinese");
            ImageHelper.DrawFullText(WaterPic, pic, "这是中文水印，仅供参考", Color.AliceBlue,new Font("Consolas",16),-45,5);
        }
        #endregion

        #region 二维码、条形码
        /// <summary>
        /// 生成二维码、条形码、带图片的二维码
        /// </summary>
        [Test]
        public void TestCodeImage()
        {
            var pic = WaterPic.Replace("water", "qrcode");
            ImageHelper.CreateQRCode("https://www.baidu.com/",250,250,pic);

            pic = WaterPic.Replace("water", "barcode");
            ImageHelper.CreateBarcode("https://www.baidu.com/", 400, 100, pic);

            pic = WaterPic.Replace("water", "qrcode_img");
            ImageHelper.CreateQRCode("https://www.baidu.com/",WaterPic,250,250,pic);


            
        }
        /// <summary>
        /// 解析二维码、条形码的内容
        /// </summary>
        [Test]
        public void TestGetCodeText()
        {
            var pic = WaterPic.Replace("water", "qrcode");
            var result = ImageHelper.GetCodeText(pic);
            Console.WriteLine(result);

            pic = WaterPic.Replace("water", "barcode");
            result = ImageHelper.GetCodeText(pic);
            Console.WriteLine(result);
        }
        #endregion

        [Test]
        public async Task TestParallelAsync()
        {
            var listOne = Enumerable.Range(0, 30);
            var list = Enumerable.Range(0,10000);
            var listResult = new List<int>();
            var listResult1 = new List<int>();
            foreach (var a in listOne.Take(5))
            {
                Parallel.ForEach(list.Take(5), item =>
                {
                    Task.Delay(200).Wait();
                    listResult.Add(item);
                });
            }
            Console.WriteLine(listResult.Count);


            foreach (var a in listOne.Take(5))
            {
                foreach (var item in list.Take(5))
                {
                    await Task.Delay(200);
                    listResult1.Add(item);
                }
            }
            Console.WriteLine(listResult1.Count);

        }
    }
}