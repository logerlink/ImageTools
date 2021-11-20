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
    public class WaterMarkTextToGifVM : BaseHandleFile
    {
        public WaterMarkTextToGifVM()
        {
            Text = "我是水印，仅供参考";
            Rotate = -45;
            FontColor = "Red";
            FontSize = 16;
            IsFull = false;
            Scale = 1;
            Opacity = 0.8;
            LocationX = "-1";
            LocationY = "-1";
            FontFile = "";
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

        

        public bool _isFull;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFull { get => _isFull; set { _isFull = value; OnPropertyChanged("IsFull"); } }

        

        #region 非全屏
        public double _opacity;
        /// <summary>
        /// 透明度
        /// </summary>
        public double Opacity { get => _opacity; set { _opacity = value; OnPropertyChanged("Opacity"); } }
        public double _scale;
        /// <summary>
        /// 水印大小
        /// </summary>
        public double Scale { get => _scale; set { _scale = value; OnPropertyChanged("Scale"); } }
        public string _locationX;
        /// <summary>
        /// 水印左上角的坐标X
        /// </summary>
        public string LocationX { get => _locationX; set { _locationX = value; OnPropertyChanged("LocationX"); } }
        
        public string _locationY;
        /// <summary>
        /// 水印左上角的坐标Y
        /// </summary>
        public string LocationY { get => _locationY; set { _locationY = value; OnPropertyChanged("LocationY"); } }

        public int _waterWidth;
        /// <summary>
        /// 水印宽度
        /// </summary>
        public int WaterWidth { get => _waterWidth; set { _waterWidth = value; OnPropertyChanged("WaterWidth"); } }
        public int _waterHeight;
        /// <summary>
        /// 水印高度
        /// </summary>
        public int WaterHeight { get => _waterHeight; set { _waterHeight = value; OnPropertyChanged("WaterHeight"); } }
        public int _rotate;
        /// <summary>
        /// 文字倾斜
        /// </summary>
        public int Rotate { get => _rotate; set { _rotate = value; OnPropertyChanged("Rotate"); } }

        public string _fontFile;
        /// <summary>
        /// 字体文件
        /// </summary>
        public string FontFile { get => _fontFile; set { _fontFile = value; OnPropertyChanged("FontFile"); } }
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

        private ICommand _selectFontFileCommand;
        /// <summary>
        /// 选择字体文件
        /// </summary>
        public ICommand SelectFontFileCommand { get { return _selectFontFileCommand ?? (_selectFontFileCommand = new Command(SelectFontFile)); } }

        private void SelectFontFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "字体文件|*.ttf;*.ttc;";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                FontFile = openFileDialog.FileName;
            }
        }

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
                        if (item.ToUpper().EndsWith("GIF"))
                        {
                            FileInfo fileInfo = new FileInfo(item);
                            var fileName = $"\\{Icon}{Guid.NewGuid().ToString()}_{fileInfo.Name}";
                            var dFile = OutPath + fileName;
                            var color = Color.Black;
                            try
                            {
                                if (FontColor.StartsWith("#")) color = ColorTranslator.FromHtml(FontColor);
                                color = Color.FromName(FontColor);
                            }
                            catch { }
                            if (IsFull || Rotate != 0)
                            {
                                ImageHelper.AddWaterMarkTextGif(item, dFile,IsFull,Text,color, FontSize,"",Opacity,Rotate,LocationX,LocationY,WaterWidth,WaterHeight,lineHeight: LineHeight, space: Space);
                            }
                            else
                            {
                                ImageHelper.AddWaterMarkTextGif(item, dFile, Text, color,FontSize,LocationX, LocationY,fontfile:FontFile);
                            }
                            if (File.Exists(dFile)) listTemp.Add(new FileModel() { FilePath = dFile, Size = StringHelper.ConvertFileSize(new FileInfo(dFile).Length) });
                            else sb.AppendLine($"【Failed】文件{item}处理失败！");
                        }
                        else
                        {
                            sb.AppendLine($"【Failed】文件{item}不支持处理。仅支持Gif类型！");
                            return;
                        }
                        

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
