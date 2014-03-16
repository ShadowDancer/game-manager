using System;
using System.Collections.Generic;
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

namespace GameManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;
        public MainWindow()
        {

            viewModel = new MainWindowViewModel();
            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                Close();
            };
            viewModel.RequestClose += handler;



            InitializeComponent();

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            DataContext = viewModel;
        }

        private void listViewSitesList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double Width = listViewSitesList.ActualWidth - 10;
            double Column1Size = Width * 0.5;
            double Column2Size = Width * 0.5;

            GridView ListViewView = listViewSitesList.View as GridView;
            ListViewView.Columns[0].Width = Column1Size;
            ListViewView.Columns[1].Width = Column1Size;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as TabControl).Focus();
        }

        private void ExecuteEditSite()
        {
            var Context = DataContext as MainWindowViewModel;
            CommandViewModel CommandModel = Context.SiteCommands.First(n => n.DisplayName == "Edit Site");
            CommandModel.Command.Execute(null);
        }

        private void listViewSitesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExecuteEditSite();
        }

        private void listViewSitesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteEditSite();
            }
        }

        private void listViewSitesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var Context = DataContext as MainWindowViewModel;

            if (listViewSitesList.SelectedItems.Count > 0)
            {
                Context.SiteSelectionChangedCommand.Command.Execute(null);
            }
            else
            {
                Context.SiteSelectionChangedClearCommand.Command.Execute(null);
            }
        }

        private void listViewSitesList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as ListBox).SelectedItems.Clear();
        }
    }
}
