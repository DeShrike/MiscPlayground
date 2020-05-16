using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour
{
    private World theWorld;

    public GameObject WallObject;
    public GameObject DoorObject;
    public GameObject NoneObject;
    public GameObject EmptyObject;

    public GameObject Container;

    private void Start()
    {
        this.theWorld = World.Current;
        if (this.theWorld.CurrentFloor != null)
        {
            Floor l = this.theWorld.CurrentFloor;

            for (int y = 0; y < l.Height; y++)
            {
                for (int x = 0; x < l.Width; x++)
                {
                    GameObject go = null;
                    TileType tt = l.GetTileType(x, y);
                    switch (tt)
                    {
                        case TileType.Empty:
                            go = (GameObject) Instantiate(this.EmptyObject, new Vector3(x, 0, y), Quaternion.Euler(90f, 0f, 0f));
                            break;
                        case TileType.None:
                            go = (GameObject)Instantiate(this.NoneObject, new Vector3(x, 0, y), Quaternion.Euler(90f, 0f, 0f));
                            break;
                        case TileType.Wall:
                            go = (GameObject)Instantiate(this.WallObject, new Vector3(x, 0, y), Quaternion.Euler(90f, 0f, 0f));
                            break;
                        case TileType.DoorEW:
                        case TileType.DoorNS:
                            go = (GameObject)Instantiate(this.DoorObject, new Vector3(x, 0, y), Quaternion.Euler(90f, 0f, 0f));
                            break;
                    }

                    if (go != null)
                    {
                        go.transform.parent = this.Container.transform;
                    }
                }
            }
        }
    }

}