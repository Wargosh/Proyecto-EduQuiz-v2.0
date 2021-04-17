using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnOptionController : MonoBehaviour
{
    public TextMeshProUGUI txtOption;
    public TextMeshProUGUI txtMessagePanel;

    [Header("Animaciones")]
    public Animator animQuestion;
    public Animator animPanelMessages;
    private Animator anim;

    [SerializeField]
    private string idBtn;
    [SerializeField]
    private string text = "";

    private bool isCorrect = false;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void SetAnswerBtnOption(string option, bool isCorrect)
    {
        txtOption.text = text = option;
        this.isCorrect = isCorrect;
    }

    public void BTN_OptionClicked ()
    {
        if (GameManager.Instance.OptionIsClicked()) {
            print("opcion es: " + isCorrect);
            if (!isCorrect){
                txtMessagePanel.text = "¡Incorrecto!";
                StartCoroutine("TimeAnimError");
            } else {
                txtMessagePanel.text = "¡Correcto!";
                StartCoroutine("TimeAnimCorrect");
            }
        }
    }

    IEnumerator TimeAnimError(){
        animQuestion.SetBool("isError", true);
        anim.SetBool("isIncorrect", true);
        animPanelMessages.SetBool("showMessage", true);

        yield return new WaitForSeconds(0.2f);
        animQuestion.SetBool("isError", false);
        yield return new WaitForSeconds(1.8f);
        // restablece animaciones
        anim.SetBool("isIncorrect", false);
        animPanelMessages.SetBool("showMessage", false);
        // invoca la proxima pregunta
        GameManager.Instance.NextQuestion();
    }

    IEnumerator TimeAnimCorrect(){
        anim.SetBool("isCorrect", true);
        animPanelMessages.SetBool("showMessage", true);
        yield return new WaitForSeconds(2f);

        // restablece animaciones
        anim.SetBool("isCorrect", false);
        animPanelMessages.SetBool("showMessage", false);
        // invoca la proxima pregunta
        GameManager.Instance.NextQuestion();
    }

    
}
