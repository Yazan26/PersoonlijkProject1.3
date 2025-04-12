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

    private Color originalColor;

    void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        originalColor = errorMessage.color;

        loginButton.onClick.AddListener(PerformLogin);
        registerButton.onClick.AddListener(Register);
    }

    public async void PerformLogin()
    {
        ResetMessageColor();

        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("❌ Vul alle velden in!");
            return;
        }

        User user = new User { email = email, password = password };
        IWebRequestReponse response = await userApiClient.Login(user);

        switch (response)
        {
            case WebRequestData<string> data:
                Debug.Log("✅ Ingelogd! Token: " + data.Data);

                PlayerPrefs.SetString("access_token", data.Data);
                PlayerPrefs.Save();

                if (webClient != null)
                {
                    webClient.SetToken(data.Data);
                }
                else
                {
                    Debug.LogWarning("⚠️ WebClient ontbreekt.");
                }

                await SceneManager.LoadSceneAsync("WorldSelector");
                await Task.Yield();
                break;

            case WebRequestError error:
                if (error.ErrorMessage.Contains("401") || error.ErrorMessage.Contains("400"))
                    ShowError("Onjuist e-mailadres of wachtwoord.");
                else
                    ShowError("Login mislukt. Probeer het later opnieuw.");
                Debug.LogError("❌ Login error: " + error.ErrorMessage);
                break;

            default:
                ShowError("Onbekende fout bij inloggen.");
                break;
        }
    }

    private async void Register()
    {
        string Email = emailInput.text;
        string Password = passwordInput.text;

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            ShowError("Vul alle velden in!");
            return;
        }

        if (Password.Length < 10 ||
            !Password.Any(char.IsUpper) ||
            !Password.Any(char.IsLower) ||
            !Password.Any(char.IsDigit) ||
            !Password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            ShowError("Wachtwoord moet minimaal 10 tekens, een hoofdletter, kleine letter, cijfer en speciaal teken bevatten.");
            return;
        }

        User user = new User { email = Email, password = Password };
        IWebRequestReponse response = await userApiClient.Register(user);

        if (response is WebRequestError error)
        {
            if (error.ErrorMessage.Contains("duplicate"))
                ShowError("Deze e-mail is al in gebruik.");
            else
                ShowError("Email is al in gebruik! log in!");
            Debug.LogError("❌ Registratie error: " + error.ErrorMessage);
        }
        else
        {
            Debug.Log("✅ Registratie gelukt!");
            ShowSuccess("Registratie succesvol! Je kunt nu inloggen.");
        }
    }


    private void ShowError(string message)
    {
        errorMessage.text = message;
        errorMessage.color = Color.red;
    }

    private void ShowSuccess(string message)
    {
        errorMessage.text = message;
        errorMessage.color = Color.green;
    }

    private void ResetMessageColor()
    {
        errorMessage.color = originalColor;
    }
}
