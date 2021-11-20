using ImageTools.UI.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    public class WaterMarkTextToImgVM : BaseHandleFile
    {
        public WaterMarkTextToImgVM()
        {
            Text = "我是水印，kkkkkkkkk";
            FontSize = 16;
            FontColor = "Red";
            Rotate = -45;
            LocationX = -1;
            LocationY = -1;
            LineHeight = 10;
            Space = 0;
            Icon = this.GetType().Name;
        }
        public string _text;
        /// <summary>
        /// 水印文字
        /// </summary>
        public string Text { get => _text; set { _text = value; OnPropertyChanged("Text"); } }

        public int _fontSize;
        /// <summary>
        /// 水印大小
        /// </summary>
        public int FontSize { get => _fontSize; set { _fontSize = value; OnPropertyChanged("FontSize"); } }

        public string _fontColor;
        /// <summary>
        /// 水印颜色
        /// </summary>
        public string FontColor { get => _fontColor; set { _fontColor = value; OnPropertyChanged("FontColor"); } }

        public int _rotate;
        /// <summary>
        /// 文字倾斜
        /// </summary>
        public int Rotate { get => _rotate; set { _rotate = value; OnPropertyChanged("Rotate"); } }

        public bool _isFull;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFull { get => _isFull; set { _isFull = value; OnPropertyChanged("IsFull"); } }

        #region 非全屏
        public int _locationX;
        /// <summary>
        /// 水印左上角的坐标X
        /// </summary>
        public int LocationX { get => _locationX; set { _locationX = value; OnPropertyChanged("LocationX"); } }
        public int _locationY;
        /// <summary>
        /// 水印左上角的坐标Y
        /// </summary>
        public int LocationY { get => _locationY; set { _locationY = value; OnPropertyChanged("LocationY"); } }
        #endregion

        #region 全屏
        public int _lineHeight;
        /// <summary>
        /// 行高
        /// </summary>
        public int LineHeight { get => _lineHeight; set { _lineHeight = value; OnPropertyChanged("LineHeight"); } }
        public int _space;
        /// <summary>
        /// 水印间隔
        /// </summary>
        public int Space { get => _space; set { _space = value; OnPropertyChanged("Space"); } }
        #endregion


        public override void HandleFile()
        {
            var sb = new StringBuilder();
            var listTemp = new ObservableCollection<FileModel>();
            sb.AppendLine($"【{DateTime.Now}】文件开始处理。");
            try
            {
                IsHandle = true;
                var filePathList = new List<string>();
                foreach (var item in FileModelList) filePathList.Add(item.FilePath);
                if (OutPath == "[桌面]") OutPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                FileResultList.Clear();
                Parallel.ForEach(filePathList, item => {
                    try
                    {
                        if (item.ToUpper().EndsWith("GIF") || item.ToUpper().EndsWith("TIFF"))
                        {
                            sb.AppendLine($"【Failed】文件{item}不支持处理。不可裁剪Gif或Tiff类型！");
                            return;
                        }
                        FileInfo fileInfo = new FileInfo(item);
                        var fileName = $"\\{Icon}{Guid.NewGuid().ToString()}_{fileInfo.Name}";
                        var dFile = OutPath + fileName;
                        var color = Color.Black;
                        try
                        {
                            if(FontColor.StartsWith("#")) color = ColorTranslator.FromHtml(FontColor);
                            color = Color.FromName(FontColor);
                        }
                        catch { }
                        if (IsFull)
                        {
                            ImageHelper.DrawFullText(item, dFile, Text, color, new Font("Consolas", FontSize), Rotate, LineHeight, Space);
                        }
                        else
                        {
                            ImageHelper.AddWaterMarkText(item, dFile, Text, FontSize, color, Rotate, LocationX, LocationY);
                        }
                        if (File.Exists(dFile)) listTemp.Add(new FileModel() { FilePath = dFile, Size = StringHelper.ConvertFileSize(new FileInfo(dFile).Length) });
                        else sb.AppendLine($"【Failed】文件{item}处理失败！");

                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"【Failed】文件{item}处理失败！{ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                sb.AppendLine($"【Failed】文件处理失败！{ex.Message}");
            }
            finally
            {
                IsHandle = false;
                sb.AppendLine($"【{DateTime.Now}】文件处理完成。");
                FileResultList = listTemp;
                Message = sb.ToString();
            }
        }

    }
}
