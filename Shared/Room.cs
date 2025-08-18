using System.Collections.Concurrent;

public class Room
{
    public List<Player> Players { get; set; } = new();
    public int MaxPlayers { get; set; } = 4;
    public int MinPlayers { get; set; } = 2;
    public string GameStatus { get; set; } = "waiting"; // waiting, playing, ended
    public int CurrentPlayerIndex { get; set; } = 0;

    // Thêm player vào phòng, trả về true nếu thành công
    public bool AddPlayer(Player p)
    {
        if (Players.Count >= MaxPlayers) return false;
        Players.Add(p);
        return true;
    }

    public Player? GetCurrentPlayer()
    {
        if (Players.Count == 0) return null;
        return Players[CurrentPlayerIndex % Players.Count];
    }

    public void NextTurn()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }

    public RoomState ToRoomState()
    {
        return new RoomState
        {
            Players = Players.Select(x => new PlayerInfo
            {
                Id = x.Id,
                Name = x.Name,
                Chips = x.Chips,
                IsTurn = (x == GetCurrentPlayer())
            }).ToList(),
            CurrentTurnId = GetCurrentPlayer()?.Id,
            GameStatus = GameStatus
        };
    }
}