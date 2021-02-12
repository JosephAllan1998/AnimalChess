using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AnimalChess.Models
{
    public class Common
    {
        public static SolidColorBrush RED { get; set; } = new SolidColorBrush(Color.FromRgb(202, 36, 48));
        public static SolidColorBrush BLUE { get; set; } = new SolidColorBrush(Color.FromRgb(51, 101, 177));
        public static SolidColorBrush YELLOW { get; set; } = new SolidColorBrush(Color.FromRgb(253, 184, 39));
        public static Brush TRANSPARENT { get; set; } = Brushes.Transparent;
    }
}
