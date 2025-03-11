using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSelectorManager : MonoBehaviour
{
    public Environment2DApiClient environmentApiClient; // API Client for retrieving worlds
    public TMP_InputField worldNameInput, maxXInput, maxYInput;
    public TMP_Text feedbackText;
    public GameObject JouwWereldenPanel;
    public Button createWorldButton, cancelButton, openCreateWorldPopupButton, RefreshButton;
    public GameObject popupPanel;
    

    // ✅ Array for the 6 world slots (Assign in Inspector)
    public Button[] worldButtons; 

    void Start()
    {
        openCreateWorldPopupButton.onClick.AddListener(ShowPopup);
        createWorldButton.onClick.AddListener(CreateWorld);
        cancelButton.onClick.AddListener(HidePopup);
        RefreshButton.onClick.AddListener(LoadWorldSlots);

        string token = PlayerPrefs.GetString("access_token", "");
        popupPanel.SetActive(false);

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No JWT token found! Redirecting to login.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }

        LoadWorldSlots(); // ✅ Populate world slots on start
    }

    public async void LoadWorldSlots()
    {
        string userId = PlayerPrefs.GetString("UserId", "");

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("❌ Geen User ID gevonden! Kan werelden niet laden.");
            feedbackText.text = "❌ Je moet ingelogd zijn om werelden te zien!";
            return;
        }

        // ✅ Fetch only the worlds owned by this user
        IWebRequestReponse response = await environmentApiClient.ReadEnvironment2Ds(userId);

        if (response is WebRequestError error)
        {
            feedbackText.text = "❌ Fout bij ophalen environments: " + error.ErrorMessage;
            return;
        }

        if (response is WebRequestData<List<Environment2D>> dataResponse)
        {
            List<Environment2D> environments = dataResponse.Data;

            // ✅ Eerst alle knoppen resetten voordat we ze vullen
            for (int i = 0; i < worldButtons.Length; i++)
            {
                Button button = worldButtons[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                buttonText.text = "Empty World Slot";
                button.onClick.RemoveAllListeners();
            }

            // ✅ Daarna vullen met bestaande werelden
            for (int i = 0; i < environments.Count && i < worldButtons.Length; i++)
            {
                Environment2D world = environments[i];
                Button button = worldButtons[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                buttonText.text = world.name;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectWorld(world.id));
            }
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
        feedbackText.text = "";

        if (environmentApiClient == null)
        {
            Debug.LogError("❌ environmentApiClient is NULL!");
            feedbackText.text = "Serverfout: API niet geladen!";
            return;
        }

        string token = PlayerPrefs.GetString("access_token", "");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No JWT token found! Redirecting to login.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }

        string ownerUserId = PlayerPrefs.GetString("UserId", "");
        if (string.IsNullOrEmpty(ownerUserId))
        {
            Debug.LogError("❌ Geen User ID gevonden! Kan wereld niet maken.");
            feedbackText.text = "❌ Je moet ingelogd zijn om een wereld te maken!";
            return;
        }

        if (string.IsNullOrEmpty(worldNameInput.text) ||
            string.IsNullOrEmpty(maxXInput.text) || string.IsNullOrEmpty(maxYInput.text))
        {
            feedbackText.text = "❌ Vul alle velden in!";
            return;
        }

        if (!int.TryParse(maxXInput.text, out int maxX) || !int.TryParse(maxYInput.text, out int maxY))
        {
            feedbackText.text = "❌ Lengte en hoogte moeten cijfers zijn!";
            return;
        }

        if (maxX < 20 || maxX > 200 || maxY < 10 || maxY > 100)
        {
            feedbackText.text = "❌ Lengte moet 20-200 zijn, Hoogte 10-100!";
            return;
        }

        string worldName = worldNameInput.text.Trim();
        if (worldName.Length < 1 || worldName.Length > 25)
        {
            feedbackText.text = "❌ Naam moet tussen 1-25 tekens zijn!";
            return;
        }

        Environment2D newWorld = new Environment2D
        {
            id = Guid.NewGuid().ToString(),
            name = worldName,
            maxWidth = maxX,
            maxHeight = maxY,
            ownerUserId = ownerUserId
        };

        IWebRequestReponse response = await environmentApiClient.CreateEnvironment(newWorld);
        if (response == null)
        {
            Debug.LogError("❌ API Response is NULL!");
            feedbackText.text = "❌ Server reageerde niet!";
            return;
        }

        switch (response)
        {
            case WebRequestData<Environment2D> dataResponse:
                Debug.Log($"✅ World '{newWorld.name}' created successfully! ID: {dataResponse.Data.id}");
                feedbackText.text = "✅ Wereld succesvol aangemaakt!";
                HidePopup();
                LoadWorldSlots(); // ✅ Refresh buttons after world creation
                break;

            case WebRequestError errorResponse:
                feedbackText.text = "❌ Creatie mislukt: " + errorResponse.ErrorMessage;
                break;

            default:
                feedbackText.text = "❌ Onbekende serverreactie!";
                throw new NotImplementedException();
        }
    }

    public void SelectWorld(string worldId)
    {
        Debug.Log($"🌍 Wereld geselecteerd: {worldId}");
        PlayerPrefs.SetString("SelectedWorldId", worldId);
        PlayerPrefs.Save();

        // ✅ Load world scene (or any other functionality)
        UnityEngine.SceneManagement.SceneManager.LoadScene("SeeEnvironmentScene");
    }
}
