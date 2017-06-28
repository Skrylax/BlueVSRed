using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIMiddle : MonoBehaviour {
    
    public GameObject middlePanel;
    public GameObject playerObject;
    PlayerInformation player;
    
    public List<Text> playerNames = new List<Text>();

    List<PlayerInformation> players = new List<PlayerInformation>();
    List<PlayerInformation> teamBlue = new List<PlayerInformation>();
    List<PlayerInformation> teamRed = new List<PlayerInformation>();

    // Use this for initialization
    void Start () {
        middlePanel.SetActive(false);
        GetTeams();
        SetPlayerNames();

        

    }
	
	// Update is called once per frame
	void Update () {
        DisplayStats();
    }

    void DisplayStats()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            middlePanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab))
            middlePanel.SetActive(false);
    }
    
    void GetTeams()
    {
        player = playerObject.GetComponent<PlayerInformation>();
        players.Add(player);

        foreach (PlayerInformation p in players)
        {
            if (player.GetComponent<TeamBlueOrRed>().teamBlue == true)
                teamBlue.Add(player);
            if (player.GetComponent<TeamBlueOrRed>().teamBlue == false)
                teamRed.Add(player);
        }
    }

    void SetPlayerNames()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerNames[i].text = players[i].pName;
        }
    }

    void TestMethod()
    {
        if (middlePanel.active == true)
        {
            foreach (PlayerInformation name in players)
            {
                if (teamBlue[0] != null)
                    Debug.Log("Blue player: " + teamBlue[0].pName);
                //if (teamRed[0] != null)
                //    Debug.Log("Red player: " + teamRed[0].pName);
            }
        }
    }
}
