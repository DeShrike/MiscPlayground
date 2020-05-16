using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GroundBuilder : MonoBehaviour
{
    public int MinX = -10;
    public int MinY = -10;
    public int MaxX = 10;
    public int MaxY = 10;
    public GameObject[] Grasses;
    public int[] Weights;

    public GameObject[] TreeTops;
    public GameObject[] TreeBottoms;

    private const float Multiplier = 2f;

    // Use this for initialization
    private void Start ()
    {
        this.BuildGround();
        this.AddTrees();
    }

    // Update is called once per frame
    private void Update ()
    {
	
	}

    private void AddTrees()
    {
        int count = (this.MaxX - this.MinX) * (this.MaxY - this.MinY) / 100;
        for (int i = 0;i<count;i++)
        {
            this.AddTree();
        }
    }

    private void AddTree()
    {
        int posx = UnityEngine.Random.Range(this.MinX + 1, this.MaxX - 2);
        int posy = UnityEngine.Random.Range(this.MinY + 1, this.MaxY - 1);
        int ix = UnityEngine.Random.Range(0, this.TreeTops.Length);

        var t = Instantiate<GameObject>(this.TreeTops[ix]);
        t.transform.position = new Vector3(posx * Multiplier, posy * Multiplier, 0);
        t.transform.parent = transform;

        var t2 = Instantiate<GameObject>(this.TreeBottoms[ix]);
        t2.transform.position = new Vector3(posx * Multiplier, (posy-1) * Multiplier, 0);
        t2.transform.parent = transform;
    }

    private void BuildGround()
    {
        for (int x=this.MinX; x<=this.MaxX;x++)
        {
            for (int y = this.MinY; y <= this.MaxY; y++)
            {
                var t = Instantiate<GameObject>(this.Grasses.Random(this.Weights));
                t.transform.position = new Vector3(x*Multiplier, y* Multiplier, 0);
                t.transform.parent = transform;
            }
        }
    }
}

public static class Extensions
{
    public static T Random<T>(this T[] enumerable, int[] weights)
    {
        int totalWeight = 0; // this stores sum of weights of all elements before current
        T selected = default(T); // currently selected element
        for (int i=0;i<enumerable.Length;i++)
        {
            T data = enumerable[i];
            int weight = weights[i]; // weight of current element
            int r = (int) UnityEngine.Random.Range(0f, (float)(totalWeight + weight)); // random value
            // int r = UnityEngine.Random.Next(totalWeight + weight); // random value
            if (r >= totalWeight) // probability of this is weight/(totalWeight+weight)
            {
                selected = data; // it is the probability of discarding last selected element and selecting current one instead
            }

            totalWeight += weight; // increase weight sum
        }

        return selected; // when iterations end, selected is some element of sequence. 
    }
}