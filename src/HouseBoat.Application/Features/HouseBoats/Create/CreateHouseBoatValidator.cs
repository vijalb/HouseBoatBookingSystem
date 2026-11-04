using FluentValidation;

namespace HouseBoat.Application.Features.HouseBoats.Create;

public class CreateHouseBoatValidator: AbstractValidator<CreateHouseBoatCommand>
{
    public CreateHouseBoatValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Capacity)
            .GreaterThan(0);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0);

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(1);
    }
}