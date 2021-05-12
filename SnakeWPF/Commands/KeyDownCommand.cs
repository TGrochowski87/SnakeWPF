using SnakeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnakeWPF.Commands
{
    /// <summary>
    /// A custom command for handling KeyDown events
    /// </summary>
    public class KeyDownCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private SnakeModel _snake;

        public KeyDownCommand(ref SnakeModel snake)
        {
            _snake = snake;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            switch (parameter)
            {
                case "W":
                    _snake.Direction = Direction.North;
                    break;
                case "A":
                    _snake.Direction = Direction.West;
                    break;
                case "S":
                    _snake.Direction = Direction.South;
                    break;
                case "D":
                    _snake.Direction = Direction.East;
                    break;
                default:
                    break;
            }
        }
    }
}
