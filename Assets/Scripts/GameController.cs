using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController S;
    public GameObject FruitPrefab;
    [HideInInspector]
    public GameObject fruit;

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
}
