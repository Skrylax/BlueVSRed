using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager networkManager;
    public Text errorText;
    public IPAddress hostAddress = IPAddress.Parse("194.168.178.15");
    public int id;
    public string username = "Unknown";
    public bool teamBlue = true;
    public int serverTimeInSeconds;
    public LobbyScreen lobbyScreen;
    public GameObject opponent;
    public GameObject player;
    private string message = "";
    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _receiveBuffer = new byte[8192];
    private string receivedData = "";
    
    void Start() {
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
        networkManager = this;
    }

    /// <summary>
    /// Try to connect to the hostAddress.
    /// </summary>
    /// <returns>True or False depending on whether the connection was successful or not. </returns>
    public bool SetupServer(string name)
    {
        try
        {
            _clientSocket.Connect(new IPEndPoint(hostAddress, 8888));

            _clientSocket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

            SendInitializeRequest(name);

            return true;
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
            if(errorText != null){
                errorText.text = "An error has occured while trying to connect!\nPlease check the entered IP address and/or verify your connection to the internet.";
                errorText.gameObject.SetActive(true);
            }
            return false;
        }
    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are received and call EndRecieve to finalize handshake
        int received = _clientSocket.EndReceive(AR);

        if (received <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[received];
        Buffer.BlockCopy(_receiveBuffer, 0, recData, 0, received);
        
        // Buffer the received data.
        receivedData += GetString(recData); // TODO: translate protocol to methods!

        // Split the data into substrings.
        string[] subData = receivedData.Split('\n');

        // Check if we read all the data.
        if (subData[subData.Length - 1].Length != 0)
            receivedData = subData[subData.Length - 1];
        else
            receivedData = "";
        
        // Process data here the way you want, all your bytes will be stored in recData
        for (int i = 0; i < subData.Length - 1; i++) {
            
            string message = subData[i];

            MessageContainer deserializedData = JsonConvert.DeserializeObject<MessageContainer>(message);

            // Create a task.
            TaskExecutor.taskExecutor.ScheduleTask(new Task(delegate
            {
                // Add whatever Unity API code you want here
                //Debug.Log("received: " + message);
                CreateTask(deserializedData.messageType, deserializedData.message);
            }));
        }

        //file.Close();
        //Start receiving again
        _clientSocket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    /// <summary>
    /// Create a task depending on the messageType received.
    /// </summary>
    /// <param name="message">Received message.</param>
    void CreateTask(string messageType, object message) {
        string messageJson = message.ToString();
        switch (messageType)
        {
            case "init":
                //Debug.Log("Initialize request received!");
                InitMessage initMessage = JsonConvert.DeserializeObject<InitMessage>(messageJson);
                this.id = initMessage.id;
                this.username = initMessage.name;
                this.teamBlue = initMessage.teamBlue;
                SceneManager.LoadScene(1);
                break;
            case "lobby":
                //Debug.Log("Lobby info received!");
                if(lobbyScreen) {
                LobbyMessage lobbyMessage = JsonConvert.DeserializeObject<LobbyMessage>(messageJson);
                lobbyScreen.connectedPlayers = lobbyMessage.connections;
                lobbyScreen.readyPlayers = lobbyMessage.readyConnections;
                lobbyScreen.canStart = lobbyMessage.canStart;
                lobbyScreen.countDown = lobbyMessage.countDown;
                lobbyScreen.UpdateText();
                if (lobbyMessage.countDown <= 0)
                    lobbyScreen.StartGame();
                }
                break;
            case "time":
                TimeMessage serverTime = JsonConvert.DeserializeObject<TimeMessage>(messageJson);
                serverTimeInSeconds = serverTime.timeInSeconds;
                break;
            case "initopponent":
                //Debug.Log("New opponent info received!");
                PlayerMessage newOpponentMessage = JsonConvert.DeserializeObject<PlayerMessage>(messageJson);
                if(GameManager.gameManager.GetPlayerWithId(newOpponentMessage.id) == null) {
                    GameObject newOpponent = Instantiate(opponent, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    newOpponent.GetComponent<PlayerInformation>().otherPlayer = true;
                    PlayerInformation opponentScript = newOpponent.GetComponent<PlayerInformation>();
                    opponentScript.id = newOpponentMessage.id;
                    opponentScript.pName = newOpponentMessage.pname;
                    opponentScript.playerNameText.text = opponentScript.pName;
                    newOpponent.GetComponent<TeamBlueOrRed>().teamBlue = newOpponentMessage.teamBlue;
                    if (newOpponent.GetComponent<TeamBlueOrRed>().teamBlue)
                    {
                        newOpponent.GetComponent<PlayerInformation>().head.GetComponent<Renderer>().material = GameManager.gameManager.teamColorBlue;
                        newOpponent.GetComponent<PlayerInformation>().body.GetComponent<Renderer>().material = GameManager.gameManager.teamColorBlue;
                        newOpponent.transform.position = new Vector3(0, 0, -20.5f);
                    }
                    else
                    {
                        newOpponent.GetComponent<PlayerInformation>().head.GetComponent<Renderer>().material = GameManager.gameManager.teamColorRed;
                        newOpponent.GetComponent<PlayerInformation>().body.GetComponent<Renderer>().material = GameManager.gameManager.teamColorRed;
                        newOpponent.transform.position = new Vector3(0, 0, 20.5f);
                    }
                    //opponentScript.health = newOpponentMessage.health;
                    //opponentScript.mana = newOpponentMessage.mana;
                    GameManager.gameManager.players.Add(opponentScript);
                    //newOpponent.transform.position = new Vector3(newOpponentMessage.position.x, newOpponentMessage.position.y, newOpponentMessage.position.z);
                    newOpponent.SetActive(true);
                }
                break;
            case "initplayer":
                //Debug.Log("Player info received!");
                GameManager.gameManager.online = true;
                GameManager.gameManager.username = username;
                PlayerMessage playerMessage = JsonConvert.DeserializeObject<PlayerMessage>(messageJson);
                player = GameManager.gameManager.player;
                //player.GetComponent<Movement>().enabled = true;
                PlayerInformation playerScript = player.GetComponent<PlayerInformation>();
                playerScript.id = playerMessage.id;
                playerScript.pName = playerMessage.pname;
                playerScript.playerNameText.text = playerScript.pName;
                GameManager.gameManager.username = playerScript.pName;
                //playerScript.health = playerMessage.health;
                //playerScript.mana = playerMessage.mana;
                GameManager.gameManager.players.Add(playerScript);
                //player.transform.position = new Vector3(playerMessage.position.x, playerMessage.position.y, playerMessage.position.z);
                GameManager.gameManager.playerInfo = player.GetComponent<PlayerInformation>();
                //StartCoroutine(GameManager.gameManager.SendPlayerData());
                player.SetActive(true);
                break;
            case "remove":
                //Debug.Log("Remove player request received!");
                PlayerMessage removeMessage = JsonConvert.DeserializeObject<PlayerMessage>(messageJson);
                PlayerInformation removeMe = GameManager.gameManager.GetPlayerWithId(removeMessage.id);
                GameManager.gameManager.players.Remove(removeMe);
                Destroy(removeMe.gameObject);
                break;
            case "playerpath":
                //Debug.Log("Opponent info received!");
                PathMessage opponentInfoMessage = JsonConvert.DeserializeObject<PathMessage>(messageJson);
                Vector3[] newPath = new Vector3[opponentInfoMessage.path.Length];
                for (int i = 0; i < opponentInfoMessage.path.Length; i++) {
                    newPath[i] = new Vector3(opponentInfoMessage.path[i].x, opponentInfoMessage.path[i].y, opponentInfoMessage.path[i].z);
                }
                PlayerMovement playerWithPath = GameManager.gameManager.GetPlayerWithId(opponentInfoMessage.id).gameObject.GetComponent<PlayerMovement>();
                playerWithPath.path = newPath;
                playerWithPath.OnPathFound(newPath, true);
                break;
            case "recall":
                //Debug.Log("Recall message received!");
                SkillMessage recallMessage = JsonConvert.DeserializeObject<SkillMessage>(messageJson);
                GameManager.gameManager.GetPlayerWithId(recallMessage.id).gameObject.GetComponent<PlayerAttack>().RecallAnimation();
                break;
            case "attack":
                //Debug.Log("Attack message received!");
                SkillMessage attackMessage = JsonConvert.DeserializeObject<SkillMessage>(messageJson);
                GameManager.gameManager.GetPlayerWithId(attackMessage.id).gameObject.GetComponent<PlayerAttack>().SwingSword();
                break;
            case "q":
                //Debug.Log("q message received!");
                SkillMessage qMessage = JsonConvert.DeserializeObject<SkillMessage>(messageJson);
                PlayerInput.playerInput.IncreaseHealth(GameManager.gameManager.GetPlayerWithId(qMessage.id).gameObject);
                break;
            case "w":
                //Debug.Log("w message received!");
                SkillMessage wMessage = JsonConvert.DeserializeObject<SkillMessage>(messageJson);
                PlayerInput.playerInput.IncreaseAttackSpeed(GameManager.gameManager.GetPlayerWithId(wMessage.id).gameObject, 10);
                break;
            case "e":
                //Debug.Log("e message received!");
                SkillMessage eMessage = JsonConvert.DeserializeObject<SkillMessage>(messageJson);
                PlayerInput.playerInput.IncreaseMoveSpeed(GameManager.gameManager.GetPlayerWithId(eMessage.id).gameObject, 3);
                break;
            case "bDamage":
                //Debug.Log("Building damage received!");
                DamageMessage bDamageMessage = JsonConvert.DeserializeObject<DamageMessage>(messageJson);
                GameManager.gameManager.GetBuildingWithId(bDamageMessage.targetId).health -= bDamageMessage.damage;
                break;
            case "pDamage":
                //Debug.Log("Player damage received!");
                DamageMessage pDamageMessage = JsonConvert.DeserializeObject<DamageMessage>(messageJson);
                GameManager.gameManager.GetPlayerWithId(pDamageMessage.targetId).health -= pDamageMessage.damage;
                break;
            default:
                Debug.Log("Unknown messageType.");
                break;
        }
    }

    /// <summary>
    /// Send a request to initialize the player.
    /// </summary>
    void SendInitializeRequest(string name) {
        
        InitMessage initMessage = new InitMessage(name);
        string initContainer = new MessageContainer("init", initMessage).ToJson() + "\n";
        networkManager.SendData(initContainer);
    }

    /// <summary>
    /// Initialize the player.
    /// </summary>
    /// <param name="newPlayerData">A PlayerData object the properties of the player.</param>
    void InitializePlayer(PlayerMessage newPlayerData)
    {
        //GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //Player playerScript = player.GetComponent<Player>();
        //playerScript.playerData = newPlayerData;
    }

    /// <summary>
    /// Send data.
    /// </summary>
    /// <param name="message">A message to send.</param>
    public void SendData(string message)
    {
        byte[] data = GetBytes(message);
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
    }

    byte[] GetBytes(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    string GetString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }
}
