using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    public class BaseHandleFile: BaseViewModel
    {
        public BaseHandleFile()
        {
            OutPath = "[桌面]";
            FileResultList = new ObservableCollection<FileModel>();
        }
        private ICommand _handleFileCommand;
        /// <summary>
        /// 处理图片操作
        /// </summary>
        public ICommand HandleFileCommand { get { return _handleFileCommand ?? (_handleFileCommand = new Command(HandleFile)); } }

        private ICommand _selectOutPathCommand;
        /// <summary>
        /// 选择存储区域操作
        /// </summary>
        public ICommand SelectOutPathCommand { get { return _selectOutPathCommand ?? (_selectOutPathCommand = new Command(SelectOutPath)); } }
        private ICommand _clearResultCommand;
        /// <summary>
        /// 清空结果
        /// </summary>
        public ICommand ClearResultCommand { get { return _clearResultCommand ?? (_clearResultCommand = new Command(ClearResult)); } }

        

        public void ClearResult()
        {
            this.Message = "";
            this.FileResultList.Clear();
        }

        public string _outPath;
        /// <summary>
        /// 输出位置
        /// </summary>
        public string OutPath { get => _outPath; set { _outPath = value; OnPropertyChanged("OutPath"); } }
        /// <summary>
        /// 需要处理的图片
        /// </summary>
        public ObservableCollection<FileModel> FileModelList { get; set; }

        private ObservableCollection<FileModel> _fileResultList;
        /// <summary>
        /// 处理成功的图片
        /// </summary>
        public ObservableCollection<FileModel> FileResultList { get => _fileResultList; set { _fileResultList = value; OnPropertyChanged("FileResultList"); } }
        public string _message;
        /// <summary>
        /// 处理日志
        /// </summary>
        public string Message { get => _message; set { _message = value; OnPropertyChanged("Message"); } }

        public bool _isHandle;
        /// <summary>
        /// 是否正在处理
        /// </summary>
        public bool IsHandle { get => _isHandle; set { _isHandle = value; OnPropertyChanged("IsHandle"); } }

        public string IsHandleVisibility { get => IsHandle ? "Visible" : "Hidden"; set { OnPropertyChanged("IsHandle"); } }



        protected string Icon { get; set; }
        protected void SelectOutPath()
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                OutPath = openFileDialog.SelectedPath;
            }
        }

        public virtual void HandleFile()
        {
        }
    }
}
