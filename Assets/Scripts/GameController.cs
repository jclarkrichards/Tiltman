using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController S;
    public GameObject FruitPrefab;
    public GameObject LifeIconPrefab;
    [HideInInspector]
    public GameObject fruit;
    List<GameObject> lifeicons;
    public List<GameObject> fruitCollection;

    private void Awake()
    {
        S = this;
        float newSize = 28 / (2f * Camera.main.aspect);
        Camera.main.orthographicSize = newSize;
        Camera.main.transform.position = new Vector3(13.5f, -newSize / 1.25f, -10);
        fruitCollection = new List<GameObject>();
    }

    // Use this for initialization
    void Start ()
    {
        //print("Pacman lives = " +AccelerometerTilt.S.lives);
        CreateLifeIcons();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ((AccelerometerTilt.S.numPelletsEaten == 10 || AccelerometerTilt.S.numPelletsEaten == 40 || AccelerometerTilt.S.numPelletsEaten == 70) && fruit == null)
        {
            //print("Create the fruit");
            CreateFruit();
        }
    }

    void CreateFruit()
    {
        fruit = Instantiate(FruitPrefab) as GameObject;
        Node node1 = NodeGroup.S.GetFruitStart();
        Node node2 = node1.neighbors[direction.LEFT];
        Vector3 middle = (node1.position - node2.position) / 2;
        fruit.transform.position = node1.position - middle;
    }

    //Draw the life icons in the lower left corner
    void CreateLifeIcons()
    {
        lifeicons = new List<GameObject>();
        float row = 32;
        float col = 0.5f;
        for (int i = 0; i < AccelerometerTilt.S.lives; i++)
        {
            GameObject icon = Instantiate(LifeIconPrefab) as GameObject;           
            icon.transform.position = new Vector3(col, -row, 0);
            lifeicons.Add(icon);
            col += 2;
            
        }
    }

    //Removes a life icon from back to front.  
    public void RemoveLifeIcon()
    {
        GameObject removedIcon = lifeicons[lifeicons.Count-1];
        bool removed = lifeicons.Remove(removedIcon);
        if(removed)
        {
            Destroy(removedIcon);
        }
    }

    // When eating fruit, display it in the bottom right corner
    public void CollectFruit()
    {
        GameObject temp = Instantiate(FruitPrefab) as GameObject;
        temp.GetComponent<Fruit>().ingame = false;
        bool inCollection = false;
        for(int i=0; i<fruitCollection.Count; i++)
        {
            if(fruitCollection[i].name == temp.name)
            {
                //inCollection = true;
                break;
            }
        }
        if(!inCollection)
        {
            float num = fruitCollection.Count;
            temp.transform.position = new Vector3(26.5f-num*2, -32, 0);
            fruitCollection.Add(temp);
        }
    }
}
