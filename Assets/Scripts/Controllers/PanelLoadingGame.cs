using UnityEngine;
using TMPro;

public class PanelLoadingGame : MonoBehaviour
{
    public GameObject panelLoading;
    public TextMeshProUGUI txtLoading;
    public TextMeshProUGUI txtVersion;

    public static PanelLoadingGame Instance { set; get; }

    private void Awake() {
        Instance = this;
    }

    public void ShowPanelLoading(){
        panelLoading.SetActive(true);
        txtLoading.text = "Cargando preguntas";
    }
    
    public void HidePanelLoading(){
        panelLoading.SetActive(false);
    }
}
