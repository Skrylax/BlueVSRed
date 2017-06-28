using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UITop : MonoBehaviour {

    public Text levelBlueText;
    public Text levelRedText;
    public Text gameTime;

    public int levelBlue;
    public int levelRed;

    public int experienceBlue;
    public int experienceRed;

    // Use this for initialization
    void Start () {
        levelBlue = 1;
        levelRed = 1;

    }

    // Update is called once per frame
    void Update () {
        GetTeamLevels();
        gameTime.text = GetGameTime();
    }

    void GetTeamLevels()
    {
        levelBlueText.text = levelBlue.ToString();
        levelRedText.text = levelRed.ToString();
    }

    public string GetGameTime()
    {
        float timePassed;
        if (!GameManager.gameManager.online)
            timePassed = Time.realtimeSinceStartup;
        else
            timePassed = NetworkManager.networkManager.serverTimeInSeconds;
            TimeSpan t = TimeSpan.FromSeconds(timePassed);

            string time = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            return time;

    }
}
