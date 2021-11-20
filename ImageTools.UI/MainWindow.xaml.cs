using ImageTools.UI.Controls;
using ImageTools.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageTools.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabItem = TC_ImageTool.SelectedItem as TabItem;
             if (tabItem.DataContext != null) return;
            var name = tabItem?.Name;
            if (string.IsNullOrWhiteSpace(name)) return;
            var type = Type.GetType($"ImageTools.UI.ViewModel.{name}VM");
            var obj = Activator.CreateInstance(type, true);
            tabItem.DataContext = obj;
        }

        private void Btn_HandleFile_PreviewMouseDown(object sender, EventArgs e)
        {
            var tabItem = TC_ImageTool.SelectedItem as TabItem;
            if (tabItem.DataContext == null) return;
            var name = tabItem?.Name;
            if (string.IsNullOrWhiteSpace(name)) return;
            var dragFileVM = (this.FindName("DF_" + name) as DragFileControl)?.DataContext as FileModelVM;
            var tabItemVM = (tabItem.DataContext as BaseHandleFile);
            if (dragFileVM == null || tabItemVM == null) return;
            (this.FindName("Out_" + name) as TextBox)?.Focus();
            tabItemVM.FileModelList = dragFileVM.FileModels;
            Dispatcher.Invoke(()=>{ tabItemVM.HandleFile(); });
        }
        private void Btn_RemoveResult_Event(object sender, EventArgs e)
        {
            var tabItem = TC_ImageTool.SelectedItem as TabItem;
            if (tabItem.DataContext == null) return;
            var name = tabItem?.Name;
            if (string.IsNullOrWhiteSpace(name)) return;
            var tabItemVM = (tabItem.DataContext as BaseHandleFile);
            Dispatcher.Invoke(() => { tabItemVM.ClearResult(); });
        }
    }
}
