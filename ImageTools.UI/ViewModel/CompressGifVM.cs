using ImageTools.UI.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    public class CompressGifVM : BaseHandleFile
    {
        public CompressGifVM()
        {
            Lossy = 120;
            Scale = 0.6;
            Optimize = "O2";
            Gap = 10;
            Icon = this.GetType().Name;
        }
        public int _lossy;
        /// <summary>
        /// 压缩质量
        /// </summary>
        public int Lossy { get => _lossy; set { _lossy = value; OnPropertyChanged("Lossy"); } }

        public double _scale;
        /// <summary>
        /// 期望大小
        /// </summary>
        public double Scale { get => _scale; set { _scale = value; OnPropertyChanged("Scale"); } }
        public string _optimize;
        /// <summary>
        /// 压缩类型
        /// </summary>
        public string Optimize { get => _optimize; set { _optimize = value; OnPropertyChanged("Optimize"); } }

        public int _gap;
        /// <summary>
        /// 抽帧步长
        /// </summary>
        public int Gap { get => _gap; set { _gap = value; OnPropertyChanged("Gap"); } }

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
                            FileInfo fileInfo = new FileInfo(item);
                            var fileName = $"\\{Icon}{Guid.NewGuid().ToString()}_{fileInfo.Name}";
                            var dFile = OutPath + fileName;
                            ImageHelper.CompressGif(item, dFile, Lossy, Scale, Optimize, Gap);
                            if (File.Exists(dFile)) listTemp.Add(new FileModel() { FilePath = dFile, Size = StringHelper.ConvertFileSize(new FileInfo(dFile).Length) });
                            else sb.AppendLine($"【Failed】文件{item}处理失败！");
                        }
                        else
                        {
                            sb.AppendLine($"【Failed】文件{item}不支持处理。仅支持压缩Gif或Tiff类型！");
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
