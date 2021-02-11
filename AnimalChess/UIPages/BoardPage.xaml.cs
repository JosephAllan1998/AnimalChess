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
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
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
                    if (image.Tag != null)
                    {
                        image.Cursor = Cursors.Hand;
                        image.PreviewMouseDown += Animal_PreviewMouseDown;

                        string tag = image.Tag.ToString();
                        string[] splits = tag.Split('_');
                        string animalName = splits[0];
                        int column = int.Parse(splits[1]);
                        int row = int.Parse(splits[2]);

                        Animal animal = GetAnimal(animalName, column, row);
                        image.Tag = animal;

                        if ((row >= 1 && row <= 3))
                        {
                            animal.Player = Player.Red;
                            canvas.Background = new SolidColorBrush(Color.FromRgb(202, 36, 48));//Red team
                        }
                        else if (row >= 7 && row <= 9)
                        {
                            animal.Player = Player.Blue;
                            canvas.Background = new SolidColorBrush(Color.FromRgb(51, 101, 177));//Blue team
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

            Position position = new Position
            {
                Column = column,
                Row = row
            };
            a.Position = position;
            return a;
        }

        private void Animal_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var panel = img.Parent as Canvas;
            var animal = img.Tag as Animal;

            //panel.Background = new SolidColorBrush(Color.FromRgb(202, 36, 48));//Red team
            //panel.Background = new SolidColorBrush(Color.FromRgb(51, 101, 177));//Blue team
            //panel.Background = new SolidColorBrush(Color.FromRgb(253, 184, 39));//Select team

            PlaySoundAnimal(animal.AnimalSound);
            AddAvailableDirection(panel, animal.Position);
        }

        private void AddAvailableDirection(Canvas c, Position p)
        {
            Image i;
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());
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
            i = ArrowImage("up_blue.png", -tagGrid, 0, p.Column, p.Row);
            c.Children.Add(i);

            i = ArrowImage("down_blue.png", tagGrid, 0, p.Column, p.Row);
            c.Children.Add(i);

            i = ArrowImage("left_blue.png", 0, -tagGrid, p.Column, p.Row);
            c.Children.Add(i);

            i = ArrowImage("right_blue.png", 0, tagGrid, p.Column, p.Row);
            c.Children.Add(i);
        }
        private void Direction_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            string[] splits = i.Tag.ToString().Split('_');

            string direction = splits[0];
            int cur_column = int.Parse(splits[1]);
            int cur_row = int.Parse(splits[2]);

            if (direction == "up")
            {
                //MessageBox.Show($"Tiến lên:\n\t- Cột {cur_column}\n\t- Dòng {cur_row - 1}");
            }
            else if (direction == "down")
            {
                //MessageBox.Show($"Lùi xuống:\n\t- Cột {cur_column}\n\t- Dòng {cur_row + 1}");
            }
            else if (direction == "left")
            {
                //MessageBox.Show($"Qua trái:\n\t- Cột {cur_column - 1}\n\t- Dòng {cur_row}");
            }
            else if (direction == "right")
            {
                //MessageBox.Show($"Qua phải:\n\t- Cột {cur_column + 1}\n\t- Dòng {cur_row}");
            }

            RemoveAvailableDirection(i.Parent as Canvas);
        }

        private void RemoveAvailableDirection(Canvas parent)
        {
            List<Image> images = FindVisualChildren<Image>(parent).ToList();

            foreach (var image in images)
            {
                string tag = image.Tag.ToString();
                if (tag.Contains("up") || tag.Contains("down") || tag.Contains("left") || tag.Contains("right"))
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
            Panel.SetZIndex(img, 0);
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
                                Panel.SetZIndex(img, 1);
                                imgPath = resourceAnimals;
                                if (row == 1)
                                {
                                    if (column == 1)
                                    {
                                        imgPath += "lion.png";
                                        img.Tag = $"lion_{column}_{row}";
                                    }
                                    if (column == columns - 2)
                                    {
                                        imgPath += "tiger.png";
                                        img.Tag = $"tiger_{column}_{row}";
                                    }
                                }

                                if (row == 2)
                                {
                                    if (column == 2)
                                    {
                                        imgPath += "dog.png";
                                        img.Tag = $"dog_{column}_{row}";
                                    }
                                    if (column == columns - 3)
                                    {
                                        imgPath += "cat.png";
                                        img.Tag = $"cat_{column}_{row}";
                                    }
                                }

                                if (row == 3)
                                {
                                    if (column == 1)
                                    {
                                        imgPath += "rat.png";
                                        img.Tag = $"rat_{column}_{row}";
                                    }
                                    if (column == 3)
                                    {
                                        imgPath += "leopard.png";
                                        img.Tag = $"leopard_{column}_{row}";
                                    }
                                    if (column == columns - 4)
                                    {
                                        imgPath += "wolf.png";
                                        img.Tag = $"wolf_{column}_{row}";
                                    }
                                    if (column == columns - 2)
                                    {
                                        imgPath += "elephant.png";
                                        img.Tag = $"elephant_{column}_{row}";
                                    }
                                }

                                if (row == 7)
                                {
                                    if (column == 1)
                                    {
                                        imgPath += "elephant.png";
                                        img.Tag = $"elephant_{column}_{row}";
                                    }
                                    if (column == 3)
                                    {
                                        imgPath += "wolf.png";
                                        img.Tag = $"wolf_{column}_{row}";
                                    }
                                    if (column == columns - 4)
                                    {
                                        imgPath += "leopard.png";
                                        img.Tag = $"leopard_{column}_{row}";
                                    }
                                    if (column == columns - 2)
                                    {
                                        imgPath += "rat.png";
                                        img.Tag = $"rat_{column}_{row}";
                                    }
                                }

                                if (row == 8)
                                {
                                    if (column == columns - 3)
                                    {
                                        imgPath += "dog.png";
                                        img.Tag = $"dog_{column}_{row}";
                                    }
                                    if (column == 2)
                                    {
                                        imgPath += "cat.png";
                                        img.Tag = $"cat_{column}_{row}";
                                    }
                                }

                                if (row == 9)
                                {
                                    if (column == columns - 2)
                                    {
                                        imgPath += "lion.png";
                                        img.Tag = $"lion_{column}_{row}";
                                    }
                                    if (column == 1)
                                    {
                                        imgPath += "tiger.png";
                                        img.Tag = $"tiger_{column}_{row}";
                                    }
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

        private Image ArrowImage(string direction, double top, double left, int cur_column, int cur_row)
        {
            Image image = new Image();
            double tagGrid = double.Parse(gridChessBoard.Tag.ToString());

            image.Source = new BitmapImage(new Uri(resourceControls + direction));
            image.Width = tagGrid;
            image.Height = tagGrid;
            image.Tag = direction.Substring(0, direction.IndexOf('_')) + $"_{cur_column}_{cur_row}";

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
                canvas.Children.Add(imgElement);
            else return null;
            return canvas;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvas = sender as Canvas;
            List<Image> l_images = FindVisualChildren<Image>(canvas).ToList();

            foreach (var image in l_images)
            {
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
