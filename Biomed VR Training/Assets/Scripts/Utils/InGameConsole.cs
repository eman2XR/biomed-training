/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
/// 
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class InGameConsole : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI uiText1;
    [SerializeField] TextMeshProUGUI uiText2;
    [SerializeField] TextMeshProUGUI uiText3;
    [SerializeField] TextMeshProUGUI uiText4;

    int counter;
    int errorCounter;

    //public RING_Controller ringController;
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.BackQuote;

    List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    bool show;
    bool collapse = true;

    // Visual elements:

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    const int margin = 20;

    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            show = !show;
        }
        ConsoleWindowWorldSpace();
    }

    void OnGUI()
    {
        if (!show)
        {
            return;
        }

        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                if (messageSameAsPrevious)
                {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
        }

        GUILayout.EndScrollView();

        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    void ConsoleWindowWorldSpace()
    {
        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            counter = i;
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                if (messageSameAsPrevious)
                {
                    break;
                }
            }
            //uiText.text = uiText.text + "\n" + (log.message) + "NUM:" + counter;
            //if (errorCounter == 1)
            //{
           
            //}
            //if (errorCounter == 2)
            //{
            //    uiText2.text = counter + ". " + (log.message);
            //    if (log.type == LogType.Error) uiText2.color = Color.red;
            //}
            //if (errorCounter == 3)
            //{
            //    uiText3.text = counter + ". " + (log.message);
            //    if (log.type == LogType.Error) uiText3.color = Color.red;
            //}
            //if (errorCounter == 4)
            //{
            //    uiText4.text = counter + ". " + (log.message);
            //    if (log.type == LogType.Error) uiText4.color = Color.red;
            //}
            //uiText.text = (log.message);
        }
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        errorCounter++;
        //if (errorCounter > 3) errorCounter = 0;

        //uiText1.text = uiText1.text + "\n" + errorCounter + ". " + (message);
        uiText1.text = errorCounter + ". " + (message);
        if (type == LogType.Exception || type == LogType.Error) uiText1.color = Color.red;
        if (type == LogType.Warning || type == LogType.Log) uiText1.color = Color.white;

        if (errorCounter == 1)
        {
            StopAllCoroutines();
            StartCoroutine(ClearConsole());
        }

        //ringController.value = message;
        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }

    IEnumerator ClearConsole()
    {
        yield return new WaitForSeconds(5);
        uiText1.text = "";
    }
}