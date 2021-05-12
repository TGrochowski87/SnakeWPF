using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnakeWPF.Models
{
    public class FoodModel
    {
        public UIElement Shape { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
}
