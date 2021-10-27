using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CBET;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI nameField;
    public GameObject leaderboardUser;

    public Transform usersList;
    public E_Results results; 

    private void OnEnable()
    {
        //PlayerPrefs.DeleteAll();

        //record the current entry
        Record(nameField.text, results.score, results.scoreQuestions, results.secondsText.text, results.skippedSteps);
        print(results.time);
        SendLeaderboard.Get().NewEntry(new LeaderboardEntry(nameField.text, results.time*1000, results.scoreQuestions, 100));

        LoadScores(); //sort the entries

        //list current and all previous entries
        for (int i = 0; i < Leaderboard.EntryCount; i++)
        {
            ScoreEntry entry = Leaderboard.GetEntry(i);
            GameObject user = Instantiate(leaderboardUser, usersList);
            if (entry.name != "" && entry.scoreQuestions != 0) 
            { 
                user.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i + 1 + ". " + entry.name;
                user.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.scoreQuestions.ToString();
                user.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "time: " + entry.time;
                user.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "skipped steps: " + entry.skippedSteps.ToString();
                print(entry.name + " " + entry.overallScore);
            }
        }
    }

    public const int EntryCount = 5;

    public struct ScoreEntry
    {
        public string name;
        public float overallScore;
        public int scoreQuestions;
        public string time;
        public int skippedSteps;

        public ScoreEntry(string name, float overallScore, int scoreQuestions, string time, int skippedSteps)
        {
            this.name = name;
            this.overallScore = overallScore;
            this.scoreQuestions = scoreQuestions;
            this.time = time;
            this.skippedSteps = skippedSteps;
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
        s_Entries.Sort((a, b) => b.overallScore.CompareTo(a.overallScore));
    }

    private static void LoadScores()
    {
        s_Entries.Clear();

        for (int i = 0; i < EntryCount; ++i)
        {
            ScoreEntry entry;
            entry.name = PlayerPrefs.GetString(PlayerPrefsBaseKey + "[" + i + "].name", "");
            entry.overallScore = PlayerPrefs.GetFloat(PlayerPrefsBaseKey + "[" + i + "].score", 0);
            entry.scoreQuestions = PlayerPrefs.GetInt(PlayerPrefsBaseKey + "[" + i + "].scoreQuestions", 0);
            entry.time = PlayerPrefs.GetString(PlayerPrefsBaseKey + "[" + i + "].time", "");
            entry.skippedSteps = PlayerPrefs.GetInt(PlayerPrefsBaseKey + "[" + i + "].skippedSteps", 0);
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
            PlayerPrefs.SetFloat(PlayerPrefsBaseKey + "[" + i + "].score", entry.overallScore);
            PlayerPrefs.SetInt(PlayerPrefsBaseKey + "[" + i + "].scoreQuestions", entry.scoreQuestions);
            PlayerPrefs.SetString(PlayerPrefsBaseKey + "[" + i + "].time", entry.time);
            PlayerPrefs.SetInt(PlayerPrefsBaseKey + "[" + i + "].skippedSteps", entry.skippedSteps);
        }
    }

    public static ScoreEntry GetEntry(int index)
    {
        return Entries[index];
    }

    public static void Record(string name, float score, int scoreQuestions, string time, int skippedSteps)
    {
        Entries.Add(new ScoreEntry(name, score, scoreQuestions, time, skippedSteps));
        SortScores();
        Entries.RemoveAt(Entries.Count - 1);
        SaveScores();
    }
}
