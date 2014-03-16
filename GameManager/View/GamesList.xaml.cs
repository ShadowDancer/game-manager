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

namespace GameManager.View
{
    /// <summary>
    /// Interaction logic for GamesList.xaml
    /// </summary>
    public partial class GamesList : UserControl
    {
        public GamesList()
        {
            InitializeComponent();
        }

        void ExecuteEditGame()
        {
            var Context = DataContext as GamesListViewModel;
            CommandViewModel CommandModel = Context.CommandList.First(n => n.DisplayName == "Edit Game");
            CommandModel.Command.Execute(null);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteEditGame();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExecuteEditGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = FilterTextBox.DataContext;
            FilterTextBox.Text = "";
        }
    }
}
