using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSelectorManager : MonoBehaviour
{
    public Environment2DApiClient environmentApiClient; 
    public TMP_InputField worldNameInput, maxXInput, maxYInput;
    public TMP_Text feedbackText;
    public GameObject JouwWereldenPanel;
    public Button createWorldButton, cancelButton, openCreateWorldPopupButton, RefreshButton, deleteButton, loadButton;
    public GameObject popupPanel;
    
    public Button[] worldButtons; 
    private string selectedWorldId;
    private string selectedWorldName;

    //---------------------------------------------------------------
    void Start()
    {
        openCreateWorldPopupButton.onClick.AddListener(ShowPopup);
        createWorldButton.onClick.AddListener(CreateWorld);
        cancelButton.onClick.AddListener(HidePopup);
        RefreshButton.onClick.AddListener(LoadWorldSlots);
        deleteButton.onClick.AddListener(DeleteSelectedWorld);
        loadButton.onClick.AddListener(LoadSelectedWorld); // ✅ New Load Button

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

    //---------------------------------------------------------------
    public async void LoadWorldSlots()
    {
        string userId = PlayerPrefs.GetString("UserId", "");

        if (string.IsNullOrEmpty(userId))
        {
            feedbackText.text = "❌ Je moet ingelogd zijn om werelden te zien!";
            return;
        }

        IWebRequestReponse response = await environmentApiClient.ReadEnvironment2Ds(userId);

        if (response is WebRequestError error)
        {
            feedbackText.text = "❌ Fout bij ophalen werelden: " + error.ErrorMessage;
            return;
        }

        if (response is WebRequestData<List<Environment2D>> dataResponse)
        {
            List<Environment2D> environments = dataResponse.Data;

            for (int i = 0; i < worldButtons.Length; i++)
            {
                if (worldButtons[i] == null) continue; // ✅ Skip if button is destroyed

                Button button = worldButtons[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                if (buttonText == null) continue; // ✅ Skip if text component is missing

                buttonText.text = "Empty World Slot";
                button.onClick.RemoveAllListeners();
                button.interactable = false; // Disable empty slots
            }

            for (int i = 0; i < environments.Count && i < worldButtons.Length; i++)
            {
                if (worldButtons[i] == null) continue; // ✅ Skip if button is destroyed

                Environment2D world = environments[i];
                Button button = worldButtons[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                if (buttonText == null) continue; // ✅ Skip if text component is missing

                buttonText.text = world.name;
                button.interactable = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectWorld(world.id, world.name)); // ✅ Corrected selection
            }
        }
    }

    //---------------------------------------------------------------
    // ✅ Select a world, but don't load or delete yet
    public void SelectWorld(string worldId, string worldName)
    {
        selectedWorldId = worldId;
        selectedWorldName = worldName;
        PlayerPrefs.SetString("SelectedWorldId", worldId);
        PlayerPrefs.Save();
        Debug.Log($"🌍 Wereld geselecteerd: {worldName} (ID: {worldId})");
        feedbackText.text = $"✅ Geselecteerde wereld: {worldName}";
    }

    //---------------------------------------------------------------
    // ✅ Load the selected world
    public void LoadSelectedWorld()
    {
        if (string.IsNullOrEmpty(selectedWorldId))
        {
            feedbackText.text = "❌ Geen wereld geselecteerd om te laden!";
            return;
        }

        Debug.Log($"🌍 Laden van wereld: {selectedWorldName} (ID: {selectedWorldId})");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SeeEnvironmentScene");
    }

    //---------------------------------------------------------------
    // ✅ Delete the selected world
    public async void DeleteSelectedWorld()
    {
        if (string.IsNullOrEmpty(selectedWorldId))
        {
            feedbackText.text = "❌ Geen wereld geselecteerd om te verwijderen!";
            return;
        }

        bool confirm = ConfirmDelete(); // ✅ Ask for confirmation
        if (!confirm) return;

        IWebRequestReponse response = await environmentApiClient.DeleteEnvironment(selectedWorldId);

        if (response is WebRequestError errorResponse)
        {
            feedbackText.text = "❌ Kon wereld niet verwijderen!";
        }
        else
        {
            Debug.Log($"✅ Wereld {selectedWorldName} verwijderd!");
            PlayerPrefs.DeleteKey("SelectedWorldId");
            selectedWorldId = null;
            selectedWorldName = null;
            feedbackText.text = "✅ Wereld verwijderd!";
            LoadWorldSlots(); // ✅ Refresh list after deletion
        }
    }

    //---------------------------------------------------------------
    // ✅ Confirmation before deleting
    private bool ConfirmDelete()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SeeEnvironmentScene";
    }

    //---------------------------------------------------------------
    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        JouwWereldenPanel.SetActive(false);
    }

    //---------------------------------------------------------------
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        JouwWereldenPanel.SetActive(true);
    }

    //---------------------------------------------------------------
    public async void CreateWorld()
    {
        feedbackText.text = "";

        if (environmentApiClient == null)
        {
            feedbackText.text = "Serverfout: API niet geladen!";
            return;
        }

        string token = PlayerPrefs.GetString("access_token", "");
        if (string.IsNullOrEmpty(token))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }

        string ownerUserId = PlayerPrefs.GetString("UserId", "");
        if (string.IsNullOrEmpty(ownerUserId))
        {
            feedbackText.text = "❌ Je moet ingelogd zijn om een wereld te maken!";
            return;
        }

        IWebRequestReponse response = await environmentApiClient.ReadEnvironment2Ds(ownerUserId);

        if (response is WebRequestData<List<Environment2D>> existingWorldsResponse)
        {
            if (existingWorldsResponse.Data.Count >= 6)
            {
                feedbackText.text = "❌ Je kunt maximaal 6 werelden hebben!";
                return;
            }
        }

        if (!int.TryParse(maxXInput.text, out int maxX) || !int.TryParse(maxYInput.text, out int maxY))
        {
            feedbackText.text = "❌ Lengte en hoogte moeten cijfers zijn!";
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

        IWebRequestReponse createResponse = await environmentApiClient.CreateEnvironment(newWorld);
        if (createResponse is WebRequestData<Environment2D> createdWorldResponse)
        {
            feedbackText.text = "✅ Wereld succesvol aangemaakt!";
            HidePopup();
            LoadWorldSlots();
        }
    }
}
