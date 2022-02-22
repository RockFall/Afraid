using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Grid {
    // A grid of tiles
    public TD_Tile[,] grid;
    public TD_Pathfinding pathFinder;
    // Position of the most bot-left tile
    public Vector3 world_origin;
    public float tileSize;
    int width = 0;
    int height = 0;

    public void CreateGrid(int width, int height, float tileSize, Vector3 offset) {
        grid = new TD_Tile[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = new TD_Tile(x, y, new Vector2(offset.x + x * tileSize, offset.y + y * tileSize));
            }
        }

        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        this.world_origin = offset;
        pathFinder = new TD_Pathfinding(this);
    }

    public TD_Tile GetTileAt(int x, int y) {
        if (x < 0 || x > width-1 || y < 0 || y > height-1) {
            return null;
        }

        return grid[x, y];
    }

    public Vector3 GetTileWorldPosition(TD_Tile tile) {
        return world_origin + (new Vector3(tile.x * tileSize, tile.y * tileSize, world_origin.z));
    }

    public TD_Tile GetTileFromWorldPoint(Vector3 point) {
        if (grid.Length < 1) {
            Debug.LogWarning("GetTileFromWorldPoint: Trying to get tile from empty grid");
        }

        Vector3Int tileCoords = ((point - world_origin) / tileSize).FloorToInt();

        if (tileCoords.x > width - 1 || tileCoords.x < 0 || tileCoords.y > height - 1 || tileCoords.y < 0) {
            return null;
        }

        return grid[tileCoords.x, tileCoords.y];
    }

    public TD_Tile[] GetNeighbours(TD_Tile tile, bool diagonals) {
        TD_Tile[] neighs;
        if (diagonals)
            neighs = new TD_Tile[8];
        else
            neighs = new TD_Tile[4];
        neighs[0] = GetTileAt(tile.x,    tile.y+1);
        neighs[1] = GetTileAt(tile.x-1,  tile.y);
        neighs[2] = GetTileAt(tile.x+1,  tile.y);
        neighs[3] = GetTileAt(tile.x,    tile.y-1);
        if (diagonals) {
            neighs[4] = GetTileAt(tile.x-1,  tile.y+1);
            neighs[5] = GetTileAt(tile.x+1,  tile.y+1);
            neighs[6] = GetTileAt(tile.x-1,  tile.y-1);
            neighs[7] = GetTileAt(tile.x+1,  tile.y-1);
        }
        return neighs;
    }
}
