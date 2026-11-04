using FluentValidation;

namespace HouseBoat.Application.Features.Bookings.Create;

public class CreateBookingValidator: AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.HouseBoatId)
            .NotEmpty()
            .WithMessage("House boat is required.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Check-in date is required.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty()
            .WithMessage("Check-out date is required.");

        RuleFor(x => x.CheckInDate)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Check-in date cannot be in the past.");

        RuleFor(x => x.CheckOutDate)
    .Must((model, checkOutDate) =>
        checkOutDate.Date > model.CheckInDate.Date)
    .WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x)
            .Must(x =>
            {
                var nights =
                    (x.CheckOutDate.Date - x.CheckInDate.Date).Days;

                return nights >= 1;
            })
            .WithMessage("Booking must be for at least 1 night.");

        RuleFor(x => x)
            .Must(x =>
            {
                var nights =
                    (x.CheckOutDate.Date - x.CheckInDate.Date).Days;

                return nights <= 7;
            })
            .WithMessage("Booking cannot exceed 7 nights.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .WithMessage("Guest count must be greater than zero.");
    }
}