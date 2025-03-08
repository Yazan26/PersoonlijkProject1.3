using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorMessage;
    public Button loginButton;
    public Button registerButton;

    private string apiBaseUrl = "https://avansict2233343.azurewebsites.net/account/AccountManagement";

    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
        registerButton.onClick.AddListener(() => StartCoroutine(Register()));
    }

    IEnumerator Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessage.text = "Vul alle velden in!";
            yield break;
        }

        string jsonBody = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(apiBaseUrl + "/Login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                errorMessage.text = "Login succesvol!";
                string token = request.downloadHandler.text;
                PlayerPrefs.SetString("token", token);
                PlayerPrefs.Save();
                Debug.Log("Token opgeslagen: " + token);
            }
            else
            {
                errorMessage.text = "Login mislukt: " + request.error;
                Debug.LogError("Login mislukt: " + request.error);
            }
        }
        
    }

    IEnumerator Register()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessage.text = "Vul alle velden in!";
            yield break;
        }

        string jsonBody = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(apiBaseUrl + "/Register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                errorMessage.text = "Registratie succesvol! Je kunt nu inloggen.";
            }
            else
            {
                errorMessage.text = "Registratie mislukt: " + request.error;
                Debug.LogError("Registratie mislukt: " + request.error);
            }
        }
    }
}
