using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ServerSlowdown.Hubs
{
	public class RoomHub : Hub
	{
		// Yksinkertainen huonelista muistissa (avaimena huonekoodi)
		private static readonly HashSet<string> ExistingRooms = new HashSet<string>();

		// Huonekohtaisten pyyntöjen määrät
		private static readonly ConcurrentDictionary<string, int> RoomClickCounts = new();
		private static IHubContext<RoomHub> _hubContext;

		private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> RoomConnections = new();

		// Pyyntöjen käsittelyyn liittyvä konteksti
		public RoomHub(IHubContext<RoomHub> hubContext)
		{
			_hubContext = hubContext;
		}

		// Luo uusi huone annetulla koodilla
		public Task CreateRoom(string roomCode)
		{
			// Tarkista onko huonekoodi jo olemassa
			if (ExistingRooms.Contains(roomCode))
			{
				throw new HubException("Huone on jo olemassa.");
			}


			ExistingRooms.Add(roomCode);
			Console.WriteLine($"Huone luotu: {roomCode}");


			return Task.CompletedTask;
		}

		// Liity huoneeseen, jos se on olemassa
		public async Task JoinRoom(string roomCode)
		{
			if (!ExistingRooms.Contains(roomCode))
			{
				throw new HubException("Huonetta ei löytynyt.");
			}

			await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
			Console.WriteLine($"Käyttäjä liittyi huoneeseen {roomCode}");

			RoomClickCounts.TryAdd(roomCode, 0);

			var connections = RoomConnections.GetOrAdd(roomCode, _ => new ConcurrentDictionary<string, byte>());
			connections.TryAdd(Context.ConnectionId, 0); // Lisää käyttäjän

			int userCount = connections.Count;

			Console.WriteLine($"Yhteyksien määrä huoneessa {roomCode}: {userCount}");

			// Lähetä käyttäjämäärä kaikille, mukaan lukien liittyjä itse
			await Clients.Group(roomCode).SendAsync("UserJoined", userCount);
			await Clients.Caller.SendAsync("UserJoined", userCount);

			// Lähetä klikkien määrä liittyjälle
			await Clients.Caller.SendAsync("ReceiveClickCount", RoomClickCounts[roomCode]);
		}


		// Hidastuspyyntöjen käsittely
		public async Task Request(string roomCode)
		{
			// Lisää pyyntö
			int newCount = RoomClickCounts.AddOrUpdate(roomCode, 1, (key, oldValue) => oldValue + 1);
			await Clients.Group(roomCode).SendAsync("ReceiveClickCount", newCount);

			_ = Task.Run(async () =>
			{
				await Task.Delay(15000);

				int updatedCount = RoomClickCounts.AddOrUpdate(roomCode, 0, (key, oldValue) =>
				{
					return Math.Max(0, oldValue - 1);
				});

				await _hubContext.Clients.Group(roomCode).SendAsync("ReceiveClickCount", updatedCount);
			});
		}


		public override async Task OnDisconnectedAsync(Exception exception)
		{
			foreach (var kvp in RoomConnections)
			{
				var roomCode = kvp.Key;
				var connections = kvp.Value;

				if (connections.TryRemove(Context.ConnectionId, out _))
				{
					int userCount = connections.Count;
					await Clients.Group(roomCode).SendAsync("UserJoined", userCount);
				}
			}

			await base.OnDisconnectedAsync(exception);
		}
	}

}
