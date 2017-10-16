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

    private void Awake()
    {
        S = this;
        float newSize = 28 / (2f * Camera.main.aspect);
        Camera.main.orthographicSize = newSize;
        Camera.main.transform.position = new Vector3(13.5f, -newSize / 1.25f, -10);
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
        if (AccelerometerTilt.S.numPelletsEaten == 10 && fruit == null)
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
}
