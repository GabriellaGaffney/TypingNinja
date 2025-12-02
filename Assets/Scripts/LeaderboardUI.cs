using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject leaderboardPanel;

    [Header("Text Areas")]
    public TMP_Text titleText;
    public TMP_Text entriesText;
    public TMP_Text statsText;

    // Call this from the main menu "Leaderboards" button
    public void OpenLeaderboardPanel()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        ShowBlueLeaderboard();  // default tab
    }

    // Call this from the "Back" / "Close" button on the panel
    public void CloseLeaderboardPanel()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    // --- Button handlers for the 3 ninja groups ---

    public void ShowBlueLeaderboard()
    {
        ShowGroup(0, "Blue Ninja Leaderboard (Levels 1–3)");
    }

    public void ShowBlackLeaderboard()
    {
        ShowGroup(1, "Black Ninja Leaderboard (Levels 4–6)");
    }

    public void ShowRedLeaderboard()
    {
        ShowGroup(2, "Red Ninja Leaderboard (Levels 7–9)");
    }

    // --- Core display logic ---

    private void ShowGroup(int groupIndex, string title)
    {
        if (titleText != null)
            titleText.text = title;

        if (entriesText == null || statsText == null)
            return;

        List<ScoreEntry> entries = LeaderboardManager.LoadGroup(groupIndex);
        GroupStats stats = LeaderboardManager.GetStatsForGroup(groupIndex);

        // Scores list
        if (entries.Count == 0)
        {
            entriesText.text = "No scores yet!";
        }
        else
        {
            entriesText.text = "";
            for (int i = 0; i < entries.Count; i++)
            {
                ScoreEntry e = entries[i];
                entriesText.text += $"{i + 1}. {e.playerName} - {e.score} (Lv {e.level})\n";
            }
        }

        // Stats block
        statsText.text =
            $"Games played: {stats.gamesPlayed}\n" +
            $"Highest score: {stats.highestScore}\n" +
            $"Average score: {stats.averageScore:F1}\n" +
            $"Highest level reached: {stats.highestLevel}";
    }
}
