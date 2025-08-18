using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


class BlackjackServer
{
    static Room room = new Room();
    static ConcurrentDictionary<string, Player> playerMap = new();

    static void Main()
    {
        Console.WriteLine("Blackjack Server starting on port 9000...");
        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();

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
        Player player = null;

        try
        {
            while (true)
            {
                var msg = await ReadMessage(stream);
                if (msg == null) break;

                switch (msg.Type)
                {
                    case "join":
                        player = new Player(playerId, msg.PlayerName, client);
                        if (!room.AddPlayer(player))
                        {
                            await SendMessage(stream, new Message { Type = "error", Chat = "Phòng đầy!" });
                            client.Close();
                            return;
                        }
                        playerMap[playerId] = player;
                        Console.WriteLine($"{msg.PlayerName} joined the room.");
                        if (room.Players.Count >= room.MinPlayers && room.GameStatus == "waiting")
                        {
                            room.GameStatus = "playing";
                        }
                        await BroadcastRoomState();
                        break;

                    case "action":
                        if (player != null && player.Id == room.GetCurrentPlayer()?.Id)
                        {
                            Console.WriteLine($"{player.Name} action: {msg.Action}");
                            if (msg.Action == "hit")
                            {
                                // Người chơi chọn hit, KHÔNG chuyển lượt, chỉ broadcast lại state
                                // (Phần này bạn có thể thêm xử lý rút bài nếu cần sau)
                                // VD: player.Hand.Add(room.DrawCard()); và cập nhật điểm
                                // 
                            }
                            else if (msg.Action == "stand")
                            {
                                // Người chơi chọn stand, mới chuyển lượt
                                room.NextTurn();
                            }
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
                        if (playerMap.TryGetValue(msg.PlayerId, out var oldPlayer))
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

    static async Task<Message> ReadMessage(NetworkStream stream)
    {
        var buffer = new byte[4096];
        int len = await stream.ReadAsync(buffer, 0, buffer.Length);
        if (len == 0) return null;
        var json = Encoding.UTF8.GetString(buffer, 0, len);
        return JsonSerializer.Deserialize<Message>(json);
    }

    static async Task SendMessage(NetworkStream stream, Message msg)
    {
        var json = JsonSerializer.Serialize(msg);
        var bytes = Encoding.UTF8.GetBytes(json);
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
        var bytes = Encoding.UTF8.GetBytes(json);
        foreach (var p in room.Players)
        {
            if (p.Connected)
            {
                try
                {
                    var stream = p.Connection.GetStream();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch { }
            }
        }
    }
}