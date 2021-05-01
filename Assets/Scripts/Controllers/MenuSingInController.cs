using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;

public class MenuSingInController : MonoBehaviour
{
    public Image imgExample;
    [Header("Texto Sing Up")]
    public TextMeshProUGUI txtMsgSingUp;

    [Header("Textos Panel de Carga")]
    public TextMeshProUGUI txtMessage;
    public TextMeshProUGUI txtVersion;
    public Slider sldLoadScene;

    [Header("Inputs")]
    // sing in
    public TMP_InputField inputSingInEmail;
    public TMP_InputField inputSingInPassword;

    // sing up
    public TMP_InputField inputSingUpUsername;
    public TMP_InputField inputSingUpEmail;
    public TMP_InputField inputSingUpPassword;
    public TMP_InputField inputSingUpCPassword;

    [Header("Paneles")]
    public CanvasGroup canvasGroupSingIn;
    public CanvasGroup canvasGroupSingUp;

    public GameObject panelLoading;
    public GameObject panelSingIn;
    public GameObject panelSingUp;

    [Header("Configuracion Firebase")]
    public string webClientId = "<your client id here>";
    public TextMeshProUGUI txtMsgServer;

    private GoogleSignInConfiguration configuration;

    [HideInInspector] public bool onConnectionEstabilished = false;

