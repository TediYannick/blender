using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;
using TMPro;

public class InertialControl : MonoBehaviour
{
    public TMP_InputField ipAddressInputField;
    public TMP_InputField portInputField;
    public Button connectButton;
    public TMP_Text debugText; 
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        connectButton.onClick.AddListener(InitiateConnection);
        Input.gyro.enabled = true;
    }

    void InitiateConnection()
    {
        string ipAddress = ipAddressInputField.text;
        int port;
        if (!int.TryParse(portInputField.text, out port))
        {
            debugText.text = "Port invalide.";
            return;
        }

        try
        {
            // Connect to the server
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            debugText.text = "Connexion au serveur TCP réussie.";
        }
        catch (SocketException e)
        {
            debugText.text = $"Erreur de connexion au serveur TCP : {e.Message}";
        }
    }

    void Update()
    {
        if (client == null || !client.Connected)
        {
            return;
        }

        // Lire les données de la centrale inertielle
        Vector3 acceleration = Input.acceleration;
        Vector3 gyro = Input.gyro.rotationRate;

        // Convertir les données en chaîne de caractères
        string message = $"acc:{acceleration.x},{acceleration.y},{acceleration.z};gyro:{gyro.x},{gyro.y},{gyro.z}";

        // Envoyer les données via TCP
        byte[] data = Encoding.ASCII.GetBytes(message);
        try
        {
            stream.Write(data, 0, data.Length);
            debugText.text = $"Données envoyées : {message}";
        }
        catch (SocketException e)
        {
            debugText.text = $"Erreur d'envoi des données : {e.Message}";
        }
    }

    void OnApplicationQuit()
    {
        // Fermer la connexion TCP
        if (stream != null)
        {
            stream.Close();
        }
        if (client != null)
        {
            client.Close();
        }
        debugText.text = "Connexion TCP fermée.";
    }
}
