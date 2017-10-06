using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum direction
{
    RIGHT,
    LEFT,
    UP,
    DOWN,
    NONE
}

public class DirectionController : MonoBehaviour
{
    public direction current_direction = direction.NONE;
    public Vector3 dirvec = new Vector3();
    List<direction> validDirections = new List<direction>();

    // Use this for initialization
    void Start ()
    {
        current_direction = direction.UP;
    }
	
    // Converts the enum type to a vector
    public Vector3 GetDirectionVector(direction D)
    {
        if (D == direction.DOWN) { return new Vector3(0, -1, 0); }
        else if (D == direction.UP) { return new Vector3(0, 1, 0); }
        else if (D == direction.LEFT) { return new Vector3(-1, 0, 0); }
        else if (D == direction.RIGHT) { return new Vector3(1, 0, 0); }
        else { return new Vector3(); }
    }

    public void SetDirectionVector(direction D)
    {
        if (D == direction.DOWN) { dirvec = new Vector3(0, -1, 0); }
        else if (D == direction.UP) { dirvec = new Vector3(0, 1, 0); }
        else if (D == direction.LEFT) { dirvec = new Vector3(-1, 0, 0); }
        else if (D == direction.RIGHT) { dirvec = new Vector3(1, 0, 0); }
        else { dirvec = new Vector3(); }
    }

    // Build a List of valid directions to choose from
    public void GetValidDirections(Node node)
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
        if(validDirections.Count == 0)
        {
            print("ping pong");
            foreach (direction key in keys)
            {
                if (GetDirectionVector(key) == dirvec * -1)
                {
                    validDirections.Add(key);
                    break;
                }

            }
        }

    }

    // Choose a random direction from a List of directions
    public direction RandomDirection(List<direction> directions)
    {
        int index = Random.Range(0, directions.Count);
        return directions[index];
    }

    public Vector3 RandomDirectionFromValidList()
    {
        direction temp = RandomDirection(validDirections);
        return GetDirectionVector(temp);
    }

    // Take the list of valid directions and determine which direction is closest to the goal.
    public void GetClosestDirection(Node node, Vector3 goal)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < validDirections.Count; i++)
        {
            Vector3 diffVec = node.position + GetDirectionVector(validDirections[i]) - goal;
            distances.Add(diffVec.sqrMagnitude);
        }
        float minVal = distances.Min();
        int index = distances.IndexOf(minVal);
        current_direction = validDirections[index];

    }
}
