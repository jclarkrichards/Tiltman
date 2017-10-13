using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    float timer = 0;
    public int points = 200;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
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
