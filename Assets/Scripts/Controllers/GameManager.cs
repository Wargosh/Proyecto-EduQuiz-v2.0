using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

[RequireComponent(typeof(CategoriesController))]
public class GameManager : MonoBehaviour
{
    [Header("Imagenes")]
    public Image imgClock;
    public Image imgQuestion;
    public Image imgBackgroundTheme;

    [Header("Textos")]
    public TextMeshProUGUI txtClock;
    public TextMeshProUGUI txtNumQuestion;
    public TextMeshProUGUI txtCategory;
    public TextMeshProUGUI txtTittleQuestion;
    public TextMeshProUGUI txtTittleMainQuestion;
    public TextMeshProUGUI txtMessagePanel;

    // Control de los botones de opcion
    [Header("Control de las opciones")]
    public BtnOptionController[] btnOptions;
    private int[] orderOptions = new int[4] { 0, 1, 2, 3 };

    // Animaciones
    [Header("Animaciones")]
    public Animator animQuestion;
    public Animator animPanelMessages;
    Animator[] animOptions = new Animator[4];

    // Control de paneles
    [Header("Paneles")]
    public GameObject panelFooterWilcards;
    public GameObject panelFooterActions;
    public CanvasGroup canvasFooterActions;

    // nuevo juego
    bool activateClock = false;
    [HideInInspector] public bool triggerContinued = true;

    // variables pregunta
    int indexQuestion = 0, indexOptionClicked = -1;

    // reloj
    const float segMax = 15;
    float segs = 15;

    public static GameManager Instance { set; get; }

    private void Awake(){
        Instance = this;
    }
    void Start() {
        for (int i = 0; i < btnOptions.Length; i++){
            animOptions[i] = btnOptions[i].gameObject.GetComponent<Animator>();
        }
    }

    void Update() {
        if (!activateClock)
            return;

        CountDownClock();
    }

    public void BTN_NewGame() {
        NewGame();
    }

    public void NewGame() {
        indexQuestion = -1;
        
        NextQuestion();
    }

    public void NextQuestion(){ 
        
        indexQuestion++;
        if (indexQuestion < ServerListener.Instance.listQuestions.questions.Count) {
            segs = segMax;
            imgClock.fillAmount = 1;
            txtClock.text = "" + segs.ToString("0");
            StartCoroutine("TimeShowNewQuestion");
            //ShowQuestion();
        }
    }

    IEnumerator TimeShowNewQuestion(){
        HideUI_Options();
        VerifyChangeTheme(txtTittleQuestion.text = ServerListener.Instance.listQuestions.questions[indexQuestion].category);
        yield return new WaitForSeconds(.5f);
        // precargar la preguna (por si es con imagen)
        LoadQuestion();

        yield return new WaitForSeconds(1f);
        ShowQuestion();
    }

    private void LoadQuestion() {
        txtCategory.text = ServerListener.Instance.listQuestions.questions[indexQuestion].category;
        if (ServerListener.Instance.listQuestions.questions[indexQuestion].images.Length == 0)
        {
            // mostrar solo pregunta
            txtTittleQuestion.gameObject.SetActive(false);
            txtTittleMainQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.text = ServerListener.Instance.listQuestions.questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(false);
        }
        else
        {
            // mostrar pregunta con imagen
            txtTittleQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.gameObject.SetActive(false);
            txtTittleQuestion.text = ServerListener.Instance.listQuestions.questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(true);
            //imgQuestion.sprite = Resources.Load<Sprite>("Images/EnDesarrollo"/* + questions[indexQuestion].img*/); // Cargar imagen
            StartCoroutine(LoadImage(ServerListener.Instance.URL_Server + "upload/questions/" + ServerListener.Instance.listQuestions.questions[indexQuestion].images[0]));
        }
    }

    public void ShowQuestion()
    {
        // numero de la pregunta
        txtNumQuestion.text = (indexQuestion + 1) + "/" + ServerListener.Instance.listQuestions.questions.Count;
        // mostrar UI panel de pregunta
        animQuestion.SetBool("isHide", false);
        // posiciones de las opciones en aleatorio
        RandomOptions();
        // habilitar panel comodines
        EnablePanelWilcards();

        StartCoroutine("ShowOptions");
    }

