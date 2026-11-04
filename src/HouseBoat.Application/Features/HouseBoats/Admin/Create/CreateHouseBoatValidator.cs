using FluentValidation;

namespace HouseBoat.Application.Features.HouseBoats.Admin.Create;

public class CreateHouseBoatValidator
    : AbstractValidator<CreateHouseBoatCommand>
{
    public CreateHouseBoatValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150)
            .WithMessage("House boat name is required and cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Description is required and cannot exceed 2000 characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Location is required and cannot exceed 200 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0)
            .WithMessage("Price per night must be greater than zero.");

        RuleFor(x => x.Bedrooms)
            .GreaterThan(0)
            .WithMessage("Bedrooms must be greater than zero.");
    }
}