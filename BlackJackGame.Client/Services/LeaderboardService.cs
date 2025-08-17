using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlackJackGame.Client
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string Name { get; set; } = "";
        public int Chips { get; set; }
        public string AvatarPath { get; set; } = "";
    }

    public static class LeaderboardService
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leaderboard.csv");

        /// <summary>
        /// Thêm 1 người chơi mới vào bảng xếp hạng và tự động sắp xếp lại.
        /// </summary>
        public static void AddEntry(LeaderboardEntry entry)
        {
            var list = Load();
            list.Add(entry);
            var sorted = list.OrderByDescending(x => x.Chips).ToList();
            for (int i = 0; i < sorted.Count; i++)
                sorted[i].Rank = i + 1;
            Save(sorted);
        }

        /// <summary>
        /// Tải dữ liệu leaderboard từ file CSV.
        /// </summary>
        public static List<LeaderboardEntry> Load(int max = 50)
        {
            var list = new List<LeaderboardEntry>();
            try
            {
                if (!File.Exists(FilePath))
                    return list;

                var lines = File.ReadAllLines(FilePath);
                foreach (var ln in lines)
                {
                    var parts = ln.Split(new[] { ',' }, 3);
                    if (parts.Length >= 2)
                    {
                        if (!int.TryParse(parts[1], out int chips))
                            chips = 0;

                        var avatar = parts.Length >= 3 ? parts[2] : "";
                        list.Add(new LeaderboardEntry
                        {
                            Name = parts[0],
                            Chips = chips,
                            AvatarPath = avatar
                        });
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi đọc file
            }
            return list.OrderByDescending(x => x.Chips)
                       .Take(max)
                       .Select((e, i) =>
                       {
                           e.Rank = i + 1;
                           return e;
                       }).ToList();
        }

        /// <summary>
        /// Lưu dữ liệu leaderboard ra file CSV.
        /// </summary>
        private static void Save(List<LeaderboardEntry> list)
        {
            try
            {
                var lines = list.Select(e =>
                    $"{Escape(e.Name)},{e.Chips},{Escape(e.AvatarPath)}");
                File.WriteAllLines(FilePath, lines);
            }
            catch
            {
                // Bỏ qua lỗi ghi file
            }
        }

        /// <summary>
        /// Thêm hoặc cập nhật điểm cho người chơi (nếu đã có).
        /// </summary>
        public static void Upsert(string name, int chips, string avatarPath = "")
        {
            var scores = Load();
            var existing = scores.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                if (chips > existing.Chips)
                    existing.Chips = chips;

                if (!string.IsNullOrWhiteSpace(avatarPath))
                    existing.AvatarPath = avatarPath;
            }
            else
            {
                scores.Add(new LeaderboardEntry
                {
                    Name = name,
                    Chips = chips,
                    AvatarPath = avatarPath
                });
            }

            var sorted = scores.OrderByDescending(x => x.Chips).ToList();
            for (int i = 0; i < sorted.Count; i++)
                sorted[i].Rank = i + 1;

            Save(sorted);
        }

        private static string Escape(string s)
        {
            return s?.Replace(",", " ") ?? "";
        }
    }
}
