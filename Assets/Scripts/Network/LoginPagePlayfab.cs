using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginPagePlayfab : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] GameObject login;
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button logInButton;
    [SerializeField] Button newAccountButton;
    [SerializeField] TextMeshProUGUI errorMessage;

    [Header("Register")]
    [SerializeField] GameObject register;
    [SerializeField] TMP_InputField registerEmailField;
    [SerializeField] TMP_InputField registerPasswordField;
    [SerializeField] TMP_InputField passwordConfirmField;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] Button registerButton;
    [SerializeField] Button backButton;
    [SerializeField] TextMeshProUGUI registerErrorMessage;

    //로그인 버튼 클릭시
    public void OnclickedLogInButton()
    {
        var loginRequest = new LoginWithEmailAddressRequest { Email = emailField.text, Password = passwordField.text };

        if(passwordField.text.Length < 8)
        {
            errorMessage.text = "Password must be at least 8 characters.";
            return;
        }

        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnLoginFailure);
    }

    public void OnclickedNewAccountButton()
    {
        login.SetActive(false);
        register.SetActive(true);
        ClearField();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공");
        SceneManager.LoadScene("Lobby");
    }
    
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("로그인 실패");
        errorMessage.text = "Invalid email or password.";
    }

    public void OnclickedRegisterButton()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = registerEmailField.text, Password = passwordConfirmField.text , Username = nickNameField.text, DisplayName = nickNameField.text };

        if(registerPasswordField.text != passwordConfirmField.text)
        {
            registerErrorMessage.text = "Password confirmation.";
            return;
        }

        if(registerPasswordField.text.Length < 8)
        {
            registerErrorMessage.text = "Password must be at least 8 characters.";
            return;
        }

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    public void OnclickedBackButton()
    {
        login.SetActive(true);
        register.SetActive(false);
        ClearField();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "Score", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장 실패"));
        login.SetActive(true);
        register.SetActive(false);
        ClearField();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        registerErrorMessage.text = "Register failed.";
    }

    private void ClearField()
    {

        //login clear
        errorMessage.text = "";
        emailField.text = "";
        passwordField.text = "";



        // register clear
        registerErrorMessage.text = "";
        registerErrorMessage.text = "";
        registerEmailField.text = "";
        passwordConfirmField.text = "";
        registerPasswordField.text = "";
        nickNameField.text = "";


    }

}
