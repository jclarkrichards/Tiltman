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
        node = NodeGroup.S.nodelist[6];
        target = NodeGroup.S.nodelist[6];
        transform.position = node.position;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        directionScript.SetDirectionVector(directionScript.current_direction);
        Vector3 pos = transform.position;
        pos += directionScript.dirvec * speed * modeScript.mode.speedMult * dt;
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
        else if(modeScript.mode.name == ModeNames.FREIGHT)
        {
            SetRandomGoal();
        }
        

        if (OvershotTarget())
        {
            node = target;
            if (node.portal)
            {             
                node = node.portalNode;
                transform.position = node.position;
            }
            directionScript.GetValidDirections(node);
            directionScript.GetClosestDirection(node, goal);       
            transform.position = node.position;
            
            if (node.neighbors.ContainsKey(directionScript.current_direction))
            {         
                target = node.neighbors[directionScript.current_direction];
            }
            else
            {
                transform.position = node.position;
                directionScript.current_direction = direction.NONE;
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
}
