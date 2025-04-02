using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldSelectorManager : MonoBehaviour
{
    private int geselecteerdeIndex = -1;

    [Header("WebClient Reference")] 
    public Environment2DApiClient environmentApiClient;
    public WebClient WebClient;
    public TMP_Text feedbackText;

    public Button[] worldButtons;
    public Button refreshButton, deleteButton, loadButton, createButton, showpanelButton, cancelbutton;

    [Header("Input Fields")] 
    public TMP_InputField wereldNaamInput;
    public TMP_InputField maxHoogteInput;
    public TMP_InputField maxBreedteInput;

    public GameObject worldspanel, createpanel;

    private List<Environment2D> huidigeWorlds = new();
    private string selectedWorldId;
    private Environment2D selectedWorld;

    private void Start()
    {
        Debug.Log("🔄 Initialiseren WorldSelectorManager...");

        refreshButton.onClick.AddListener(LoadWorldSlots);
        deleteButton.onClick.AddListener(DeleteSelectedWorld);
        loadButton.onClick.AddListener(LoadSelectedWorld);
        createButton.onClick.AddListener(CreateNewWorld);
        showpanelButton.onClick.AddListener(ShowPanel);
        cancelbutton.onClick.AddListener(CancelWorld);

        LoadWorldSlots();
    }

    void ShowPanel()
    {
        Debug.Log("📂 Create Panel tonen...");
        worldspanel.SetActive(false);
        createpanel.SetActive(true);
    }

    void CancelWorld()
    {
        Debug.Log("❌ Create Panel annuleren...");
        worldspanel.SetActive(true);
        createpanel.SetActive(false);
    }

    public async void LoadWorldSlots()
    {
        Debug.Log("🌍 Werelden worden geladen...");
        selectedWorld = null;
        feedbackText.text = "Werelden laden...";

        var response = await WebClient.SendGetRequest("/environment2D");

        if (response is WebRequestData<string> data)
        {
            Debug.Log("📥 Ontvangen JSON:\n" + data.Data);
            huidigeWorlds = JsonHelper.ParseJsonArray<Environment2D>(data.Data);

            for (int i = 0; i < huidigeWorlds.Count; i++)
            {
                Environment2D a = huidigeWorlds[i];
                Debug.Log($"🧾 [{i}] Naam: {a.Name} | MaxWidth: {a.MaxWidth} | MaxHeight: {a.MaxHeight} | Id: {a.Id} | userId: {a.OwnerUserID}");
            }

            feedbackText.text = "✅ Werelden geladen!";
            ShowWorldSlots();
        }
        else
        {
            Debug.LogError("❌ Fout bij ophalen van werelden.");
            feedbackText.text = "❌ Fout bij laden van werelden!";
        }
    }

    private void ShowWorldSlots()
    {
        Debug.Log("📣 ShowWorldSlots() aangeroepen met " + huidigeWorlds.Count + " werelden.");

        for (int i = 0; i < worldButtons.Length; i++)
        {
            Button button = worldButtons[i];
            button.onClick.RemoveAllListeners();

            if (i < huidigeWorlds.Count)
            {
                Environment2D environment2d = huidigeWorlds[i];
                string id = environment2d.Id;

                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = environment2d.Name;

                button.interactable = true;
                button.name = id;

                button.onClick.AddListener(() => SelectWorldById(id));
                Debug.Log($"📌 Knop {i} toegewezen aan wereld: {environment2d.Name} (id: {id})");
            }
            else
            {
                TextMeshProUGUI buttonText = worldButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = "leeg Wereld slot";

                button.interactable = false;
                button.name = "leeg";
            }
        }
    }

    private void SelectWorldById(string id)
    {
        Debug.Log($"🔍 SelectWorldById aangeroepen voor id: {id}");

        selectedWorld = huidigeWorlds.Find(a => a.Id == id);
        if (selectedWorld != null)
        {
            selectedWorldId = selectedWorld.Id;
            wereldNaamInput.text = selectedWorld.Name;
            maxHoogteInput.text = selectedWorld.MaxHeight.ToString();
            maxBreedteInput.text = selectedWorld.MaxWidth.ToString();

            Debug.Log($"✅ Wereld geselecteerd: {selectedWorld.Name} (id: {selectedWorld.Id})");
        }
        else
        {
            Debug.LogWarning("⚠️ Wereld niet gevonden!");
            feedbackText.text = "❌ Wereld niet gevonden!";
        }
    }

    private void SelectWorld(int index)
    {
        if (index < 0 || index >= huidigeWorlds.Count)
        {
            feedbackText.text = "❌ Ongeldige index!";
            Debug.LogWarning("⚠️ Ongeldige index bij SelectWorld.");
            return;
        }

        geselecteerdeIndex = index;
        Environment2D environment2D = huidigeWorlds[index];

        wereldNaamInput.text = environment2D.Name;
        maxHoogteInput.text = environment2D.MaxHeight.ToString();
        maxBreedteInput.text = environment2D.MaxWidth.ToString();

        Debug.Log($"✅ Geselecteerde wereld via index: {environment2D.Name} (id: {environment2D.Id})");
    }

    public void LoadSelectedWorld()
    {
        Debug.Log("▶️ LoadSelectedWorld aangeroepen");

        if (string.IsNullOrEmpty(selectedWorldId))
        {
            feedbackText.text = "❌ Geen wereld geselecteerd!";
            Debug.LogWarning("⚠️ Geen wereld geselecteerd!");
            return;
        }

        PlayerPrefs.SetString("SelectedWorldId", selectedWorldId);
        PlayerPrefs.Save();

        feedbackText.text = $"🌍 Laden van wereld: {selectedWorld.Name}";
        Debug.Log($"🚀 Laden van wereldscene voor wereld: {selectedWorld.Name} (id: {selectedWorld.Id})");

        SceneManager.LoadScene("SeeEnvironmentScene");
    }

    public async void CreateNewWorld()
    {
        Debug.Log("🛠️ CreateNewWorld aangeroepen");

        if (string.IsNullOrWhiteSpace(wereldNaamInput.text) ||
            !int.TryParse(maxBreedteInput.text, out int maxWidth) ||
            !int.TryParse(maxHoogteInput.text, out int maxHeight))
        {
            feedbackText.text = "❌ Vul alle velden correct in!";
            Debug.LogWarning("⚠️ Ongeldige invoer bij wereld aanmaken!");
            return;
        }

        Environment2D nieuweEnvironment2D = new Environment2D
        {
            Name = wereldNaamInput.text,
            MaxWidth = maxWidth,
            MaxHeight = maxHeight
        };

        string json = JsonUtility.ToJson(nieuweEnvironment2D);
        json = RemoveidFieldFromJson(json);

        Debug.Log("📤 JSON verstuurd:\n" + json);
        var response = await WebClient.SendPostRequest("/environment2D/createworld", json);

        if (response is WebRequestData<string>)
        {
            Debug.Log("✅ Wereld succesvol aangemaakt!");
            LoadWorldSlots();
            CancelWorld(); // automatisch terug naar lijst
        }
        else
        {
            Debug.LogError("❌ Fout bij aanmaken van wereld! " + response);
            feedbackText.text = "❌ Fout bij aanmaken van wereld!";
        }
    }

    private string RemoveidFieldFromJson(string json)
    {
        json = json.Replace("\"Id\":\"\",", "");
        json = json.Replace(",\"Id\":\"\"", "");
        json = json.Replace("\"OwnerUserID\":\"\",", "");
        json = json.Replace(",\"OwnerUserID\":\"\"", "");
        return json;
    }

    public async void DeleteSelectedWorld()
    {
        Debug.Log("🗑️ DeleteSelectedWorld aangeroepen");

        if (selectedWorld == null)
        {
            feedbackText.text = "❌ Geen wereld geselecteerd!";
            Debug.LogWarning("⚠️ Geen wereld geselecteerd bij verwijderen.");
            return;
        }

        var response = await WebClient.SendDeleteRequest($"/environment2D/{selectedWorld.Id}");

        if (response is WebRequestData<string> || response is WebRequestData<object>)
        {
            Debug.Log("✅ Wereld verwijderd!");
            LoadWorldSlots();
        }
        else
        {
            Debug.LogError("❌ Fout bij verwijderen van wereld!");
            feedbackText.text = "❌ Fout bij verwijderen van wereld!";
        }
    }
}
