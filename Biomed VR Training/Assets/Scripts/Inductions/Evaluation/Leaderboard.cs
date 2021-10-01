using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI nameField;
    public GameObject leaderboardUser;

    public Transform usersList;
    public E_Results results; 

    private void OnEnable()
    {
        Record(nameField.text, results.score);

        //Record("test", 50);
        for (int i = 0; i < Leaderboard.EntryCount; i++)
        {
            ScoreEntry entry = Leaderboard.GetEntry(i);
            GameObject user = Instantiate(leaderboardUser, usersList);
            user.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i+1 + ". " + entry.name;
            print(entry.name + " " + entry.score);
        }
    }

    public const int EntryCount = 5;

    public struct ScoreEntry
    {
        public string name;
        public int score;

        public ScoreEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    private static List<ScoreEntry> s_Entries;

    private static List<ScoreEntry> Entries
    {
        get
        {
            if (s_Entries == null)
            {
                s_Entries = new List<ScoreEntry>();
                LoadScores();
            }
            return s_Entries;
        }
    }

    private const string PlayerPrefsBaseKey = "leaderboard";

    private static void SortScores()
    {
        s_Entries.Sort((a, b) => b.score.CompareTo(a.score));
    }

    private static void LoadScores()
    {
        s_Entries.Clear();

        for (int i = 0; i < EntryCount; ++i)
        {
            ScoreEntry entry;
            entry.name = PlayerPrefs.GetString(PlayerPrefsBaseKey + "[" + i + "].name", "");
            entry.score = PlayerPrefs.GetInt(PlayerPrefsBaseKey + "[" + i + "].score", 0);
            s_Entries.Add(entry);
        }

        SortScores();
    }

    private static void SaveScores()
    {
        for (int i = 0; i < EntryCount; ++i)
        {
            var entry = s_Entries[i];
            PlayerPrefs.SetString(PlayerPrefsBaseKey + "[" + i + "].name", entry.name);
            PlayerPrefs.SetInt(PlayerPrefsBaseKey + "[" + i + "].score", entry.score);
        }
    }

    public static ScoreEntry GetEntry(int index)
    {
        return Entries[index];
    }

    public static void Record(string name, int score)
    {
        Entries.Add(new ScoreEntry(name, score));
        SortScores();
        Entries.RemoveAt(Entries.Count - 1);
        SaveScores();
    }

}
