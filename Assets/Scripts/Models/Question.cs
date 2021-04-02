using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
	public string _id;
	public string question; // texto de la pregunta.
	public string images = ""; // ruta de la imagen si llega una.
	public string status;
	public List<Option> options = new List<Option>(); // informacion de las opciones (texto, correcta/incorrecta)
}

[System.Serializable]
public class Option
{
	public string option; // texto de la opcion.
	public bool status = false; // ruta de la imagen si llega una.
}