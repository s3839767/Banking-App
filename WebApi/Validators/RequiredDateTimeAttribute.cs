using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApi.Validators;

// Reference: https://stackoverflow.com/a/40808293
public class RequiredDateTimeAttribute : ValidationAttribute
{
    public RequiredDateTimeAttribute()
    {
        ErrorMessage = "The {0} field is required";
    }

    public override bool IsValid(object value)
    {
        if(value is not DateTime time)
            throw new ArgumentException("value must be a DateTime object");
            
        return time != DateTime.MinValue;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
    }
}
