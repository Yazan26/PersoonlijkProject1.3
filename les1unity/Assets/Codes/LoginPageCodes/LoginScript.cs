using System;
using System.Linq;
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
    public WebClient webClient;
    

    void Start()
    {
        PlayerPrefs.DeleteAll(); // Remove saved login state on startup
        PlayerPrefs.Save();

        loginButton.onClick.AddListener(PerformLogin);
        registerButton.onClick.AddListener(Register);
    }

    public async void PerformLogin()
    {
        string Email = emailInput.text;
        string Password = passwordInput.text;

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            errorMessage.text = "❌ Vul alle velden in!";
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

                // Save token in PlayerPrefs
                PlayerPrefs.SetString("access_token", dataResponse.Data);
                PlayerPrefs.Save();

                // Set token in WebClient (for use in requests)
                if (webClient != null)
                {
                    webClient.SetToken(dataResponse.Data);
                }
                else
                {
                    Debug.LogError("❌ WebClient not found in the scene!");
                }

                // Get current user ID and store it
                // IWebRequestReponse userResponse = await userApiClient.GetCurrentUser();
                // if (userResponse is WebRequestData<string> userIdData)
                // {
                //     PlayerPrefs.SetString("UserId", userIdData.Data);
                //     PlayerPrefs.Save();
                //     Debug.Log("✅ User ID opgeslagen: " + userIdData.Data);
                // }
                // else
                // {
                //     Debug.LogError("❌ Kan User ID niet ophalen!");
                //     errorMessage.text = "Fout bij ophalen van gebruikersgegevens.";
                //     return;
                // }

                // Go to the next scene
                await SceneManager.LoadSceneAsync("WorldSelector");
                await Task.Yield();
                break;

            case WebRequestError errorResponse:
                if (errorResponse.ErrorMessage.Contains("401"))
                {
                    errorMessage.text = "Onjuist wachtwoord of Email.";
                }
                else
                {
                    errorMessage.text = "Login mislukt. Probeer opnieuw.";
                }
                Debug.LogError("Login failed: " + errorResponse.ErrorMessage);
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

        // Password strength validation
        if (Password.Length < 10 || 
            !Password.Any(char.IsUpper) || 
            !Password.Any(char.IsLower) || 
            !Password.Any(char.IsDigit) || 
            !Password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            errorMessage.text = "Wachtwoord moet minimaal 10 tekens, een hoofdletter, een kleine letter, een cijfer en een speciaal teken bevatten.";
            return;
        }

        User user = new User
        {
            email = Email,
            password = Password
        };

        IWebRequestReponse response = await userApiClient.Register(user);

        switch (response)
        {
            case WebRequestData<string>:
                Debug.Log("✅ Registratie succesvol!");
                errorMessage.text = "Registratie succesvol! Je kunt nu inloggen.";
                break;

            case WebRequestError errorResponse:
                if (errorResponse.ErrorMessage.Contains("duplicate"))
                {
                    errorMessage.text = "Deze Email is al in gebruik!";
                }
                else
                {
                    errorMessage.text = "Registratie mislukt: " + errorResponse.ErrorMessage;
                }
                Debug.LogError("Registratie mislukt: " + errorResponse.ErrorMessage);
                break;

            default:
                throw new NotImplementedException("Unhandled response type: " + response.GetType());
        }
    }
}
