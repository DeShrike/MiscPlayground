using UnityEngine;
using System.Collections;

public enum TileType : byte
{
    None, Wall, StoneWall1, StoneWall2, StoneWall3, StoneWallX, MetalWall1, MetalWall3, MetalWallX, Empty, Ceiling, DoorEW, DoorNS
}
public enum Direction : byte
{
    North, East, South, West
}

public enum FirnitureType : byte
{
    None, Spawn, WoodenLogs, IronOre, Well, FoodDispenser,
    TeleportBeacon, Bed, Elevator, WorkBench, Door,
    Key, Lamp, Furnace, StorageBox, Teleporter
}

public enum InventoryItemType
{
    Water, Food, WoodenLogs, Planks, IronOre, IronBars,
}
