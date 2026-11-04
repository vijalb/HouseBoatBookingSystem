using Microsoft.Extensions.AI;

namespace HouseBoat.Application.Features.AI.BookingAssistant.Tools;

public static class SearchAvailabilityAIFunction
{
    public static AIFunction Create(
        SearchHouseBoatAvailabilityTool tool)
    {
        return AIFunctionFactory.Create(
            async (
                string location,
                DateTime checkInDate,
                DateTime checkOutDate,
                int guestCount,
                CancellationToken cancellationToken) =>
            {
                var request = new SearchAvailabilityRequest(
                    location,
                    checkInDate,
                    checkOutDate,
                    guestCount);

                return await tool.ExecuteAsync(
                    request,
                    cancellationToken);
            },
            SearchAvailabilityToolDefinition.Name,
            SearchAvailabilityToolDefinition.Description);
    }
}