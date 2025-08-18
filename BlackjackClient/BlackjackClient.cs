using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class BlackjackClient
{
    static string playerId = null;
    static string playerName = null;

    static async Task Main()
    {
        Console.WriteLine("Nhap ten cua ban:");
        playerName = Console.ReadLine();
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 9000);
        var stream = client.GetStream();

        // Gửi join
        var joinMsg = new Message { Type = "join", PlayerName = playerName };
        await SendMessage(stream, joinMsg);

        _ = Task.Run(() => Listen(stream));

        while (true)
        {
            var line = Console.ReadLine();
            if (line.StartsWith("/chat "))
            {
                var chatMsg = new Message { Type = "chat", PlayerId = playerId, Chat = line.Substring(6) };
                await SendMessage(stream, chatMsg);
            }
            else if (line == "hit" || line == "stand")
            {
                var actionMsg = new Message { Type = "action", PlayerId = playerId, Action = line };
                await SendMessage(stream, actionMsg);
            }
        }
    }

    static async Task Listen(NetworkStream stream)
    {
        var buffer = new byte[4096];
        while (true)
        {
            int len = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (len == 0) break;
            var json = Encoding.UTF8.GetString(buffer, 0, len);
            var msg = JsonSerializer.Deserialize<Message>(json);
            switch (msg.Type)
            {
                case "state":
                    if (msg.RoomState != null)
                    {
                        Console.Clear();
                        Console.WriteLine("--- Trang thai phong ---");
                        foreach (var p in msg.RoomState.Players)
                        {
                            Console.WriteLine($"{p.Name} : {p.Chips} chip {(p.IsTurn ? "<-- Den luot" : "")}");
                            if (p.Name == playerName) playerId = p.Id;
                        }
                        Console.WriteLine($"Trang thai: {msg.RoomState.GameStatus}");
                        Console.WriteLine("Lenh: hit | stand | /chat noi dung");
                    }
                    break;
                case "chat":
                    Console.WriteLine($"[CHAT] {msg.PlayerName}: {msg.Chat}");
                    break;
                case "error":
                    Console.WriteLine($"[LOI]: {msg.Chat}");
                    break;
            }
        }
    }

    static async Task SendMessage(NetworkStream stream, Message msg)
    {
        var json = JsonSerializer.Serialize(msg);
        var bytes = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }
}