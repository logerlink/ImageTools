using ImageTools.UI.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    public class CodeImgVM : BaseHandleFile
    {
        public CodeImgVM()
        {
            Text = "Test测试测试";
            Count = 2;
            Width = 300;
            Height = 300;
            Icon = this.GetType().Name;
        }
        public int _count;
        /// <summary>
        /// 生成数量
        /// </summary>
        public int Count { get => _count; set { _count = value; OnPropertyChanged("Count"); } }

        public int _width;
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get => _width; set { _width = value; OnPropertyChanged("Width"); } }

        public int _height;
        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get => _height; set { _height = value; OnPropertyChanged("Height"); } }

        public string _text;
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get => _text; set { _text = value; OnPropertyChanged("Text"); } }

        public string _waterFile;
        /// <summary>
        /// 中间图片
        /// </summary>
        public string WaterFile { get => _waterFile; set { _waterFile = value; OnPropertyChanged("WaterFile"); } }


        private ICommand _selectWaterFileCommand;
        /// <summary>
        /// 选择水印图片
        /// </summary>
        public ICommand SelectWaterFileCommand { get { return _selectWaterFileCommand ?? (_selectWaterFileCommand = new Command(SelectWaterFile)); } }

        private ICommand _createCodeCommand;
        /// <summary>
        /// 创建水印文件
        /// </summary>
        public ICommand CreateCodeCommand { get { return _createCodeCommand ?? (_createCodeCommand = new ActionCommand<string>(CreateCode)); } }
        /// <summary>
        /// 创建水印文件
        /// </summary>
        /// <param name="codeType"></param>
        private void CreateCode(string codeType)
        {
            var sb = new StringBuilder();
            var listTemp = new ObservableCollection<FileModel>();
            sb.AppendLine($"【{DateTime.Now}】文件开始处理。");
            try
            {
                IsHandle = true;
                if (OutPath == "[桌面]") OutPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                FileResultList.Clear();
                Parallel.For(0,Count, item => {
                    try
                    {
                        var dFile = OutPath + $"\\{Guid.NewGuid().ToString()}.jpg";
                        if (codeType == "BAR")
                        {
                            if (new Regex("[^\x00-\xff]").IsMatch(Text)) throw new Exception("条形码不支持中文");
                            ImageHelper.CreateBarcode(Text,Width,Height,dFile);
                        }
                        else if(codeType == "QR")
                        {
                            ImageHelper.CreateQRCode(Text, Width, Height, dFile);
                        }else if (codeType == "QRIMG")
                        {
                            if (!File.Exists(WaterFile)) throw new Exception($"图片文件【{WaterFile}】不存在");
                            ImageHelper.CreateQRCode(Text, WaterFile, Width, Height, dFile);
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
        /// <summary>
        /// 选择水印图片
        /// </summary>
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
            sb.AppendLine($"【{DateTime.Now}】文件开始处理。");
            try
            {
                IsHandle = true;
                var filePathList = new List<string>();
                foreach (var item in FileModelList) filePathList.Add(item.FilePath);
                FileResultList.Clear();
                Parallel.ForEach(filePathList, item => {
                    try
                    {
                        var text = ImageHelper.GetCodeText(item);
                        sb.AppendLine($"【Succee】文件【{item}】内容为：{text}");

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
                Message = sb.ToString();
            }
        }

    }
}
