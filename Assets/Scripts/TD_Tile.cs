using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Tile
{
    public Vector2 world_pos;
    public bool available = true;
    public int x;
    public int y;

    // Constructor
    public TD_Tile(int x, int y, Vector3 pos) {
        this.x = x;
        this.y = y;
        this.world_pos = pos;
    }
}
