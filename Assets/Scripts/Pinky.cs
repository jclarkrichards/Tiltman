using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pinky : MonoBehaviour {

    public static Pinky S;
    Node node;
    Node target;
    public Vector3 goal = new Vector3();
    bool overshot_target = true;
    float speed = 5;
    [HideInInspector]
    public DirectionController directionScript;
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
        node = NodeGroup.S.GetPinkyStart();
        target = NodeGroup.S.GetPinkyStart().neighbors[direction.UP];
        transform.position = node.position;
        directionScript.current_direction = direction.UP;
        directionScript.SetDirectionVector(direction.UP);
    }

    public void SetStartingConditions()
    {
        node = NodeGroup.S.GetPinkyStart();
        target = NodeGroup.S.GetPinkyStart().neighbors[direction.UP];
        transform.position = node.position;
        directionScript.current_direction = direction.UP;
        directionScript.SetDirectionVector(direction.UP);
        modeScript.AddStartMode();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(!Pauser.S.paused)
        {
            //print(modeScript.mode.name);
            //print(directionScript.current_direction);
            //print(directionScript.dirvec);
            //print(" ");
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
                        //print("Pinky Made it home");
                        modeScript.GetNextMode();
                        directionScript.guider.Clear();
                        directionScript.guider.Push(direction.LEFT);
                        directionScript.guider.Push(direction.UP);
                        directionScript.startGuiding = true;
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
                    print("PINKY");
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
        goal = new Vector3(4,4,0);
    }

    public void SetChaseGoal()
    {
        goal = AccelerometerTilt.S.transform.position + AccelerometerTilt.S.dirvec * 16 * 4;
        
        //goal = AccelerometerTilt.S.transform.position;
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
        if (node.portal || target.portal)
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
}
