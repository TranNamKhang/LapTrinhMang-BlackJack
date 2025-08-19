using System.Net.Sockets;

namespace Blackjack.Shared;

public class Player
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Chips { get; set; } = 5000;
    public bool IsTurn { get; set; }
    public TcpClient Connection { get; set; }
    public bool Connected { get; set; } = true;
    public List<string> Hand { get; } = new List<string>();
    public bool HasStood { get; set; }

    public Player(string id, string name, TcpClient conn)
    {
        Id = id;
        Name = name;
        Connection = conn;
    }

}