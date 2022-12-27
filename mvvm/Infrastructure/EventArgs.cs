using System;
using System.Diagnostics.CodeAnalysis;

namespace MVVM.Infrastructure
{
    /// <summary>Events args with generic parameter</summary>
    public class EventArgs<TArgument> : EventArgs
    {
        /// <summary>Argument parameter</summary>
        public TArgument Argument { get; set; }

        /// <summary>Initialize new instance of <see cref="EventArgs"/></summary>
        public EventArgs()
        { }

        /// <summary>Initialize new instance of <see cref="EventArgs"/></summary>
        /// <param name="Argument">Argument parameter</param>
        public EventArgs(TArgument Argument) => this.Argument = Argument;

        /// <summary>String representation</summary>
        public override string ToString() => Argument.ToString();

        #region [Operators]

        public static implicit operator TArgument([NotNull] EventArgs<TArgument> Args) => Args.Argument;

        public static implicit operator EventArgs<TArgument>(TArgument Argument) => new(Argument);

        #endregion [Operators]
    }
}