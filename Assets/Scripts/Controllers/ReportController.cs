using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ReportController : MonoBehaviour
{
    public GameObject contentReportQuestion;
    public TMP_Dropdown cbxReason;
    public TextMeshProUGUI txtMessage;
    public Button btnSendReport;

    public static ReportController Instance { set; get; }

    private void Awake() {
        Instance = this;
    }

    public void BTN_SendReport () {
        print ("Reporte enviado: " + cbxReason.options[cbxReason.value].text);
        GameManager.Instance.activateReport = false;
        DisableButtonSendReport();
    }

    public void EnableButtonSendReport () {
        btnSendReport.interactable = true;
    }

    private void DisableButtonSendReport () {
        txtMessage.text = "Tu reporte ha sido enviado y sera posteriormente revisado.<br><br>Gracias por aportar a nuestra comunidad.";
        btnSendReport.interactable = false;
    }

    public void EnableContentReportQuestion () {
        contentReportQuestion.SetActive(true);
        txtMessage.text = "";
    }

    public void DisableContentReportQuestion () {
        txtMessage.text = "";
        contentReportQuestion.SetActive(false);
    }
}
