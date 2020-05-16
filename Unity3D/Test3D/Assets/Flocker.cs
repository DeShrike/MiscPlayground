using UnityEngine;
using System.Collections.Generic;

public class Flocker : MonoBehaviour {

    public GameObject FishPrefab;
    public int FishCount;
    public GameObject Container;

    public float Strength1 = 0.1f;
    public float Strength2 = 0.1f;
    public float Strength3 = 0.1f;

    private GameObject[] Fishes;
    private GameObject SelectedFish = null;
    private Camera Camera;
    private Vector3[] Velocities;

	void Start ()
    {
        this.Camera = Camera.main;
        var fishes = new List<GameObject>();
        var vels = new List<Vector3>();

        for (int i = 0; i < this.FishCount ; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-5,5), Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject go = (GameObject)Instantiate(this.FishPrefab, pos, Quaternion.identity);
            go.transform.parent = this.Container.transform;
            go.name = "Fish " + i.ToString();

            fishes.Add(go);
            vels.Add(Vector3.zero);

            if (this.SelectedFish == null)
            {
                this.SelectedFish = go;
            }
        }

        this.Fishes = fishes.ToArray();
        this.Velocities = vels.ToArray();
	}
	
	void Update ()
	{
	    this.CheckMouse();
        this.MoveFishes();
        this.PositionCamera();
	}

    private void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.SelectedFish = this.Fishes[Random.Range(0, this.Fishes.Length)];
        }
    }

    private void PositionCamera()
    {
        Vector3 pos = this.SelectedFish.transform.position + Vector3.back * 10f + Vector3.up * 2f + Vector3.left;
        this.Camera.transform.position = pos;
    }

    private void MoveFishes()
    {
        Vector3 v1, v2, v3;
        for (int i = 0; i < this.Fishes.Length; i++)
        {
            v1 = this.Rule1(this.Fishes[i]);
            v2 = this.Rule2(this.Fishes[i]);
            v3 = this.Rule3(this.Fishes[i], i);

            //Debug.Log(v1 + " " + v2 + " " + v3);

            this.Velocities[i] = this.Velocities[i] + v1 * this.Strength1 + v2 * this.Strength2 + v3 * this.Strength3;
            
            this.Fishes[i].transform.position = this.Fishes[i].transform.position + this.Velocities[i] * Time.deltaTime;
        }
    }

    private Vector3 Rule1(GameObject fish)
    {
        Vector3 v = Vector3.zero;
        for (int i = 0; i < this.Fishes.Length; i++)
        {
            GameObject f = this.Fishes[i];
            if (!f.Equals(fish))
            {
                v += f.transform.position;
            }
        }

        v = v / (this.Fishes.Length - 1);

        return (v - fish.transform.position) / 10f;
    }

    private Vector3 Rule2(GameObject fish)
    {
        Vector3 v = Vector3.zero;
        for (int i = 0; i < this.Fishes.Length; i++)
        {
            GameObject f = this.Fishes[i];
            if (!f.Equals(fish))
            {
                if (Vector3.Distance(f.transform.position, fish.transform.position) < 5f)
                {
                    v = v - (f.transform.position - fish.transform.position);
                }
            }
        }

        return v;
    }

    private Vector3 Rule3(GameObject fish, int ix)
    {
        Vector3 v = Vector3.zero;
        for (int i = 0; i < this.Fishes.Length; i++)
        {
            GameObject f = this.Fishes[i];
            if (!f.Equals(fish))
            {
                v += this.Velocities[i];
            }
        }

        v /= (this.Fishes.Length - 1);

        return (v - this.Velocities[ix]) / 8f;
    }
}
