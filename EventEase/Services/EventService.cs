using EventEase.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace EventEase.Services
{
    public class EventService : IEventService
    {
        readonly IJSRuntime _js;
        const string IndexKey = "eventease:ids";
        List<EventModel>? _cache;
        readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public EventService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<List<EventModel>> GetAllEventsAsync()
        {
            // return cached copy when available
            if (_cache != null)
                return _cache;

            try
            {
                var allJson = await _js.InvokeAsync<string>("eventEase.getAllEvents");
                var list = JsonSerializer.Deserialize<List<EventModel>>(allJson, _opts) ?? new List<EventModel>();
                _cache = list.OrderBy(e => e.Date).ToList();
                return _cache;
            }
            catch
            {
                return new List<EventModel>();
            }
        }

        public async Task<EventModel?> GetEventAsync(Guid id)
        {
            // attempt to read from cache first
            if (_cache != null)
            {
                var found = _cache.FirstOrDefault(x => x.Id == id);
                if (found != null) return found;
            }

            var key = GetStorageKey(id);
            var j = await _js.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(j)) return null;
            try
            {
                var m = JsonSerializer.Deserialize<EventModel>(j, _opts);
                return m;
            }
            catch { return null; }
        }

        public async Task SaveEventAsync(EventModel model)
        {
            var key = GetStorageKey(model.Id);
            var j = JsonSerializer.Serialize(model);
            await _js.InvokeVoidAsync("localStorage.setItem", key, j);
            await UpdateIndexAsync(model.Id);

            // update cache
            if (_cache == null) _cache = new List<EventModel>();
            var idx = _cache.FindIndex(x => x.Id == model.Id);
            if (idx >= 0) _cache[idx] = model;
            else _cache.Add(model);
            _cache = _cache.OrderBy(e => e.Date).ToList();
        }

        public async Task<List<RegistrationModel>> GetRegistrationsAsync(Guid eventId)
        {
            var key = GetRegistrationKey(eventId);
            var raw = await _js.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(raw)) return new List<RegistrationModel>();
            try
            {
                return JsonSerializer.Deserialize<List<RegistrationModel>>(raw) ?? new List<RegistrationModel>();
            }
            catch { return new List<RegistrationModel>(); }
        }

        public async Task AddRegistrationAsync(Guid eventId, RegistrationModel registration)
        {
            var regs = await GetRegistrationsAsync(eventId);
            registration.Id = Guid.NewGuid();
            registration.RegisteredAt = DateTime.Now;
            regs.Add(registration);
            var key = GetRegistrationKey(eventId);
            var j = JsonSerializer.Serialize(regs);
            await _js.InvokeVoidAsync("localStorage.setItem", key, j);
        }

        public async Task MarkAttendanceAsync(Guid eventId, Guid registrationId, string checkedBy)
        {
            var regs = await GetRegistrationsAsync(eventId);
            var idx = regs.FindIndex(r => r.Id == registrationId);
            if (idx >= 0)
            {
                regs[idx].Attended = true;
                regs[idx].CheckedAt = DateTime.Now;
                regs[idx].CheckedBy = checkedBy;
                var key = GetRegistrationKey(eventId);
                var j = JsonSerializer.Serialize(regs);
                await _js.InvokeVoidAsync("localStorage.setItem", key, j);
            }
        }

        async Task UpdateIndexAsync(Guid id)
        {
            try
            {
                var raw = await _js.InvokeAsync<string>("localStorage.getItem", IndexKey);
                List<Guid> ids;
                if (string.IsNullOrEmpty(raw)) ids = new List<Guid>();
                else ids = JsonSerializer.Deserialize<List<Guid>>(raw) ?? new List<Guid>();
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                    var j = JsonSerializer.Serialize(ids);
                    await _js.InvokeVoidAsync("localStorage.setItem", IndexKey, j);
                }
            }
            catch { }
        }

        static string GetStorageKey(Guid id) => $"eventease:event:{id}";
        static string GetRegistrationKey(Guid id) => $"eventease:registration:{id}";

        public Task RefreshAsync()
        {
            _cache = null;
            return Task.CompletedTask;
        }
    }
}
