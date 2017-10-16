using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Blinky : MonoBehaviour
{
    public static Blinky S;
    Node node;
    Node target;
    public Vector3 goal = new Vector3();
    bool overshot_target = true;   
    float speed = 5;
    DirectionController directionScript;
    [HideInInspector]
    public ModeController modeScript;

    private void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start()
    {
        directionScript = GetComponent<DirectionController>();
        modeScript = GetComponent<ModeController>();
        //node = NodeGroup.S.nodelist[6];
        //target = NodeGroup.S.nodelist[6];
        node = NodeGroup.S.GetBlinkyStart();
        target = NodeGroup.S.GetBlinkyStart();
        transform.position = node.position;
        //print("NEW BLINKY START: " + transform.position);
        directionScript.current_direction = direction.LEFT;
        directionScript.SetDirectionVector(direction.LEFT);
        //print("Start direction: " + directionScript.dirvec);
    }

    public void SetStartingConditions()
    {
        node = NodeGroup.S.GetBlinkyStart();
        target = NodeGroup.S.GetBlinkyStart();
        transform.position = node.position;      
        directionScript.current_direction = direction.LEFT;
        directionScript.SetDirectionVector(direction.LEFT);
        modeScript.AddStartMode();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Pauser.S.paused)
        {
            float dt = Time.deltaTime;
            ReverseDirection();
            directionScript.SetDirectionVector(directionScript.current_direction);
            Vector3 pos = transform.position;
            float speedMod = ModifySpeed();
            pos += directionScript.dirvec * speedMod * dt;
            transform.position = pos;

            modeScript.ModeUpdate(dt);
            if (modeScript.mode.name == ModeNames.CHASE)
            {
                SetChaseGoal();
            }
            else if (modeScript.mode.name == ModeNames.SCATTER)
            {
                SetScatterGoal();
            }
            else if (modeScript.mode.name == ModeNames.FREIGHT)
            {
                SetRandomGoal();
            }
            else if (modeScript.mode.name == ModeNames.SPAWN)
            {
                SetSpawnGoal();
            }


            if (OvershotTarget())
            {
                node = target;
                if (node.portal)
                {
                    node = node.portalNode;
                    transform.position = node.position;
                }
                transform.position = node.position;
                if (modeScript.mode.name == ModeNames.SPAWN)
                {
                    if (transform.position == goal)
                    {
                        //print("Blinky Made it home");
                        modeScript.GetNextMode();
                        //print("MODE: " + modeScript.mode.name);
                        directionScript.guider.Clear();
                        directionScript.guider.Push(direction.LEFT);
                        directionScript.guider.Push(direction.UP);
                        directionScript.startGuiding = true;
                        directionScript.exitHome = true;
                    }
                }
                directionScript.GetValidDirections(node, modeScript);
                directionScript.GetClosestDirection(node, goal);
                

                if (node.neighbors.ContainsKey(directionScript.current_direction))
                {
                    target = node.neighbors[directionScript.current_direction];
                }
                else
                {
                    transform.position = node.position;
                    directionScript.current_direction = direction.NONE;
                }
                /*
                if (directionScript.guider.Count > 0 && directionScript.exitHome && directionScript.startGuiding)
                {
                    print("BLINKY");
                    print(directionScript.current_direction);
                    Pauser.S.paused = true;
                }
                */
            }
        }
        
    }
    
    bool OvershotTarget()
    {
        Vector3 vec1 = target.position - node.position;
        Vector3 vec2 = transform.position - node.position;
        float node2Target = vec1.sqrMagnitude;
        float node2Self = vec2.sqrMagnitude;
        return node2Self > node2Target;
    }
    
    
    public void SetScatterGoal()
    {
        goal = new Vector3(24, 4, 0);
    }

    public void SetChaseGoal()
    {
        goal = AccelerometerTilt.S.transform.position;
    }

    public void SetRandomGoal()
    {
        goal = directionScript.RandomDirectionFromValidList();
    }

    public void SetSpawnGoal()
    {
        //goal = new Vector3(13.5f, -14, 0);
        goal = NodeGroup.S.spawn.position;
    }

    float ModifySpeed()
    {
        if(node.portal || target.portal)
        {
            return speed / 2.0f;
        }
        return speed * modeScript.mode.speedMult;
    }

    public void ReverseDirection()
    {
        if (modeScript.reverse)
        {
            directionScript.ReverseDirection();
            Node temp = node;
            node = target;
            target = temp;
            modeScript.reverse = false;
        }
    }

    public void SetFreightMode()
    {      
        print("GUIDING: " + directionScript.startGuiding);
        print("FREIGHT or SPAWN: " + modeScript.FreightOrSpawnMode());
        if (!directionScript.startGuiding && !modeScript.FreightOrSpawnMode())
        {
            print("REVERSE BLINKY");
            modeScript.reverse = true;
        }
        modeScript.SetFreightMode();
    }
}
