using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

    public Text rupee_text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int num_player_rupees = player_control.instance.rupee_count;
        rupee_text.text = "Rupees: " + num_player_rupees.ToString();
	}
}
