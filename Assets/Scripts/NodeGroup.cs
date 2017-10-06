using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeGroup : MonoBehaviour
{
    public static NodeGroup S;
    public GameObject nodePrefab;
    public GameObject wallPrefab;
    LevelLayout level = new LevelLayout();
    public List<Node> nodelist = new List<Node>();
    public List<Node> homelist = new List<Node>();
    Vector3 offset = new Vector3();//-13.5f, -16, 0);
    Stack<Node> nodestack = new Stack<Node>();
    public char[,] levelArray;
    public char[,] homeArray;
    List<Node> portalNodes = new List<Node>();
    //public int rows;
    //public int cols;
    Node homeNode;
    

    private void Awake ()
    {
        S = this;
        levelArray = level.levelArray;
        homeArray = level.homeArray;
        //rows = level.rows;
        //cols = level.cols;
        //print(homeArray.GetLength(0) + ", " + homeArray.GetLength(1));
   
        CreateLinkedNodelist(nodelist, levelArray);
        CreateLinkedNodelist(homelist, homeArray);
        MoveHomeNodes();
        DrawMazeBackground(levelArray);
        ShowNodes(nodelist);
        ShowNodes(homelist);
      
        float newSize = 28 / (2f * Camera.main.aspect);
        Camera.main.orthographicSize = newSize;
        Camera.main.transform.position = new Vector3(13.5f, -newSize/1.25f, -10);
    
        List<int> portalList = new List<int>();
        for(int i=0; i<nodelist.Count; i++)
        {
            if(nodelist[i].portal)
            {
                portalList.Add(i);
            }
        }

        if(portalList.Count >= 2)
        {
            nodelist[portalList[0]].AddPortalNeighbor(nodelist[portalList[1]]);
            nodelist[portalList[1]].AddPortalNeighbor(nodelist[portalList[0]]);
        }
    }
	


    // Create a List of Nodes where all of the Nodes are linked together based on these rules
    void CreateLinkedNodelist(List<Node> nlist, char[,] nodearray)
    {
        Node startNode = FindFirstNode(nodearray);
        nodestack.Push(startNode);
        
        while(nodestack.Count > 0)
        {           
            Node n = nodestack.Pop();
            AddNode(n, nlist);
            Node leftNode = GetPathNode(direction.LEFT, n, nlist, nodearray);
            Node rightNode = GetPathNode(direction.RIGHT, n, nlist, nodearray);
            Node upNode = GetPathNode(direction.UP, n, nlist, nodearray);
            Node downNode = GetPathNode(direction.DOWN, n, nlist, nodearray);

            n.AddNeighbor(leftNode, direction.LEFT);
            n.AddNeighbor(rightNode, direction.RIGHT);
            n.AddNeighbor(upNode, direction.UP);
            n.AddNeighbor(downNode, direction.DOWN);
            AddNodeToStack(leftNode, nlist);
            AddNodeToStack(rightNode, nlist);
            AddNodeToStack(upNode, nlist);
            AddNodeToStack(downNode, nlist);          
        }       
    }

    // Looks in the levelArray and finds the first Node which is the first instance of '+'
    Node FindFirstNode(char[,] nodearray)
    {
        int rows = nodearray.GetLength(0);
        int cols = nodearray.GetLength(1);
        for(int row=0; row<rows; row++)
        {
            for(int col=0; col<cols; col++)
            {
                if(nodearray[row, col] == '+' || nodearray[row, col] == 'n' || nodearray[row, col] == 'N')
                {
                    return CreateNode(row, col, offset);
                }
            }
        }
        return null; // No nodes were found, this should never happen
    }

    // Return either a Node or null.  Follow a path in a specific direction and return the node at the end of the path.
    Node GetPathNode(direction d, Node n, List<Node> nlist, char[,] nodearray)
    {
        Node temp = FollowPath(d, n, nodearray);
        return GetNodeFromNode(temp, nlist);
    }

    // Add a Node to the nodelist if it already does not exist in the list
    void AddNode(Node n, List<Node> nlist)
    {
        bool inList = NodeInList(n, nlist);
        if(!inList)
        {
            nlist.Add(n);
        }
    }

    // Add node n to the stack if it isn't null and if it already is not in the nodelist
    void AddNodeToStack(Node n, List<Node> nlist)
    {
        if(n != null)
        {
            //print(n.position + "  "+n.row + "  "+n.col);
            if(!NodeInList(n, nlist))
                nodestack.Push(n);  
        }
    }


    // Checks if Node n is in the nodelist.  Returns true if it is in the list
    bool NodeInList(Node n, List<Node> nlist)
    {
        for(int i=0; i<nlist.Count; i++)
        {
            if(n.position.x == nlist[i].position.x &&
               n.position.y == nlist[i].position.y)
            {
                return true;
            }
        }
        return false;
    }

    // Look for Node n in the nodelist.  If it is there, return that node.  If not then return Node n
    Node GetNodeFromNode(Node n, List<Node> nlist)
    {
        if(n != null)
        {
            for(int i=0; i<nlist.Count; i++)
            {
                if(n.position.x == nlist[i].position.x && 
                   n.position.y == nlist[i].position.y)
                {
                    return nlist[i];
                }           
            }
        }
        return n;
    }

    Node FollowPath(direction d, Node n, char[,] nodearray)
    {
        if (d == direction.LEFT && n.col - 1 >= 0)
        {
            return PathToFollow(d, n.row, n.col - 1, '-', nodearray);
        }
        else if (d == direction.RIGHT && n.col + 1 < level.cols)
        {
            return PathToFollow(d, n.row, n.col + 1, '-', nodearray);
        }

        else if (d == direction.UP && n.row-1 >= 0)
        {
            return PathToFollow(d, n.row-1, n.col, '|', nodearray);
        }
        else if (d == direction.DOWN && n.row+1 < level.rows)
        {
            return PathToFollow(d, n.row+1, n.col, '|', nodearray);
        }
        return null;
    }

    // Follow a path UP, DOWN, LEFT, or RIGHT along '-' and '|' symbols until you reach a '+' symbol
    Node PathToFollow(direction d, int row, int col, char symbol, char[,] nodearray)
    {
        if(nodearray[row, col] == symbol || nodearray[row, col] == '+' || nodearray[row, col] == 'a' || nodearray[row, col] == 'p' || nodearray[row, col] == 'P' || nodearray[row, col] == 'H')
        {
            while(nodearray[row, col] != '+' && nodearray[row, col] != 'a' && nodearray[row, col] != 'n' && nodearray[row, col] != 'N' && nodearray[row, col] != 'H')
            {
                if(d == direction.LEFT) { col -= 1; }
                else if(d == direction.RIGHT) { col += 1; }
                else if(d == direction.UP) { row -= 1; }
                else if(d == direction.DOWN) { row += 1; }              
            }
            if(nodearray[row, col] == 'H')
            {
                homeNode = CreateNode(row, col, offset);
                return homeNode;
            }
            if(nodearray[row, col] == 'a')
            {
                // Make a portal node
                return CreateNode(row, col, offset, true);
            }
            return CreateNode(row, col, offset);
        }
        else
        {
            return null;
        }
    }





    // This just loops through the levelArray and creates nodes without linking them.  
    void CreateUnlinkedNodelist()
    {
        for (int row = 0; row < level.rows; row++)
        {
            for (int col = 0; col < level.cols; col++)
            {
                char val = levelArray[row, col];
                if (val == '+')
                {                  
                    nodelist.Add(CreateNode(row, col, offset));
                }
            }
        }      
    }



    void ShowNodes(List<Node> nlist)
    {
        //This is just for visual purposes to show where the nodes are
        for (int i = 0; i < nlist.Count; i++)
        {
            GameObject temp = Instantiate(nodePrefab) as GameObject;
            temp.transform.position = nlist[i].position;
        }
    }


    Node CreateNode(int row, int col, Vector3 offset, bool portal=false)
    {     
        Vector3 temp = Camera.main.WorldToScreenPoint(new Vector3(col, row, 0) + offset);// / 4.30f;
        Vector3 temp2 = Camera.main.ScreenToWorldPoint(temp);
        return new Node(temp2.x, temp2.y, row, col, p:portal);     
    }

    // A visual representation of the maze.  Doesn't affect gameplay.
    void DrawMazeBackground(char[,] nodearray)
    {
        int rows = nodearray.GetLength(0);
        int cols = nodearray.GetLength(1);
        for(int row=0; row<rows; row++)
        {
            for(int col=0; col<cols; col++)
            {
                if(levelArray[row, col] == 'm' || levelArray[row,col] == 'w')
                {
                    GameObject wall = Instantiate(wallPrefab) as GameObject;
                    wall.transform.position = new Vector3(col, -row, 0);
                }
            }
        }
    }

    
    void MoveHomeNodes()
    {
        print(homeNode.row + ", " + homeNode.col);
        Node nodeA = GetNodeFromNode(homeNode, nodelist);
        Node nodeB = nodeA.neighbors[direction.LEFT];
        Vector3 mid = (nodeA.position + nodeB.position) / 2.0f;
        print(nodeA.position);
        print(nodeB.position);
        print(mid);
        Vector3 vec = new Vector3(homelist[0].position.x, homelist[0].position.y, 0);
        for(int i=0; i<homelist.Count; i++)
        {
            homelist[i].position -= vec;
            homelist[i].position += mid;
        }
        nodeA.neighbors[direction.LEFT] = homelist[0];
        nodeB.neighbors[direction.RIGHT] = homelist[0];
        homelist[0].neighbors[direction.RIGHT] = nodeA;
        homelist[0].neighbors[direction.LEFT] = nodeB;
    }


}
