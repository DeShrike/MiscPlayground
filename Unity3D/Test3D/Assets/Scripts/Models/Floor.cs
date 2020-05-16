using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor
{
    private int minRoomCount = 10;

    private int maxRoomCount = 30;

    private int minRoomSizeX = 5;

    private int minRoomSizeY = 5;

    private int maxRoomSizeX = 15;

    private int maxRoomSizeY = 15;

    private int minCorridorWidth = 2;

    private int maxCorridorWidth = 2;

    private int minCorridorLength = 4;

    private int maxCorridorLength = 15;

    public Floor(World world)
    {
        this.World = world;
        this.Rooms=new List<Room>();
        this.Corridors=new List<Corridor>();
    }

    public List<Room> Rooms { get; set; }

    public List<Corridor> Corridors { get; set; }

    public World World { get; set; }

    public int RoomCount { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public byte[][] map { get; internal set; }
    public byte[][] furnitureMap { get; internal set; }

    public void Create(int width, int height)
    {
        this.Width = width;
        this.Height = height;

        this.map = new byte[this.Height][];
        this.furnitureMap = new byte[this.Height][];
        for (int y = 0; y < this.Height; y++)
        {
            this.map[y] = new byte[this.Width];
            this.furnitureMap[y] = new byte[this.Width];
        }

        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                this.map[x][y] = (byte)TileType.None;
                this.furnitureMap[x][y] = (byte)FirnitureType.None;
            }
        }

        Room ri = this.TryPlaceFirstRoom();
        if (ri == null)
        {
            return;
        }

        this.Rooms.Add(ri);

        this.RoomCount++;
        Corridor oldCi = null;
        Room oldRi = ri;
        int rooms = Random.Range(this.minRoomCount, this.maxRoomCount + 1);
        while (this.RoomCount < rooms)
        {
            Corridor newCi;
            ri = this.TryPlaceRoom(oldRi, oldCi, out newCi);
            if (ri != null)
            {
                this.Corridors.Add(newCi);
                oldRi.Corridors.Add(newCi);
                this.Rooms.Add(ri);
                oldRi = ri;
                oldCi = newCi;
                this.RoomCount++;
            }
            else
            {
                Room rr = this.Rooms.FirstOrDefault(_ => _.Id == oldRi.Id - 1);
                if (rr == null)
                {
                    break;
                }

                oldRi = rr;
                if (oldRi.Corridors.Count == 0)
                {
                    break;
                }

                oldCi = rr.Corridors.Last();
            }

        }

        this.RebuildMap();

        this.PlacePlayer();
        this.PlaceLights();
    }

    #region Firniture

    private void PlacePlayer()
    {
        Room r = this.Rooms.FirstOrDefault();
        if (r == null)
        {
            Debug.LogWarning("No first room");
            return;
        }

        int x = r.X1 + r.Width / 2;
        int y = r.Y1 + r.Height / 2;

        this.furnitureMap[x][y] = (byte)FirnitureType.Spawn;
    }

    private void PlaceLights()
    {
        foreach (Room r in this.Rooms)
        {
            this.PlaceLights(r);
        }
    }

    private void PlaceLights(Room r)
    {
        int x = r.X1 + r.Width / 2;
        int y = r.Y1 + r.Height / 2;
        this.furnitureMap[x+1][y] = (byte)FirnitureType.Lamp;

        //int size = r.Width * r.Height;

        //this.furnitureMap[r.X1+1][r.Y1+1] = (byte)FirnitureType.Lamp;
        //this.furnitureMap[r.X2-1][r.Y2-1] = (byte)FirnitureType.Lamp;

        //if (size > 36)
        //{
        //    this.furnitureMap[r.X1+1][r.Y2-1] = (byte)FirnitureType.Lamp;
        //    this.furnitureMap[r.X2-1][r.Y1+1] = (byte)FirnitureType.Lamp;
        //}
    }

    #endregion

    private void RebuildMap()
    {
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                this.map[x][y] = (byte)TileType.None;
            }
        }

        foreach (Corridor c in this.Corridors)
        {
            this.SetMapBytes(c);
        }

        foreach (Room r in this.Rooms)
        {
            this.SetMapBytes(r, true);
        }

        // nu deuren verwijderen die op een hoek staan
        for (int y = 1; y < this.Height-1; y++)
        {
            for (int x = 1; x < this.Width-1; x++)
            {
                if (this.map[x][y] == (byte)TileType.DoorNS)
                {
                    if ( (this.map[x][y - 1] != (byte)TileType.Wall  && this.map[x][y - 1] != (byte)TileType.DoorNS)
                        || (this.map[x][y + 1] != (byte)TileType.Wall && this.map[x][y + 1] != (byte)TileType.DoorNS)
                        || this.map[x-1][y] != (byte)TileType.Empty 
                        || this.map[x+1][y ] != (byte)TileType.Empty)
                    {
                        this.map[x][y] = (byte)TileType.Wall;
                    }
                }

                if (this.map[x][y] == (byte)TileType.DoorEW)
                {
                    if ((this.map[x-1][y ] != (byte)TileType.Wall && this.map[x - 1][y] != (byte)TileType.DoorEW)
                        || (this.map[x+1][y ] != (byte)TileType.Wall && this.map[x + 1][y] != (byte)TileType.DoorEW)
                        || this.map[x][y - 1] != (byte)TileType.Empty
                        || this.map[x][y + 1] != (byte)TileType.Empty)
                    {
                        this.map[x][y] = (byte)TileType.Wall;
                    }
                }
            }
        }
    }

    private Room TryPlaceFirstRoom()
    {
        Room ri = new Room();

        ri.Id = 1;
        ri.Width = Random.Range(this.minRoomSizeX, this.maxRoomSizeX);
        ri.Height = Random.Range(this.minRoomSizeY, this.maxRoomSizeY);

        ri.X1 = Random.Range(1, this.Width - ri.Width - 1);
        ri.Y1 = Random.Range(1, this.Height - ri.Height - 1);

        this.SetMapBytes(ri);

        // TODO : spawn zetten

        return ri;
    }

    private Room TryPlaceRoom(Room previousRi, Corridor previousCi, out Corridor newCi)
    {
        Room ri = new Room();
        ri.Id = this.Rooms.Count + 1;

        newCi = new Corridor();
        newCi.Id = this.Corridors.Count + 1;

        bool ok = false;
        int poging = 0;
        while (!ok)
        {
            poging++;
            if (poging > 20)
            {
                ri = null;
                break;
            }

            Direction d = (Direction)Random.Range(0, 4);
            Direction reverse = (Direction)(((int)d + 2) % 4);
            if (previousCi !=null && reverse == previousCi.Direction)
            {
                d = (Direction)(((int)d + 1) % 4);
            }

            newCi.Direction = d;
            int length = Random.Range(this.minCorridorLength, this.maxCorridorLength + 1);
            int width = Random.Range(this.minCorridorWidth, this.maxCorridorWidth + 1);

            // Debug.Log("Direction: " + d);
            // Debug.Log("Corridor Length: " + length);

            if (d == Direction.North)
            {
                newCi.Y1 = previousRi.Y2 + 1;
                newCi.Height = length;
                newCi.X1 = Random.Range(previousRi.X1+1, previousRi.X2-2);
                newCi.Width = width;
            }
            else if (d == Direction.South)
            {
                newCi.Y1 = previousRi.Y1  - length;
                newCi.Height = length;
                newCi.X1 = Random.Range(previousRi.X1+1, previousRi.X2-2);
                newCi.Width = width;
            }
            else if (d == Direction.East)
            {
                newCi.X1 = previousRi.X2 + 1;
                newCi.Width = length;
                newCi.Y1 = Random.Range(previousRi.Y1+1, previousRi.Y2-2);
                newCi.Height = width;
            }
            else if (d == Direction.West)
            {
                newCi.X1 = previousRi.X1  - length;
                newCi.Width = length;
                newCi.Y1 = Random.Range(previousRi.Y1+1, previousRi.Y2-2);
                newCi.Height = width;
            }

            // test of corridor niks overlapt
            if (this.Overlaps(newCi))
            {
                ////Debug.Log("Overlapping corridor");
                continue;
            }

            // Nu proberen daar een kamer aan te plakken
            int rwidth = Random.Range(this.minRoomSizeX, this.maxRoomSizeX + 1);
            int rheight = Random.Range(this.minRoomSizeY, this.maxRoomSizeY + 1);

            if (d == Direction.North)
            {
                ri.Y1 = newCi.Y2 + 1;
                ri.Height = rheight;
                ri.X1 = Random.Range(newCi.X1 - rwidth + 2, newCi.X2-2);
                ri.Width = rwidth;
            }
            else if (d == Direction.South)
            {
                ri.Y1 = newCi.Y1 - rheight ;
                ri.Height = rheight;
                ri.X1 = Random.Range(newCi.X1 - rwidth + 2, newCi.X2-2);
                ri.Width = rwidth;
            }
            else if (d == Direction.East)
            {
                ri.X1 = newCi.X2 + 1;
                ri.Y1 = Random.Range(newCi.Y1 - rheight+2, newCi.Y2-2);
                ri.Height = rheight;
                ri.Width = rwidth;
            }
            else if (d == Direction.West)
            {
                ri.X1 = newCi.X1 - rwidth;
                ri.Height = rheight;
                ri.Y1 = Random.Range(newCi.Y1 - rheight + 2, newCi.Y2-2);
                ri.Width = rwidth;
            }

            // test of kamer niks overlapt
            if (this.Overlaps(ri))
            {
                ////Debug.Log("Overlapping room");
                continue;
            }

            ////Debug.Log(string.Format("Corridor: {0} {1} - {2} {3}", ri.X1, ri.Y1, ri.X2, ri.Y2));

            // nu controleren of de room niets overlapt en niet buiten wereld valt
            if (ri.X1 < 1 || ri.X1 >= this.Width - 1 || ri.X2 >= this.Width - 1 || ri.Y1 < 1 || ri.Y1 >= this.Height - 1 || ri.Y2 >= this.Height - 1)
            {
                ////Debug.Log("Bad");
            }
            else
            {
                // TODO deuren toevoegen

                // TODO lampen zetten

                // 

                ok = true;
            }

            if (ok)
            {
                this.SetMapBytes(newCi);
                this.SetMapBytes(ri);
            }

        }

        return ri;
    }

    private bool Overlaps(Corridor ci)
    {
        if (ci.X1 < 0 || ci.X2 >= this.Width)
        {
            return true;
        }

        if (ci.Y1 < 0 || ci.Y2 >= this.Height)
        {
            return true;
        }

        for (int y = ci.Y1; y <= ci.Y2; y++)
        {
            for (int x = ci.X1; x <= ci.X2; x++)
            {
                if (this.map[x][y] == (byte)TileType.Empty)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool Overlaps(Room ri)
    {
        if (ri.X1 < 0 || ri.X2 >= this.Width)
        {
            return true;
        }

        if (ri.Y1 < 0 || ri.Y2 >= this.Height)
        {
            return true;
        }

        for (int y = ri.Y1; y <= ri.Y2; y++)
        {
            for (int x = ri.X1; x <= ri.X2; x++)
            {
                if (this.map[x][y] == (byte)TileType.Empty)
                    //if (this.map[x][y] != (byte)TileType.None)
                    {
                        return true;
                }
            }
        }

        return false;
    }

    private void SetMapBytes(Room ri, bool addExtraDoors = false)
    {
        int extraRoomX = 0;
        int extraRoomY = 0;
        int extraDoorKansA = 5;
        int extraDoorKansB = 2;

        for (int y = ri.Y1; y <= ri.Y2; y++)
        {
            for (int x = ri.X1; x <= ri.X2; x++)
            {
                this.map[x][y] = (byte)TileType.Empty;
            }
        }

        for (int y = ri.Y1 - 1; y <= ri.Y2 + 1; y++)
        {
            if (this.map[ri.X1 - 1][y] == (byte)TileType.None)
            {
                this.map[ri.X1 - 1][y] = (byte)TileType.Wall;
            }
            else if (this.map[ri.X1 - 1][y] == (byte)TileType.Wall && addExtraDoors && Random.Range(0, extraDoorKansA) == extraDoorKansB)
            {
                if (extraRoomY != y && extraRoomX != ri.X1 - 1)
                {
                    this.map[ri.X1 - 1][y] = (byte)TileType.DoorNS;
                    extraRoomX = ri.X1 - 1;
                    extraRoomY = y;
                }
            }

            if (this.map[ri.X2 + 1][y] == (byte)TileType.None)
            {
                this.map[ri.X2 + 1][y] = (byte)TileType.Wall;
            }
            else if (this.map[ri.X2 + 1][y] == (byte)TileType.Wall && addExtraDoors && Random.Range(0, extraDoorKansA) == extraDoorKansB)
            {
                if (extraRoomY != y && extraRoomX != ri.X2 + 1)
                {
                    this.map[ri.X2 + 1][y] = (byte)TileType.DoorNS;
                    extraRoomX = ri.X2 + 1;
                    extraRoomY = y;
                }
            }
        }

        for (int x = ri.X1 - 0; x <= ri.X2 + 0; x++)
        {
            if (this.map[x][ri.Y1 - 1] == (byte)TileType.None)
            {
                this.map[x][ri.Y1 - 1] = (byte)TileType.Wall;
            }
            else if (this.map[x][ri.Y1 - 1] == (byte)TileType.Wall && addExtraDoors && Random.Range(0, extraDoorKansA) == extraDoorKansB)
            {
                if (extraRoomX != x && extraRoomY != ri.Y1 - 1)
                {
                    this.map[x][ri.Y1 - 1] = (byte)TileType.DoorEW;
                    extraRoomY = ri.Y1 - 1;
                    extraRoomX = x;
                }
            }

            if (this.map[x][ri.Y2 + 1] == (byte)TileType.None)
            {
                this.map[x][ri.Y2 + 1] = (byte)TileType.Wall;
            }
            else if (this.map[x][ri.Y2 + 1] == (byte)TileType.Wall && addExtraDoors && Random.Range(0, extraDoorKansA) == extraDoorKansB)
            {
                if (extraRoomX != x && extraRoomY != ri.Y2 + 1)
                {
                    this.map[x][ri.Y2 + 1] = (byte)TileType.DoorEW;
                    extraRoomY = ri.Y2 + 1;
                    extraRoomX = x;
                }
            }
        }
    }

    private void SetMapBytes(Corridor ri)
    {
        for (int y = ri.Y1; y <= ri.Y2; y++)
        {
            for (int x = ri.X1; x <= ri.X2; x++)
            {
                this.map[x][y] = (byte)TileType.Empty;
            }
        }

        if (ri.Direction == Direction.North || ri.Direction == Direction.South)
        {
            for (int y = ri.Y1 ; y <= ri.Y2 ; y++)
            {
                if (this.map[ri.X1 - 1][y] == (byte)TileType.None)
                    this.map[ri.X1 - 1][y] = (byte)TileType.Wall;

                if (this.map[ri.X2 + 1][y] == (byte)TileType.None)
                    this.map[ri.X2 + 1][y] = (byte)TileType.Wall;
            }

            this.map[ri.X1][ri.Y1] = (byte)TileType.DoorEW;
            this.map[ri.X1][ri.Y2] = (byte)TileType.DoorEW;

            this.map[ri.X1+1][ri.Y1] = (byte)TileType.DoorEW;
            this.map[ri.X1+1][ri.Y2] = (byte)TileType.DoorEW;
        }
        else
        {
            for (int x = ri.X1 ; x <= ri.X2 ; x++)
            {
                if (this.map[x][ri.Y1 - 1] == (byte)TileType.None)
                    this.map[x][ri.Y1 - 1] = (byte)TileType.Wall;

                if (this.map[x][ri.Y2 + 1] == (byte)TileType.None)
                    this.map[x][ri.Y2 + 1] = (byte)TileType.Wall;
            }

            this.map[ri.X1][ri.Y1] = (byte)TileType.DoorNS;
            this.map[ri.X2][ri.Y1] = (byte)TileType.DoorNS;

            this.map[ri.X1][ri.Y1+1] = (byte)TileType.DoorNS;
            this.map[ri.X2][ri.Y1+1] = (byte)TileType.DoorNS;
        }
    }

    #region Helpers
    public TileType GetTileType(int x, int y)
    {
        if (x < 0 || x >= this.Width)
        {
            return TileType.None;
        }

        if (y < 0 || y >= this.Height)
        {
            return TileType.None;
        }

        return (TileType)this.map[x][y];
    }

    private int CountNeighbours(int x, int y, TileType type)
    {
        int c = 0;

        if (this.GetTileType(x, y + 1) == type)
        {
            c++;
        }

        if (this.GetTileType(x, y - 1) == type)
        {
            c++;
        }

        if (this.GetTileType(x-1, y ) == type)
        {
            c++;
        }

        if (this.GetTileType(x+1, y ) == type)
        {
            c++;
        }

        return c;
    }

    #endregion
}