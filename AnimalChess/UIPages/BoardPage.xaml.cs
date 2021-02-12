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
        public List<Animal> animals = new List<Animal>();
        public static Image SelectedAnimal;
        public Team round { get; set; } = Team.Blue;
        public int rows { get; set; }
        public int columns { get; set; }

        private static string resourceAnimals, resourceControls;
        private static string resourceOthers, pathAudio;
        private static string exe = AppDomain.CurrentDomain.BaseDirectory;
        public BoardPage()
        {
            InitializeComponent();
        }
        public BoardPage(MainWindow _mainWindow)
        {
            InitializeComponent();

            mainWindow = _mainWindow;

            string bgResource = mainWindow.bgGrid.ImageSource.ToString();
            string resourcePath = bgResource.Substring(0, bgResource.LastIndexOf('/'));
            resourceAnimals = resourcePath + "/Animals/";
            resourceControls = resourcePath + "/Controls/";
            resourceOthers = resourcePath + "/Others/";

            pathAudio = Path.Combine(exe, "Audios");

            UILoadBoard();
            UILoadAnimal();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double t = (this.Width - 30) / 9;
            //double t = (this.ActualHeight - 70) / 11;
            double t = this.ActualHeight / 11;
            this.gridChessBoard.Tag = t;
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
                    Canvas canvas = Container(i, j);

                    if (canvas != null)
                    {
                        gridChessBoard.Children.Add(canvas);
                        Grid.SetRow(canvas, i);
                        Grid.SetColumn(canvas, j);
                    }
                }
            }
        }

        private void UILoadAnimal()
        {
            List<Canvas> list_canvas = FindVisualChildren<Canvas>(gridChessBoard).ToList();
            foreach (var canvas in list_canvas)
            {
                List<Image> images = FindVisualChildren<Image>(canvas).ToList();
                foreach (var image in images)
                {
                    if (image.Source.ToString().Contains(resourceAnimals))
                    {
                        image.Cursor = Cursors.Hand;
                        image.PreviewMouseDown += Animal_PreviewMouseDown;

                        Canvas c = image.Parent as Canvas;
                        string animalName = image.Source.ToString().Replace(resourceAnimals, "").Replace(".png", "");
                        int column = Grid.GetColumn(c);
                        int row = Grid.GetRow(c);

                        Animal animal = GetAnimal(animalName, column, row);
                        image.Tag = animal;

                        if ((row >= 1 && row <= 3))
                        {
                            animal.Team = Team.Red;
                            canvas.Background = Common.RED;
                        }
                        else if (row >= 7 && row <= 9)
                        {
                            animal.Team = Team.Blue;
                            canvas.Background = Common.BLUE;
                        }

                        animals.Add(animal);
                    }
                }
            }
        }

        private Animal GetAnimal(string animal_name, int column, int row)
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
            return a;
        }

        private void Animal_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var panel = img.Parent as Canvas;
            var animal = img.Tag as Animal;

            //Chặn lượt chơi
            if (round == Team.Red && panel.Background == Common.BLUE) return;
            else if (round == Team.Blue && panel.Background == Common.RED) return;

            if (SelectedAnimal != null && SelectedAnimal != img)
            {
                var animal2 = SelectedAnimal.Tag as Animal;
                var panel2 = SelectedAnimal.Parent as Canvas;
                if (animal2.Team == Team.Blue)
                    panel2.Background = Common.BLUE;
                else panel2.Background = Common.RED;

                RemoveAvailableDirection(panel2);
                Panel.SetZIndex(panel2, 5);
            }

            var cur_background = panel.Background as SolidColorBrush;
            if (cur_background != Common.YELLOW)
            {
                panel.Background = Common.YELLOW;
                AddAvailableDirection(panel, animal);
                SelectedAnimal = img;
                Panel.SetZIndex(panel, 10);
            }
            else
            {
                if (animal.Team == Team.Blue)
                    panel.Background = Common.BLUE;
                else panel.Background = Common.RED;

                RemoveAvailableDirection(panel);
                Panel.SetZIndex(panel, 5);
            }

            PlaySoundAnimal(animal.AnimalSound);
        }
        private int CountImage(Canvas c, string element)
        {
            try
            {
                var imgs = FindVisualChildren<Image>(c).ToList();
                return imgs.Where(x => x.Source.ToString().Contains(element)).ToList().Count;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        private Area DeterminedArea(Canvas c, Animal a = null)
        {
            var images = FindVisualChildren<Image>(c).ToList();
            if (CountImage(c, "jungle") != 0 && images.Count == 1) return Area.Border;
            else if (CountImage(c, "water") != 0 && images.Count == 1) return Area.Water;
            else if (CountImage(c, "trap") != 0 && images.Count == 1) return Area.Trap;
            else if (CountImage(c, "cave") != 0 && images.Count == 1) return Area.Cave;
            else //images.Count == 2
            {
                //if (a != null)
                //{

                Image animal_direction = images.Where(x => x.Source.ToString().Contains(resourceAnimals)).SingleOrDefault();
                Image other_direction = images.Where(x => x.Source.ToString().Contains(resourceOthers)).SingleOrDefault();
                Kiểm tra các hướng cho đúng ===>>> Nên dùng giấy nháp
                if (images[0] == null) return Area.Available;
                if (images[0].Source.ToString().Contains(resourceAnimals))
                {
                    Animal b = images[0].Tag as Animal;

                    if (a.Team == b.Team) return Area.SameTeam;
                    else if (a.Cur_LV == Level.Elephant && b.Cur_LV == Level.Rat) return Area.HigherLevel;
                    else if (a.Cur_LV == Level.Rat && b.Cur_LV == Level.Elephant) return Area.LessOrEqualLevel;
                    else if (a.Cur_LV >= b.Cur_LV) return Area.LessOrEqualLevel;
                    else return Area.HigherLevel;
                }
                else return Area.Available;
                //}
                //else return Area.Available;
            }
        }

        private Canvas FindCanvas(int column, int row)
        {
            List<Canvas> canvas = FindVisualChildren<Canvas>(gridChessBoard).ToList();
            Canvas c = canvas.Where(x => Grid.GetColumn(x) == column && Grid.GetRow(x) == row).SingleOrDefault();
            return c;
        }

        private bool IsDirectionAvailable(Direction drt, Canvas c, Animal a = null)
        {
            /*
                - Có con thú khác:
                + Cùng nhóm => false
                + Khác nhóm
                    . Có level >= Con thú đó => Có thể ăn => true
                    . Có level < Con thú đó
                        1. Bẫy => Có thể ăn => true
                        2. Còn lại => false
            - Chướng ngại vật
                + Bẫy: Cur_LV = 0 => true
                + Nước:
                    . Swim Available => true
                    . Swim NotAvailable => false
                    . Swim Jump
                        1. Có con thú khác => false
                        2. Không có con thú khác => true
                + Hang:
                    . Cùng nhóm => false
                    . Khác nhóm => Chiến thâng và kết thúc => true
                         */

            int col = Grid.GetColumn(c);
            int row = Grid.GetRow(c);
            Canvas tempCanvas;
            List<Image> images = new List<Image>();
            Area area = new Area();

            switch (drt)
            {
                case Direction.Up:
                    {
                        tempCanvas = FindCanvas(col, row - 1);
                        images = FindVisualChildren<Image>(tempCanvas).ToList();
                        area = DeterminedArea(tempCanvas, a);
                        break;
                    }
                case Direction.Down:
                    {
                        tempCanvas = FindCanvas(col, row + 1);
                        images = FindVisualChildren<Image>(tempCanvas).ToList();
                        area = DeterminedArea(tempCanvas, a);
                        break;
                    }
                case Direction.Left:
                    {
                        tempCanvas = FindCanvas(col - 1, row);
                        images = FindVisualChildren<Image>(tempCanvas).ToList();
                        area = DeterminedArea(tempCanvas, a);
                        break;
                    }
                case Direction.Right:
                    {
                        tempCanvas = FindCanvas(col + 1, row);
                        images = FindVisualChildren<Image>(tempCanvas).ToList();
                        area = DeterminedArea(tempCanvas, a);
                        break;
                    }
            }

            if (area == Area.Cave)
            {
                if ((a.Team == Team.Blue && images.Where(x => x.Tag.ToString() == "BLUE").ToList().Count != 0)
                    || (a.Team == Team.Red && images.Where(x => x.Tag.ToString() == "RED").ToList().Count != 0))
                    return false;
            }

            if (area == Area.Border || area == Area.SameTeam || area == Area.HigherLevel
                || (area == Area.Water && a.Swim != Swim.Available))
                return false;
            return true;
        }

        private void AddAvailableDirection(Canvas c, Animal a)
        {
            Image i;
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());
            if (IsDirectionAvailable(Direction.Up, c, a))
            {
                i = ArrowImage("up_blue.png", -tagGrid, 0, a);
                c.Children.Add(i);
            }
            if (IsDirectionAvailable(Direction.Down, c, a))
            {
                i = ArrowImage("down_blue.png", tagGrid, 0, a);
                c.Children.Add(i);
            }
            if (IsDirectionAvailable(Direction.Left, c, a))
            {
                i = ArrowImage("left_blue.png", 0, -tagGrid, a);
                c.Children.Add(i);
            }
            if (IsDirectionAvailable(Direction.Right, c, a))
            {
                i = ArrowImage("right_blue.png", 0, tagGrid, a);
                c.Children.Add(i);
            }
        }
        private void Direction_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            var animal = i.Tag as Animal;
            string direction = i.Name;
            Canvas canvasSource = i.Parent as Canvas;
            RemoveAvailableDirection(canvasSource);
            Canvas canvasDirectory;
            int cur_column = Grid.GetColumn(canvasSource);
            int cur_row = Grid.GetRow(canvasSource); ;

            switch (direction)
            {
                case "up":
                    {
                        canvasDirectory = FindCanvas(cur_column, cur_row - 1);
                        break;
                    }
                case "down":
                    {
                        canvasDirectory = FindCanvas(cur_column, cur_row + 1);
                        break;
                    }
                case "left":
                    {
                        canvasDirectory = FindCanvas(cur_column - 1, cur_row);
                        break;
                    }
                case "right":
                    {
                        canvasDirectory = FindCanvas(cur_column + 1, cur_row);
                        break;
                    }
                default:
                    canvasDirectory = null;
                    break;
            }

            if (canvasDirectory != null)
            {
                int colDirectory = Grid.GetColumn(canvasDirectory);
                int rowDirectory = Grid.GetRow(canvasDirectory);
                //cur_column cur_row

                #region Responsive Swap
                List<Image> imagesSource = FindVisualChildren<Image>(canvasSource).ToList();
                List<Image> imagesDirectory = FindVisualChildren<Image>(canvasDirectory).ToList();
                if (imagesSource.Count == 1 && imagesDirectory.Count == 0)
                {
                    Grid.SetColumn(canvasSource, colDirectory);
                    Grid.SetRow(canvasSource, rowDirectory);
                    Grid.SetColumn(canvasDirectory, cur_column);
                    Grid.SetRow(canvasDirectory, cur_row);

                    if (animal.Team == Team.Blue)
                        canvasSource.Background = Common.BLUE;
                    else canvasSource.Background = Common.RED;
                }
                else
                {
                    Image selected_animal = imagesSource.Where(x => x.Source.ToString()
                                                        .Contains(resourceAnimals))
                                                        .SingleOrDefault();

                    Image selected_other = imagesSource.Where(x => x.Source.ToString()
                                                       .Contains(resourceOthers))
                                                       .SingleOrDefault();

                    Image swap_animal = imagesDirectory.Where(x => x.Source.ToString()
                                                        .Contains(resourceAnimals))
                                                        .SingleOrDefault();

                    Image swap_other = imagesDirectory.Where(x => x.Source.ToString()
                                                       .Contains(resourceOthers))
                                                       .SingleOrDefault();

                    if (selected_animal != null)
                    {
                        // 1 Animal và 1 Others đổi chỗ
                        if (selected_animal != null && selected_other == null
                            && swap_animal == null && swap_other != null)
                        {
                            canvasSource.Children.Clear();
                            Panel.SetZIndex(canvasSource, 0);
                            canvasDirectory.Children.Add(selected_animal);
                        }

                        // 1 Animal và 2 (Others và Animal)
                        if (selected_animal != null && selected_other == null
                            && swap_animal != null && swap_other != null)
                        {
                            canvasSource.Children.Clear();
                            Panel.SetZIndex(canvasSource, 0);
                            canvasDirectory.Children.Remove(swap_animal);
                            canvasDirectory.Children.Add(selected_animal);
                        }

                        // 2(1 Animal và 1 Others) đổi chỗ với 1 (Others or Empty)
                        if (selected_animal != null && selected_other != null
                            && swap_animal == null && (swap_other == null || swap_other != null))
                        {
                            canvasSource.Children.Remove(selected_animal);
                            Panel.SetZIndex(canvasSource, 0);
                            canvasDirectory.Children.Add(selected_animal);
                            Panel.SetZIndex(canvasDirectory, 5);
                        }

                        // 2(1 Animal và 1 (Others or Empty)) đổi chỗ với 1 Animal
                        if (selected_animal != null && (selected_other != null || selected_other == null)
                            && swap_animal != null && swap_other == null)
                        {
                            canvasSource.Children.Remove(selected_animal);
                            Panel.SetZIndex(canvasDirectory, 0);
                            canvasDirectory.Children.Remove(swap_animal);
                            canvasDirectory.Children.Add(selected_animal);
                            Panel.SetZIndex(canvasDirectory, 5);
                        }

                        if (animal.Team == Team.Blue)
                            canvasDirectory.Background = Common.BLUE;
                        else canvasDirectory.Background = Common.RED;
                        canvasSource.Background = Common.TRANSPARENT;
                    }
                }
                //if()
                #endregion


            }
            else { }

            //if (animal.Team == Team.Blue)
            //    canvasSource.Background = Common.BLUE;
            //else canvasSource.Background = Common.RED;

            if (round == Team.Blue)
                round = Team.Red;
            else round = Team.Blue;
        }

        private void RemoveAvailableDirection(Canvas parent)
        {
            List<Image> images = FindVisualChildren<Image>(parent).ToList();

            foreach (var image in images)
            {
                string name = image.Name;
                if (name.Contains("up") || name.Contains("down") || name.Contains("left") || name.Contains("right"))
                {
                    parent.Children.Remove(image);
                }
            }
        }

        private void PlaySoundAnimal(string wavFilename)
        {
            try
            {
                SoundPlayer sound = new SoundPlayer();
                sound.SoundLocation = Path.Combine(pathAudio, wavFilename);
                sound.Play();
            }
            catch (Exception) { }
        }

        #region Element
        private Image BackgroundImage(int row, int column)
        {
            string imgPath = resourceOthers;
            Image img = new Image();
            if (row == 0 || column == 0 || row == rows - 1 || column == columns - 1)//Vị trí biên
            {
                imgPath += "jungle.png";
            }
            else
            {
                int mid = (int)(columns / 2);
                if (row == 1 && column == mid || row == rows - 2 && column == mid)
                {
                    imgPath += "cave.png";
                    if (row == 1) img.Tag = "RED";
                    else img.Tag = "BLUE";
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
                                imgPath = resourceAnimals;
                                if (row == 1)
                                {
                                    if (column == 1) imgPath += "lion.png";
                                    if (column == columns - 2) imgPath += "tiger.png";
                                }

                                if (row == 2)
                                {
                                    if (column == 2) imgPath += "dog.png";
                                    if (column == columns - 3) imgPath += "cat.png";
                                }

                                if (row == 3)
                                {
                                    if (column == 1) imgPath += "rat.png";
                                    if (column == 3) imgPath += "leopard.png";
                                    if (column == columns - 4) imgPath += "wolf.png";
                                    if (column == columns - 2) imgPath += "elephant.png";
                                }

                                if (row == 7)
                                {
                                    if (column == 1) imgPath += "elephant.png";
                                    if (column == 3) imgPath += "wolf.png";
                                    if (column == columns - 4) imgPath += "leopard.png";
                                    if (column == columns - 2) imgPath += "rat.png";
                                }

                                if (row == 8)
                                {
                                    if (column == columns - 3) imgPath += "dog.png";
                                    if (column == 2) imgPath += "cat.png";
                                }

                                if (row == 9)
                                {
                                    if (column == columns - 2) imgPath += "lion.png";
                                    if (column == 1) imgPath += "tiger.png";
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                img.Source = new BitmapImage(new Uri(imgPath));
                img.Width = double.Parse(this.gridChessBoard.Tag.ToString());
                img.Height = img.Width;
                Canvas.SetLeft(img, 0);
                Canvas.SetTop(img, 0);
            }
            catch (Exception) { img = null; }

            return img;
        }

        private Image ArrowImage(string direction, double top, double left, Animal a)
        {
            Image image = new Image();
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());

            image.Source = new BitmapImage(new Uri(resourceControls + direction));
            image.Width = tagGrid;
            image.Height = tagGrid;
            image.Tag = a;
            image.Name = direction.Substring(0, direction.IndexOf('_'));

            Canvas.SetTop(image, top);
            Canvas.SetLeft(image, left);
            Panel.SetZIndex(image, 3);

            image.PreviewMouseDown += Direction_PreviewMouseDown;

            return image;
        }

        private Canvas Container(int row, int column)
        {
            Canvas canvas = new Canvas();
            canvas.SizeChanged += Canvas_SizeChanged;
            Image imgElement = BackgroundImage(row, column);
            if (imgElement != null)
            {
                if (imgElement.Source.ToString().Contains(resourceAnimals))
                    Panel.SetZIndex(canvas, 5);
                else Panel.SetZIndex(canvas, 0);
                canvas.Children.Add(imgElement);
            }
            else canvas.SizeChanged -= Canvas_SizeChanged;//return null;
            return canvas;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvas = sender as Canvas;
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());
            List<Image> l_images = FindVisualChildren<Image>(canvas).ToList();

            foreach (var image in l_images)
            {
                string sourceImage = image.Source.ToString();
                if (sourceImage.Contains(resourceControls))
                {
                    if (sourceImage.Contains("up"))
                    {
                        Canvas.SetTop(image, -tagGrid);
                        Canvas.SetLeft(image, 0);
                    }
                    if (sourceImage.Contains("down"))
                    {
                        Canvas.SetTop(image, tagGrid);
                        Canvas.SetLeft(image, 0);
                    }
                    if (sourceImage.Contains("left"))
                    {
                        Canvas.SetTop(image, 0);
                        Canvas.SetLeft(image, -tagGrid);
                    }
                    if (sourceImage.Contains("right"))
                    {
                        Canvas.SetTop(image, 0);
                        Canvas.SetLeft(image, tagGrid);
                    }
                }
                image.Width = double.Parse(gridChessBoard.Tag.ToString());
                image.Height = double.Parse(gridChessBoard.Tag.ToString());
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
