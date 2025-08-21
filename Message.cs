using System.Text.Json.Serialization;

namespace Blackjack.Shared;

public class Message
{
    public string? Type { get; set; } // join, state, action, chat, reconnect, leave, server_shutdown
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
    public List<string> DealerHand { get; set; } = new List<string>();
    public int DealerTotal { get; set; }
    public int DeckCount { get; set; }
}

public class PlayerInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int Chips { get; set; }
    public bool IsTurn { get; set; }
    public List<string> Hand { get; set; } = new List<string>();
    public int Total { get; set; }
    public bool HasStood { get; set; }
    public bool IsBusted { get; set; }
}