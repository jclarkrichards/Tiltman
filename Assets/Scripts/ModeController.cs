using System.Collections;
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
    public bool reverse;

    // Use this for initialization
    void Start ()
    {
        SetupModeStack();
        mode = modes.Pop();
        reverse = false;
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
                GetNextMode();
                //modeTimer = 0;
                //print(mode.name);
            }
        }
    }

    public void SetFreightMode()
    {
        if(mode.name != ModeNames.FREIGHT && mode.name != ModeNames.SPAWN)
        {
            //print("Not in Freight or spawn mode");
            if(mode.time != 0)
            {
                float dt = mode.time - modeTimer;
                modes.Push(new Mode(nameVar: mode.name, timeVar: dt));
            }
            else
            {
                modes.Push(new Mode(nameVar: mode.name));
            }
            //reverse = true;
            mode = new Mode(nameVar: ModeNames.FREIGHT, timeVar: 7, speedMultVar: 0.5f);
            modeTimer = 0;
        }
        else if(mode.name == ModeNames.FREIGHT)
        {
            //print("In freight");
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
        if(mode.name == ModeNames.CHASE || mode.name == ModeNames.SCATTER)
        {
            reverse = true;
        }
        mode = modes.Pop();
        modeTimer = 0;
        
    }

    // If ghosts have to restart, add a scatter mode
    public void AddStartMode()
    {
        mode = new Mode(nameVar: ModeNames.SCATTER, timeVar: 5);
        modeTimer = 0;
    }

    // Return true if ghost is in either freight or spawn mode.  This is just a convenience function
    public bool FreightOrSpawnMode()
    {
        return mode.name == ModeNames.FREIGHT || mode.name == ModeNames.SPAWN;
    }
}
