using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_control : MonoBehaviour {

    public float walking_velocity = 1.0f;
    public int rupee_count = 0;

    public static player_control instance;

	// Use this for initialization
	void Start () {
        if(instance != null)
        {
            Debug.LogError("Multiple Link objects detected! There is more than one Link.");
        }
        instance = this; //Singleton
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");

        if (horizontal_input != 0.0f)
        {
            vertical_input = 0.0f;
        }
        GetComponent<Rigidbody>().velocity = new Vector3(horizontal_input, vertical_input, 0) * walking_velocity;
	}

    void OnTriggerEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Rupee")
        {
            
            Destroy(coll.gameObject);
            rupee_count++;
        }
        
    }
}
