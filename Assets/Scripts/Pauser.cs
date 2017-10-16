using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PauseType
{
    death,
    munch,
    finish,
    gameover,
    none
}

public class Pauser : MonoBehaviour
{
    public static Pauser S;
    public bool paused;
    //float pauseTime = 0.5f;
    float pauseTimer = 0;
    //public bool gamePaused = false;
    //float deathPauseTime = 2;
    //public bool deathPaused = false;
    public PauseType pause;


    private void Awake()
    {
        S = this;
        pause = PauseType.none;
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
        if (pause == PauseType.munch)
        {
            PauseHandler(0.5f);
           
        }
        else if(pause == PauseType.death)
        {
            PauseHandler(2);
            
        }
        else if(pause == PauseType.finish)
        {
            PauseHandler(3);
        }
        else if(pause == PauseType.gameover)
        {
            PauseHandler(0, noUnpause:true);
        }
        else
        {
            if (Input.GetKeyDown("space")) { paused = !paused; }
        }
    }

    void Unpause()
    {
        pauseTimer = 0;
        paused = false;
        pause = PauseType.none;
    }

    void PauseHandler(float ptime, bool noUnpause=false)
    {
        pauseTimer += Time.deltaTime;
        paused = true;
        if (pauseTimer >= ptime && !noUnpause)
        {
            Unpause();
        }
    }

    
}
