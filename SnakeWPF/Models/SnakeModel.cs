using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SnakeWPF.Models
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public class SnakeModel
    {
        public UIElement HeadShape { get; set; }
        public int HeadX { get; set; }
        public int HeadY { get; set; }
        public ObservableCollection<BodyElementModel> Body { get; set; }
        public Direction Direction { get; set; }
    }
}
