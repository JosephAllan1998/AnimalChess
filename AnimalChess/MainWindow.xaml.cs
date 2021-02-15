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
using System.Diagnostics;
using System.Xml.Linq;
using AnimalChess.Models;
using System.IO;

namespace AnimalChess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BoardPage board { get; set; }
        public static string save_file_name { get; } = "SaveGame.xml";
        public static string resourceDirections, resourceAnimals;
        public static string resourceOthers, pathAudio, resourceControls;
        public static string exe = AppDomain.CurrentDomain.BaseDirectory;
        public static Stack<History> stack_undo = new Stack<History>();
        public static Stack<History> stack_redo = new Stack<History>();
        public MainWindow()
        {
            InitializeComponent();
            string bgResource = this.bgGrid.ImageSource.ToString();
            string resourcePath = bgResource.Substring(0, bgResource.LastIndexOf('/'));
            resourceAnimals = resourcePath + "/Animals/";
            resourceControls = resourcePath + "/Controls/";
            resourceDirections = resourcePath + "/Directions/";
            resourceOthers = resourcePath + "/Others/";
            pathAudio = Path.Combine(exe, "Audios");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //UILoad();
            //Load thuộc tính
        }

        #region Action game
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var miNewGame = sender as MenuItem;
            miNewGame.IsEnabled = false;

            miResetGame.IsEnabled = true;
            miSaveGame.IsEnabled = true;

            board = new BoardPage(this);
            frameLoad.Navigate(board);
        }

        #region Mode & Level
        private void OnePlayer_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                miLevel.IsEnabled = true;
            }
            catch (Exception)
            {

            }
        }

        private void TwoPlayer_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                miLevel.IsEnabled = false;
            }
            catch (Exception)
            {

            }
        }

        private void Easy_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Medium_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Hard_Checked(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (stack_undo.Count == 1)
                miUndo.IsEnabled = false;

            History history = stack_undo.Pop();
            stack_redo.Push(history);
            miRedo.IsEnabled = true;
            board.Round = history.Round;

            Grid grid_source = board.FindVisualChildren<Grid>(board.gridChessBoard)
                .Where(x => Grid.GetColumn(x) == history.ColumnSource && Grid.GetRow(x) == history.RowSource)
                .SingleOrDefault();

            Grid grid_destination = board.FindVisualChildren<Grid>(board.gridChessBoard)
                .Where(x => Grid.GetColumn(x) == history.ColumnDestination && Grid.GetRow(x) == history.RowDestination)
                .SingleOrDefault();

            grid_source.Children.Clear();
            grid_destination.Children.Clear();

            //grid_source.Children.Add(history.Source);
            grid_destination.Children.Add(history.Source);
            Lỗi ở đây nhé !
            //Grid.SetColumn(history.Source, history.ColumnSource);
            //Grid.SetRow(history.Source, history.RowSource);

            if (history.Destination != null)
                grid_destination.Children.Add(history.Destination);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (stack_redo.Count == 1)
                miRedo.IsEnabled = false;
            History history = stack_redo.Pop();
            stack_undo.Push(history);
            miUndo.IsEnabled = true;
            board.Round = history.Round;

            Grid grid_source = board.FindVisualChildren<Grid>(board.gridChessBoard)
                .Where(x => Grid.GetColumn(x) == history.ColumnSource && Grid.GetRow(x) == history.RowSource)
                .SingleOrDefault();

            Grid grid_destination = board.FindVisualChildren<Grid>(board.gridChessBoard)
                .Where(x => Grid.GetColumn(x) == history.ColumnDestination && Grid.GetRow(x) == history.RowDestination)
                .SingleOrDefault();

            grid_source.Children.Clear();
            grid_destination.Children.Clear();

            grid_source.Children.Add(history.Source);
            if (history.Destination != null)
                grid_destination.Children.Add(history.Destination);
        }
        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            frameLoad.NavigationService.RemoveBackEntry();
            board = new BoardPage(this);
            frameLoad.Navigate(board);
        }

        #region Save and reload
        public void SaveProcessGame()
        {
            if (board == null) return;

            XDocument xmlSave = new XDocument();
            XElement xSaveProcess = new XElement("SaveProcess");

            XElement xRound = new XElement("Round");
            XElement xAnimals = new XElement("Animals");

            #region Round
            if (board.Round == Team.Blue)
                xRound.Add("Blue");
            else xRound.Add("Red");
            #endregion

            #region Animals
            List<Grid> grids = board.FindVisualChildren<Grid>(board.gridChessBoard).ToList();
            grids = grids.Where(x => x.Children.Count != 0).ToList();
            foreach (var grid in grids)
            {
                Image image = board.FindVisualChildren<Image>(grid).SingleOrDefault();
                int col = Grid.GetColumn(grid);
                int row = Grid.GetRow(grid);

                if (image != null)
                {
                    Animal animal = image.Tag as Animal;
                    if (animal != null)
                    {
                        XElement xAnimal = new XElement("Animal");

                        XAttribute x_name = new XAttribute("Name", animal.Name);
                        string team = animal.Team.ToString();
                        XAttribute x_team = new XAttribute("Team", team);
                        XAttribute x_level = new XAttribute("Level", animal.Cur_LV.ToString());
                        string image_name = $"{team}_{animal.Name}.png".ToLower();
                        XAttribute x_imageName = new XAttribute("ImageName", image_name);
                        XAttribute x_column = new XAttribute("Column", col);
                        XAttribute x_row = new XAttribute("Row", row);

                        xAnimal.Add(x_name, x_team, x_level, x_imageName, x_column, x_row);
                        xAnimals.Add(xAnimal);
                    }
                }
            }
            #endregion

            xSaveProcess.Add(xRound);
            xSaveProcess.Add(xAnimals);

            xmlSave.Add(xSaveProcess);
            xmlSave.Save(save_file_name);
        }

        private void ReloadProcessGame()
        {
            try
            {
                List<MyProcess> myProcesses = new List<MyProcess>();
                Team round = Team.Blue;
                XDocument xml = XDocument.Load(save_file_name);
                List<XNode> listInfo = xml.DescendantNodes().ToList();
                board = new BoardPage(this);
                int length = listInfo.Count;

                for (int i = 0; i < length; i++)
                {
                    if (listInfo[i] is XElement)
                    {
                        XElement element = listInfo[i] as XElement;

                        if (element.Name == "Round")
                        {
                            if (element.Value == "Blue")
                                round = Team.Blue;
                            else round = Team.Red;
                        }
                        if (element.Name == "Animal")
                        {
                            string name = element.Attribute("Name").Value;
                            string team = element.Attribute("Team").Value;
                            string level = element.Attribute("Level").Value;
                            string image_name = element.Attribute("ImageName").Value;
                            string col = element.Attribute("Column").Value;
                            string row = element.Attribute("Row").Value;

                            Animal a = board.GetAnimal(name.ToLower(), 0);

                            if (level == "Dangerous")
                                a.Cur_LV = Level.Dangerous;

                            if (team == "Red")
                            {
                                board.RedAnimals.Add(a);
                                a.Team = Team.Red;
                            }
                            else
                            {
                                board.BlueAnimals.Add(a);
                                a.Team = Team.Blue;
                            }

                            Image img = board.ReloadImage(image_name, a);

                            MyProcess p = new MyProcess
                            {
                                image = img,
                                animal = a,
                                Column = int.Parse(col),
                                Row = int.Parse(row)
                            };

                            myProcesses.Add(p);
                        }
                    }
                }

                frameLoad.NavigationService.RemoveBackEntry();

                board.Round = round;
                board.ReloadLastSave(myProcesses);
                frameLoad.Navigate(board);
            }
            catch (Exception)
            {
                string caption = "Error";
                string message = "Reload game failed";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;

                //message = ex.Message;
                MessageBox.Show(message, caption, button, image);

                //frameLoad.NavigationService.RemoveBackEntry();
                //board = new BoardPage(this);
                //frameLoad.Navigate(board);
            }
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            SaveProcessGame();
        }
        private void ReloadGame_Click(object sender, RoutedEventArgs e)
        {
            ReloadProcessGame();
            miResetGame.IsEnabled = true;
        }

        #endregion
        private void CloseGame_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Help information!");
            //Load page hướng dẫn chơi
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
            Image image = sender as Image;
            if (WindowState == WindowState.Normal)//Nếu cửa sổ đang bình thường
            {
                WindowState = WindowState.Maximized;//Phóng to
                image.Source = new BitmapImage(new Uri(resourceControls + @"restore.png", UriKind.Absolute));
            }
            else if (this.WindowState == WindowState.Maximized)//Nếu cửa sổ full-screen
            {
                WindowState = WindowState.Normal;//Bình thường
                image.Source = new BitmapImage(new Uri(resourceControls + @"maximize.png", UriKind.Absolute));
            }
        }

        private void Minimize_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Visible;
        }
        private void CollapseOrExpand_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            ToolTip t = new ToolTip();
            string imageSource = image.Source.ToString();
            if (imageSource.Contains("collapse"))
            {
                t.Content = "Expand Menu";
                image.ToolTip = t;
                dropDownMenu.Visibility = Visibility.Collapsed;
                image.Source = new BitmapImage(new Uri(resourceControls + @"expand.png", UriKind.Absolute));
            }
            else
            {
                t.Content = "Collapse Menu";
                image.ToolTip = t;
                dropDownMenu.Visibility = Visibility.Visible;
                image.Source = new BitmapImage(new Uri(resourceControls + @"collapse.png", UriKind.Absolute));
            }
        }

        private void Setting_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion

    }
}
