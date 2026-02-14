
using FluentValidation;
using ResourceTracker.Application.Common.Helper;

namespace ResourceTracker.Application.Features.Commands.PictureCommands.CreatePicture
{
    public class CreatePictureValidator : AbstractValidator<CreatePictureCommand>
    {
        public CreatePictureValidator() 
        { 
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Picture name is required.")
                .MaximumLength(255)
                .WithMessage("Picture name must not exceed 255 characters.");
            RuleFor(x => x.AltText)
                .MaximumLength(200)
                .WithMessage("Alt text must not exceed 200 characters.");
            RuleFor(x => x.LinkedEntityId)
                .GreaterThan(0)
                .WithMessage("Linked ID is required.");
            RuleFor(x => x.ImageUploadType)
                .IsInEnum()
                .WithMessage("Invalid image upload type.");
            RuleFor(x => x.Data)
                .NotNull()
                .WithMessage("Picture file is required.")
                .Must(file => file.Length > 0)
                .WithMessage("Picture file cannot be empty.")
                .Must(file => file.Length <= ImageHelper.MaxFileSizeInBytes)
                .WithMessage("Picture file must not exceed 5MB.")
                .Must(file => !string.IsNullOrWhiteSpace(file.ContentType) && file.ContentType.StartsWith("image/"))
                .WithMessage($"Only {string.Join(", ", ImageHelper.AllowedExtensions)} images are allowed.")
                .Must(file =>
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return ImageHelper.AllowedExtensions.Contains(extension);
                })
                .WithMessage("Invalid image file extension.");
        }
    }
}
