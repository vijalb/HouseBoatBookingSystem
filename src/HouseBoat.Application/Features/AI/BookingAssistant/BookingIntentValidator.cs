using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public class BookingIntentValidator : AbstractValidator<BookingIntent>
    {
        private static readonly string[] AllowedIntents =
        [
            "SearchAvailability",
        "BookHouseBoat",
        "CancelBooking",
        "General"
        ];

        public BookingIntentValidator()
        {
            RuleFor(x => x.Intent)
                .Must(intent => !string.IsNullOrWhiteSpace(intent)
                    && AllowedIntents.Contains(intent, StringComparer.OrdinalIgnoreCase))
                .WithMessage("The AI returned an unsupported intent.");

            RuleFor(x => x.GuestCount)
                .GreaterThan(0)
                .When(x => x.GuestCount.HasValue)
                .WithMessage("Guest count must be greater than zero.");

            RuleFor(x => x.CheckOutDate)
                .Must((intent, checkOut) =>
                    !checkOut.HasValue
                    || !intent.CheckInDate.HasValue
                    || checkOut.Value.Date > intent.CheckInDate.Value.Date)
                .WithMessage("Check-out date must be after check-in date.");
        }
    }
}