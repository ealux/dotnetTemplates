using MVVM.Commands.Base;
using MVVM.Infrastructure;
using MVVM.ViewModels.Base;
using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace MVVM.Commands
{
    /// <summary> LambdaCommand (RelayCommnad) provides ctor-step (can-)execute methods release </summary>
    [MarkupExtensionReturnType(typeof(LambdaCommand))]
    public class LambdaCommand : Command
    {
        public static LambdaCommand OnExecute(Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null) => new(ExecuteAction, CanExecute);

        public static LambdaCommand OnExecute(Action ExecuteAction, Func<object?, bool>? CanExecute = null) => new(ExecuteAction, CanExecute);

        #region [Events]

        /// <summary>Invokes on command cancellation</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled?.Invoke(this, args ?? EventArgs.Empty);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting(CancelEventArgs args) => StartExecuting?.Invoke(this, args);

        public event EventHandler<EventArgs<object?>>? CompleteExecuting;

        protected virtual void OnCompleteExecuting(EventArgs<object?> args) => CompleteExecuting?.Invoke(this, args);

        #endregion [Events]

        #region [Fields]

        /// <summary>Main execution action</summary>
        protected Action<object?>? _ExecuteAction;

        /// <summary>Check if command can be executed function</summary>
        protected Func<object?, bool>? _CanExecute;

        #endregion [Fields]

        #region [Properties]

        /// <summary>Check if command can be executed function</summary>
        public Func<object?, bool>? CanExecuteDelegate
        {
            get => _CanExecute;
            set
            {
                if (ReferenceEquals(_CanExecute, value)) return;
                _CanExecute = value ?? (_ => true);
                OnPropertyChanged(nameof(CanExecuteDelegate));
            }
        }

        #endregion [Properties]

        #region [Constructors]

        protected LambdaCommand()
        { }

        public LambdaCommand(Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null)
            : this()
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = CanExecute;
        }

        public LambdaCommand(Action<object?> ExecuteAction, Func<bool>? CanExecute) : this(ExecuteAction, CanExecute is null ? null : _ => CanExecute!())
        {
        }

        public LambdaCommand(Action ExecuteAction, Func<object?, bool>? CanExecute = null) : this(_ => ExecuteAction(), CanExecute)
        {
        }

        public LambdaCommand(Action ExecuteAction, Func<bool>? CanExecute) : this(_ => ExecuteAction(), CanExecute is null ? null : _ => CanExecute!())
        {
        }

        #endregion [Constructors]

        #region [Methods]

        /// <summary>Command execute</summary>
        /// <param name="parameter">Runtime proccess parameter</param>
        /// <exception cref="InvalidOperationException">Method for command exectution is not defined</exception>
        public override void Execute(object? parameter)
        {
            if (_ExecuteAction is null) throw new InvalidOperationException("Метод выполнения команды не определён");
            if (!CanExecute(parameter)) return;
            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }
            _ExecuteAction(parameter);
            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        /// <summary>Check if command can be executed function</summary>
        /// <param name="parameter">Runtime proccess parameter</param>
        /// <returns><see cref="true"/> if command can be executed</returns>
        public override bool CanExecute(object? parameter) =>
            ViewModel.IsDesignMode || IsCanExecute && (_CanExecute?.Invoke(parameter) ?? true);

        /// <summary>Check if command can be executed function</summary>
        public void CanExecuteCheck() => OnCanExecuteChanged();

        #endregion [Methods]

        public static implicit operator LambdaCommand(Action execute) => ToLambdaCommand(execute);

        public static implicit operator LambdaCommand(Action<object?> execute) => ToLambdaCommand(execute);

        public static implicit operator LambdaCommand((Action Execute, Func<bool> CanExecute) info) => new(info.Execute, info.CanExecute);

        public static implicit operator LambdaCommand((Action<object?> Execute, Func<object?, bool> CanExecute) info) => new(info.Execute, info.CanExecute);

        public static LambdaCommand ToLambdaCommand(Action execute) => new(execute);

        public static LambdaCommand ToLambdaCommand(Action<object?> execute) => new(execute);
    }

    /// <summary> Generic LambdaCommand (RelayCommnad) provides ctor-step (can-)execute methods release </summary>
    public class LambdaCommand<T> : Command
    {
        #region [Events]

        /// <summary>Invoke on command cancel</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled?.Invoke(this, args ?? EventArgs.Empty);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting(CancelEventArgs args) => StartExecuting?.Invoke(this, args);

        public event EventHandler<EventArgs<object?>>? CompleteExecuting;

        protected virtual void OnCompleteExecuting(EventArgs<object?> args) => CompleteExecuting?.Invoke(this, args);

        #endregion [Events]

        #region [Fields]

        /// <summary>Main execution action</summary>
        protected Action<T?>? _ExecuteAction;

        /// <summary>Check if command can be executed function</summary>
        protected Func<T?, bool>? _CanExecute;

        #endregion [Fields]

        #region [Properties]

        /// <summary>Check if command can be executed function</summary>
        public Func<T?, bool>? CanExecuteDelegate
        {
            get => _CanExecute;
            set
            {
                if (ReferenceEquals(_CanExecute, value)) return;
                _CanExecute = value;
                OnPropertyChanged();
            }
        }

        #endregion [Properties]

        #region [Constructors]

        /// <summary> Implicit constructor for inheritance, making fucntions by code </summary>
        protected LambdaCommand()
        { }

        public LambdaCommand(Action<T?> ExecuteAction, Func<bool>? CanExecute)
            : this(ExecuteAction, CanExecute is null ? null : new Func<T?, bool>(_ => CanExecute()))
        { }

        public LambdaCommand(Action<T?> ExecuteAction, Func<T?, bool>? CanExecute = null)
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = ViewModel.IsDesignMode ? (_ => true) : CanExecute;
        }

        #endregion [Constructors]

        #region [Methods]

        /// <summary> ValueConverter </summary>
        public static T ConvertParameter(object? parameter)
        {
            if (parameter is null) return default!;
            if (parameter is T result) return result;

            var command_type = typeof(T);
            var parameter_type = parameter.GetType();

            if (command_type.IsAssignableFrom(parameter_type))
                return (T)parameter;

            var command_type_converter = TypeDescriptor.GetConverter(command_type);
            if (command_type_converter.CanConvertFrom(parameter_type))
                return ((T)command_type_converter.ConvertFrom(parameter))!;

            var parameter_converter = TypeDescriptor.GetConverter(parameter_type);
            if (parameter_converter.CanConvertTo(command_type))
                return (T)parameter_converter.ConvertFrom(parameter)!;

            return default!;
        }

        public override void Execute(object? parameter)
        {
            var execute_action = _ExecuteAction
                    ?? throw new InvalidOperationException(@"Метод выполнения команды не определён");

            if (parameter is not T value)
                value = parameter is null
                    ? default!
                    : ConvertParameter(parameter);

            if (!CanExecute(value)) return;

            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }

            if (_CanExecute?.Invoke(value!) == false) return;
            execute_action.Invoke(value!);
            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        public override bool CanExecute(object? obj)
        {
            if (ViewModel.IsDesignMode) return true;
            if (!IsCanExecute) return false;
            return _CanExecute is not { } can_execute || obj switch
            {
                null => can_execute(default!),
                T parameter => can_execute(parameter),
                _ => can_execute(ConvertParameter(obj))
            };
        }

        public void CanExecuteCheck() => OnCanExecuteChanged();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _ExecuteAction = null;
            _CanExecute = null;
        }

        #endregion [Methods]

        public static implicit operator LambdaCommand<T?>(Action<T?> execute) => new(execute);

        public static implicit operator LambdaCommand<T?>((Action<T?> Execute, Func<T?, bool> CanExecute) info) => new(info.Execute, info.CanExecute);
    }
}