using FluentValidation;

namespace HouseBoat.Application.Features.Reviews.Create;

public class CreateReviewValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.HouseBoatId)
            .NotEmpty()
            .WithMessage("House boat is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage(
                "Comment is required and cannot exceed 1000 characters.");
    }
}