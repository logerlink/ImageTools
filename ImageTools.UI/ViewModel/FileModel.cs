using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    public class FileModelVM:BaseViewModel
    {
        private ObservableCollection<FileModel> _fileModels;
        public ObservableCollection<FileModel> FileModels { get => _fileModels; set { _fileModels = value; OnPropertyChanged("FileModels"); } }

        private ICommand _removeFileCommand;
        public ICommand RemoveFileCommand { get { return _removeFileCommand ?? (_removeFileCommand = new ActionCommand<string>(RemoveFile)); } }

        private void RemoveFile(string filePath)
        {
            foreach (var item in FileModels)
            {
                if (filePath == item.FilePath)
                {
                    FileModels.Remove(item);
                    break;
                }
            }
            
        }
    }

    public class FileModel
    {
        public string FilePath { get; set; }
        public string Size { get; set; }
    }

}
