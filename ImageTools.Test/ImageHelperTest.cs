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

        #region �ֽ����ļ�
        [Test]
        public void TestFile2Bytes()
        {
            //��ȡ�ֽ�����
            var bytes = ImageHelper.File2Bytes(CityPic);
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test]
        public void TestBytes2File()
        {
            var bytes = ImageHelper.File2Bytes(CityPic);
            //��city���ֽ�����д��cityTemp�ļ���
            var tempPath = CityPic.Replace("city", "cityTemp");
            ImageHelper.Bytes2File(tempPath, bytes);
            Assert.IsTrue(File.Exists(tempPath));
        }
        #endregion

        #region �ļ���ɾ�Ĳ�
        [Test]
        public void TestFileInfo()
        {
            //��city���Ƶ� city_copy
            var tempPath = CityPic.Replace("city", "city_copy");
            ImageHelper.FileCopyTo(CityPic, tempPath);
            Assert.IsTrue(File.Exists(tempPath));

            //��city_copy ������Ϊ city_rename
            var renamePath = tempPath.Replace("copy", "rename");
            ImageHelper.FileChangeTo(tempPath, renamePath);
            Assert.IsTrue(File.Exists(renamePath));
            //ɾ�� city_rename
            ImageHelper.FileDelete(renamePath);
            Assert.IsFalse(File.Exists(renamePath));
            //��ȡcity����Ϣ
            var fileInfo = ImageHelper.FileGetInfo(CityPic);
            Console.WriteLine(fileInfo);
            Assert.IsNotNull(fileInfo);
        }
        #endregion

        #region ��������ͼƬ
        [Test]
        public void TestDownloadImg()
        {
            var baidu = "https://www.baidu.com/img/flexible/logo/pc/result.png";
            var dFile = ImageHelper.DownloadImageByUrl(baidu);
            Console.WriteLine(dFile);
            Assert.IsTrue(File.Exists(dFile));
        }
        #endregion

        #region base64��ͼƬ
        /// <summary>
        /// ͼƬתbase64 ֧��ͼƬ��gif
        /// </summary>
        [Test]
        public void TestImageToBase64()
        {
            var base64Str = ImageHelper.ImageToBase64(WaterPic);
            Assert.IsNotEmpty(base64Str);
        }
        /// <summary>
        /// base64תͼƬ��gif
        /// </summary>
        [Test]
        public void TestBase64ToImage()
        {
            var base64Str = "data:image/jpg;base64,....";
            //�� TestImageToBase64 ���ɵ�base64�ַ���  ת��ͼƬ
            var waterBase64 = WaterPic.Replace("water","water_base64");
            ImageHelper.Base64ToImage(base64Str, waterBase64);
            Assert.IsTrue(File.Exists(waterBase64));
        }

        #endregion

        #region ����ͼ ��ѹ��
        /// <summary>
        /// ������ͨͼƬ������ͼ
        /// </summary>
        [Test]
        public void TestResizeImage()
        {
            var waterThum = WaterPic.Replace("water", "water_thum");
            ImageHelper.ResizeImage(WaterPic, waterThum);
            Assert.IsTrue(File.Exists(waterThum));
        }
        /// <summary>
        /// ����gif������ͼ
        /// </summary>
        [Test]
        public void TestResizeGif()
        {
            var starThum = StarPic.Replace("star", "star_thum");
            ImageHelper.ResizeGif(StarPic, starThum);
            Assert.IsTrue(File.Exists(starThum));
        }

        #endregion

        #region ѹ��
        /// <summary>
        /// ѹ��ͼƬ
        /// </summary>
        [Test]
        public void TestCompressImage()
        {
            // ������  ��ԭ����1.9M ���� 2.7M
            var sharkCompress = SharkPic.Replace("shark", "shark_compress");
            ImageHelper.CompressImage(SharkPic, sharkCompress);
            Assert.IsTrue(File.Exists(sharkCompress));

            // ѹ���ɹ�  3.6M ѹ����800K
            var carCompress = CarPic.Replace("car", "car_compress");
            ImageHelper.CompressImage(CarPic, carCompress);
            Assert.IsTrue(File.Exists(carCompress));
        }
        /// <summary>
        /// ����gifsicle ѹ��gif
        /// </summary>
        [Test]
        public void TestCompressGif()
        {
            // ѹ���ɹ� 9M ѹ����1M   ͼƬ��С��Сһ��  ���ʲ�
            var cityCompress = CityPic.Replace("city", "city_compress");
            var result = ImageHelper.CompressGif(CityPic, cityCompress);
            Assert.IsTrue(result.isOk);

            // ѹ���ɹ� 9M ѹ����3M   ͼƬ��С����  ���ʽϺ�
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

        #region ͼƬ���ˮӡ
        /// <summary>
        /// ��ͼƬ�������ˮӡ
        /// </summary>
        [Test]
        public void TestWatermarkText()
        {
            var waterMarkText = WaterPic.Replace("water", "water_markText");
            //�������ˮӡ��logerlink���ֺ�12�����½ǣ������ɫ��������б-45��
            ImageHelper.AddWaterMarkText(WaterPic, waterMarkText, "logerlink",12, System.Drawing.Color.White,-45);
            Assert.IsTrue(File.Exists(waterMarkText));
            
        }
        /// <summary>
        /// ��ͼƬ���ͼƬˮӡ
        /// </summary>
        [Test]
        public void TestWatermarkImg()
        {
            var waterMarkImg = WaterPic.Replace("water", "water_markImg");
            //���ͼƬˮӡ��ˮӡ��С����80%��ˮӡ͸���ȣ�80%
            ImageHelper.AddWaterMarkImg(WaterPic, waterMarkImg, DiskPic,0.8,0.8);
            Assert.IsTrue(File.Exists(waterMarkImg));
        }

        #endregion

        #region gif���ˮӡ
        /// <summary>
        /// ����ffmpeg��gif�������ˮӡ
        /// </summary>
        [Test]
        public void TestAddWaterMarkTextGif()
        {
            var pic = StarPic.Replace("star", "star_markGif");
            //�������ˮӡ
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "logerlink@outlook.com",Color.AliceBlue);
            pic = StarPic.Replace("star", "star_markGifChinese");
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "��������ˮӡ�������ο�", Color.AliceBlue);
            pic = StarPic.Replace("star", "star_markGifCenter");
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            var fontfilePath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\simsun.ttc";
            fontfilePath = fontfilePath.Replace("\\", "/").Replace(":", "\\:");
            //�������ˮӡ  ָ��ffmpeg��fontfile 
            //"W/2" "H/2" ����λ��
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, "logerlink@outlook.com", Color.AliceBlue, 30,"W/2","H/2", ffmpegPath, fontfilePath);
        }
        /// <summary>
        /// ����ffmpeg��gif���ͼƬˮӡ
        /// </summary>
        [Test]
        public void TestAddWaterMarkImgGif()
        {
            var pic = StarPic.Replace("star", "star_markImgGif");
            //���ͼƬˮӡ
            ImageHelper.AddWaterMarkImgGif(StarPic, pic, DiskPic,0);
            pic = StarPic.Replace("star", "star_markImgGifCenter");
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            //���ͼƬˮӡ  ָ��ffmpeg   "W/2" "H/2" ����λ��  ˳ʱ����ת90��
            ImageHelper.AddWaterMarkImgGif(StarPic, pic, DiskPic,200,-1,0.5,"W/2","H/2",ffmpegPath,1);
        }
        #endregion

        #region ȫ��ˮӡ
        /// <summary>
        /// ��������Gifȫ��ˮӡ
        /// </summary>
        [Test]
        public void TestFullMarkGif()
        {
            var pic = StarPic.Replace("star", "star_fullMarkGif");
            //ImageHelper.AddWaterMarkTextGif(StarPic, pic, true, "logerlink@outlook.com", Color.AliceBlue);
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            //pic = StarPic.Replace("star", "star_fullMarkGifChinese");
            //ImageHelper.AddWaterMarkTextGif(StarPic, pic, true, "��������ˮӡ�������ο�", Color.AliceBlue, 20, ffmpegPath, rotate: -45);

            pic = StarPic.Replace("star", "star_fullMarkGifChineseNot");
            ImageHelper.AddWaterMarkTextGif(StarPic, pic, false, "�������ķ�ȫ��ˮӡ�������ο�", Color.AliceBlue, 20, ffmpegPath,rotate:-45);
        }

        /// <summary>
        /// ��������ͼƬȫ��ˮӡ
        /// </summary>
        [Test]
        public void TestFullMarkImage()
        {
            var pic = WaterPic.Replace("water", "water_fullMark");
            ImageHelper.DrawFullText(WaterPic, pic, "logerlink@outlook.com", Color.AliceBlue,space:50);
            var ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Tools\\ffmpeg\\ffmpeg.exe";
            pic = WaterPic.Replace("water", "water_fullMarkChinese");
            ImageHelper.DrawFullText(WaterPic, pic, "��������ˮӡ�������ο�", Color.AliceBlue,new Font("Consolas",16),-45,5);
        }
        #endregion

        #region ��ά�롢������
        /// <summary>
        /// ���ɶ�ά�롢�����롢��ͼƬ�Ķ�ά��
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
        /// ������ά�롢�����������
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