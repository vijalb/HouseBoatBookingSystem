using FluentValidation;

namespace HouseBoat.Application.Features.Bookings.Admin.GetAll;

public class GetAllBookingsValidator
    : AbstractValidator<GetAllBookingsQuery>
{
    private static readonly string[] AllowedSortFields =
    {
        "createdAt",
        "checkInDate",
        "checkOutDate",
        "totalAmount",
        "guestCount"
    };

    public GetAllBookingsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage(
                "Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage(
                "Page size must be between 1 and 50.");

        RuleFor(x => x.CheckInTo)
            .GreaterThanOrEqualTo(x => x.CheckInFrom)
            .When(x =>
                x.CheckInFrom.HasValue &&
                x.CheckInTo.HasValue)
            .WithMessage(
                "Check-in end date must be greater than or equal to the start date.");

        RuleFor(x => x.SortBy)
            .Must(sortBy =>
                string.IsNullOrWhiteSpace(sortBy) ||
                AllowedSortFields.Contains(
                    sortBy.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            .WithMessage(
                "Invalid sort field.");

        RuleFor(x => x.SortDirection)
            .Must(direction =>
                string.IsNullOrWhiteSpace(direction) ||
                direction.Equals(
                    "asc",
                    StringComparison.OrdinalIgnoreCase) ||
                direction.Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase))
            .WithMessage(
                "Sort direction must be 'asc' or 'desc'.");
    }
}