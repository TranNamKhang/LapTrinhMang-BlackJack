using System.Net.Sockets;

public class Player
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Chips { get; set; } = 5000;
    public bool IsTurn { get; set; }
    public TcpClient Connection { get; set; }
    public bool Connected { get; set; } = true;

    public Player(string id, string name, TcpClient conn)
    {
        Id = id;
        Name = name;
        Connection = conn;
    }
}