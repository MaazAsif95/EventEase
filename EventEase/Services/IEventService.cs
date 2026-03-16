using EventEase.Models;

namespace EventEase.Services
{
    public interface IEventService
    {
        Task<List<EventModel>> GetAllEventsAsync();
        Task<EventModel?> GetEventAsync(Guid id);
        Task SaveEventAsync(EventModel model);
        Task RefreshAsync();
        Task<List<RegistrationModel>> GetRegistrationsAsync(Guid eventId);
        Task AddRegistrationAsync(Guid eventId, RegistrationModel registration);
        Task MarkAttendanceAsync(Guid eventId, Guid registrationId, string checkedBy);
    }
}
