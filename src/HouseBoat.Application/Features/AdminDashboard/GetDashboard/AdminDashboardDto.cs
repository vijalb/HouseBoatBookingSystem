namespace HouseBoat.Application.Features.AdminDashboard.GetDashboard;

public class AdminDashboardDto
{
    public int TotalHouseBoats { get; set; }

    public int ActiveHouseBoats { get; set; }

    public int InactiveHouseBoats { get; set; }

    public int TotalBookings { get; set; }

    public int PendingBookings { get; set; }

    public int ConfirmedBookings { get; set; }

    public int CompletedBookings { get; set; }

    public int CancelledBookings { get; set; }

    public int TotalReviews { get; set; }

    public double AverageRating { get; set; }
}