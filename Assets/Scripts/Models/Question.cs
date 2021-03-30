using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
	public string question; // texto de la pregunta.
	public string img = ""; // ruta de la imagen si llega una.
	public List<Option> options = new List<Option>(); // informacion de las opciones (texto, correcta/incorrecta)
}

[System.Serializable]
public class Option
{
	public string text; // texto de la opcion.
	public bool isCorrect = false; // ruta de la imagen si llega una.
}