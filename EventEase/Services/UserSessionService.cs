using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace EventEase.Services
{
    public class SessionState
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string[]? Roles { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
        public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    }

    public class UserSessionService
    {
        const string StorageKey = "eventease:session";
        readonly IJSRuntime _js;
        readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public SessionState? Current { get; private set; }

        public event Action? OnChanged;

        public UserSessionService(IJSRuntime js) => _js = js;

        public async Task LoadAsync()
        {
            try
            {
                var raw = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrEmpty(raw))
                    Current = JsonSerializer.Deserialize<SessionState>(raw, _opts);
            }
            catch { }

            Notify();
        }

        public async Task StartAsync(string userId, string userName, string[]? roles = null)
        {
            Current = new SessionState
            {
                UserId = userId,
                UserName = userName,
                Roles = roles,
                StartedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            await PersistAsync();
            Notify();
        }

        public async Task EndAsync()
        {
            Current = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            Notify();
        }

        public async Task TouchAsync()
        {
            if (Current == null) return;
            Current.LastActivityAt = DateTime.UtcNow;
            await PersistAsync();
            Notify();
        }

        public bool IsExpired(TimeSpan timeout) =>
            Current == null || (DateTime.UtcNow - Current.LastActivityAt) > timeout;

        public async Task PersistAsync()
        {
            if (Current == null) return;
            var j = JsonSerializer.Serialize(Current);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, j);
        }

        void Notify() => OnChanged?.Invoke();
    }
}
