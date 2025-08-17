using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlackJackGame.Shared
{
    public static class LeaderboardService
    {
        private static readonly string LeaderboardFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BlackJackGame",
            "leaderboard.txt");

        public static List<LeaderboardEntry> Load()
        {
            try
            {
                if (File.Exists(LeaderboardFilePath))
                {
                    var lines = File.ReadAllLines(LeaderboardFilePath);
                    var entries = new List<LeaderboardEntry>();
                    
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                var entry = new LeaderboardEntry
                                {
                                    PlayerName = parts[0],
                                    Score = int.TryParse(parts[1], out int score) ? score : 0,
                                    Date = DateTime.TryParse(parts[2], out DateTime date) ? date : DateTime.Now,
                                    Wins = int.TryParse(parts[3], out int wins) ? wins : 0,
                                    Losses = int.TryParse(parts[4], out int losses) ? losses : 0,
                                    Ties = int.TryParse(parts[5], out int ties) ? ties : 0
                                };
                                entries.Add(entry);
                            }
                        }
                    }
                    
                    return entries;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading leaderboard: {ex.Message}");
            }
            
            return new List<LeaderboardEntry>();
        }

        public static void Save(List<LeaderboardEntry> entries)
        {
            try
            {
                string directory = Path.GetDirectoryName(LeaderboardFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var lines = new List<string>();
                foreach (var entry in entries)
                {
                    var line = $"{entry.PlayerName}|{entry.Score}|{entry.Date:yyyy-MM-dd HH:mm:ss}|{entry.Wins}|{entry.Losses}|{entry.Ties}";
                    lines.Add(line);
                }
                
                File.WriteAllLines(LeaderboardFilePath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving leaderboard: {ex.Message}");
            }
        }

        public static void AddEntry(string playerName, int wins, int losses, int ties)
        {
            var entries = Load();
            var entry = new LeaderboardEntry
            {
                PlayerName = playerName,
                Score = wins * 3 + ties,
                Date = DateTime.Now,
                Wins = wins,
                Losses = losses,
                Ties = ties
            };
            
            entries.Add(entry);
            entries = entries.OrderByDescending(e => e.Score).ThenByDescending(e => e.Date).Take(100).ToList();
            Save(entries);
        }
    }
}
