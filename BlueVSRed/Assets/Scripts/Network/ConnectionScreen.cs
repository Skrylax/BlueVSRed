using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using Newtonsoft.Json;

public class ConnectionScreen : MonoBehaviour {

    public InputField userNameField;
    public InputField ipField;
    public Text connectionStatus;
    public Button submitButton;

    private string usernameInput;
    private string userIpInput;
    private IPAddress ipAddress;
    private bool successfulConnection;


    void Start() {
        //LobbyMessage lobbyMessage = new LobbyMessage(true);
        //string message = new MessageContainer("lobby", lobbyMessage).ToJson() + "\n";
        //connectionStatus.gameObject.SetActive(true);
        //connectionStatus.text = message;
        //LobbyMessage lobMessage = JsonConvert.DeserializeObject<LobbyMessage>(message);
        //connectionStatus.text += "YOLO";
    }

    public void ConnectToServer() {
        if (userNameField.text == "") {
            connectionStatus.text = "Please enter an username!";
            connectionStatus.gameObject.SetActive(true);
            return;
        }

        // Save the user input into variables.
        userIpInput = (ipField.text == "localhost") ? userIpInput = "127.0.0.1" : userIpInput = ipField.text;
        usernameInput = userNameField.text;

        // Check if the given input is a valid IP address.
        if(IPAddress.TryParse(userIpInput, out ipAddress)){
            userNameField.gameObject.SetActive(false);
            ipField.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
            connectionStatus.text = "Connecting to: " + ipAddress.ToString();
            connectionStatus.gameObject.SetActive(true);

            NetworkManager.networkManager.hostAddress = ipAddress;
            successfulConnection = NetworkManager.networkManager.SetupServer(usernameInput);

            if (successfulConnection){
                connectionStatus.text = "Connection successful!";
            }
            else {
                userNameField.gameObject.SetActive(true);
                ipField.gameObject.SetActive(true);
                submitButton.gameObject.SetActive(true);
            }


        }else{
            connectionStatus.text = "Given IP Address is invalid.";
            connectionStatus.gameObject.SetActive(true);

        }
    }
}
