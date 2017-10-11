using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is basically the Pacman class
public class AccelerometerTilt : MonoBehaviour
{
    public static AccelerometerTilt S;
    //public GameObject[] nodesObjects;
    
    //Node[] nodes; // Contains all of the nodes
    public direction dir;
    Vector3 dirvec;
    Node node;
    Node target;
    direction tiltDirection;
    bool overshot_target = true;
    float speed = 5;
    float score = 0;
    float ghostScore = 200;
    public int numPelletsEaten = 0;

    private void Awake()
    {
        S = this;
        dir = direction.LEFT;
        dirvec = GetDirectionVector(dir);
        tiltDirection = dir;
        //print("AWAKE: " + dir);
    }

    // Use this for initialization
    void Start ()
    {   
        //print("DIRECTION = " + dir);
        //print("So many nodes to choose from");
        //print(NodeGroup.S.nodelist.Count);
        node = NodeGroup.S.GetTiltmanStart();
        //node = NodeGroup.S.tiltmanStart;
        target = NodeGroup.S.GetTiltmanStart().neighbors[direction.LEFT];
        //print(target.neighbors[direction.LEFT].position);
        //node = NodeGroup.S.nodelist[0];
        //target = NodeGroup.S.nodelist[0];
        Vector3 halfway = (node.position - target.position) / 2;
        transform.position = node.position - halfway;
        //dir = direction.LEFT;
	}

    void SetStartingConditions()
    {
        dir = direction.LEFT;
        dirvec = GetDirectionVector(dir);
        tiltDirection = dir;
        node = NodeGroup.S.GetTiltmanStart();
        target = NodeGroup.S.GetTiltmanStart().neighbors[direction.LEFT];     
        Vector3 halfway = (node.position - target.position) / 2;
        transform.position = node.position - halfway;
        //Pauser.S.paused = true;
        print("STARTING POSITIONS");
    }
	
	// Update is called once per frame

	void Update ()
    {
        if(!Pauser.S.paused)
        {
            //print("UPDATING");
            //dirvec = GetDirectionVector(dir);
            //Vector3 pos = transform.position;
            //pos += dirvec * speed * Time.deltaTime;
            //transform.position = pos;

            EatPellets();
            EatGhosts();
            EatFruit();

            tiltDirection = GetTiltDirection();  // Direction the player is indicating
                                                 // If we are stopped on a Node
            if (dir == direction.NONE)
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
                if (dircheck == dirvec * -1)
                {
                    dir = tiltDirection;
                    Node temp = node;
                    node = target;
                    target = temp;

                }
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
                // Should we continue in this direction or stop?
                // The direction we are tilting takes precedence
                //print("HOME: " + node.homegate);
                if (node.neighbors.ContainsKey(tiltDirection) && !node.homegate)
                {
                   
                    //print("Tilt direction!");
                    transform.position = node.position;
                    dir = tiltDirection;
                    target = node.neighbors[tiltDirection];
                   
                    
                }
                else // Tilting direction is no good, can we keep moving in current direction?
                {
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
                }
                //transform.position = node.position;
                //dir = direction.NONE;

            }
            //print(Pauser.S.paused);
            if(!Pauser.S.paused)
            {
                dirvec = GetDirectionVector(dir);
                Vector3 pos = transform.position;
                pos += dirvec * speed * Time.deltaTime;
                transform.position = pos;
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


    // Deal with eating pellets.  If a power pellet was eaten, then put ghosts in FREIGHT mode
    void EatPellets()
    {      
        for(int p=0; p<PelletGroup.S.pelletList.Count; p++)
        {
            GameObject pellet = PelletGroup.S.pelletList[p];
            Vector3 d = transform.position - pellet.transform.position;
            float radius = pellet.GetComponent<SphereCollider>().radius;
            float dSquared = d.sqrMagnitude;
            float rSquared = Mathf.Pow((radius + radius), 2);
            if(dSquared <= rSquared)
            {
                numPelletsEaten += 1;
                score += pellet.GetComponent<Pellet>().points;
                if(pellet.GetComponent<Pellet>().points == 50)
                {
                    //Power pellet so put ghost in FREIGHT mode
                    ghostScore = 200;
                    Blinky.S.modeScript.SetFreightMode();
                    Pinky.S.modeScript.SetFreightMode();
                }
                //print(score);
                bool test = PelletGroup.S.pelletList.Remove(pellet);
                //print(test);
                //print(PelletGroup.S.pelletList.Count);               
                Destroy(pellet);    
                if(PelletGroup.S.pelletList.Count == 0)
                {
                    print("All pellets gone.  Restart the level or onto next level");
                }
                break;
            }
        }
    }

    // Check to see if Pacman is colliding with any of the ghosts
    void EatGhosts()
    {
        if(EatGhost(Blinky.S.gameObject))
        {
            if(Blinky.S.modeScript.mode.name == ModeNames.FREIGHT)
            {
                Blinky.S.modeScript.SetRespawnMode();
                score += ghostScore;
                ghostScore *= 2;
                //print("Eat Blinky");
            }
            else
            {
                if(Blinky.S.modeScript.mode.name != ModeNames.SPAWN)
                {
                    print("Death by Blinky");
                    RestartLevel();
                }
                
            }
        }

        if(EatGhost(Pinky.S.gameObject))
        {
            if (Pinky.S.modeScript.mode.name == ModeNames.FREIGHT)
            {
                Pinky.S.modeScript.SetRespawnMode();
                score += ghostScore;
                ghostScore *= 2;
                //print("Eat Pinky");
            }
            else
            {
                if(Pinky.S.modeScript.mode.name != ModeNames.SPAWN)
                {
                    print("Death by Pinky");
                    RestartLevel();
                }
                
            }
        }
    }

    bool EatGhost(GameObject ghost)
    {
        Vector3 d = transform.position - ghost.transform.position;
        float dSquared = d.sqrMagnitude;
        float rSquared = Mathf.Pow((GetComponent<SphereCollider>().radius + ghost.GetComponent<SphereCollider>().radius), 2);
        if (dSquared <= rSquared)
        {
            return true;
        }
        return false;
    }

    void EatFruit()
    {
        if(GameController.S.fruit != null)
        {
            Vector3 d = transform.position - GameController.S.fruit.transform.position;
            float dSquared = d.sqrMagnitude;
            float rSquared = Mathf.Pow((GetComponent<SphereCollider>().radius + GameController.S.fruit.GetComponent<SphereCollider>().radius), 2);
            if(dSquared <= rSquared)
            {
                print("Eating the fruit");
                score += GameController.S.fruit.GetComponent<Fruit>().points;
                Destroy(GameController.S.fruit.gameObject);
            }
        }      
    }

    void RestartLevel()
    {
        SetStartingConditions();
        Blinky.S.SetStartingConditions();
        Pinky.S.SetStartingConditions();
        Pauser.S.paused = true;
    }
}
