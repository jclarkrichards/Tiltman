using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ghost : MonoBehaviour
{
    direction dir = direction.NONE;
    Vector3 dirvec = new Vector3();
    Node node;
    Node target;
    Vector3 goal = new Vector3();
    bool overshot_target = true;
    List<direction> validDirections = new List<direction>();
    float speed = 5;
    Stack<Mode> modes = new Stack<Mode>();
    Mode mode;
    float modeTimer = 0;

    // Use this for initialization
    void Start ()
    {
        SetupModeStack();
        mode = modes.Pop();
        node = NodeGroup.S.nodelist[6];
        target = NodeGroup.S.nodelist[6];
        transform.position = node.position;
        dir = direction.UP;
    }
	
	// Update is called once per frame
	void Update ()
    {
        dirvec = GetDirectionVector(dir);
        Vector3 pos = transform.position;
        pos += dirvec * speed * Time.deltaTime;
        transform.position = pos;
        ModeUpdate(Time.deltaTime);
        if (mode.name == "CHASE")
        {
            SetChaseGoal();
        }
        else if (mode.name == "SCATTER")
        {
            SetScatterGoal();
        }
        //tiltDirection = GetTiltDirection();  // Direction the player is indicating

        // If we are stopped on a Node
        if (dir == direction.NONE)
        {
            //if (node.neighbors.ContainsKey(tiltDirection))
            //{
            //    dir = tiltDirection;
            //    target = node.neighbors[tiltDirection];
                //print("new target acquired");
                //print(target.position);
            //}
        }
        else // We are moving from a node to another node
        {
            //Vector3 dircheck = GetDirectionVector(tiltDirection);
            //if (dircheck == dirvec * -1)
            //{
            //    dir = tiltDirection;
            //    Node temp = node;
            //    node = target;
            //    target = temp;

            //}
        }


        if (OvershotTarget())
        {


            node = target;
            if (node.portal)
            {
                //print("This is a portal");
                //print("We portal to node at " + node.portalNode.row + ", " + node.portalNode.col);
                node = node.portalNode;
                transform.position = node.position;
            }
            GetValidDirections();
            dir = GetClosestDirection();
            //dir = RandomDirection(validDirections);
            transform.position = node.position;
            //target = node.neighbors[dir];
            // Should we continue in this direction or stop?
            // The direction we are tilting takes precedence
            //if (node.neighbors.ContainsKey(tiltDirection))
            //{
                //print("Tilt direction!");
                //transform.position = node.position;
                //dir = tiltDirection;
                //target = node.neighbors[tiltDirection];
            //}
            //else // Tilting direction is no good, can we keep moving in current direction?
            //{
                if (node.neighbors.ContainsKey(dir))
                {
                    //print("Keep going!");
                    target = node.neighbors[dir];
                }
                else
                {
                    //print("STOP!");
                    transform.position = node.position;
                    dir = direction.NONE;
                }
            //}
            //transform.position = node.position;
            //dir = direction.NONE;

        }



        //print(xAxis + "   " + yAxis);
        
    }

    Vector3 GetDirectionVector(direction D)
    {
        if (D == direction.DOWN) { return new Vector3(0, -1, 0); }
        else if (D == direction.UP) { return new Vector3(0, 1, 0); }
        else if (D == direction.LEFT) { return new Vector3(-1, 0, 0); }
        else if (D == direction.RIGHT) { return new Vector3(1, 0, 0); }
        else { return new Vector3(); }
    }

    bool OvershotTarget()
    {
        Vector3 vec1 = target.position - node.position;
        Vector3 vec2 = transform.position - node.position;
        float node2Target = vec1.sqrMagnitude;
        float node2Self = vec2.sqrMagnitude;
        //print(target.position + "  " + node.position+ "  " + node2Target + "  " + node2Self);
        return node2Self > node2Target;
    }

    // Build a List of valid directions to choose from
    void GetValidDirections()
    {
        validDirections.Clear();
        Dictionary<direction, Node>.KeyCollection keys = node.neighbors.Keys;
        foreach(direction key in keys)
        {
            if(GetDirectionVector(key) != dirvec * -1)
            {
                validDirections.Add(key);
            }
            
        }
       
    }

    // Choose a random direction for a List of directions
    direction RandomDirection(List<direction> directions)
    {
        int index = Random.Range(0, directions.Count);
        return directions[index];
    }

    // Take the list of valid directions and determine which direction is closest to the goal.
    direction GetClosestDirection()
    {
        List<float> distances = new List<float>();
        for(int i=0; i<validDirections.Count; i++)
        {
            Vector3 diffVec = node.position + GetDirectionVector(validDirections[i]) - goal;
            distances.Add(diffVec.sqrMagnitude);
        }
        float minVal = distances.Min();
        int index = distances.IndexOf(minVal);
        return validDirections[index];
            
    }

    void SetupModeStack()
    {       
        modes.Push(new Mode(name = "CHASE"));
        modes.Push(new Mode(name = "SCATTER", timeVar: 5));
        modes.Push(new Mode(name = "CHASE", timeVar: 20));
        modes.Push(new Mode(name = "SCATTER", timeVar: 7));
        modes.Push(new Mode(name = "CHASE", timeVar: 20));
        modes.Push(new Mode(name = "SCATTER", timeVar: 7));
        modes.Push(new Mode(name = "CHASE", timeVar: 20));
        modes.Push(new Mode(name = "SCATTER", timeVar: 7));
    }

    // Change modes when it is time to do so
    void ModeUpdate(float dt)
    {
        modeTimer += dt;
        if(mode.time != 0)
        {
            if(modeTimer >= mode.time)
            {
                mode = modes.Pop();
                modeTimer = 0;
                print(mode.name);
            }
        }
    }

    void SetScatterGoal()
    {
        goal = new Vector3();
    }

    public void SetChaseGoal()
    {
        goal = AccelerometerTilt.S.transform.position;
    }
}
