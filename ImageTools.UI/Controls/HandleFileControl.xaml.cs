using ImageTools.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// HandleFileControl.xaml 的交互逻辑
    /// </summary>
    public partial class HandleFileControl : UserControl
    {
        public delegate void LonginEventHandler(object sender, EventArgs e);
        public event LonginEventHandler BtnHandleFilePreviewMouseDownEvent;
        public event LonginEventHandler BtnRemoveResultEvent;

        public static readonly DependencyProperty FileResultListProperty = DependencyProperty.Register("FileResultList", typeof(ObservableCollection<FileModel>), typeof(HandleFileControl), new PropertyMetadata());
        public ObservableCollection<FileModel> FileResultList
        {
            get => (ObservableCollection<FileModel>)GetValue(FileResultListProperty);
            set => SetValue(FileResultListProperty, value);
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(HandleFileControl), new PropertyMetadata());
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty HandleTextProperty = DependencyProperty.Register("HandleText", typeof(string), typeof(DragFileControl), new PropertyMetadata("立即执行"));
        public string HandleText
        {
            get => (string)GetValue(HandleTextProperty);
            set => SetValue(HandleTextProperty, value);
        }


        public HandleFileControl()
        {
            InitializeComponent();
            SP_Handle.DataContext = this;
        }

        private void Btn_HandleFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //https://blog.csdn.net/qq_31839695/article/details/119349547
            BtnHandleFilePreviewMouseDownEvent?.Invoke(sender,e);
        }

        private void Btn_RemoveResult_Click(object sender, RoutedEventArgs e)
        {
            BtnRemoveResultEvent?.Invoke(sender, e);
        }

        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button).Tag?.ToString();
            if (string.IsNullOrWhiteSpace(tag)) return;
            System.Diagnostics.Process.Start("explorer.exe", tag);
        }
    }
}
