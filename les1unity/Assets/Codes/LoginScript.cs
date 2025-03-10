using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


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
        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(Register);
    }

    private async void Login()
    {
        string Email = emailInput.text;
        string Password = passwordInput.text;

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            errorMessage.text = "Vul alle velden in!";
            return;
        }

        // ✅ Create a User object and pass it to Login()
        User user = new User
        {
            email = Email,
            password = Password
        };
        // if (userApiClient == null)
        // {
        //     Debug.LogError("❌ userApiClient is NULL! Make sure it's assigned in the Inspector.");
        //     return;
        // }
        var response = await userApiClient.Login(user);

        if (response is WebRequestData<string> successRespone)
        {
            string token = successRespone.Data;
            PlayerPrefs.SetString("access_token", token);
            errorMessage.text = "Login succesvol!";
            WebClient webClient = FindObjectOfType<WebClient>();
            if (webClient != null)
            {
                webClient.SetToken(token);
            }
            else
            {
                Debug.LogError("❌ WebClient not found in the scene!");
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("WorldSelector"); // Switch to the next scene
        }
        else if (response is WebRequestError errorResponse)
        {
            errorMessage.text = "Login mislukt: " + errorResponse.ErrorMessage;
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

        var response = await userApiClient.Register(user);

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
