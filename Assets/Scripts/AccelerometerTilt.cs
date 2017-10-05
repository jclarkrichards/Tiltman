using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum direction
{
    RIGHT,
    LEFT,
    UP,
    DOWN, 
    NONE
}*/

// This is basically the NodeGroup class
public class AccelerometerTilt : MonoBehaviour
{
    //public GameObject[] nodesObjects;
    
    //Node[] nodes; // Contains all of the nodes
    direction dir = direction.NONE;
    Vector3 dirvec = new Vector3();
    Node node;
    Node target;
    direction tiltDirection = direction.NONE;
    bool overshot_target = true;

	// Use this for initialization
	void Start ()
    {
        //print("So many nodes to choose from");
        //print(NodeGroup.S.nodelist.Count);
        node = NodeGroup.S.nodelist[0];
        target = NodeGroup.S.nodelist[0];
        transform.position = node.position;
     
	}
	
	// Update is called once per frame

	void Update ()
    {
        
        tiltDirection = GetTiltDirection();  // Direction the player is indicating
        
        // If we are stopped on a Node
        if(dir == direction.NONE)
        {
            if (node.neighbors.ContainsKey(tiltDirection))
            {
                dir = tiltDirection;
                target = node.neighbors[tiltDirection];
                //print("new target acquired");
                //print(target.position);
            }
        }
        else // We are moving from a node to another node
        {
            Vector3 dircheck = GetDirectionVector(tiltDirection);
            if(dircheck == dirvec * -1)
            {
                dir = tiltDirection;
                Node temp = node;
                node = target;
                target = temp;
                   
            }
        }
        
        
        if(OvershotTarget())
        {
            
            
            node = target;
            if(node.portal)
            {
                //print("This is a portal");
                //print("We portal to node at " + node.portalNode.row + ", " + node.portalNode.col);
                node = node.portalNode;
                transform.position = node.position;
            }
            // Should we continue in this direction or stop?
            // The direction we are tilting takes precedence
            if(node.neighbors.ContainsKey(tiltDirection))
            {
                //print("Tilt direction!");
                transform.position = node.position;
                dir = tiltDirection;
                target = node.neighbors[tiltDirection];
            }
            else // Tilting direction is no good, can we keep moving in current direction?
            {
                if(node.neighbors.ContainsKey(dir))
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
            }
            //transform.position = node.position;
            //dir = direction.NONE;
            
        }


        
        //print(xAxis + "   " + yAxis);
        dirvec = GetDirectionVector(dir);
        Vector3 pos = transform.position;
        pos += dirvec * 2 * Time.deltaTime;
        transform.position = pos;
		
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

    direction GetTiltDirection()
    {
        //For testing the tilt on this computer, comment out when deploying to the tablet
        
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        if (xAxis > 0) { return direction.RIGHT; }
        else if (xAxis < 0) { return direction.LEFT; }
        else if (yAxis > 0) { return direction.UP; }
        else if (yAxis < 0) { return direction.DOWN; }
        else { return direction.NONE; }
        
        //Uncomment when deploying to the tablet.  
        /*
        if(Mathf.Abs(Input.acceleration.x) > Mathf.Abs(Input.acceleration.y))
        {
            if(Input.acceleration.x > 0)
            {
                return direction.RIGHT;
            }
            else
            {               
                return direction.LEFT;
            }
        }
        else
        {         
            if(Input.acceleration.y > 0)

            {              
                return direction.UP;
            }
            else
            {              
                return direction.DOWN;
            }
        }
        */
    }

    void LinkNodes()
    {
        /*
        for(int i=0; i<nodesObjects.Length; i++)
        {
            nodes[i] = new Node(nodesObjects[i].transform.position);
        }
        
        nodes[0].AddNeighbor(nodes[1], direction.RIGHT);
        nodes[0].AddNeighbor(nodes[2], direction.DOWN);

        nodes[1].AddNeighbor(nodes[0], direction.LEFT);
        nodes[1].AddNeighbor(nodes[3], direction.DOWN);

        nodes[2].AddNeighbor(nodes[0], direction.UP);
        nodes[2].AddNeighbor(nodes[3], direction.RIGHT);
        nodes[2].AddNeighbor(nodes[5], direction.DOWN);

        nodes[3].AddNeighbor(nodes[1], direction.UP);
        nodes[3].AddNeighbor(nodes[2], direction.LEFT);
        nodes[3].AddNeighbor(nodes[4], direction.RIGHT);

        nodes[4].AddNeighbor(nodes[3], direction.LEFT);
        nodes[4].AddNeighbor(nodes[6], direction.DOWN);

        nodes[5].AddNeighbor(nodes[2], direction.UP);
        nodes[5].AddNeighbor(nodes[6], direction.RIGHT);

        nodes[6].AddNeighbor(nodes[4], direction.UP);
        nodes[6].AddNeighbor(nodes[5], direction.LEFT);
        */
    }
}
