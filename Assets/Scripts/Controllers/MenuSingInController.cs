using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSingInController : MonoBehaviour
{
    public TextMeshProUGUI txtMessage;
    public TextMeshProUGUI txtVersion;
    public Slider sldLoadScene;

    [HideInInspector] public bool onConnectionEstabilished = false;

    public static MenuSingInController Instance { set; get; }
    private void Awake() {
        Instance = this;
        onConnectionEstabilished = false; // declara que aun no se ha establecido conexion con el servidor
    }
    void Start () {
        txtVersion.text = "";
        StartCoroutine("TimeSliderLoadBar");
    }

    void LoadScene () {
        
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

        StartCoroutine("LoadSceneAsync");
    }
}
