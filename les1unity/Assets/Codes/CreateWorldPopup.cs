using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class CreateWorldPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_InputField worldNameInput;
    public TMP_InputField maxXInput;
    public TMP_InputField maxYInput;
    public Button createButton;
    public Button cancelButton;

    private string apiUrl = "http://localhost:7113/Enviroment2D";
    private string token;

    void Start()
    {
        token = PlayerPrefs.GetString("access_token", "");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Geen JWT-token gevonden! Gebruiker moet opnieuw inloggen.");
            return;
        }

        createButton.onClick.AddListener(() => StartCoroutine(CreateWorld()));
        cancelButton.onClick.AddListener(HidePopup);
        
        popupPanel.SetActive(false); // Popup standaard verbergen
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    IEnumerator CreateWorld()
    {
        string worldName = worldNameInput.text;
        if (string.IsNullOrEmpty(worldName))
        {
            Debug.LogError("Wereldnaam mag niet leeg zijn!");
            yield break;
        }

        if (!int.TryParse(maxXInput.text, out int maxX) || maxX < 20 || maxX > 200)
        {
            Debug.LogError("Max X moet een getal tussen 20 en 200 zijn!");
            yield break;
        }

        if (!int.TryParse(maxYInput.text, out int maxY) || maxY < 10 || maxY > 100)
        {
            Debug.LogError("Max Y moet een getal tussen 10 en 100 zijn!");
            yield break;
        }

        string jsonData = "{\"Name\":\"" + worldName + "\", \"MaxLength\":" + maxX + ", \"MaxHeight\":" + maxY + "}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Nieuwe wereld aangemaakt!");
                HidePopup();
            }
            else
            {
                Debug.LogError("Fout bij aanmaken wereld: " + request.error);
            }
        }
    }
}
