using UnityEngine;
using TMPro;

public class WilcardButtonsController : MonoBehaviour
{
    public CanvasGroup canvasGroupFooter;

    public TextMeshProUGUI[] txtButtons;
    
    public static WilcardButtonsController Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    public void BTN_C_Correct () { 
        print("comodin de respuesta correcta");
        DisableWilcardButtons();
        GameManager.Instance.EjectWilcard_CorrectQuestion();
    }

    public void BTN_C_5050 () { 
        print("comodin de 50/50");
        DisableWilcardButtons();
        GameManager.Instance.EjectWilcard_50_50();
    }
    public void BTN_C_Plus10Seconds () { 
        print("comodin de +10 segundos extra");
        DisableWilcardButtons();
        GameManager.Instance.EjectWilcard_Plus10Seconds();
    }

    public void EnableWilcardButtons () {
        canvasGroupFooter.blocksRaycasts = true;
        canvasGroupFooter.interactable = true;

        for (int i = 0; i < txtButtons.Length; i++){
            txtButtons[i].color = new Color(1f,1f,1f, 1f);
        }
    }
    public void DisableWilcardButtons () {
        canvasGroupFooter.blocksRaycasts = false;
        canvasGroupFooter.interactable = false;

        for (int i = 0; i < txtButtons.Length; i++){
            txtButtons[i].color = new Color(1f,1f,1f, .5f);
        }
    }
}
