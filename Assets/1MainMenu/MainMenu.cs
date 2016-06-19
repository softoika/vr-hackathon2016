using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPrezenter()
    {
        DataManager.instance.setPrezenter(true);
        Application.LoadLevel("MainClassChange");
    }
    public void OnAudience()
    {
        DataManager.instance.setPrezenter(false);
        Application.LoadLevel("MainClassChange");
    }
}
