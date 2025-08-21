using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.IO;
using Blackjack.Shared;

namespace Blackjack.Client;

class BlackjackClient
{
    static string? playerId = null;
    static string? playerName = null;

    static async Task Main()
    {
        Console.WriteLine("Nhap ten cua ban:");
        playerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Ten khong hop le.");
            return;
        }
        using var client = new TcpClient();
        await ConnectWithRetry(client, "127.0.0.1", 9000);
        var stream = client.GetStream();

        Console.CancelKeyPress += async (s, e) =>
        {
            e.Cancel = true;
            try
            {
                await SendMessage(stream, new Message { Type = "leave", PlayerId = playerId, PlayerName = playerName });
            }
            catch { }
            Environment.Exit(0);
        };

        var joinMsg = new Message { Type = "join", PlayerName = playerName };
        await SendMessage(stream, joinMsg);

        _ = Task.Run(() => Listen(stream));

        while (true)
        {
            var line = Console.ReadLine();
            if (line == null) break;
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

    static async Task ConnectWithRetry(TcpClient client, string host, int port)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                await client.ConnectAsync(host, port);
                return;
            }
            catch
            {
                attempt++;
                int delayMs = Math.Min(30000, (int)Math.Pow(2, Math.Min(8, attempt)) * 250); // exponential backoff up to 30s
                Console.WriteLine($"Khong ket noi duoc, thu lai sau {delayMs}ms...");
                await Task.Delay(delayMs);
            }
        }
    }

    static async Task Listen(NetworkStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) break;
            var msg = JsonSerializer.Deserialize<Message>(line);
            if (msg == null) continue;
            switch (msg.Type)
            {
                case "state":
                    if (msg.RoomState != null)
                    {
                        Console.Clear();
                        Console.WriteLine("--- Trang thai phong ---");
                        foreach (var p in msg.RoomState.Players)
                        {
                            Console.WriteLine($"{p.Name} : {p.Chips} chip | Hand: {string.Join(",", p.Hand)} (Total {p.Total}) {(p.IsTurn ? "<-- Den luot" : "")}");
                            if (p.Name == playerName) playerId = p.Id;
                        }
                        Console.WriteLine($"Trang thai: {msg.RoomState.GameStatus}");
                        Console.WriteLine($"Dealer: {string.Join(",", msg.RoomState.DealerHand)} (Total {msg.RoomState.DealerTotal}) | Con trong bo: {msg.RoomState.DeckCount}");
                        Console.WriteLine("Lenh: hit | stand | /chat noi dung");
                    }
                    break;
                case "chat":
                    Console.WriteLine($"[CHAT] {msg.PlayerName}: {msg.Chat}");
                    break;
                case "error":
                    Console.WriteLine($"[LOI]: {msg.Chat}");
                    break;
                case "server_shutdown":
                    Console.WriteLine("[THONG BAO]: Server da tat. Dang thu ket noi lai...");
                    // let Listen return; outer loop not present, so exit app
                    return;
            }
        }
    }

    static async Task SendMessage(NetworkStream stream, Message msg)
    {
        var json = JsonSerializer.Serialize(msg) + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }
}