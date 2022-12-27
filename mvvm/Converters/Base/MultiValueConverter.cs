using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MVVM.Converters.Base
{
    /// <summary>Multi value converter</summary>
    [MarkupExtensionReturnType(typeof(MultiValueConverter))]
    public abstract class MultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>Convert values</summary>
        /// <param name="values">Massive of value to convert from</param>
        /// <param name="t">Target type</param>
        /// <param name="p">Parameter</param>
        /// <param name="c">Culture information</param>
        /// <returns>Converted value</returns>
        protected abstract object? Convert(object[]? values, Type? t, object? p, CultureInfo? c);

        /// <summary>Convert value back</summary>
        /// <param name="v">Value to be converted back</param>
        /// <param name="types">Massive of target types</param>
        /// <param name="p">Parameter</param>
        /// <param name="c">Culture information</param>
        /// <returns>Converted back values (massive)</returns>
        protected virtual object[]? ConvertBack(object? v, Type[]? types, object? p, CultureInfo? c) => 
            throw new NotSupportedException("Convert back is not supported");

        #region [MarkupExtension]

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider sp) => this;

        #endregion [MarkupExtension]

        #region [IMultiValueConverter]

        /// <inheritdoc />
        object? IMultiValueConverter.Convert(object[]? values, Type? t, object? p, CultureInfo? c) => 
            Convert(values, t, p, c);

        /// <inheritdoc />
        object[]? IMultiValueConverter.ConvertBack(object? v, Type[]? types, object? p, CultureInfo? c) => 
            ConvertBack(v, types, p, c);

        #endregion [IMultiValueConverter]
    }
}