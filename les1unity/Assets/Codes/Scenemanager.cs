using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
public class SceneManager : MonoBehaviour
{
   public GameObject popupPanel; // Panel voor nieuwe wereld aanmaken
    public TMP_InputField worldNameInput, maxXInput, maxYInput;
    public Button createWorldButton, cancelButton, openCreateWorldPopupButton;

    private string apiUrl = "https://avansict2233343.azurewebsites.net/Enviroment2D";
    private string token;

    void Start()
    {
        token = PlayerPrefs.GetString("access_token", "");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Geen JWT-token gevonden! Gebruiker moet opnieuw inloggen.");
            return;
        }

        // Koppel knoppen aan methodes
        openCreateWorldPopupButton.onClick.AddListener(ShowPopup);
        createWorldButton.onClick.AddListener(() => StartCoroutine(CreateWorld()));
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
            Debug.LogError("Max X moet tussen 20 en 200 zijn!");
            yield break;
        }

        if (!int.TryParse(maxYInput.text, out int maxY) || maxY < 10 || maxY > 100)
        {
            Debug.LogError("Max Y moet tussen 10 en 100 zijn!");
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
