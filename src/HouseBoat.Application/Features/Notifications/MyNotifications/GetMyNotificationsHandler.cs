using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.Notifications.MyNotifications
{
    public class GetMyNotificationsHandler
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;

        public GetMyNotificationsHandler(
            IApplicationDbContext context,
            IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<List<NotificationDto>> Handle(
            GetMyNotificationsQuery query,
            CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    ReadAt = x.ReadAt
                })
                .ToListAsync(cancellationToken);

            return notifications;
        }
    }
}
