//using CatchCo;
using System.Collections.Generic;
using UnityEngine;

public class CreateCityGrid : MonoBehaviour
{
    [SerializeField] GameObject cityBlockPrefab = null;
    [SerializeField] private float blockScale = 1f;
    [SerializeField] private float gridRows = 1f;
    [SerializeField] private float gridColumns = 1f;
    // [SerializeField] private bool randomRotation = false; //optional for now
    private Vector3 blockOffset = new Vector3(0.5f, 0, 0.5f);
    private GameObject tempGameObject = null;

    //[ExposeMethodInEditor]//makes button to run in editor
    public void MakeGrid()
    {
        //clearing current grid\
        List<Transform> children = new List<Transform>();
        foreach (Transform child in gameObject.transform)
        {
            children.Add(child);
        }
        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        //making new grid
        for (int i = 0; i < gridRows; i++)
        {
            for (int j = 0; j < gridColumns; j++)
            {
                tempGameObject = Instantiate(cityBlockPrefab, gameObject.transform.position + (blockOffset * blockScale) + new Vector3(j * blockScale, 0, i * blockScale), Quaternion.Euler(90, 0, 0));
                tempGameObject.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
                tempGameObject.transform.parent = gameObject.transform;
            }
        }
    }
}

