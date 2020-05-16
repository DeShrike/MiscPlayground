using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class MeshController : MonoBehaviour
{
    private World theWorld;

    void Start()
    {
        this.theWorld = World.Current;
        if (this.theWorld.CurrentFloor == null)
        {
            return;
        }

        this.BuildMeshes();
    }

    private void BuildMeshes()
    {
        Floor l = this.theWorld.CurrentFloor;

        this.BuildFloorCeilingMeshes(l);

        MeshBuilder wbuilder = this.GetMeshBuilder(TileType.Wall);

        this.BuildNorthFacingWallMeshes(wbuilder, l);
        this.BuildSouthFacingWallMeshes(wbuilder, l);
        this.BuildWestFacingWallMeshes(wbuilder, l);
        this.BuildEastFacingWallMeshes(wbuilder, l);

        wbuilder.BuildMesh();
    }

    private void BuildFloorCeilingMeshes(Floor l)
    {
        MeshBuilder builder = this.GetMeshBuilder(TileType.Empty);

        MeshBuilder cbuilder = this.GetMeshBuilder(TileType.Ceiling);

        if (l != null)
        {
            foreach (var room in l.Rooms)
            {
                Debug.Log(room);

                builder.AddFloorOrCeilingToMesh(room, false);
                cbuilder.AddFloorOrCeilingToMesh(room, true);
            }

            foreach (var cor in l.Corridors)
            {
                builder.AddFloorOrCeilingToMesh(cor, false);
                cbuilder.AddFloorOrCeilingToMesh(cor, true);
            }
        }

        builder.BuildMesh();
        //cbuilder.BuildMesh();
    }

    private void BuildNorthFacingWallMeshes(MeshBuilder wbuilder, Floor l)
    {
        int x2 = -1;
        int x1 = -1;
        TileType workType = TileType.Wall;
        for (int y = 0; y < l.Height - 1; y++)
        {
            for (int x = 0; x < l.Width; x++)
            {
                TileType deze = (TileType)l.map[x][y];
                TileType hoger = (TileType)l.map[x][y + 1];
                if (deze == workType && this.CheckType(hoger, TileType.Empty, TileType.DoorEW, TileType.DoorNS))
                {
                    if (x1 == -1)
                    {
                        x1 = x;
                        x2 = x;
                    }
                    else
                    {
                        x2 = x;
                    }
                }
                else
                {
                    if (x1 != -1)
                    {
                        wbuilder.AddNorthFacingWallToMesh(y + 1, x1, x2 + 1);
                        x1 = x2 = -1;
                    }
                }
            }

            if (x1 != -1)
            {
                wbuilder.AddNorthFacingWallToMesh(y + 1, x1, x2 + 1);
                x1 = x2 = -1;
            }
        }
    }

    private void BuildSouthFacingWallMeshes(MeshBuilder wbuilder, Floor l)
    {
        int x2 = -1;
        int x1 = -1;
        TileType workType = TileType.Wall;
        for (int y = 1; y < l.Height; y++)
        {
            for (int x = 0; x < l.Width; x++)
            {
                TileType deze = (TileType)l.map[x][y];
                TileType lager = (TileType)l.map[x][y - 1];
                if (deze == workType && this.CheckType(lager, TileType.Empty, TileType.DoorEW, TileType.DoorNS))
                {
                    if (x1 == -1)
                    {
                        x1 = x;
                        x2 = x;
                    }
                    else
                    {
                        x2 = x;
                    }
                }
                else
                {
                    if (x1 != -1)
                    {
                        wbuilder.AddSouthFacingWallToMesh(y, x1, x2 + 1);
                        x1 = x2 = -1;
                    }
                }
            }

            if (x1 != -1)
            {
                wbuilder.AddSouthFacingWallToMesh(y, x1, x2 + 1);
                x1 = x2 = -1;
            }
        }
    }

    private bool CheckType(TileType c, TileType t1, TileType t2, TileType t3)
    {
        return c == t1 || c == t2 || c == t3;
    }

    private void BuildWestFacingWallMeshes(MeshBuilder wbuilder, Floor l)
    {
        int y2 = -1;
        int y1 = -1;
        TileType workType = TileType.Wall;
        for (int x = 0; x < l.Width - 1; x++)
        {
            for (int y = 0; y < l.Height; y++)
            {
                TileType deze = (TileType)l.map[x][y];
                TileType rechts = (TileType)l.map[x+1][y];
                if (deze == workType && this.CheckType(rechts, TileType.Empty, TileType.DoorEW, TileType.DoorNS))
                {
                    if (y1 == -1)
                    {
                        y1 = y;
                        y2 = y;
                    }
                    else
                    {
                        y2 = y;
                    }
                }
                else
                {
                    if (y1 != -1)
                    {
                        wbuilder.AddWestFacingWallToMesh(x+1,  y1, y2 + 1);
                        y1 = y2 = -1;
                    }
                }
            }

            if (y1 != -1)
            {
                wbuilder.AddWestFacingWallToMesh(x+1, y1, y2 + 1);
                y1 = y2 = -1;
            }
        }
    }

    private void BuildEastFacingWallMeshes(MeshBuilder wbuilder, Floor l)
    {
        int y2 = -1;
        int y1 = -1;
        TileType workType = TileType.Wall;
        for (int x = 1; x < l.Width ; x++)
        {
            for (int y = 0; y < l.Height; y++)
            {
                TileType deze = (TileType)l.map[x][y];
                TileType links = (TileType)l.map[x - 1][y];
                if (deze == workType && this.CheckType(links, TileType.Empty, TileType.DoorEW, TileType.DoorNS))
                {
                    if (y1 == -1)
                    {
                        y1 = y;
                        y2 = y;
                    }
                    else
                    {
                        y2 = y;
                    }
                }
                else
                {
                    if (y1 != -1)
                    {
                        wbuilder.AddEastFacingWallToMesh(x, y1, y2 + 1);
                        y1 = y2 = -1;
                    }
                }
            }

            if (y1 != -1)
            {
                wbuilder.AddEastFacingWallToMesh(x, y1, y2 + 1);
                y1 = y2 = -1;
            }
        }
    }

    private MeshBuilder GetMeshBuilder(TileType type)
    {
        for (int i=0;i<this.transform.childCount;i++)
        {
            var child = this.transform.GetChild(i);
            var script = child.GetComponent<MeshBuilder>();
            if (script != null)
            {
                if (script.TileType == type)
                {
                    return script;
                }
            }
        }

        return null;
    }
}
