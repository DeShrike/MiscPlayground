using UnityEngine;
using System.Collections;

public class FurnitureController : MonoBehaviour
{
    private World theWorld;

    public GameObject LightPrefab;

    public GameObject Player;

    // Use this for initialization
    void Start ()
    {
        this.theWorld = World.Current;
        if (this.theWorld.CurrentFloor == null)
        {
            return;
        }

        this.AddFirniture();
    }

    // Update is called once per frame
    private void AddFirniture()
    {
        Floor l = this.theWorld.CurrentFloor;
        for (int x = 0; x < l.Width; x++)
        {
            for (int y = 0; y < l.Height; y++)
            {
                switch ((FirnitureType)l.furnitureMap[x][y])
                {
                    case FirnitureType.Spawn:
                        this.Player.transform.position = new Vector3(x, 1, y);
                        break;
                    case FirnitureType.Lamp:
                        {
                            GameObject li = (GameObject)Instantiate(this.LightPrefab, new Vector3(x, 0, y), Quaternion.identity);
                            li.transform.parent = this.gameObject.transform;
                        }
                        break;
                }
            }
        }
    }
}
