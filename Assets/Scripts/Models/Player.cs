[System.Serializable]
public class Player 
{
	public string _id;			// id de DB
	public string idSocket;		// id de SocketIO generado
	public string username;		// texto de la pregunta.
	public string email;		// categoria de la pregunta.
	public string image;		// ruta de la imagen si llega una.
	public int _points = 0;		// Puntos
	public int _xp = 0;			// Experiencia
	public int _level = 1;		// Nivel
	public int _coins = 0;      // Monedas
	public string image_google = "";
}
