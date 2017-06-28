using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;
    public string username = "Unknown";
    public GameObject player;
    public PlayerInformation playerInfo;
    public List<PlayerInformation> players;
    public List<MinionInformation> buildings;
    public bool online = false;
    public Material teamColorRed;
    public Material teamColorBlue;
    public bool appClosing = false;

    void Start() {
        gameManager = this;
        playerInfo = player.gameObject.GetComponent<PlayerInformation>();
        try{
            username = NetworkManager.networkManager.username;
            player.GetComponent<TeamBlueOrRed>().teamBlue = NetworkManager.networkManager.teamBlue;

            if (player.GetComponent<TeamBlueOrRed>().teamBlue)
            {
                player.GetComponent<PlayerInformation>().head.GetComponent<Renderer>().material = teamColorBlue;
                player.GetComponent<PlayerInformation>().body.GetComponent<Renderer>().material = teamColorBlue;
                player.transform.position = new Vector3(0, 0, -20.5f);
            }
            else
            {
                player.GetComponent<PlayerInformation>().head.GetComponent<Renderer>().material = teamColorRed;
                player.GetComponent<PlayerInformation>().body.GetComponent<Renderer>().material = teamColorRed;
                player.transform.position = new Vector3(0, 0, 20.5f);
            }
        }
        catch(Exception e){}

        players = new List<PlayerInformation>();
        
        if(username != "Unknown")
            InitPlayer();

        // Set building id's.
        for (int i = 0; i < buildings.Count; i++) {
            buildings[i].buildingId = i;
        }
        
    }

    void InitPlayer() {
        PlayerMessage playerMessage = new PlayerMessage(-1, username, -1, -1, new Vector3(0,0,0));
        string message = new MessageContainer("initplayer", playerMessage).ToJson() + "\n";
        NetworkManager.networkManager.SendData(message);
    }

    public PlayerInformation GetPlayerWithId(int id)
    {
        foreach (PlayerInformation element in players)
        {
            if (element.id == id)
                return element;
        }
        return null;
    }

    public MinionInformation GetBuildingWithId(int id)
    {
        foreach (MinionInformation element in buildings)
        {
            if (element.buildingId == id)
                return element;
        }
        return null;
    }

    public IEnumerator SendPlayerData() {
        while(true){
            PlayerMessage playerMessage = new PlayerMessage(playerInfo.id, playerInfo.name, playerInfo.health, playerInfo.mana, player.transform.position);
            string message = new MessageContainer("syncplayer", playerMessage).ToJson() + "\n";
            NetworkManager.networkManager.SendData(message);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    void OnApplicationQuit()
    {
        this.appClosing = true;
    }

}
