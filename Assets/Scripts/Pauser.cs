using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{
    public static Pauser S;
    public bool paused;

    private void Awake()
    {
        S = this;
    }
    // Use this for initialization
    void Start ()
    {
        //print("Creating the pauser");
        paused = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("space")) { paused = !paused; }
    }
}
