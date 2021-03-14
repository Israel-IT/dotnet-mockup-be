namespace DummyWebApp.BLL.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.ResultConstants;

    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var required = new RequiredAttribute();
            var length = new MinLengthAttribute(IdentityPasswordConstants.RequiredLength);

            if (!required.IsValid(value))
                return required.GetValidationResult(value, validationContext);

            return !length.IsValid(value)
                ? length.GetValidationResult(value, validationContext)
                : ValidationResult.Success;
        }
    }
}