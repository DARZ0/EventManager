using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace EventEaseApp
{
    public class EventService
    {

        private readonly ILocalStorageService localStorage;
        private const string EventsKey = "events";
        private const string RegistrationsKey = "registrations";
        private const string UserRegistrationsKey = "userRegistrations";

        private List<Event> events = new();
        private List<Registration> registrations = new();
        private Dictionary<string, List<int>> userRegistrations = new();
        private int nextId = 1;

        public EventService(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task InitializeAsync()
        {
            await LoadEvents();
            await LoadRegistrations();
            await LoadUserRegistrations();
        }

        public List<Event> GetEvents() => events ?? new List<Event>();

        public List<Registration> GetRegistrations() => registrations ?? new List<Registration>();

        public List<Event> GetRegisteredEvents(string userName)
        {
            var ids = GetRegisteredEventIds(userName);
            return events.Where(e => ids.Contains(e.Id)).ToList();
        }
        public List<int> GetRegisteredEventIds(string userName)
        {
            return userRegistrations.TryGetValue(userName, out var ids) ? ids : new List<int>();
        }
        public bool HasRegistered(int eventId, string userName)
        {
            return userRegistrations.TryGetValue(userName, out var ids) && ids.Contains(eventId);
        }

        public async Task RegisterUser(Registration newRegistration, string userName)
        {
            if (newRegistration.RegisteredEvent != null && !string.IsNullOrWhiteSpace(userName))
            {
                int eventId = newRegistration.RegisteredEvent.Id;
                newRegistration.RegisteredEvent = events.FirstOrDefault(e => e.Id == eventId);
                if (!HasRegistered(eventId, userName))
                {
                    registrations.Add(newRegistration);
                }
                if (!userRegistrations.ContainsKey(userName))
                    userRegistrations[userName] = new List<int>();

                userRegistrations[userName].Add(eventId);

                await SaveRegistrations();
                await SaveUserRegistrations();
            }
        }

        public async Task AddEvent(Event newEvent)
        {
            var eventToAdd = new Event
            {
                Id = nextId++,
                Name = newEvent.Name,
                Description = newEvent.Description,
                Location = newEvent.Location,
                Date = newEvent.Date
            };
            events.Add(eventToAdd);
            await SaveEvents();
        }

        public async Task DeleteEvent(Event eventToDelete)
        {
            events.Remove(eventToDelete);

            // Remove all registrations for this event
            registrations.RemoveAll(r => r.RegisteredEvent?.Id == eventToDelete.Id);

            // Remove event ID from all user registrations
            foreach (var user in userRegistrations.Keys.ToList())
            {
                userRegistrations[user].Remove(eventToDelete.Id);
                if (userRegistrations[user].Count == 0)
                    userRegistrations.Remove(user);
            }

            await SaveEvents();
            await SaveRegistrations();
            await SaveUserRegistrations();
        }

        public async Task UpdateEvent(Event updatedEvent)
        {
            var index = events.FindIndex(e => e.Id == updatedEvent.Id);
            if (index != -1)
            {
                events[index] = updatedEvent;
                await SaveEvents();
            }
        }

        private async Task LoadEvents()
        {
            events = await localStorage.GetItemAsync<List<Event>>(EventsKey) ?? new List<Event>();
            nextId = events.Any() ? events.Max(e => e.Id) + 1 : 1;
        }

        private async Task LoadRegistrations()
        {
            registrations = await localStorage.GetItemAsync<List<Registration>>(RegistrationsKey) ?? new List<Registration>();
        }

        private async Task LoadUserRegistrations()
        {
            userRegistrations = await localStorage.GetItemAsync<Dictionary<string, List<int>>>(UserRegistrationsKey)
                                ?? new Dictionary<string, List<int>>();
        }

        public async Task SaveEvents()
        {
            await localStorage.SetItemAsync(EventsKey, events);
        }

        public async Task SaveRegistrations()
        {
            await localStorage.SetItemAsync(RegistrationsKey, registrations);
        }

        public async Task SaveUserRegistrations()
        {
            await localStorage.SetItemAsync(UserRegistrationsKey, userRegistrations);
        }
    }
}
