using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MVVM.Converters.Base
{
    /// <summary>Value converter</summary>
    [MarkupExtensionReturnType(typeof(ValueConverter))]
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        /// <summary>Convert values</summary>
        /// <param name="v">Value to convert from</param>
        /// <param name="t">Target type</param>
        /// <param name="p">Parameter</param>
        /// <param name="c">Culture information</param>
        /// <returns>Converted value</returns>
        protected abstract object? Convert(object? v, Type t, object? p, CultureInfo c);

        /// <summary>Convert value back</summary>
        /// <param name="v">Value to be converted back</param>
        /// <param name="t">Target type</param>
        /// <param name="p">Parameter</param>
        /// <param name="c">Culture information</param>
        /// <returns>Converted back value</returns>
        protected virtual object? ConvertBack(object? v, Type t, object? p, CultureInfo c) =>
            throw new NotSupportedException("Convert back is not supported");

        #region [MarkupExtension]

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider sp) => this;

        #endregion [MarkupExtension]

        #region [IValueConverter]

        /// <inheritdoc />
        object? IValueConverter.Convert(object? v, Type t, object? p, CultureInfo c) => Convert(v, t, p, c);

        /// <inheritdoc />
        object? IValueConverter.ConvertBack(object? v, Type t, object? p, CultureInfo c) => ConvertBack(v, t, p, c);

        #endregion [IValueConverter]
    }
}