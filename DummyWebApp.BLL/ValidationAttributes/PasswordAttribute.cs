namespace DummyWebApp.BLL.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.ResultConstants;
    using Microsoft.Extensions.DependencyInjection;
    using ValidationConstants.Auth;

    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var required = new RequiredAttribute();
            var length = new MinLengthAttribute(IdentityPasswordConstants.RequiredLength);

            if (!required.IsValid(value))
                return new ValidationResult(SharedValidationConstants.RequiredPassword);

            return !length.IsValid(value)
                ? new ValidationResult(SharedValidationConstants.InvalidPasswordLength)
                : ValidationResult.Success;
        }
    }
}