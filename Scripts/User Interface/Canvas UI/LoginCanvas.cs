using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using System.Text.RegularExpressions;
using GameCookInterface;
using TMPro;

public class LoginCanvas : CanvasUI, IFirebaseAuth {


    private static FirebaseAuth auth;
    public static FirebaseUser user;

    [Header("Sign In InputField")] //이메일 로그인 field
    [SerializeField] private InputField signInEmail;
    [SerializeField] private InputField signInPassword;
    [Header("Sign Up InputField")] // 신규 등록 field
    [SerializeField] private InputField signUpEmail;
    [SerializeField] private InputField signUpPassword;
    [SerializeField] private InputField signUpConfirm;
    [Header("Display Panel")]
    [SerializeField] private TextMeshProUGUI displayTag;
    [SerializeField] private GameObject signInPanel;
    [SerializeField] private GameObject signUpPanel;

    [Header("Error Message")]
    [SerializeField] private  Image emailError;
    [SerializeField] private  Image passwordError;
    [SerializeField] private  Image confirmError;

    [Header("Button Interaction")]
    [SerializeField] private  Button logInButton;
    [SerializeField] private  Button RegisgerButton;
    [SerializeField] private  Button AnonymousButton;

    [SerializeField] private  Button signInDisplay;
    [SerializeField] private  Button signUpDisplay;


    private void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        AuthStateChanged(this, null);
    }
    private void Start()    {
        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);

        signInDisplay.onClick.AddListener(() => ShowSignInPanel());
        signUpDisplay.onClick.AddListener(() => ShowSignUpPanel());

        logInButton.onClick.AddListener(() => FirebaseSignIn(signInEmail.text, signInPassword.text));
        RegisgerButton.onClick.AddListener(() => FirebaseSignUp(signUpEmail.text, signUpPassword.text));
        AnonymousButton.onClick.AddListener(() => FirebaseAnonymous());
    }

    private void ShowSignInPanel(){
        SoundOnBySwitch();
        displayTag.text = "SignIn";
        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);
    }
    private void ShowSignUpPanel()
    {
        SoundOnBySwitch();
        displayTag.text = "SignUp";
        signInPanel.SetActive(false);
        signUpPanel.SetActive(true);
        signUpEmail.text = "";
        signUpPassword.text = "";
        signUpConfirm.text = "";
        emailError.gameObject.SetActive(false);
        passwordError.gameObject.SetActive(false);
        confirmError.gameObject.SetActive(false);
    }
    public static void AuthLogOut()
    {
        if(auth.CurrentUser != null){
            auth.SignOut();
            auth.StateChanged -= AuthStateChanged;
        }
        
    }
    public void RigisterSuccess()
    {
        AudioManager.Instance.PlayUISound(UISoundType.Success);
        displayTag.text = "SignIn";

        signInEmail.text = signUpEmail.text;
        signInPassword.text = signUpPassword.text;

        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);

    }
    public void RigisterFail()
    {
        AudioManager.Instance.PlayUISound(UISoundType.ButtonError);
    }
    bool isSignUpError;
    public bool CheckSignUpError()
    {
        isSignUpError = false;
        CheckEmailError();
        CheckPasswordError();
        CheckConfirmError();
        return isSignUpError;
    }
    public void CheckEmailError()
    {
        if (!IsValidEmail(signUpEmail.text)){
            emailError.gameObject.SetActive(true);
            isSignUpError = true;
        }
        else emailError.gameObject.SetActive(false);
    }
    public void CheckPasswordError()
    {
        if (signUpPassword.text.Length < 6)
        {
            passwordError.gameObject.SetActive(true);
            isSignUpError = true;
        }
        else passwordError.gameObject.SetActive(false);
    }
    public void CheckConfirmError()
    {
        if (signUpPassword.text != signUpConfirm.text)
        {
            confirmError.gameObject.SetActive(true);
            isSignUpError = true;
        }
        else confirmError.gameObject.SetActive(false);
    }
    bool IsValidEmail(string strIn)
    {
        return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }


    protected override void OnEnable()
    {
        auth.StateChanged += AuthStateChanged;
        base.OnEnable();
    }
    
    private static void AuthStateChanged(object sender, EventArgs eventArgs) //로그인 / 로그아웃 상태 체크
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                AudioManager.Instance.PlayUISound(UISoundType.Access);
                LevelSceneManager.Instance.SwitchGamePower(true);
            }
        }
    }
    public void FirebaseSignIn(string email, string password)
    {
        AudioManager.Instance.PlayUISound(UISoundType.Click);
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            FirebaseUser user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        });
    }
    public void FirebaseSignUp(string email, string password)
    {
        if (CheckSignUpError()){
            RigisterFail();
            return;
        }
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled){
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted){
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            FirebaseUser newUser = task.Result; // 신규 유저 생성
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            DatabaseManager.Instance.NewRegistration();
        });
        RigisterSuccess();
    }

    public void FirebaseAnonymous()
    {
        AudioManager.Instance.PlayUISound(UISoundType.Click);
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                // User is now signed in.
                FirebaseUser newUser = task.Result;
                Debug.Log(string.Format("FirebaseUser:{0}\nEmail:{1}", newUser.UserId, newUser.Email));
            }
            else
            {
                Debug.Log("failed");
            }
        });
    }
}
