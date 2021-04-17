using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    // Control de los botones de opcion
    [Header("Control de las opciones")]
    public BtnOptionController[] btnOptions;
    private int[] orderOptions = new int[4] { 0, 1, 2, 3 };

    // Animaciones
    [Header("Animaciones")]
    public Animator animQuestion;
    Animator[] animOptions = new Animator[4];

    // nuevo juego
    bool activateClock = false;

    // variables pregunta
    int indexQuestion = 0, indexOptionClicked = -1;

    // reloj
    const float segMax = 20;
    float segs = 20;

    public static GameManager Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < btnOptions.Length; i++){
            animOptions[i] = btnOptions[i].gameObject.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!activateClock)
            return;

        CountDownClock();
    }

    public void BTN_NewGame()
    {
        NewGame();
    }

    public void NewGame()
    {
        indexQuestion = -1;
        
        NextQuestion();
    }

    public void NextQuestion(){ 
        indexQuestion++;
        if (indexQuestion < ServerListener.Instance.listQuestions.questions.Count) {
            segs = segMax;
            imgClock.fillAmount = 1;
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

        StartCoroutine("ShowOptions");
    }

    IEnumerator ShowOptions(){
        // llenar texto de las opciones
        for (int i = 0; i < btnOptions.Length; i++)
        {
            yield return new WaitForSeconds(.25f);
            animOptions[i].SetBool("isHide", false);
            btnOptions[i].SetAnswerBtnOption(
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].option,
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].status);
        }

        // activar cuenta atras
        activateClock = true;
    }
    
    private void VerifyChangeTheme (string category){
        imgBackgroundTheme.sprite = CategoriesController.Instance.ChangeBackgroundTheme(category);
    }
    private void HideUI_Options () {
        animQuestion.SetBool("isHide", true);
        for (int i = 0; i < animOptions.Length; i++){
            animOptions[i].SetBool("isHide", true);
            animOptions[i].SetBool("isCorrect", false);
        }
    }

    public bool OptionIsClicked () {
        activateClock = false;

        if (indexOptionClicked != indexQuestion) {
            indexOptionClicked = indexQuestion;
            FindOptionCorrect();
            return true;
        }
        return false;
    }

    private void FindOptionCorrect(){
        for (int i = 0; i < animOptions.Length; i++){
            if (ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].status)
                animOptions[i].SetBool("isCorrect", true);
        }
    }

    IEnumerator LoadImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        imgQuestion.sprite = Sprite.Create(www.texture, new Rect(0, 0,
            www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void ResetGame()
    {
        NewGame();
    }

    /// <summary>
    /// Mantiene la cuenta atras del reloj por pregunta con el tiempo maximo establecido
    /// </summary>
    private void CountDownClock ()
    {
        if (segs > 0)
        {
            segs -= Time.deltaTime;
            imgClock.fillAmount -= Time.deltaTime / segMax;
            txtClock.text = "" + segs.ToString("00");
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
