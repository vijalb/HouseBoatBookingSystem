using FluentValidation;

namespace HouseBoat.Application.Features.Reviews.GetByHouseBoat;

public class GetReviewsByHouseBoatValidator
    : AbstractValidator<GetReviewsByHouseBoatQuery>
{
    public GetReviewsByHouseBoatValidator()
    {
        RuleFor(x => x.HouseBoatId)
            .NotEmpty()
            .WithMessage("House boat is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");
    }
}