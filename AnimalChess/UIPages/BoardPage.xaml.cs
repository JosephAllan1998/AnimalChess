using AnimalChess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AnimalChess.UIPages
{
    /// <summary>
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Page, IDisposable
    {
        public MainWindow mainWindow { get; set; }
        public List<Animal> RedAnimals = new List<Animal>();
        public List<Animal> BlueAnimals = new List<Animal>();
        public static Image SelectedAnimal;
        public Team Round { get; set; } = Team.Blue;
        public int rows { get; set; }
        public int columns { get; set; }

        public BoardPage()
        {
            InitializeComponent();
        }
        public BoardPage(MainWindow _mainWindow)
        {
            InitializeComponent();

            mainWindow = _mainWindow;
            UILoadBoard();
            UILoadAnimal();
        }

        public void ReloadLastSave(List<MyProcess> myProcesses)
        {
            List<Grid> grids = FindVisualChildren<Grid>(gridChessBoard).ToList();
            foreach (var grid in grids)
            {
                int col = Grid.GetColumn(grid);
                int row = Grid.GetRow(grid);
                var p = myProcesses.Where(x => x.Row == row && x.Column == col).SingleOrDefault();

                grid.Children.Clear();
                if(p != null) grid.Children.Add(p.image);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double t = (this.Width - 30) / 9;
            //double t = (this.ActualHeight - 70) / 11;
            double t = this.ActualHeight / 11;
            gridChessBoard.Tag = t;
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void UILoadBoard()
        {
            rows = gridChessBoard.RowDefinitions.Count;
            columns = gridChessBoard.ColumnDefinitions.Count;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Grid grid = Container(i, j);
                    gridChessBoard.Children.Add(grid);
                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);
                }
            }
        }

        private void UILoadAnimal()
        {
            List<Grid> grids = FindVisualChildren<Grid>(gridChessBoard).ToList();
            foreach (var grid in grids)
            {
                List<Image> images = FindVisualChildren<Image>(grid).ToList();
                foreach (var image in images)
                {
                    image.Cursor = Cursors.Hand;
                    image.PreviewMouseDown += Animal_PreviewMouseDown;

                    Grid g = image.Parent as Grid;
                    string animalName = image.Source.ToString().Replace(MainWindow.resourceAnimals, "")
                        .Replace(".png", "").Replace("red_", "").Replace("blue_", "");
                    int column = Grid.GetColumn(g);
                    int row = Grid.GetRow(g);

                    Animal animal = GetAnimal(animalName, row);
                    image.Tag = animal;
                    if (animal.Team == Team.Red)
                        RedAnimals.Add(animal);
                    else BlueAnimals.Add(animal);
                }
            }
        }

        public Animal GetAnimal(string animal_name, int row)
        {
            Animal a;
            if (animal_name == "elephant") a = new Elephant();
            else if (animal_name == "lion") a = new Lion();
            else if (animal_name == "tiger") a = new Tiger();
            else if (animal_name == "leopard") a = new Leopard();
            else if (animal_name == "dog") a = new Dog();
            else if (animal_name == "wolf") a = new Wolf();
            else if (animal_name == "cat") a = new Cat();
            else a = new Rat();

            if ((row >= 1 && row <= 3)) a.Team = Team.Red;
            else if (row >= 7 && row <= 9) a.Team = Team.Blue;

            return a;
        }

        private void Animal_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var panel = img.Parent as Grid;
            var animal = img.Tag as Animal;

            //Chặn lượt chơi
            if (Round == Team.Red && animal.Team == Team.Blue) return;
            else if (Round == Team.Blue && animal.Team == Team.Red) return;

            if (SelectedAnimal != null && SelectedAnimal != img)
            {
                var animal2 = SelectedAnimal.Tag as Animal;
                var panel2 = SelectedAnimal.Parent as Grid;
                if (animal2.Team == Team.Blue)
                    SelectedAnimal.Source = new BitmapImage(new Uri(SelectedAnimal.Source.ToString().Replace("select", "blue")));
                else SelectedAnimal.Source = new BitmapImage(new Uri(SelectedAnimal.Source.ToString().Replace("select", "red")));

                RemoveAvailableDirection(panel2);
                Panel.SetZIndex(panel2, 0);
            }

            var cur_imageSource = img.Source.ToString();
            if (!cur_imageSource.Contains("select"))
            {
                img.Source = new BitmapImage(new Uri(img.Source.ToString().Replace("blue", "select").Replace("red", "select")));
                AddAvailableDirection(panel, animal);
                SelectedAnimal = img;
                Panel.SetZIndex(panel, 1);
            }
            else
            {
                if (animal.Team == Team.Blue)
                    img.Source = new BitmapImage(new Uri(img.Source.ToString().Replace("select", "blue")));
                else img.Source = new BitmapImage(new Uri(img.Source.ToString().Replace("select", "red")));

                RemoveAvailableDirection(panel);
                Panel.SetZIndex(panel, 0);
            }

            PlaySoundAnimal(animal.AnimalSound);
        }

        private Area DeterminedArea(Direction d, Grid g, Animal a)
        {
            if (g == null) return Area.Border;

            var image = FindVisualChildren<Image>(g).SingleOrDefault();
            if (image != null)
            {
                Animal b = image.Tag as Animal;
                if (g.Tag != null)
                {
                    string gridTag = g.Tag.ToString();

                    if (gridTag == Common.TRAP) return Area.LessOrEqualLevel;
                    else if (gridTag == Common.WATER)
                    {
                        if (a.Swim == Swim.Available && a.Cur_LV >= b.Cur_LV)
                            return Area.LessOrEqualLevel;
                        else return Area.CannotSwim;
                    }
                    else return Area.Available;
                }
                else
                {
                    if (a.Team == b.Team) return Area.SameTeam;
                    else if (a.Cur_LV == Level.Elephant && b.Cur_LV == Level.Rat) return Area.HigherLevel;
                    else if (a.Cur_LV == Level.Rat && b.Cur_LV == Level.Elephant) return Area.LessOrEqualLevel;
                    else if (a.Cur_LV >= b.Cur_LV) return Area.LessOrEqualLevel;
                    else return Area.HigherLevel;
                }
            }
            else
            {
                if (g.Tag != null)
                {
                    string gridTag = g.Tag.ToString();
                    if (gridTag == Common.BORDER) return Area.Border;
                    else if (gridTag == Common.WATER) return Area.Water;
                    else if (gridTag == Common.TRAP) return Area.Trap;
                    else if (gridTag == Common.BLUE_CAVE) return Area.Blue_Cave;
                    else if (gridTag == Common.RED_CAVE) return Area.Red_Cave;
                    else return Area.Available;
                }
                else return Area.Available;
            }
        }

        private Grid FindGridDirection(int column, int row)
        {
            List<Grid> grids = FindVisualChildren<Grid>(gridChessBoard).ToList();
            Grid g = grids.Where(x => Grid.GetColumn(x) == column && Grid.GetRow(x) == row).SingleOrDefault();
            return g;
        }

        private bool IsDirectionAvailable(Direction drt, Grid g, Animal a)
        {
            int cur_col = Grid.GetColumn(g);
            int cur_row = Grid.GetRow(g);
            Grid tempGrid = new Grid();
            Area area = new Area();

            switch (drt)
            {
                case Direction.Up:
                    {
                        tempGrid = FindGridDirection(cur_col, cur_row - 1);
                        area = DeterminedArea(Direction.Up, tempGrid, a);
                        break;
                    }
                case Direction.Down:
                    {
                        tempGrid = FindGridDirection(cur_col, cur_row + 1);
                        area = DeterminedArea(Direction.Down, tempGrid, a);
                        break;
                    }
                case Direction.Left:
                    {
                        tempGrid = FindGridDirection(cur_col - 1, cur_row);
                        area = DeterminedArea(Direction.Left, tempGrid, a);
                        break;
                    }
                case Direction.Right:
                    {
                        tempGrid = FindGridDirection(cur_col + 1, cur_row);
                        area = DeterminedArea(Direction.Right, tempGrid, a);
                        break;
                    }
            }

            if ((a.Team == Team.Blue && area == Area.Blue_Cave)
                || (a.Team == Team.Red && area == Area.Red_Cave))
                return false;

            if (area == Area.Border || area == Area.CannotSwim || area == Area.SameTeam
                || area == Area.HigherLevel || (area == Area.Water && a.Swim != Swim.Available))
                return false;
            return true;
        }
        private bool IsDirectionAvailableJump(Direction drt, Grid g, Animal a)
        {
            if (a.Cur_LV != Level.Lion && a.Cur_LV != Level.Tiger)
                return false;

            int cur_col = Grid.GetColumn(g);
            int cur_row = Grid.GetRow(g);
            Grid tempGrid = new Grid();
            Area area = new Area();

            for (int i = 1; i <= 3; i++)
            {
                switch (drt)
                {
                    case Direction.Left:
                        {
                            tempGrid = FindGridDirection(cur_col - i, cur_row);
                            area = DeterminedArea(Direction.Left, tempGrid, a);
                            break;
                        }
                    case Direction.Right:
                        {
                            tempGrid = FindGridDirection(cur_col + i, cur_row);
                            area = DeterminedArea(Direction.Right, tempGrid, a);
                            break;
                        }
                    default: return false;
                }

                var image = FindVisualChildren<Image>(tempGrid).SingleOrDefault();
                if (area == Area.Water || area == Area.CannotSwim)
                {
                    if (i != 3 && image != null)
                        return false;
                }
                else if (i == 3 && image != null) // Đến đich có vật cản
                {
                    Animal b = image.Tag as Animal;
                    if (a.Team == b.Team || (a.Team != b.Team && a.Cur_LV < b.Cur_LV))
                        return false;
                }
                else if (i != 3 && (area != Area.Water && area != Area.CannotSwim))
                    return false;

            }
            return true;
        }
        private void AddAvailableDirection(Grid g, Animal a)
        {
            Canvas c = new Canvas();
            Image i;
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());
            if (IsDirectionAvailable(Direction.Up, g, a))
            {
                i = ArrowImage(Direction.Up, -tagGrid, 0, a);
                c.Children.Add(i);
            }
            if (IsDirectionAvailable(Direction.Down, g, a))
            {
                i = ArrowImage(Direction.Down, tagGrid, 0, a);
                c.Children.Add(i);
            }

            if (IsDirectionAvailableJump(Direction.Left, g, a))
            {
                i = ArrowImage(Direction.Left, 0, -tagGrid * 3, a);
                c.Children.Add(i);
            }
            else if (IsDirectionAvailable(Direction.Left, g, a))
            {
                i = ArrowImage(Direction.Left, 0, -tagGrid, a);
                c.Children.Add(i);
            }

            if (IsDirectionAvailableJump(Direction.Right, g, a))
            {
                i = ArrowImage(Direction.Right, 0, tagGrid * 3, a);
                c.Children.Add(i);
            }
            else if (IsDirectionAvailable(Direction.Right, g, a))
            {
                i = ArrowImage(Direction.Right, 0, tagGrid, a);
                c.Children.Add(i);
            }

            g.Children.Add(c);
        }
        private void Direction_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            int desPos = int.Parse(i.Tag.ToString());
            var animal = SelectedAnimal.Tag as Animal;

            if (animal.Team == Team.Blue)
            {
                animal = BlueAnimals.Where(x => x == animal).SingleOrDefault();
                SelectedAnimal.Source = new BitmapImage(new Uri(SelectedAnimal.Source.ToString().Replace("select", "blue")));
            }
            else
            {
                animal = RedAnimals.Where(x => x == animal).SingleOrDefault();
                SelectedAnimal.Source = new BitmapImage(new Uri(SelectedAnimal.Source.ToString().Replace("select", "red")));
            }

            string direction = i.Name;
            Canvas canvasDirection = i.Parent as Canvas;
            Grid gridSource = canvasDirection.Parent as Grid;
            RemoveAvailableDirection(gridSource);
            Grid gridDestination = null;
            int cur_column = Grid.GetColumn(gridSource);
            int cur_row = Grid.GetRow(gridSource);

            History history = new History
            {
                ColumnSource = cur_column,
                RowSource = cur_row,
                Round = Round
            };

            switch (direction)
            {
                case "up":
                    {
                        gridDestination = FindGridDirection(cur_column, cur_row - desPos);
                        break;
                    }
                case "down":
                    {
                        gridDestination = FindGridDirection(cur_column, cur_row + desPos);
                        break;
                    }
                case "left":
                    {
                        gridDestination = FindGridDirection(cur_column - desPos, cur_row);
                        break;
                    }
                case "right":
                    {
                        gridDestination = FindGridDirection(cur_column + desPos, cur_row);
                        break;
                    }
            }

            Image swap_animal;
            Animal b;
            if (gridDestination != null)
            {
                swap_animal = FindVisualChildren<Image>(gridDestination).SingleOrDefault();

                #region Responsive Swap
                if (swap_animal == null)
                {
                    gridSource.Children.Remove(SelectedAnimal);
                    gridDestination.Children.Add(SelectedAnimal);
                }
                else
                {
                    gridSource.Children.Remove(SelectedAnimal);
                    gridDestination.Children.Remove(swap_animal);
                    gridDestination.Children.Add(SelectedAnimal);

                    b = swap_animal.Tag as Animal;
                    if (b.Team == Team.Blue)
                    {
                        BlueAnimals.Remove(b);
                        if (BlueAnimals.Count == 0)
                        {
                            WinPopup(Team.Red);
                        }
                    }
                    else
                    {
                        RedAnimals.Remove(b);
                        if (RedAnimals.Count == 0)
                        {
                            WinPopup(Team.Blue);
                        }
                    }

                }

                if (gridDestination.Tag != null)
                {
                    string gridTag = gridDestination.Tag.ToString();

                    switch (gridTag)
                    {
                        case "TRAP":
                            {
                                animal.Cur_LV = Level.Dangerous;
                                break;
                            }
                        case "BLUE_CAVE":
                            {
                                WinPopup(Team.Red);
                                break;
                            }
                        case "RED_CAVE":
                            {
                                WinPopup(Team.Blue);
                                break;
                            }
                        default:
                            animal.Cur_LV = animal.Temp_LV;
                            break;
                    }
                }
                else animal.Cur_LV = animal.Temp_LV;

                Panel.SetZIndex(gridSource, 0);
                Panel.SetZIndex(gridDestination, 0);

                history.Destination = swap_animal;
                SelectedAnimal.Tag = animal;
                history.Source = SelectedAnimal;

                MainWindow.stack_undo.Push(history);
                mainWindow.miUndo.IsEnabled = true;
                #endregion
            }

            if (Round == Team.Blue)
                Round = Team.Red;
            else Round = Team.Blue;
        }

        private void WinPopup(Team team)
        {
            this.IsHitTestVisible = false;
            string caption = $"VICTORY";
            string message = team == Team.Blue ? $"Blue Win\n" : $"Red Win\n";
            message += $"Do you want to replay ?";
            MessageBoxButton boxButton = MessageBoxButton.YesNo;
            MessageBoxImage boxImage = MessageBoxImage.Question;
            MessageBoxResult boxResult = MessageBoxResult.Yes;

            var ask = MessageBox.Show(message, caption, boxButton, boxImage, boxResult);
            if(ask == MessageBoxResult.Yes)
            {
                mainWindow.frameLoad.NavigationService.RemoveBackEntry();
                mainWindow.frameLoad.Navigate(new BoardPage(mainWindow));
            }
        }

        private void RemoveAvailableDirection(Grid grid)
        {
            Canvas canvas = FindVisualChildren<Canvas>(grid).SingleOrDefault();
            if (canvas == null) return;
            else grid.Children.Remove(canvas);
        }

        private void PlaySoundAnimal(string wavFilename)
        {
            try
            {
                SoundPlayer sound = new SoundPlayer();
                sound.SoundLocation = Path.Combine(MainWindow.pathAudio, wavFilename);
                sound.Play();
            }
            catch (Exception) { }
        }

        #region Element
        public Image ReloadImage(string source, Animal animal)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(MainWindow.resourceAnimals + source));
            image.Tag = animal;
            image.Cursor = Cursors.Hand;
            image.PreviewMouseDown += Animal_PreviewMouseDown;
            return image;
        }
        private Image DeterminedImage(int row, int column)
        {
            string imgPath = MainWindow.resourceOthers;
            Image image = new Image();
            if (row == 0 || column == 0 || row == rows - 1 || column == columns - 1)//Vị trí biên
            {
                imgPath += "wood.png";
            }
            else
            {
                int mid = (int)(columns / 2);
                if (row == 1 && column == mid || row == rows - 2 && column == mid)
                {
                    imgPath += "cave.png";
                }
                else
                {
                    if (row == 1 && (column == mid - 1 || column == mid + 1)
                        || row == rows - 2 && (column == mid - 1 || column == mid + 1)
                        || column == mid && (row == 2 || row == rows - 3))
                    {
                        imgPath += "trap.png";
                    }
                    else
                    {
                        if ((column == 2 || column == 3 || column == columns - 3 || column == columns - 4)
                            && (row >= 4 && row <= 6))
                        {
                            imgPath += "water.png";
                        }
                        else
                        {
                            if ((row >= 1 && row <= 3) || (row >= 7 && row <= 9))
                            {
                                imgPath = MainWindow.resourceAnimals;
                                if (row == 1)
                                {
                                    if (column == 1) imgPath += "red_lion.png";
                                    if (column == columns - 2) imgPath += "red_tiger.png";
                                }

                                if (row == 2)
                                {
                                    if (column == 2) imgPath += "red_dog.png";
                                    if (column == columns - 3) imgPath += "red_cat.png";
                                }

                                if (row == 3)
                                {
                                    if (column == 1) imgPath += "red_rat.png";
                                    if (column == 3) imgPath += "red_leopard.png";
                                    if (column == columns - 4) imgPath += "red_wolf.png";
                                    if (column == columns - 2) imgPath += "red_elephant.png";
                                }

                                if (row == 7)
                                {
                                    if (column == 1) imgPath += "blue_elephant.png";
                                    if (column == 3) imgPath += "blue_wolf.png";
                                    if (column == columns - 4) imgPath += "blue_leopard.png";
                                    if (column == columns - 2) imgPath += "blue_rat.png";
                                }

                                if (row == 8)
                                {
                                    if (column == columns - 3) imgPath += "blue_dog.png";
                                    if (column == 2) imgPath += "blue_cat.png";
                                }

                                if (row == 9)
                                {
                                    if (column == columns - 2) imgPath += "blue_lion.png";
                                    if (column == 1) imgPath += "blue_tiger.png";
                                }
                            }
                        }
                    }
                }
            }

            try { image.Source = new BitmapImage(new Uri(imgPath)); }
            catch (Exception) { image.Source = new BitmapImage(new Uri(MainWindow.resourceOthers + "grass.png")); }

            return image;
        }
        private string FindDirectionArrowName(Direction dir, Animal a)
        {
            string result = string.Empty;
            switch (a.Team)
            {
                case Team.Red:
                    result += "red_";
                    break;
                case Team.Blue:
                    result += "blue_";
                    break;
            }

            switch (dir)
            {
                case Direction.Up:
                    result += "up.png";
                    break;
                case Direction.Down:
                    result += "down.png";
                    break;
                case Direction.Left:
                    result += "left.png";
                    break;
                case Direction.Right:
                    result += "right.png";
                    break;
            }
            return result;
        }
        private Image ArrowImage(Direction dir, double top, double left, Animal a)
        {
            Image image = new Image();
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());

            image.Width = tagGrid;
            image.Height = tagGrid;
            image.Source = new BitmapImage(new Uri(MainWindow.resourceDirections + FindDirectionArrowName(dir, a)));

            if (Math.Abs(top) == (3 * tagGrid) || Math.Abs(left) == (3 * tagGrid))
                image.Tag = Common.JUMP;
            else image.Tag = Common.WALK;

            image.Name = image.Source.ToString().Replace(MainWindow.resourceDirections, string.Empty)
                .Replace("blue_", string.Empty).Replace("red_", string.Empty).Replace(".png", string.Empty);

            Canvas.SetLeft(image, left);
            Canvas.SetTop(image, top);
            Panel.SetZIndex(image, 0);

            image.PreviewMouseDown += Direction_PreviewMouseDown;

            return image;
        }
        private Grid Container(int row, int column)
        {
            Grid grid = new Grid();
            grid.SizeChanged += Grid_SizeChanged;
            Image image = DeterminedImage(row, column);
            string imgSource = image.Source.ToString();
            ImageBrush myBrush = new ImageBrush();

            if (imgSource.Contains(MainWindow.resourceOthers))
            {
                myBrush.ImageSource = image.Source;
                grid.Background = myBrush;

                if (imgSource.Contains("wood")) grid.Tag = Common.BORDER;
                else if (imgSource.Contains("trap")) grid.Tag = Common.TRAP;
                else if (imgSource.Contains("water")) grid.Tag = Common.WATER;
                else if (imgSource.Contains("cave") && row == 1) grid.Tag = Common.RED_CAVE;
                else if (imgSource.Contains("cave") && row == rows - 2) grid.Tag = Common.BLUE_CAVE;
                //else grid.Tag = Common.GRASS;
            }
            else
            {
                myBrush.ImageSource = new BitmapImage(new Uri(MainWindow.resourceOthers + "grass.png"));
                grid.Background = myBrush;
                grid.Children.Add(image);
            }

            return grid;
        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());
            var grid = sender as Grid;
            var canvas = FindVisualChildren<Canvas>(grid).SingleOrDefault();
            List<Image> images = FindVisualChildren<Image>(canvas).ToList();

            foreach (var image in images)
            {
                int jump = 1;
                try { jump = int.Parse(image.Tag.ToString()); }
                catch (Exception) { jump = 1; }

                string sourceImage = image.Source.ToString();
                if (sourceImage.Contains("up"))
                {

                    Canvas.SetTop(image, -tagGrid * jump);
                    Canvas.SetLeft(image, 0);
                }
                if (sourceImage.Contains("down"))
                {
                    Canvas.SetTop(image, tagGrid * jump);
                    Canvas.SetLeft(image, 0);
                }
                if (sourceImage.Contains("left"))
                {
                    Canvas.SetTop(image, 0);
                    Canvas.SetLeft(image, -tagGrid * jump);
                }
                if (sourceImage.Contains("right"))
                {
                    Canvas.SetTop(image, 0);
                    Canvas.SetLeft(image, tagGrid * jump);
                }
                image.Width = tagGrid;
                image.Height = tagGrid;
            }
        }
        #endregion

        #region Dispose
        // Other managed resource this class uses.
        private Component component = new Component();
        // Track whether Dispose has been called.                                                                                          
        private bool disposed = false;
        // Pointer to an external unmanaged resource.
        private IntPtr handle;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;

                Debug.WriteLine("Dispose BoardPage ~");
            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~BoardPage()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
