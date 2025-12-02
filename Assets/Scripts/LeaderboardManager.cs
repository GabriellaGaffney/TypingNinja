using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
    public int level;

    public ScoreEntry(string name, int score, int level)
    {
        this.playerName = name;
        this.score = score;
        this.level = level;
    }
}

public struct GroupStats
{
    public int gamesPlayed;
    public int highestScore;
    public float averageScore;
    public int highestLevel;
}

public static class LeaderboardManager
{
    // 0 = levels 1–3 (Blue)
    // 1 = levels 4–6 (Black)
    // 2 = levels 7–9 (Red)
    private static readonly string[] Keys = { "LB_1_3", "LB_4_6", "LB_7_9" };
    private const int MaxEntriesPerGroup = 5;

    [System.Serializable]
    private class EntryListWrapper
    {
        public List<ScoreEntry> entries = new();
    }

    /// <summary>
    /// Convert level number to leaderboard group index.
    /// 1–3 → 0, 4–6 → 1, 7–9 → 2
    /// </summary>
    public static int GetGroupIndexFromLevel(int levelNum)
    {
        int group = (levelNum - 1) / 3;
        return Mathf.Clamp(group, 0, Keys.Length - 1);
    }

    public static List<ScoreEntry> LoadGroup(int groupIndex)
    {
        groupIndex = Mathf.Clamp(groupIndex, 0, Keys.Length - 1);
        string key = Keys[groupIndex];

        string json = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(json))
            return new List<ScoreEntry>();

        EntryListWrapper wrapper = JsonUtility.FromJson<EntryListWrapper>(json);
        return wrapper?.entries ?? new List<ScoreEntry>();
    }

    public static void SaveGroup(int groupIndex, List<ScoreEntry> entries)
    {
        groupIndex = Mathf.Clamp(groupIndex, 0, Keys.Length - 1);
        string key = Keys[groupIndex];

        EntryListWrapper wrapper = new EntryListWrapper { entries = entries };
        string json = JsonUtility.ToJson(wrapper);

        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Add a score for the current level, into the correct group leaderboard.
    /// </summary>
    public static void AddScore(int levelNum, string playerName, int score)
    {
        int groupIndex = GetGroupIndexFromLevel(levelNum);
        List<ScoreEntry> list = LoadGroup(groupIndex);

        list.Add(new ScoreEntry(playerName, score, levelNum));

        // Sort descending by score
        list.Sort((a, b) => b.score.CompareTo(a.score));

        // Keep only top N
        if (list.Count > MaxEntriesPerGroup)
        {
            list.RemoveRange(MaxEntriesPerGroup, list.Count - MaxEntriesPerGroup);
        }

        SaveGroup(groupIndex, list);

        Debug.Log($"Saved score {score} for {playerName} in group {groupIndex} (level {levelNum}).");
    }

    /// <summary>
    /// Compute stats (games played, highest score, average score, highest level) for a group.
    /// </summary>
    public static GroupStats GetStatsForGroup(int groupIndex)
    {
        List<ScoreEntry> entries = LoadGroup(groupIndex);
        GroupStats stats = new GroupStats
        {
            gamesPlayed = entries.Count,
            highestScore = 0,
            averageScore = 0f,
            highestLevel = 0
        };

        if (entries.Count == 0)
            return stats;

        int totalScore = 0;
        int maxScore = int.MinValue;
        int maxLevel = int.MinValue;

        foreach (var e in entries)
        {
            totalScore += e.score;
            if (e.score > maxScore) maxScore = e.score;
            if (e.level > maxLevel) maxLevel = e.level;
        }

        stats.highestScore = maxScore;
        stats.averageScore = (float)totalScore / entries.Count;
        stats.highestLevel = maxLevel;

        return stats;
    }
}
