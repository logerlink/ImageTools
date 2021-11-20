using ImageTools.UI.Helper;
using ImageTools.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageTools.UI.Controls
{
    /// <summary>
    /// DragFileControl.xaml 的交互逻辑
    /// </summary>
    public partial class DragFileControl : UserControl
    {
        
        public DragFileControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        
        public static readonly DependencyProperty TipMessageProperty = DependencyProperty.Register("TipMessage", typeof(string), typeof(DragFileControl), new PropertyMetadata());
        /// <summary>
        /// 拖动区域的提示信息
        /// </summary>
        public string TipMessage
        {
            get => (string)GetValue(TipMessageProperty);
            set => SetValue(TipMessageProperty, value);
        }

        

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var vm = (this.DataContext as FileModelVM);
                if (vm == null)
                {
                    vm = new FileModelVM();
                    var list = files.Where(x => ImageHelper.IsPicture(x)).Select(x => {
                        return new FileModel() { FilePath = x,Size = StringHelper.ConvertFileSize(new FileInfo(x).Length) };
                    }).ToList();
                    ObservableCollection<FileModel> fileModels = new ObservableCollection<FileModel>();
                    list.ForEach(x => fileModels.Add(x));
                    vm.FileModels = fileModels;
                    this.DataContext = vm;
                }
                else
                {
                    var exceptModels = files.Except(vm.FileModels.Select(x => x.FilePath)).Where(x => ImageHelper.IsPicture(x)).ToList();
                    exceptModels.ForEach(x => vm.FileModels.Add(new FileModel() { FilePath = x, Size = StringHelper.ConvertFileSize(new FileInfo(x).Length) }));
                }
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
            }
        }

        
    }
}
