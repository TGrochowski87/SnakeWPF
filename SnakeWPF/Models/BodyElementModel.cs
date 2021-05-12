using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SnakeWPF.Models
{
    public class BodyElementModel
    {
        public UIElement Shape { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
}
