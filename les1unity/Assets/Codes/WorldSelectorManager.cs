using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WorldSelectorManager : MonoBehaviour
{
    public Environment2DApiClient environmentApiClient; // API Client for creating worlds
    public TMP_InputField worldNameInput, maxXInput, maxYInput;
    public GameObject JouwWereldenPanel;
    public Button createWorldButton, cancelButton, openCreateWorldPopupButton;
    public GameObject popupPanel;

    void Start()
    {
        openCreateWorldPopupButton.onClick.AddListener(ShowPopup);
        createWorldButton.onClick.AddListener(CreateWorld);
        cancelButton.onClick.AddListener(HidePopup);
        
        string token = PlayerPrefs.GetString("access_token", "");
        createWorldButton.onClick.AddListener(CreateWorld);
        popupPanel.SetActive(false); // Hide the popup by default

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No JWT token found! Redirecting to login.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        JouwWereldenPanel.SetActive(false);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
        JouwWereldenPanel.SetActive(true);
    }

    public async void CreateWorld()
    {
        string token = PlayerPrefs.GetString("access_token", "");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No JWT token found! Redirecting to login.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }

        string worldName = worldNameInput.text;
        int maxX = int.Parse(maxXInput.text);
        int maxY = int.Parse(maxYInput.text);

        Environment2D newWorld = new Environment2D
        {
            name = worldName,
            maxLength = maxX,
            maxHeight = maxY
        };

        IWebRequestReponse response = await environmentApiClient.CreateEnvironment(newWorld);

        if (response is WebRequestData<Environment2D>)
        {
            Debug.Log("✅ World created successfully!");
            HidePopup();
        }
        else if (response is WebRequestError errorResponse)
        {
            Debug.LogError("❌ World creation failed: " + errorResponse.ErrorMessage);
        }
    }
}