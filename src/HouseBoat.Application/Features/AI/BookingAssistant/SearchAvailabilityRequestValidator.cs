using FluentValidation;
using HouseBoat.Application.Features.AI.BookingAssistant.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public class SearchAvailabilityRequestValidator : AbstractValidator<SearchAvailabilityRequest>
    {
        public SearchAvailabilityRequestValidator()
        {
            RuleFor(x => x.CheckInDate)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Check-in date cannot be in the past.");

            RuleFor(x => x.CheckOutDate)
                .GreaterThan(x => x.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.GuestCount)
                .GreaterThan(0)
                .WithMessage("Guest count must be greater than zero.");
        }
    }
}