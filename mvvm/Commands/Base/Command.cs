using MVVM.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;

namespace MVVM.Commands.Base
{
    public abstract class Command : MarkupExtension, ICommand, INotifyPropertyChanged, IDisposable
    {
        #region [Static methods]

        public static LambdaCommand New(Action onExecute, Func<bool>? canExecute = null) => new(onExecute, canExecute);

        public static LambdaCommand New(Action<object?> onExecute, Func<object?, bool>? canExecute = null) => new(onExecute, canExecute);

        public static LambdaCommand New(Action<object?> onExecute, Func<bool>? canExecute) => new(onExecute, canExecute);

        public static LambdaCommand<T> New<T>(Action<T?> onExecute, Func<T?, bool>? canExecute = null) => new(onExecute, canExecute);

        public static LambdaCommand<T> New<T>(Action<T?> onExecute, Func<bool>? canExecute) => new(onExecute, canExecute);

        public static LambdaCommandAsync New(Func<Task> onExecute, Func<bool>? canExecute) => new(onExecute, canExecute);

        public static LambdaCommandAsync New(Func<object?, Task> onExecute, Func<object?, bool>? canExecute = null) => new(onExecute, canExecute);

        public static LambdaCommandAsync New(Func<object?, Task> onExecute, Func<bool>? canExecute) => new(onExecute, canExecute);

        public static LambdaCommandAsync<T> New<T>(Func<T?, Task> onExecute, Func<T?, bool>? canExecute = null) => new(onExecute, canExecute);

        public static LambdaCommandAsync<T> New<T>(Func<T?, Task> onExecute, Func<bool>? canExecute) => new(onExecute, canExecute);

        #endregion [Static methods]

        #region [Events]

        #region INotifyPropertyChanged

        private event PropertyChangedEventHandler? propertyChangedHandlers;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => propertyChangedHandlers += value;
            remove => propertyChangedHandlers -= value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            propertyChangedHandlers?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion INotifyPropertyChanged

        #region ICommand

        private event EventHandler? canExecuteChangedHandlers;

        /// <summary> Invoke on command CanExecute capability changes </summary>
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                canExecuteChangedHandlers += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                canExecuteChangedHandlers -= value;
            }
        }

        protected virtual void OnCanExecuteChanged(EventArgs? e = null) =>
            canExecuteChangedHandlers?.Invoke(this, e ?? EventArgs.Empty);

        #endregion ICommand

        #endregion [Events]

        #region [Properties]

        private WeakReference? _targetObjectReference;
        private WeakReference? _rootObjectReference;
        private WeakReference? _targetPropertyReference;

        protected object? TargetObject => _targetObjectReference?.Target;
        protected object? RootObject => _rootObjectReference?.Target;
        protected object? TargetProperty => _targetPropertyReference?.Target;

        #region IsCanExecute

        /// <summary> Can command be executed </summary>
        private bool _isCanExecute;

        /// <summary> Can command be executed </summary>
        public bool IsCanExecute
        {
            get => _isCanExecute;
            set
            {
                if (_isCanExecute == value) return;
                _isCanExecute = value;
                OnPropertyChanged(nameof(IsCanExecute));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion IsCanExecute

        #endregion [Properties]

        #region [MarkupExtension]

        /// <<inheritdoc/>
        public override object ProvideValue(IServiceProvider sp)
        {
            var target_value_provider = (IProvideValueTarget)sp.GetService(typeof(IProvideValueTarget))!;
            if (target_value_provider != null)
            {
                var target = target_value_provider.TargetObject;
                _targetObjectReference = target is null ? null : new WeakReference(target);
                var target_property = target_value_provider.TargetProperty;
                _targetPropertyReference = target_property is null ? null : new WeakReference(target_property);
            }

            var root_object_provider = (IRootObjectProvider)sp.GetService(typeof(IRootObjectProvider))!;
            if (root_object_provider != null)
            {
                var root = root_object_provider.RootObject;
                _rootObjectReference = root is null ? null : new WeakReference(root);
            }

            return this;
        }

        #endregion [MarkupExtension]

        #region [ICommand]

        public virtual bool CanExecute(object? parameter) => ViewModel.IsDesignMode || _isCanExecute;

        public abstract void Execute(object? parameter);

        bool ICommand.CanExecute(object? parameter) => CanExecute(parameter);

        void ICommand.Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            Execute(parameter);
        }

        #endregion [ICommand]

        #region [IDisposable]

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;
            _disposed = true;
        }

        #endregion [IDisposable]
    }
}