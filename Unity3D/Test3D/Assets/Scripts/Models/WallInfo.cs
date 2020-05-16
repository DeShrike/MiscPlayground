using UnityEngine;
using System.Collections;

public class WallInfo
{
    public bool North { get; set; }
    public bool South { get; set; }
    public bool West { get; set; }
    public bool East { get; set; }

    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public int Type { get; set; }

}

public class DoorInfo
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public int Type { get; set; }

}

public class PlayerInfo
{
    public int CurrentLevel { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
}
