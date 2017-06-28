using UnityEngine;
using System.Collections;

public class TeamInformation : MonoBehaviour {

    public float experienceBlue;
    public float experienceRed;
    public float currentExpBlue;
    public float currentExpRed;
    public float levelUpBlueExp;
    public float levelUpRedExp;

    public RectTransform expBarBlue;
    public RectTransform expBarRed;
    
    public TeamInformation teamBlue;
    public TeamInformation teamRed;

    // Use this for initialization
    void Start () {
        experienceBlue = 0;
        experienceRed = 0;
        levelUpBlueExp = 100;
        levelUpRedExp = 100;
    }
	
	// Update is called once per frame
	void Update () {

        experienceBlue += 1f;
        experienceRed += 1f;

        SetExp();
	}

    void SetExp()
    {
        currentExpBlue = GetComponent<TeamInformation>().experienceBlue;
        if (currentExpBlue >= 0 && currentExpBlue <= 100)
        {
            expBarBlue.localScale = new Vector3((currentExpBlue / levelUpBlueExp), 1, 1);
        }

        currentExpRed = GetComponent<TeamInformation>().experienceRed;
        if (currentExpRed >= 0 && currentExpRed <= 100)
        {
            expBarRed.localScale = new Vector3((currentExpRed / levelUpRedExp), 1, 1);
        }
    }
}
