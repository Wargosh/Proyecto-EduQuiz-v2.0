using UnityEngine;

public class CategoriesController : MonoBehaviour
{
    public static CategoriesController Instance { set; get; }
    void Awake()
    {
        Instance = this;
    }

    public Sprite ChangeBackgroundTheme (string category) {
        switch (category)
        {
            case "Matemáticas":
                return Resources.Load<Sprite>("Images/Categorias/f_matematica");
            case "Literatura":
                return Resources.Load<Sprite>("Images/Categorias/f_literatura");
            case "Astronomía":
                return Resources.Load<Sprite>("Images/Categorias/f_astronomia");
            case "Ciencia":
                return Resources.Load<Sprite>("Images/Categorias/f_ciencia");
            case "Deporte":
                return Resources.Load<Sprite>("Images/Categorias/f_deportes");
            case "Entretenimiento":
                return Resources.Load<Sprite>("Images/Categorias/f_entretenimiento");
            case "Física":
                return Resources.Load<Sprite>("Images/Categorias/f_fisica");
            case "Geografía":
                return Resources.Load<Sprite>("Images/Categorias/f_geografia");
            case "Historia":
                return Resources.Load<Sprite>("Images/Categorias/f_historia");
            case "Música":
                return Resources.Load<Sprite>("Images/Categorias/f_musica");
            default:
                return Resources.Load<Sprite>("Images/Categorias/f_ciencia");
        }
    }
}
