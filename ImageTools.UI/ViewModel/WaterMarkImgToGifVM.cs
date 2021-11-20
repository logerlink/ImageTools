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
    public class WaterMarkImgToGifVM : BaseHandleFile
    {
        public WaterMarkImgToGifVM()
        {
            WaterFile = "";
            ScaleX = -1;
            ScaleY = -1;
            LocationX = "-1";
            LocationY = "-1";
            Opacity = 0.8;
            Transpose = 4;
            Icon = this.GetType().Name;
        }
        public string _waterFile;
        /// <summary>
        /// 水印图片位置
        /// </summary>
        public string WaterFile { get => _waterFile; set { _waterFile = value; OnPropertyChanged("WaterFile"); } }

        public double _scaleX;
        /// <summary>
        /// 水印大小宽
        /// </summary>
        public double ScaleX { get => _scaleX; set { _scaleX = value; OnPropertyChanged("ScaleX"); } }
        public double _scaleY;
        /// <summary>
        /// 水印大小Y
        /// </summary>
        public double ScaleY { get => _scaleY; set { _scaleY = value; OnPropertyChanged("ScaleY"); } }

        public double _opacity;
        /// <summary>
        /// 透明度
        /// </summary>
        public double Opacity { get => _opacity; set { _opacity = value; OnPropertyChanged("Opacity"); } }


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

        public int _transpose;
        /// <summary>
        /// 水印倾斜
        /// </summary>
        public int Transpose { get => _transpose; set { _transpose = value; OnPropertyChanged("Transpose"); } }



        private ICommand _selectWaterFileCommand;
        /// <summary>
        /// 选择水印图片
        /// </summary>
        public ICommand SelectWaterFileCommand { get { return _selectWaterFileCommand ?? (_selectWaterFileCommand = new Command(SelectWaterFile)); } }

        private void SelectWaterFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "图片|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                WaterFile = openFileDialog.FileName;
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
                if (string.IsNullOrEmpty(WaterFile)) throw new Exception("请提供水印图片");
                Parallel.ForEach(filePathList, item => {
                    try
                    {
                        if (item.ToUpper().EndsWith("GIF"))
                        {
                            FileInfo fileInfo = new FileInfo(item);
                            var fileName = $"\\{Icon}{Guid.NewGuid().ToString()}_{fileInfo.Name}";
                            var dFile = OutPath + fileName;
                            if (Transpose < 0 || Transpose > 4) Transpose = 4;
                            ImageHelper.AddWaterMarkImgGif(item, dFile, WaterFile, ScaleX,ScaleY, Opacity, LocationX, LocationY,transpose: Transpose);
                            if (File.Exists(dFile)) listTemp.Add(new FileModel() { FilePath = dFile, Size = StringHelper.ConvertFileSize(new FileInfo(dFile).Length) });
                            else sb.AppendLine($"【Failed】文件{item}处理失败！");
                        }
                        else
                        {
                            sb.AppendLine($"【Failed】文件{item}不支持处理。仅支持Gif或Tiff类型！");
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
