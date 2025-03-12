using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorMessage;
    public Button loginButton;
    public Button registerButton;
    public UserApiClient userApiClient;

    private void Awake()
    {
        userApiClient = FindObjectOfType<UserApiClient>();
    }

    void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // deletes previous login data
        loginButton.onClick.AddListener(PerformLogin);
        registerButton.onClick.AddListener(Register);
    }

   public async void PerformLogin()
{
    string Email = emailInput.text;
    string Password = passwordInput.text;

    if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
    {
        errorMessage.text = "Vul alle velden in!";
        return;
    }

    User user = new User
    {
        email = Email,
        password = Password
    };

    IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

    switch (webRequestResponse)
    {
        case WebRequestData<string> dataResponse:
            Debug.Log("✅ Login successful! Token received: " + dataResponse.Data);
            PlayerPrefs.SetString("access_token", dataResponse.Data);
            PlayerPrefs.Save();

            WebClient webClient = FindObjectOfType<WebClient>();
            if (webClient != null)
            {
                webClient.SetToken(dataResponse.Data);
            }
            else
            {
                Debug.LogError("❌ WebClient not found in the scene!");
            }

            // ✅ **Haalt de juiste `userId` op via /me en slaat deze op**
            IWebRequestReponse userResponse = await userApiClient.GetCurrentUser();
            if (userResponse is WebRequestData<string> userIdData)
            {
                PlayerPrefs.SetString("UserId", userIdData.Data);
                PlayerPrefs.Save();
                Debug.Log("✅ User ID opgeslagen: " + userIdData.Data);
            }
            else
            {
                Debug.LogError("❌ Kan User ID niet ophalen!");
                errorMessage.text = "Fout bij ophalen van gebruikersgegevens.";
                return;
            }

            await SceneManager.LoadSceneAsync("WorldSelector");
            await Task.Yield();
            break;

        case WebRequestError errorResponse:
            errorMessage.text = "Geen account gevonden met deze gegevens!";
            Debug.LogError("❌ Login failed: " + errorResponse.ErrorMessage);
            break;

        default:
            throw new NotImplementedException("Unhandled response type: " + webRequestResponse.GetType());
    }
}


    private async void Register()
    {
        string Email = emailInput.text;
        string Password = passwordInput.text;

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            errorMessage.text = "Vul alle velden in!";
            return;
        }

        User user = new User
        {
            email = Email,
            password = Password
        };

        IWebRequestReponse response = await userApiClient.Register(user);

        if (response is WebRequestData<string>)
        {
            errorMessage.text = "Registratie succesvol! Je kunt nu inloggen.";
        }
        else if (response is WebRequestError errorResponse)
        {
            errorMessage.text = "Registratie mislukt: " + errorResponse.ErrorMessage;
        }
    }
}
