using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletGroup : MonoBehaviour
{
    public static PelletGroup S;
    public GameObject pelletPrefab;
    public GameObject powerpelletPrefab;
    [HideInInspector]
    public List<GameObject> pelletList = new List<GameObject>();
    //public List<Pellet> pelletTest = new List<Pellet>();
    public char[,] levelArray;

    private void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start ()
    {
        levelArray = NodeGroup.S.levelArray;
        CreatePelletList();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void CreatePelletList()
    {
        pelletList.Clear();
        int rows = NodeGroup.S.levelArray.GetLength(0);
        int cols = NodeGroup.S.levelArray.GetLength(1);
        //print("ROWS AND COLS");
        //print(NodeGroup.S.rows + ", " + NodeGroup.S.cols);
        for(int row=0; row<rows; row++)
        {
            for(int col=0; col<cols; col++)
            {
                if(levelArray[row, col] == 'p' || levelArray[row, col] == 'n' || levelArray[row, col] == 'T')
                {
                    GameObject pellet = Instantiate(pelletPrefab) as GameObject;
                    Vector3 temp = Camera.main.WorldToScreenPoint(new Vector3(col, row, 0));
                    Vector3 temp2 = Camera.main.ScreenToWorldPoint(temp);
                    pellet.transform.position = new Vector3(temp2.x, -temp2.y, 0);
                    pellet.GetComponent<Pellet>().points = 10;
                    pelletList.Add(pellet);
                }

                if (levelArray[row, col] == 'P' || levelArray[row, col] == 'N')
                {
                    GameObject pellet = Instantiate(powerpelletPrefab) as GameObject;
                    Vector3 temp = Camera.main.WorldToScreenPoint(new Vector3(col, row, 0));
                    Vector3 temp2 = Camera.main.ScreenToWorldPoint(temp);
                    pellet.transform.position = new Vector3(temp2.x, -temp2.y, 0);
                    pellet.GetComponent<Pellet>().points = 50;
                    pelletList.Add(pellet);
                }
            }
        }
    }
}
