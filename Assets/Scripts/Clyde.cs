using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : MonoBehaviour
{
    public static Clyde S;
    Node node;
    Node target;
    public Vector3 goal = new Vector3();
    bool overshot_target = true;
    float speed = 5;
    DirectionController directionScript;
    [HideInInspector]
    public ModeController modeScript;
    direction initialDirection = direction.UP;

    private void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start()
    {
        directionScript = GetComponent<DirectionController>();
        modeScript = GetComponent<ModeController>();
        //SetStartingConditions();
        node = NodeGroup.S.GetClydeStart();
        target = NodeGroup.S.GetClydeStart().neighbors[initialDirection];
        transform.position = node.position;
        directionScript.current_direction = initialDirection;
        directionScript.SetDirectionVector(initialDirection);
        directionScript.exitHome = false;
        directionScript.guider.Clear();
        directionScript.guider.Push(direction.LEFT);
        directionScript.guider.Push(direction.UP);
        directionScript.guider.Push(direction.LEFT);
    }

    public void SetStartingConditions()
    {
        node = NodeGroup.S.GetClydeStart();
        target = NodeGroup.S.GetClydeStart().neighbors[initialDirection];
        transform.position = node.position;
        directionScript.current_direction = initialDirection;
        directionScript.SetDirectionVector(initialDirection);
        modeScript.AddStartMode();
        directionScript.exitHome = false;
        directionScript.guider.Clear();
        directionScript.guider.Push(direction.LEFT);
        directionScript.guider.Push(direction.UP);
        directionScript.guider.Push(direction.LEFT);
        directionScript.startGuiding = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Pauser.S.paused)
        {
            if (AccelerometerTilt.S.numPelletsEaten >= 60)
            {
                directionScript.exitHome = true;
            }
            
            //print(modeScript.mode.name);
            //print(directionScript.current_direction);
            //print(directionScript.dirvec);
            //print(" ");
            float dt = Time.deltaTime;
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
                if (directionScript.exitHome)
                {
                    if (target == NodeGroup.S.clydeStart)
                    {
                        directionScript.startGuiding = true;
                    }
                }
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
                        //print("Clyde Made it home");
                        modeScript.GetNextMode();
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
                    print("CLYDE");
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
        goal = new Vector3(0, -31, 0);
    }

    public void SetChaseGoal()
    {
        Vector3 d = AccelerometerTilt.S.transform.position - transform.position;
        float ds = d.sqrMagnitude;
        if(ds <= Mathf.Pow(16*8, 2))
        {
            SetScatterGoal();
        }
        else
        {
            goal = AccelerometerTilt.S.transform.position;
        }
        
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
}
