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
        public static SolidColorBrush RED { get; } = new SolidColorBrush(Color.FromRgb(202, 36, 48));
        public static SolidColorBrush BLUE { get; } = new SolidColorBrush(Color.FromRgb(51, 101, 177));
        public static SolidColorBrush YELLOW { get; } = new SolidColorBrush(Color.FromRgb(253, 184, 39));
        public static Brush TRANSPARENT { get; } = Brushes.Transparent;
        public static string BORDER { get; } = "BORDER";
        public static string TRAP { get; } = "TRAP";
        public static string WATER { get; } = "WATER";
        public static string RED_CAVE { get; } = "RED_CAVE";
        public static string BLUE_CAVE { get; } = "BLUE_CAVE";
        public static string GRASS { get; } = "GRASS";
        public static int WALK { get; } = 1;
        public static int JUMP { get; } = 3;
    }
}
