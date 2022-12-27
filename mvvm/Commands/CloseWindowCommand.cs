using MVVM.Commands.Base;
using System.Windows;

namespace MVVM.Commands
{
    public class CloseWindowCommand : Command
    {
        public override bool CanExecute(object? parameter) => parameter is Window;

        public override void Execute(object? parameter) => (parameter as Window)?.Close();
    }
}