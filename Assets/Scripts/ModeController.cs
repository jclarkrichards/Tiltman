﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModeNames
{
    SCATTER,
    CHASE,
    FREIGHT, 
    SPAWN
}

public class ModeController : MonoBehaviour
{
    Stack<Mode> modes = new Stack<Mode>();
    public Mode mode;
    float modeTimer = 0;

    // Use this for initialization
    void Start ()
    {
        SetupModeStack();
        mode = modes.Pop();
    }
	

    void SetupModeStack()
    {
        modes.Push(new Mode(nameVar: ModeNames.CHASE));
        modes.Push(new Mode(nameVar: ModeNames.SCATTER, timeVar: 5));
        modes.Push(new Mode(nameVar: ModeNames.CHASE, timeVar: 20));
        modes.Push(new Mode(nameVar: ModeNames.SCATTER, timeVar: 7));
        modes.Push(new Mode(nameVar: ModeNames.CHASE, timeVar: 20));
        modes.Push(new Mode(nameVar: ModeNames.SCATTER, timeVar: 7));
        modes.Push(new Mode(nameVar: ModeNames.CHASE, timeVar: 20));
        modes.Push(new Mode(nameVar: ModeNames.SCATTER, timeVar: 7));
    }

    // Change modes when it is time to do so
    public void ModeUpdate(float dt)
    {
        modeTimer += dt;
        if (mode.time != 0)
        {
            if (modeTimer >= mode.time)
            {
                mode = modes.Pop();
                modeTimer = 0;
                //print(mode.name);
            }
        }
    }

    public void SetFreightMode()
    {
        if(mode.name != ModeNames.FREIGHT && mode.name != ModeNames.SPAWN)
        {
            print("Not in Freight or spawn mode");
            if(mode.time != 0)
            {
                float dt = mode.time - modeTimer;
                modes.Push(new Mode(nameVar: mode.name, timeVar: dt));
            }
            else
            {
                modes.Push(new Mode(nameVar: mode.name));
            }
            mode = new Mode(nameVar: ModeNames.FREIGHT, timeVar: 7, speedMultVar: 0.5f);
            modeTimer = 0;
        }
        else
        {
            print("In freight or spawn mode");
            mode = new Mode(nameVar: ModeNames.FREIGHT, timeVar: 7, speedMultVar: 0.5f);
            modeTimer = 0;
        }
    }

    public void SetRespawnMode()
    {
        mode = new Mode(nameVar: ModeNames.SPAWN, speedMultVar: 2);
        modeTimer = 0;
    }

    public void GetNextMode()
    {
        mode = modes.Pop();
    }

    // If ghosts have to restart, add a scatter mode
    public void AddStartMode()
    {
        mode = new Mode(nameVar: ModeNames.SCATTER, timeVar: 5);
        modeTimer = 0;
    }
}
