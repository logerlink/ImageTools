﻿using ImageTools.UI.Helper;
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
    public class CompressImgVM:BaseHandleFile
    {
        public CompressImgVM()
        {
            Quality = 60;
            ExpectSize = 500;
            Icon = this.GetType().Name;
        }
        public int _quality;
        /// <summary>
        /// 压缩质量
        /// </summary>
        public int Quality { get => _quality; set { _quality = value; OnPropertyChanged("Quality"); } }

        public int _expectSize;
        /// <summary>
        /// 期望大小
        /// </summary>
        public int ExpectSize { get => _expectSize; set { _expectSize = value; OnPropertyChanged("ExpectSize"); } }

        public override void HandleFile()
        {
            Application.Current.MainWindow.Dispatcher.Invoke(()=> {
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
                                sb.AppendLine($"【Failed】文件{item}不支持处理。不可解压Gif或Tiff类型！");
                                return;
                            }
                            FileInfo fileInfo = new FileInfo(item);
                            var fileName = $"\\{Icon}{Guid.NewGuid().ToString()}_{fileInfo.Name}";
                            var dFile = OutPath + fileName;
                            ImageHelper.CompressImage(item, dFile, Quality, ExpectSize);
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
            });
            
        }

    }
}
