using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapNode : MonoBehaviour
{
    public int width = 256;
    public int height = 256;

    public float scale = 1;
    public float heightRequirement;

    public GameObject prefab;
    public List<Vector3> roomPoints = new List<Vector3>();
    public List<GameObject> objects = new List<GameObject>();

    private void Start()
    {
        Refresh();
    }
    Texture2D GenerateNoise()
    {
        Texture2D texture = new Texture2D(width,height);

        //noise
        for(int x = 0; x<width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color col = CalculateColor(x,y);
                texture.SetPixel(x,y,col);
            }
        }
        texture.Apply();
        return texture;

    }
    Color CalculateColor(int x, int y)
    {
        float xCoor = (float)x / (float)width * scale;
        float yCoor = (float)y / (float)height * scale;

        float sample = Mathf.PerlinNoise(xCoor, yCoor);

        if(sample >= heightRequirement)
        {
            Debug.Log("height!");
            roomPoints.Add(new Vector3(x, y));
        }


        return new Color(sample, sample, sample);
    }

    [ContextMenu("RefreshNoise")]
    void Refresh()
    {
        roomPoints.Clear();
        for(int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        var rend = GetComponent<Renderer>();
        rend.material.mainTexture = GenerateNoise();
        foreach (Vector3 room in roomPoints)
        {
            var spawn = Instantiate(prefab);
            spawn.transform.position = room;
            objects.Add(spawn);
        }
    }
}