    public static MenuSingInController Instance { set; get; }
    private void Awake() {
        Instance = this;
        onConnectionEstabilished = false; // declara que aun no se ha establecido conexion con el servidor
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true
        };
    }

    void Start () {
        txtVersion.text = "";

        ShowPanelLoading();
        StartCoroutine("TimeSliderLoadBar");
    }

    private void LoadScene () {
        ShowPanelLoading();
        StartCoroutine("LoadSceneAsync");
    }

    IEnumerator TimeSliderLoadBar () {
        bool loading = true;
        float firstLoading = 0f;
        sldLoadScene.value = firstLoading;

        txtMessage.text = "Verificando version";
        txtVersion.text = "(Alpha) " + Application.version;

        while (loading) {
            yield return new WaitForSeconds(0.025f);
            if (firstLoading >= 0.05f) {
                loading = false;
            }
            sldLoadScene.value = firstLoading += 0.01f; 
        }

        txtMessage.text = "Intentando conectar al servidor...";
        bool waitForConnectionServer = true;
        while (waitForConnectionServer) {
            yield return new WaitForSeconds(0.2f);
            if (onConnectionEstabilished) {
                waitForConnectionServer = false;
                StartCoroutine("TimeSliderLoadBar_ConnectionServer");
                break;
            }
        }
        yield return null;
    }

    IEnumerator TimeSliderLoadBar_ConnectionServer () {
        bool loading = true;
        while (loading) {
            yield return new WaitForSeconds(0.025f);
            if (sldLoadScene.value >= 0.1f) {
                loading = false;
            }
            sldLoadScene.value += 0.01f; 
        }

        txtMessage.text = "Cargando Datos";

        // verificar si existe una sesion almacenada
        string _username = PlayerPrefs.GetString("Player:Username", "");
        string _email = PlayerPrefs.GetString("Player:Email", "");
        string _password = PlayerPrefs.GetString("Player:Password", "");
        string _methodSingIn = PlayerPrefs.GetString("SingIn:Method", "");
        if (_username != "" && _email != "" && _methodSingIn != "") {
            if (_methodSingIn == "Normal") {
                txtMessage.text = "Cargando tus datos";
                LoginAutomatic(_email, _password);
            }
            if(_methodSingIn == "Google") {
                txtMessage.text = "Cargando tus datos";
                BTN_OnSignInGoogle();
            }
        } else
            ShowPanelSingIn();
    }

    IEnumerator LoadSceneAsync () {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("main_menu");
        // No permitir que aparezca la escena mientras no haya cargado completamente
        asyncOperation.allowSceneActivation = false;
        
        while (!asyncOperation.isDone) {
            txtMessage.text = "Cargando Juego";
            sldLoadScene.value = asyncOperation.progress;

            // Comprobar si la carga finalizo
            if (asyncOperation.progress >= 0.9f) {
                // Activar la escena
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    /* ****************************** Panel Controls ****************************** */
    public void ShowPanelLoading () {
        panelLoading.SetActive(true);
        panelSingIn.SetActive(false);
        EnableInputsPanelSingIn();
    }

    public void ShowPanelSingIn () {
        panelLoading.SetActive(false);
        panelSingIn.SetActive(true);
        panelSingUp.SetActive(false);
    }

    public void ShowPanelSingUp () {
        panelLoading.SetActive(false);
        panelSingIn.SetActive(false);
        panelSingUp.SetActive(true);
        // clear inputs
        inputSingUpCPassword.text = inputSingUpEmail.text = inputSingUpPassword.text = inputSingUpUsername.text = "";
    }

    public void EnableInputsPanelSingIn () {
        canvasGroupSingIn.interactable = true;
        canvasGroupSingIn.blocksRaycasts = true;
    }

    public void DisableInputsPanelSingIn () {
        canvasGroupSingIn.interactable = false;
        canvasGroupSingIn.blocksRaycasts = false;
    }

    public void EnableInputsPanelSingUp () {
        canvasGroupSingUp.interactable = true;
        canvasGroupSingUp.blocksRaycasts = true;
    }

    public void DisableInputsPanelSingUp () {
        canvasGroupSingUp.interactable = false;
        canvasGroupSingUp.blocksRaycasts = false;
    }

    /* ****************************** Sing In Normal ****************************** */
    private void LoginAutomatic (string email, string pass) {
        WWWForm form = new WWWForm();
        form.AddField("_email", email);
        form.AddField("_password", pass);

        StartCoroutine(OnSingIn_ToServer(form));
    }

    public void BTN_OnSingInNormal () {
        DisableInputsPanelSingIn();
        txtMsgServer.text = "";
        if (inputSingInEmail.text.Length > 0 && inputSingInPassword.text.Length > 0) {
            WWWForm form = new WWWForm();
            form.AddField("_email", inputSingInEmail.text);
            form.AddField("_password", inputSingInPassword.text);
            // define el metodo de logueo como normal...
            PlayerPrefs.SetString("SingIn:Method", "Normal");
            PlayerPrefs.SetString("Player:Password", inputSingInPassword.text);

            StartCoroutine(OnSingIn_ToServer(form));
        } else {
            EnableInputsPanelSingIn();
        }
    }

    IEnumerator OnSingIn_ToServer(WWWForm form) {
        UnityWebRequest www = UnityWebRequest.Post(ServerListener.Instance.URL_Server + "login/game", form);
        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            txtMsgServer.text = www.error;
        } else {
            if (www.downloadHandler.text.Contains("Email o clave incorrecta.")) {
                ShowPanelSingIn();
                txtMsgServer.text = "Email o clave incorrecta.";
            } else {
                Debug.Log("Result: " + www.downloadHandler.text);
                // almacenar preguntas en una lista
                JsonUtility.FromJsonOverwrite(www.downloadHandler.text, ServerListener.Instance._player);

                PlayerPrefs.SetString("Player:Username", ServerListener.Instance._player.username);
                PlayerPrefs.SetString("Player:Email", ServerListener.Instance._player.email);

                // ya una vez logueado, procede a terminar de cargar la escena
                LoadScene();
            }
        }
        EnableInputsPanelSingIn();
    }

    /* ****************************** Sing Up Normal ****************************** */
    public void BTN_OnSingUpNormal () {
        DisableInputsPanelSingUp();
        txtMsgSingUp.text = "";
        if (inputSingUpUsername.text.Length > 0 && inputSingUpEmail.text.Length > 0 && 
            inputSingUpPassword.text.Length > 0 && inputSingUpCPassword.text.Length > 0) {
            if (inputSingUpPassword.text == inputSingUpCPassword.text) {
                WWWForm form = new WWWForm();
                form.AddField("_username", inputSingUpUsername.text);
                form.AddField("_email", inputSingUpEmail.text);
                form.AddField("_password", inputSingUpPassword.text);
                form.AddField("_method", "Normal");

                // define el metodo de logueo como normal...
                PlayerPrefs.SetString("SingIn:Method", "Normal");
                PlayerPrefs.SetString("Player:Password", inputSingUpPassword.text);

                StartCoroutine(OnSingUP_ToServer(form));
            } else {
                txtMsgSingUp.text = "Las claves no coinciden";
                EnableInputsPanelSingUp();
            }
        } else {
            txtMsgSingUp.text = "Por favor, llena todos los campos.";
            EnableInputsPanelSingUp();
        }
    }

    IEnumerator OnSingUP_ToServer (WWWForm form) {
        UnityWebRequest www = UnityWebRequest.Post(ServerListener.Instance.URL_Server + "singup/game", form);
        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            txtMsgSingUp.text = www.error;
        } else {
            Debug.Log("Result: " + www.downloadHandler.text);
            // almacenar preguntas en una lista
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, ServerListener.Instance._player);

            PlayerPrefs.SetString("Player:Username", ServerListener.Instance._player.username);
            PlayerPrefs.SetString("Player:Email", ServerListener.Instance._player.email);

            // ya una vez registrado, procede a terminar de cargar la escena
            LoadScene();
        }
        EnableInputsPanelSingUp();
    }

    /* ****************************** Sing In With Firebase ****************************** */
    public void BTN_OnSignInGoogle () {
        DisableInputsPanelSingIn();
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    public void OnSignOut () {
        AddStatusText("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect () {
        AddStatusText("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private List<string> messages = new List<string>();
    void AddStatusText (string text) {
        if (messages.Count == 5) {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages) {
            txt += "\n" + s;
        }
        txtMsgServer.text = txt;
    }

    internal void OnAuthenticationFinished (Task<GoogleSignInUser> task) {
        if (task.IsFaulted) {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator()) {
                if (enumerator.MoveNext()) {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    AddStatusText("Got Error: " + error.Status + " " + error.Message);
                } else {
                    AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        } else if (task.IsCanceled) {
            AddStatusText("Canceled");
        } else {
            AddStatusText("Welcome: " + task.Result.DisplayName + "!");
            AddStatusText("Email: " + task.Result.Email + "!");

            ServerListener.Instance._player.image_google = task.Result.ImageUrl.ToString();
            OnSingUp_WithGoogle(task.Result.DisplayName, task.Result.Email, "EduQuiz12345");
        }
        EnableInputsPanelSingIn();
    }

    private void OnSingUp_WithGoogle (string user, string email, string password) {
        DisableInputsPanelSingUp();
        txtMsgSingUp.text = "";

        WWWForm form = new WWWForm();
        form.AddField("_username", user);
        form.AddField("_email", email);
        form.AddField("_password", password);
        form.AddField("_method", "Google");

        // define el metodo de logueo como normal...
        PlayerPrefs.SetString("SingIn:Method", "Google");
        PlayerPrefs.SetString("Player:Password", password);

        StartCoroutine(OnSingUP_ToServer(form));

        EnableInputsPanelSingUp();
    }
}