using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScreen : MonoBehaviour {

    public bool isReady;
    public bool canStart = false;
    public int connectedPlayers;
    public int readyPlayers;
    public int countDown = 3;
    public Text connectedText;
    public Text readyText;
    public Button readyButton;
    bool gameStarted = false;
	// Use this for initialization
	void Start () {
        NetworkManager.networkManager.lobbyScreen = this;

        isReady = false;
        connectedPlayers = 1;
        readyPlayers = 0;

        SendLobbyMessage();
	}

    public void ReadyButton() {

        if (!isReady) {
            isReady = !isReady;
            readyPlayers++;
            UpdateText();
        }
        else {
            isReady = !isReady;
            readyPlayers--;
            UpdateText();
        }
        SendLobbyMessage();
    }

    public void UpdateText() {
        if (!gameStarted)
        {
            if (canStart)
            {
                connectedText.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(false);
                readyText.text = "Game starting in: " + countDown;
            }
            else
            {
                connectedText.text = connectedPlayers + " player(s) connected.";
                readyText.text = readyPlayers + " of " + connectedPlayers + " player(s) ready.";
            }
        }
    }

    public void StartGame() {
        gameStarted = true;
        SceneManager.LoadScene(2);
    }

    void SendLobbyMessage() {
        LobbyMessage lobbyMessage = new LobbyMessage(isReady);
        string message = new MessageContainer("lobby", lobbyMessage).ToJson() + "\n";
        NetworkManager.networkManager.SendData(message);
    }
}
