using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TMP InputField

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;

    // Username-related fields
    [SerializeField] private GameObject usernamePanel; 
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button saveUsernameButton;

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        // ALWAYS show username panel first
        ShowUsernamePanel();
        Pause();
    }

    private void ShowUsernamePanel()
    {
        Debug.Log("Showing username panel");
        
        if (usernamePanel != null)
        {
            usernamePanel.SetActive(true);
            
            // Focus on input field
            if (usernameInput != null)
            {
                usernameInput.Select();
                usernameInput.ActivateInputField();
                Debug.Log("Input field focused");
            }
            else
            {
                Debug.LogError("usernameInput is null!");
            }
        }
        else
        {
            Debug.LogError("usernamePanel is null!");
        }
    }

    public void SaveUsername()
    {
        Debug.Log("SaveUsername() called");
        
        if (usernameInput == null)
        {
            Debug.LogError("usernameInput is null in SaveUsername!");
            return;
        }
        
        string enteredName = usernameInput.text.Trim();
        Debug.Log($"Entered name: '{enteredName}'");

        if (!string.IsNullOrEmpty(enteredName))
        {
            PlayerPrefs.SetString("username", enteredName);
            PlayerPrefs.Save();
            usernamePanel.SetActive(false);
            Debug.Log("Username saved: " + enteredName);
        }
        else
        {
            Debug.LogWarning("Username cannot be empty!");
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void Play()
    {
        // Check if username is set before allowing play
        if (!PlayerPrefs.HasKey("username"))
        {
            Debug.LogWarning("Cannot start game: No username set!");
            ShowUsernamePanel();
            return;
        }

        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        // Updated to use modern API
        Pipes[] pipes = FindObjectsByType<Pipes>(FindObjectsSortMode.None);

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        gameOver.SetActive(true);
        
        // Upload the score when game ends
        if (PlayerPrefs.HasKey("username"))
        {
            // Check if ScoreUploader exists before trying to use it
            ScoreUploader scoreUploader = FindFirstObjectByType<ScoreUploader>();
            if (scoreUploader != null)
            {
                scoreUploader.UploadScore(score, "FlappyBird");
            }
            else
            {
                Debug.LogWarning("ScoreUploader not found in scene! Score will not be uploaded.");
            }
        }
        else
        {
            Debug.LogWarning("Cannot upload score: No username set!");
        }

        Pause();
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    // Method to clear username (for testing)
    public void ClearUsername()
    {
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.Save();
        Debug.Log("Username cleared. Restart the game to see username input again.");
    }
}