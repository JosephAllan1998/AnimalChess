using AnimalChess.UIPages;
using System;
using System.Collections.Generic;
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

namespace AnimalChess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BoardPage board { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //UILoad();
        }

        private void UILoad()
        {
            using (board = new BoardPage(this))
            {
                frameLoad.Navigate(board);
            }
        }

        #region Action game
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            UILoad();
        }

        private void ReloadGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseGame_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help information!");
        }

        #endregion
    }
}
