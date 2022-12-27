using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MVVM.ValidationRules
{
    /// <summary>Check on regex corresponded</summary>
    public class RegExpValidationRule : ValidationRule
    {
        #region [Properties]

        /// <summary>Error message</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Allow null reference</summary>
        public bool AllowNull { get; set; }

        /// <summary>Message when <see langword="null"/> if <see cref="AllowNull"/> == <see langword="false"/>.</summary>
        public string? NullReferenceMessage { get; set; }

        /// <summary>Error format message</summary>
        public string? FormatErrorMessage { get; set; }

        /// <summary>Allow non-string values (methods <see cref="object.ToString"/> will be invoked)</summary>
        public bool AllowNotString { get; set; }

        /// <summary>Error text if non-string value provided</summary>
        public string? NotStringErrorMessage { get; set; }

        /// <summary>Regular expression</summary>
        [ConstructorArgument(nameof(Expression))]
        public string? Expression { get; set; }

        #endregion [Properties]

        /// <summary>Default constructor</summary>
        public RegExpValidationRule()
        { }

        /// <summary>Constructor</summary>
        /// <param name="Expression">Regular expression to check</param>
        public RegExpValidationRule(string Expression) => this.Expression = Expression;

        /// <summary>Check if value is valid for regex</summary>
        /// <param name="value">Value to validate</param>
        /// <param name="c">Current culture information</param>
        /// <returns><see cref="ValidationResult.ValidResult"/> if value is valid</returns>
        public override ValidationResult Validate(object? value, CultureInfo c)
        {
            if (Expression is not { Length: > 0 } expr) return ValidationResult.ValidResult;
            var valid = ValidationResult.ValidResult;
            if (value is null)
                return AllowNull
                    ? valid
                    : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Value is not provided");

            if (value is not string str)
                return AllowNotString
                    ? valid
                    : new ValidationResult(false, NotStringErrorMessage ?? ErrorMessage ?? $"Value {value} is not a string");

            var match = Regex.Match(str, expr);
            return match.Success
                ? valid
                : new ValidationResult(false, FormatErrorMessage ?? ErrorMessage ?? $"Expression {expr} is not found at {str}");
        }
    }
}