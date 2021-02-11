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

        #region Responsive Window
        private void DragMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ChangeStyle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            #region
            /*Test size map screen
            
            #region Khai báo
            string resourcePath = @"pack://application:,,,/Resources/Images/Controls/";
            string restore = "restore.png", maximize = "maximize.png";
            string restoreImage = resourcePath + restore, maximizeImage = resourcePath + maximize;

            double cur_screen_width = SystemParameters.WorkArea.Width;
            double cur_screen_height = SystemParameters.WorkArea.Height;

            string curImage = imgChangeStyle.Source.ToString().Replace(resourcePath, "");
            #endregion

            #region 
            if (curImage.Contains(restore))
            {
                this.Height = cur_screen_height;
                this.Width = (double.Parse(board.Tag.ToString()) * 9) + 30;
                imgChangeStyle.Source = new BitmapImage(new Uri(maximizeImage, UriKind.Absolute));
            }
            else
            {
                //this.Height = cur_screen_height;
                //this.Width = (double.Parse(board.Tag.ToString()) * 9) + 30;
                WindowState = WindowState.Normal;
                imgChangeStyle.Source = new BitmapImage(new Uri(restoreImage, UriKind.Absolute));
            }
            #endregion

            */
            #endregion
            if (WindowState == WindowState.Normal)//Nếu cửa sổ đang bình thường
            {
                WindowState = WindowState.Maximized;//Phóng to
                imgChangeStyle.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Images/Controls/restore.png", UriKind.Absolute));
            }
            else if (this.WindowState == WindowState.Maximized)//Nếu cửa sổ full-screen
            {
                WindowState = WindowState.Normal;//Bình thường
                imgChangeStyle.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Images/Controls/maximize.png", UriKind.Absolute));
            }
        }

        private void Minimize_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Visible;
        }
        #endregion
    }
}
