using UnityEngine;
using System;
using System.IO;
using TMPro;

public class TrialManager : MonoBehaviour
{
    private string trialFilePath;
    private DateTime startTime;
    private bool trialActive = true;

    [SerializeField] private TextMeshProUGUI countdownText;

    private static TrialManager Instance;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        trialFilePath = Path.Combine(Application.persistentDataPath, "trial.dat");
        //Debug.Log($"Trial File Path: {trialFilePath}");
    }

    private void Start()
    {
        if (!File.Exists(trialFilePath)) {
            //Debug.Log("First time launching the game. Setting trial start time.");
            SaveTrialStartTime();
        }

        if (IsTrialExpired()) {
            //Debug.Log("Trial expired!");
            LockGame();
        } else {
            //Debug.Log("Trial active. Starting timer.");
            StartCoroutine(MonitorTrialTime());
        }
    }

    private void SaveTrialStartTime()
    {
        // Save the current UTC time as a simple string
        startTime = DateTime.UtcNow;
        File.WriteAllText(trialFilePath, startTime.ToString("o")); // ISO 8601 format
        //Debug.Log($"Trial start time saved: {startTime} (UTC)");
    }

    private bool IsTrialExpired()
    {
        // Read the trial start time from the file
        if (File.Exists(trialFilePath)) {
            string fileContent = File.ReadAllText(trialFilePath);
            //Debug.Log($"Trial file content: {fileContent}");

            if (DateTime.TryParse(fileContent, null, System.Globalization.DateTimeStyles.RoundtripKind, out startTime)) {
                //Debug.Log($"Parsed Start Time: {startTime} (UTC)");

                // Check elapsed time
                double minutesElapsed = (DateTime.UtcNow - startTime).TotalMinutes;
                //Debug.Log($"Current Time: {DateTime.UtcNow} (UTC), Minutes Elapsed: {minutesElapsed}");

                return minutesElapsed > 15; // Trial expires after 15 minutes
            } else {
                Debug.LogError("Failed to parse trial start time.");
                return true; // Treat as expired if invalid file content
            }
        } else {
            Debug.LogError("Trial file missing. Assuming expired.");
            return true; // Treat as expired if no file
        }
    }

    private void LockGame()
    {
        trialActive = false;
        //Debug.Log("Trial expired. Locking game.");
        // Show trial expired UI
        GameManager.Instance.OpenTrialExpiredPanel();
        Time.timeScale = 0f; // Pause the game
    }

    private System.Collections.IEnumerator MonitorTrialTime()
    {
        while (trialActive) {
            yield return new WaitForSeconds(1f); // Check every 5 seconds
            if (IsTrialExpired()) {
                LockGame();
                break;
            }

            UpdateCountdownUI();
        }
    }

    private void UpdateCountdownUI()
    {
        double totalSecondsLeft = Math.Max((15 * 60) - (DateTime.UtcNow - startTime).TotalSeconds, 0); // Total seconds left
        int minutesLeft = Mathf.FloorToInt((float)totalSecondsLeft / 60); // Calculate whole minutes
        int secondsLeft = Mathf.FloorToInt((float)totalSecondsLeft % 60); // Remaining seconds

        countdownText.text = $"Trial Time Remaining - {minutesLeft:D2}:{secondsLeft:D2}"; // Format as MM:SS
    }


    [ContextMenu("Reset Trial")]
    public void ResetTrial()
    {
        if (File.Exists(trialFilePath)) {
            File.Delete(trialFilePath);
            Debug.Log("Trial file deleted.");
        }

        SaveTrialStartTime();
        trialActive = true;
        StartCoroutine(MonitorTrialTime());
        Debug.Log("Trial reset.");
    }

    [ContextMenu("Debug Trial File")]
    public void DebugTrialFile()
    {
        if (File.Exists(trialFilePath)) {
            string fileContent = File.ReadAllText(trialFilePath);
            Debug.Log($"Trial file content: {fileContent}");
        } else {
            Debug.Log("Trial file not found.");
        }
    }
}