    IEnumerator ShowOptions(){
        // llenar texto de las opciones
        for (int i = 0; i < btnOptions.Length; i++)
        {
            btnOptions[i].SetAnswerBtnOption(
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].option,
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].status);
            yield return new WaitForSeconds(.25f);
            animOptions[i].SetBool("isHide", false);
        }

        // activar cuenta atras
        activateClock = true;
        // habilitar comodines
        WilcardButtonsController.Instance.EnableWilcardButtons();
        // habilitar el uso del boton continuar
        triggerContinued = true;
    }
    
    private void VerifyChangeTheme (string category){
        imgBackgroundTheme.sprite = CategoriesController.Instance.ChangeBackgroundTheme(category);
    }
    private void HideUI_Options () {
        animQuestion.SetBool("isHide", true);
        for (int i = 0; i < animOptions.Length; i++){
            animOptions[i].SetBool("isHide", true);
            animOptions[i].SetBool("isCorrect", false);
            animOptions[i].SetBool("isIncorrect", false);
        }
    }

    public bool OptionIsClicked () {
        activateClock = false;
        WilcardButtonsController.Instance.DisableWilcardButtons();
        // mostrar botones de accion luego de n tiempo
        StartCoroutine("TimeBtnContinue");

        if (indexOptionClicked != indexQuestion) {
            indexOptionClicked = indexQuestion;
            FindOptionCorrect();
            return true;
        }
        return false;
    }

    IEnumerator TimeBtnContinue () {
        yield return new WaitForSeconds(1.5f);
        EnablePanelActions();
    }

    private void FindOptionCorrect(){
        for (int i = 0; i < animOptions.Length; i++){
            if (ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].status)
                animOptions[i].SetBool("isCorrect", true);
        }
    }

    IEnumerator LoadImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imgQuestion.sprite = Sprite.Create(myTexture, new Rect(0, 0,
            myTexture.width, myTexture.height), new Vector2(0, 0));
        }
    }

    public void ResetGame()
    {
        NewGame();
    }

    private void EnableButtonsAction(){
        canvasFooterActions.interactable = true;
        canvasFooterActions.blocksRaycasts = true;
    }

    public void DisableButtonsAction(){
        canvasFooterActions.interactable = false;
        canvasFooterActions.blocksRaycasts = false;
    }

    private void EnablePanelWilcards(){
        panelFooterWilcards.SetActive(true);
        panelFooterActions.SetActive(false);
    }

    private void EnablePanelActions(){
        EnableButtonsAction();

        panelFooterWilcards.SetActive(false);
        panelFooterActions.SetActive(true);
    }

    /* *************************************** COMODINES *************************************** */
    public void EjectWilcard_CorrectQuestion(){
        OptionIsClicked();
        CorrectQuestion();
    }
    public void EjectWilcard_50_50(){
        int indexCorrect = -1, auxIndex2 = -1, countOp = 0;
        for (int i = 0; i < btnOptions.Length; i++){
            if (btnOptions[i].isCorrect){
                indexCorrect = i;
                break;
            }
        }
        do
        {
            int rr = Random.Range(0, 4);
            if (countOp == 0  && rr != indexCorrect) {
                auxIndex2 = rr;
                animOptions[rr].SetBool("isHide", true);
                countOp++;
            }
            if (countOp == 1  && rr != indexCorrect && rr != auxIndex2) {
                animOptions[rr].SetBool("isHide", true);
                countOp++;
            }
        } while (countOp < 2);
    }
    public void EjectWilcard_Plus10Seconds() {
        if (activateClock && segs > 0f)
            segs += 10;
    }
    public void CorrectQuestion(){
        ShowMessagePanel("¡Correcto!");
        print("Correcto!");
    }

    public void IncorrectQuestion(string message = "¡Incorrecto!"){
        ShowMessagePanel(message);
        print("Incorrecto...");
    }

    private void ShowMessagePanel (string message) {
        txtMessagePanel.text = message;
        animPanelMessages.SetBool("showMessage", true);
    }

    public void BTN_Continue () {
        // evita que el boton se ejecute mas de una vez por pregunta
        if (triggerContinued)
            triggerContinued = false;
        else
            return;
        
        DisableButtonsAction();
        // restablece animaciones
        animPanelMessages.SetBool("showMessage", false);
        // invoca la proxima pregunta
        NextQuestion();
    }

    /// <summary>
    /// Mantiene la cuenta atras del reloj por pregunta con el tiempo maximo establecido
    /// </summary>
    private void CountDownClock ()
    {
        if (segs > 0)
        {
            segs -= Time.deltaTime;
            imgClock.fillAmount = (float)(segs / segMax);
            txtClock.text = "" + segs.ToString("0");
        } else if (segs <= 0f) {
            OptionIsClicked();
            IncorrectQuestion("¡Tiempo fuera!");
        }
    }

    private void RandomOptions ()
    {
        int steps = 5;
        int aux;
        for (int i = 0; i < steps; i++)
        {
            int index1 = Random.Range(0, orderOptions.Length);
            int index2 = Random.Range(0, orderOptions.Length);
            if (index1 == index2)
            {
                float prob = Random.Range(0f, 1f);
                if (prob <= 0.5f)
                {
                    if (index2 == (orderOptions.Length - 1))
                        index2 = 0;
                    else
                        index2++;
                }
                else
                {
                    if (index2 == 0)
                        index2 = orderOptions.Length - 1; // 3
                    else
                        index2--;
                }
            }
            aux = orderOptions[index1];
            orderOptions[index1] = orderOptions[index2];
            orderOptions[index2] = aux;
        }
    }
}
