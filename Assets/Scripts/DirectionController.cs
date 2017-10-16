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
    [HideInInspector]
    public bool exitHome;
    public bool startGuiding = false;
    public Stack<direction> guider = new Stack<direction>();

    // Use this for initialization
    void Awake ()
    {
        current_direction = direction.UP;
        exitHome = true;     
        //print("num elements in guider: " + guider.Count);
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
    public void GetValidDirections(Node node, ModeController mode)
    {
        validDirections.Clear();
        if (guider.Count > 0 && exitHome && startGuiding)
        {
            //print("NEXT: " + guider.Peek());
            validDirections.Add(guider.Pop());
            //Pauser.S.paused = true;
            if(guider.Count == 0)
            {
                startGuiding = false;
            }
        }
        else
        {
            Dictionary<direction, Node>.KeyCollection keys = node.neighbors.Keys;
            foreach (direction key in keys)
            {
                if (!(node.homegate && key == direction.DOWN && mode.mode.name != ModeNames.SPAWN))
                {
                    if (GetDirectionVector(key) != dirvec * -1)
                    {
                        if (exitHome)
                        {
                            if(!(node.restrictUP && key == direction.UP))
                            {
                                //print("DO NOT MOVE UP!");
                                validDirections.Add(key);
                            }
                            
                            
                        }
                        else
                        {
                            if (key != direction.LEFT && key != direction.RIGHT)
                            {
                                validDirections.Add(key);
                            }
                        }

                    }
                }
            }

            if (validDirections.Count == 0)
            {
                //print("ping pong");
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
        if (validDirections.Count > 1)
        {
            for (int i = 0; i < validDirections.Count; i++)
            {
                Vector3 diffVec = node.position + GetDirectionVector(validDirections[i]) - goal;
                distances.Add(diffVec.sqrMagnitude);
            }
            float minVal = distances.Min();
            int index = distances.IndexOf(minVal);
            current_direction = validDirections[index];
        }
        else
        {
            current_direction = validDirections[0];
        }

    }

    public void ReverseDirection()
    {
        if(current_direction == direction.UP) { current_direction = direction.DOWN; }
        else if(current_direction == direction.DOWN) { current_direction = direction.UP; }
        else if(current_direction == direction.LEFT) { current_direction = direction.RIGHT; }   
        else if(current_direction == direction.RIGHT) { current_direction = direction.LEFT; }
    }
}
