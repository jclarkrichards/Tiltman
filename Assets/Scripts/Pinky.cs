using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pinky : MonoBehaviour {
    
    direction dir = direction.NONE;
    Vector3 dirvec = new Vector3();
    Node node;
    Node target;
    Vector3 goal = new Vector3();
    bool overshot_target = true;
    List<direction> validDirections = new List<direction>();
    float speed = 5;
   

    // Use this for initialization
    void Start()
    {
        node = NodeGroup.S.nodelist[6];
        target = NodeGroup.S.nodelist[6];
        transform.position = node.position;
        dir = direction.UP;
    }

    // Update is called once per frame
    void Update()
    {
        dirvec = GetDirectionVector(dir);  //Current direction vector
        Vector3 pos = transform.position;
        pos += dirvec * speed * Time.deltaTime;  //Update position using current direction vector
        transform.position = pos;
        GetComponent<ModeController>().ModeUpdate(Time.deltaTime);
        if (GetComponent<ModeController>().mode.name == ModeNames.CHASE)
        {
            SetChaseGoal();
        }
        else if (GetComponent<ModeController>().mode.name == ModeNames.SCATTER)
        {
            SetScatterGoal();
        }       
      
        if (OvershotTarget())
        {
            node = target;
            if (node.portal)
            {
                node = node.portalNode;
                transform.position = node.position;
            }
            GetValidDirections();
            dir = GetClosestDirection();          
            transform.position = node.position;
           
            if (node.neighbors.ContainsKey(dir))
            {
                target = node.neighbors[dir];
            }
            else
            {
                transform.position = node.position;
                dir = direction.NONE;
            }
        }
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
        foreach (direction key in keys)
        {
            if (GetDirectionVector(key) != dirvec * -1)
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
        for (int i = 0; i < validDirections.Count; i++)
        {
            Vector3 diffVec = node.position + GetDirectionVector(validDirections[i]) - goal;
            distances.Add(diffVec.sqrMagnitude);
        }
        float minVal = distances.Min();
        int index = distances.IndexOf(minVal);
        return validDirections[index];

    }

  
    
    void SetScatterGoal()
    {
        goal = new Vector3();
    }

    void SetChaseGoal()
    {
        goal = AccelerometerTilt.S.transform.position;
    }
}
