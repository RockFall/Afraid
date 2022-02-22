using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_GameCreator : MonoBehaviour
{
    GameObject gridGO;
    TD_Grid grid;

    TD_Tile lastMouseOverTile;
    HashSet<TD_Tile> pathTiles;

    // Start is called before the first frame update
    void Start()
    {
        gridGO = new GameObject("Grid");

        grid = new TD_Grid();
        grid.CreateGrid(10, 10, 1, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        if (grid == null || grid.grid.Length < 1) {
            return;
        }

        TD_Tile mouseOverTile = grid.GetTileFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        TD_Tile firstDragTile = null;
        if (TD_InputController.dragging.isDragging) {
            firstDragTile = grid.GetTileFromWorldPoint(TD_InputController.dragging.start_pos);

            if (mouseOverTile != lastMouseOverTile) {
                if (firstDragTile != null && mouseOverTile != null) {
                    pathTiles = new HashSet<TD_Tile>(grid.pathFinder.FindPath(firstDragTile, mouseOverTile));
                    lastMouseOverTile = mouseOverTile;
                }
            }
        }

        

        foreach (TD_Tile tile in grid.grid) {
            Gizmos.color = new Color(1, 0, 0, 1);
            if (tile == mouseOverTile) {
                if (TD_InputController.dragging.isDragging)
                    Gizmos.color = new Color(1, 1, 0, 1);
                else
                    Gizmos.color = new Color(0.22f, 0.69f, 0.87f, 1);
            }
            if (tile == firstDragTile && TD_InputController.dragging.isDragging) {
                Gizmos.color = new Color(0, 1, 0, 1);
            }
            if (pathTiles != null && pathTiles.Contains(tile)) {
                Gizmos.color = new Color(1, 0.64f, 0, 1);
            }

            Gizmos.DrawCube(new Vector3(tile.world_pos.x + grid.tileSize/2.0f, tile.world_pos.y + grid.tileSize / 2.0f, 0), new Vector3(grid.tileSize - 0.1f, grid.tileSize - 0.1f, 1.0f));
        }
    }
}
