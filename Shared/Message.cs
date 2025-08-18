using System.Text.Json.Serialization;

public class Message
{
    public string? Type { get; set; } // join, state, action, chat, reconnect
    public string? PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? Action { get; set; } // hit/stand
    public string? Chat { get; set; }
    public RoomState? RoomState { get; set; }
}

public class RoomState
{
    public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
    public string? CurrentTurnId { get; set; }
    public string? GameStatus { get; set; } // waiting, playing, ended
}

public class PlayerInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int Chips { get; set; }
    public bool IsTurn { get; set; }
}