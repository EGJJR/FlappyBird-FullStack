using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreUploader : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiUrl = "https://darkslategray-trout-261514.hostingersite.com/add_score.php";
    
    /// <summary>
    /// Uploads the score to the server
    /// </summary>
    /// <param name="score">The score to upload</param>
    /// <param name="level">The level name</param>
    public void UploadScore(int score, string level)
    {
        // Check if username exists
        if (!PlayerPrefs.HasKey("username"))
        {
            Debug.LogError("Cannot upload score: No username found in PlayerPrefs!");
            return;
        }
        
        StartCoroutine(UploadScoreCoroutine(score, level));
    }
    
    private IEnumerator UploadScoreCoroutine(int score, string level)
    {
        string username = PlayerPrefs.GetString("username");
        
        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", score.ToString());
        form.AddField("level", level);
        
        Debug.Log($"Uploading score: {score} for user: {username} on level: {level}");
        Debug.Log($"API URL: {apiUrl}");
        
        // Create UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            // Set timeout
            request.timeout = 15;
            
            // Allow redirects
            request.redirectLimit = 5;
            
            // Send the request
            yield return request.SendWebRequest();
            
            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Score upload failed: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler?.text}");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Score upload failed (HTTP Error): {request.responseCode} - {request.error}");
                Debug.LogError($"Response: {request.downloadHandler?.text}");
            }
            else
            {
                Debug.Log("Score uploaded successfully!");
                Debug.Log($"Response Code: {request.responseCode}");
                Debug.Log($"Response: {request.downloadHandler?.text}");
            }
        }
    }
    
    /// <summary>
    /// Alternative method to upload score with default level
    /// </summary>
    /// <param name="score">The score to upload</param>
    public void UploadScore(int score)
    {
        UploadScore(score, "FlappyBird");
    }
    
    /// <summary>
    /// Test the API connection
    /// </summary>
    public void TestAPIConnection()
    {
        Debug.Log("Testing API connection...");
        UploadScore(0, "Test");
    }
}