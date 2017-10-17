using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    //public string name;
    float timer = 0;
    public int points = 200;
    public bool ingame = true;

	// Use this for initialization
	void Start ()
    {
        //name = "general";
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (ingame)
        {
            if (!Pauser.S.paused)
            {
                timer += Time.deltaTime;
                if (timer >= 10)
                {
                    Destroy(gameObject);
                }
            }
        }
	}
}
