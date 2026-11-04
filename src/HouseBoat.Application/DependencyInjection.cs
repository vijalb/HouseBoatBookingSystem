using FluentValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using HouseBoat.Application.Features.AdminDashboard.GetDashboard;
using HouseBoat.Application.Features.AI.BookingAssistant;
using HouseBoat.Application.Features.AI.BookingAssistant.Tools;
using HouseBoat.Application.Features.AI.Chat;
using HouseBoat.Application.Features.Authentication.Login;
using HouseBoat.Application.Features.Authentication.Register;
using HouseBoat.Application.Features.Bookings.Admin.Cancel;
using HouseBoat.Application.Features.Bookings.Admin.Complete;
using HouseBoat.Application.Features.Bookings.Admin.Confirm;
using HouseBoat.Application.Features.Bookings.Admin.GetAll;
using HouseBoat.Application.Features.Bookings.Admin.GetById;
using HouseBoat.Application.Features.Bookings.Cancel;
using HouseBoat.Application.Features.Bookings.Create;
using HouseBoat.Application.Features.Bookings.GetById;
using HouseBoat.Application.Features.Bookings.MyBookings;
using HouseBoat.Application.Features.HouseBoats.Admin.Activate;
using HouseBoat.Application.Features.HouseBoats.Admin.Deactivate;
using HouseBoat.Application.Features.HouseBoats.Admin.Update;
using HouseBoat.Application.Features.HouseBoats.Create;
using HouseBoat.Application.Features.HouseBoats.GetAll;
using HouseBoat.Application.Features.HouseBoats.GetById;
using HouseBoat.Application.Features.Notifications.MarkAllAsRead;
using HouseBoat.Application.Features.Notifications.MarkAsRead;
using HouseBoat.Application.Features.Notifications.MyNotifications;
using HouseBoat.Application.Features.Reviews.Create;
using HouseBoat.Application.Features.Reviews.Delete;
using HouseBoat.Application.Features.Reviews.GetByHouseBoat;
using HouseBoat.Application.Features.Reviews.GetById;
using Microsoft.Extensions.DependencyInjection;

namespace HouseBoat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);
        services.AddFluentValidationAutoValidation();

        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<CreateHouseBoatHandler>();
        services.AddScoped<GetHouseBoatsHandler>();
        services.AddScoped<GetHouseBoatByIdHandler>();
        services.AddScoped<CreateBookingHandler>();
        services.AddScoped<GetMyBookingsHandler>();
        services.AddScoped<GetBookingByIdHandler>();
        services.AddScoped<CancelBookingHandler>();
        services.AddScoped<GetAllBookingsHandler>();
        services.AddScoped<GetAdminBookingByIdHandler>();
        services.AddScoped<ConfirmBookingHandler>();
        services.AddScoped<AdminCancelBookingHandler>();
        services.AddScoped<CompleteBookingHandler>();
        services.AddScoped<CreateHouseBoatHandler>();
        services.AddScoped<UpdateHouseBoatHandler>();
        services.AddScoped<DeactivateHouseBoatHandler>();
        services.AddScoped<ActivateHouseBoatHandler>();
        services.AddScoped<CreateReviewHandler>();
        services.AddScoped<GetReviewsByHouseBoatHandler>();
        services.AddScoped<GetReviewByIdHandler>();
        services.AddScoped<DeleteReviewHandler>();
        services.AddScoped<GetAdminDashboardHandler>();
        services.AddScoped<GetMyNotificationsHandler>();
        services.AddScoped<MarkNotificationAsReadHandler>();
        services.AddScoped<MarkAllNotificationsAsReadHandler>();
        services.AddScoped<ChatWithAIHandler>();
        services.AddScoped<BookingAssistantHandler>();
        services.AddScoped<SearchHouseBoatAvailabilityTool>();
        services.AddScoped<IBookingConversationStore, BookingConversationStore>();
        return services;
    }
}