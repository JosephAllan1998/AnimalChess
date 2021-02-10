using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

namespace AnimalChess.UIPages
{
    /// <summary>
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Page, IDisposable
    {
        public MainWindow mainWindow { get; set; }
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
                    Canvas canvas = new Canvas();
                    canvas = Container(i, j);
                    gridChessBoard.Children.Add(canvas);
                    Grid.SetRow(canvas, i);
                    Grid.SetColumn(canvas, j);
                    
                }
            }
        }

        #region Element
        private Image BackgroundImage(int row, int column)
        {
            string bg = this.mainWindow.bgGrid.ImageSource.ToString();
            string resourcePath = bg.Substring(0, bg.LastIndexOf('/'));
            string imgPath = resourcePath + "/Others/";
            Image img;

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
                                imgPath = resourcePath + "/Animals/";
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
                img = new Image { Source = new BitmapImage(new Uri(imgPath)) };
            }
            catch (Exception)
            {
                imgPath = resourcePath + "/Others/grass.png";
                img = new Image { Source = new BitmapImage(new Uri(imgPath)) };
            }

            return img;
        }

        private Canvas Container(int row, int column)
        {
            Canvas canvas = new Canvas();
            canvas.SizeChanged += Canvas_SizeChanged;
            Image img = BackgroundImage(row, column);
            img.Width = double.Parse(this.gridChessBoard.Tag.ToString());
            img.Height = img.Width;

            if(img.Source.ToString().Contains("Animals"))
            {
                img.Cursor = Cursors.Hand;
                if((row >= 1 && row <= 3)) canvas.Background = new SolidColorBrush(Color.FromRgb(255,0,0));
                else if(row >= 7 && row <= 9) canvas.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            }

            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            canvas.Children.Add(img);
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double t = (this.Width - 30) / 9;
            //double t = (this.ActualHeight - 70) / 11;
            double t = this.ActualHeight / 11;
            this.gridChessBoard.Tag = t;
        }

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
