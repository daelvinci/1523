using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Attributes.ValidationAttributes
{
    public class AllowedFileTypeAttribute:ValidationAttribute
    {
        private readonly string[] _types;

        public AllowedFileTypeAttribute(params string[] types)
        {
            _types = types;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();

            if (value is IFormFile)
                files.Add((IFormFile)value);

            else if (value is List<IFormFile>)
                files.AddRange((List<IFormFile>)value);

            foreach (var item in files)
            {
                if(!_types.Contains(item.ContentType))
                {
                    return new ValidationResult($"The type must be {String.Join(',',_types)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
