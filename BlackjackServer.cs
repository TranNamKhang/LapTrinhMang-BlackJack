using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.IO;
using Blackjack.Shared;


namespace Blackjack.Server;

class BlackjackServer
{
    static readonly Room room = new Room();
    static ConcurrentDictionary<string, Player> playerMap = new();

    static void Main()
    {
        Console.WriteLine("Blackjack Server starting on port 9000...");
        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();

        Console.CancelKeyPress += async (s, e) =>
        {
            e.Cancel = true; // we will exit gracefully
            Console.WriteLine("\nShutting down server...");
            await BroadcastAll(new Message { Type = "server_shutdown", Chat = "Server is shutting down." });
            listener.Stop();
            Environment.Exit(0);
        };

        while (true)
        {
            var client = listener.AcceptTcpClient();
            _ = Task.Run(() => HandleClient(client));
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        string playerId = Guid.NewGuid().ToString();
        Player? player = null;

        try
        {
            while (true)
            {
                var msg = await ReadMessage(stream);
                if (msg == null) break;

                switch (msg.Type)
                {
                    case "join":
                        var safeName = msg.PlayerName ?? "Guest";
                        player = new Player(playerId, safeName, client);
                        if (!room.AddPlayer(player))
                        {
                            await SendMessage(stream, new Message { Type = "error", Chat = "Phòng đầy!" });
                            client.Close();
                            return;
                        }
                        playerMap[playerId] = player;
                        Console.WriteLine($"{msg.PlayerName} joined the room.");
                        if (room.GetPlayerCount() >= room.MinPlayers && room.GameStatus == "waiting")
                        {
                            room.StartGame();
                        }
                        await BroadcastRoomState();
                        break;

                    case "action":
                        if (player != null && player.Id == room.GetCurrentPlayer()?.Id)
                        {
                            Console.WriteLine($"{player.Name} action: {msg.Action}");
                            if (msg.Action == "hit")
                            {
                                room.PlayerHit(player);
                                var total = GetPlayerTotal(player);
                                if (total > 21)
                                {
                                    room.PlayerStand(player); // auto-stand on bust for flow
                                    room.NextTurn();
                                }
                            }
                            else if (msg.Action == "stand")
                            {
                                room.PlayerStand(player);
                                room.NextTurn();
                            }
                            room.ResolveIfNeeded();
                            await BroadcastRoomState();
                        }
                        break;

                    case "chat":
                        if (player != null)
                        {
                            await BroadcastAll(new Message
                            {
                                Type = "chat",
                                PlayerId = player.Id,
                                PlayerName = player.Name,
                                Chat = msg.Chat
                            });
                        }
                        break;

                    case "reconnect":
                        if (msg.PlayerId != null && playerMap.TryGetValue(msg.PlayerId, out var oldPlayer))
                        {
                            oldPlayer.Connection = client;
                            oldPlayer.Connected = true;
                            player = oldPlayer;
                            await SendMessage(stream, new Message { Type = "state", RoomState = room.ToRoomState() });
                        }
                        break;
                }
            }
        }
        catch
        {
            // ignore
        }
        finally
        {
            if (player != null)
            {
                player.Connected = false;
                Console.WriteLine($"{player.Name} disconnected.");
                // Không xóa player để hỗ trợ reconnect
            }
        }
    }

    static async Task<Message?> ReadMessage(NetworkStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
        var line = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(line)) return null;
        return JsonSerializer.Deserialize<Message>(line);
    }

    static async Task SendMessage(NetworkStream stream, Message msg)
    {
        var json = JsonSerializer.Serialize(msg);
        var line = json + "\n";
        var bytes = Encoding.UTF8.GetBytes(line);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }

    static async Task BroadcastRoomState()
    {
        var msg = new Message { Type = "state", RoomState = room.ToRoomState() };
        await BroadcastAll(msg);
    }

    static async Task BroadcastAll(Message msg)
    {
        var json = JsonSerializer.Serialize(msg);
        var line = json + "\n";
        var bytes = Encoding.UTF8.GetBytes(line);
        var recipients = room.GetConnectedPlayersSnapshot();
        foreach (var p in recipients)
        {
            try
            {
                var pStream = p.Connection.GetStream();
                await pStream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch { }
        }
    }

    static int GetPlayerTotal(Player p)
    {
        // helper to avoid referencing Room internals from here; mirror calculation by sending the state
        // Server can infer from last broadcast, but we re-calc minimal here
        int total = 0;
        int aces = 0;
        foreach (var card in p.Hand)
        {
            var rank = card.Trim().Length >= 2 ? new string(card.TakeWhile(char.IsLetterOrDigit).ToArray()) : card;
            switch (rank)
            {
                case "A":
                    aces++;
                    total += 11;
                    break;
                case "K":
                case "Q":
                case "J":
                case "10":
                    total += 10;
                    break;
                default:
                    if (int.TryParse(rank, out int v)) total += v;
                    break;
            }
        }
        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }
        return total;
    }
}