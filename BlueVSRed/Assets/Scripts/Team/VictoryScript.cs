using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScript : MonoBehaviour {

    public GameObject coreBlue;
    public GameObject coreRed;
    public Image blue;
    public Image red;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (coreRed.GetComponent<MinionInformation>().health <= 0)
        {
            StartCoroutine("VictoryBlue");
        }

        if(coreBlue.GetComponent<MinionInformation>().health <= 0)
        {
            StartCoroutine("VictoryRed");
        }
    }

    IEnumerator VictoryBlue()
    {
        yield return new WaitForSeconds(3);
        blue.gameObject.SetActive(true);
        //SceneManager.LoadScene(3);
    }

    IEnumerator VictoryRed()
    {
        yield return new WaitForSeconds(3);
        red.gameObject.SetActive(true);
        //SceneManager.LoadScene(4);
    }
}
