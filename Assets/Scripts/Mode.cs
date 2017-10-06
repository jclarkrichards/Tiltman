using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode
{
    public ModeNames name;
    public float time;
    public float speedMult;

    public Mode(ModeNames nameVar, float timeVar=0, float speedMultVar=1)
    {
        name = nameVar;
        time = timeVar;
        speedMult = speedMultVar;
    }
	
}
