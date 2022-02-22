using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class TD_Pathfinding
{
    struct PathVertex<T> {
        public T data;
        public PathEdge<T>[] edges;
    }
    struct PathEdge<T> {
        public float cost;
        public PathVertex<T> node;
    }
    class PathGraph_TDTile {
        public Dictionary<TD_Tile, PathVertex<TD_Tile>> tileVertexMap;
        public TD_Grid grid;

        public PathGraph_TDTile(TD_Grid grid_tiles) {
            // Initialize
            Dictionary<TD_Tile, PathVertex<TD_Tile>> nodes = new Dictionary<TD_Tile, PathVertex<TD_Tile>>();
            
            this.grid = grid_tiles;
            // Creates a vertex for each tile
            foreach (TD_Tile tile in grid.grid) {
                PathVertex<TD_Tile> vertex = new PathVertex<TD_Tile>();
                vertex.data = tile;

                nodes.Add(tile, vertex);
            }

            tileVertexMap = new Dictionary<TD_Tile, PathVertex<TD_Tile>>();
            // Creates an edge for each vertex
            int edgeCount = 0;
            foreach (TD_Tile tile in nodes.Keys) {
                PathVertex<TD_Tile> vertex = nodes[tile];

                List<PathEdge<TD_Tile>> edges = new List<PathEdge<TD_Tile>>();

                TD_Tile[] neighbours = grid.GetNeighbours(tile, true);

                // One edge to each walkable non corner-cutting neighbour
                foreach (TD_Tile neigh in neighbours) {
                    if (neigh != null && neigh.available) {
                        if (IsCuttingCorner(tile, neigh)) {
                            continue;
                        }
                        // Creates the edge
                        PathEdge<TD_Tile> edge = new PathEdge<TD_Tile>();
                        edge.cost = 1;
                        edge.node = nodes[neigh];
                        if (nodes[neigh] == null) {
                            int a = 3;
                        }
                        // Temporarily stores the edge
                        edges.Add(edge);
                        edgeCount++;
                    }
                }
                vertex.edges = edges.ToArray();
                tileVertexMap[tile] = vertex;
            }
            
        }

        bool IsCuttingCorner(TD_Tile curr, TD_Tile neigh) {
            if (Mathf.Abs(curr.x - neigh.x) == 1 && Mathf.Abs(curr.y - neigh.y) == 1) {
                // We are diagonals
                int dX = neigh.x - curr.x;
                int dY = neigh.y - curr.y;

                if (grid.grid[curr.x + dX, curr.y].available == false) {
                    // Cutting edge
                    return true;
                }

                if (grid.grid[curr.x, curr.y + dY].available == false) {
                    // Cutting edge
                    return true;
                }
            }
            return false;
        }
    }

    // Graph stored to be used in algorithm
    PathGraph_TDTile graph;

    public TD_Pathfinding(TD_Grid grid) {
        if (graph == null) {
            graph = new PathGraph_TDTile(grid);
        }
    }

    // TODO: Make possible to remove and add edges
    public void RecalculateGraph() {

    }

    public Queue<TD_Tile> FindPath(TD_Tile startTile, TD_Tile endTile) {
        if (startTile == null || endTile == null) {
            return null;
        }

        // A dictionay of all valid, walkable nodes
        Dictionary<TD_Tile, PathVertex< TD_Tile>> nodes = graph.tileVertexMap;

        // Make sure our start/end are in the list of nodes
        if (nodes.ContainsKey(startTile) == false) {
            Debug.LogError("Path_AStar: Starting tile isn't in the list of nodes!");
            return null;
        }
        if (nodes.ContainsKey(endTile) == false) {
            Debug.LogError("Path_AStar: Ending tile isn't in the list of nodes!");
            return null;
        }

        PathVertex<TD_Tile> start = nodes[startTile];
        PathVertex<TD_Tile> goal = nodes[endTile];

        // Mostly following this pseudocode:
        // https://en.wikipedia.org/wiki/A*_search_algorithm

        List<PathVertex<TD_Tile>> ClosedSet = new List<PathVertex<TD_Tile>>();

        // Priority Queue
        SimplePriorityQueue<PathVertex<TD_Tile>> OpenSet = new SimplePriorityQueue<PathVertex<TD_Tile>>();
        OpenSet.Enqueue(start, 0);

        // Dictionary of the pair Tile/Cheapest next tile.
        Dictionary<PathVertex<TD_Tile>, PathVertex<TD_Tile>> Came_From = new Dictionary<PathVertex<TD_Tile>, PathVertex<TD_Tile>>();

        // g_score[n] is the cost of the cheapest path from start to n currently known.
        Dictionary<PathVertex<TD_Tile>, float> g_score = new Dictionary<PathVertex<TD_Tile>, float>();
        foreach (PathVertex<TD_Tile> node in nodes.Values) {
            g_score[node] = Mathf.Infinity; // Default value of infinity
        }
        g_score[start] = 0;

        // f_score[node] represents our current best guess as to
        // how short a path from start to finish can be if it goes through node.
        Dictionary<PathVertex<TD_Tile>, float> f_score = new Dictionary<PathVertex<TD_Tile>, float>();
        foreach (PathVertex<TD_Tile> node in nodes.Values) {
            f_score[node] = Mathf.Infinity; // Default value of infinity
        }
        f_score[start] = heuristic_cost_estimate(start, goal);

        while (OpenSet.Count > 0) {
            PathVertex<TD_Tile> current = OpenSet.Dequeue();
            if (current.edges == null) {
                int a = 3;
            }

            if (current.data == goal.data) {
                // Goal achieved
                return Repath(Came_From, current);
            }

            ClosedSet.Add(current);

            foreach (PathEdge<TD_Tile> neighbour in current.edges) {
                PathVertex< TD_Tile> neighbourNode = neighbour.node;
                if (ClosedSet.Contains(neighbourNode)) {
                    continue; // Ignore this, already completed neighbour
                }
                float movement_cost_to_neighbour = DistBetween(current, neighbourNode) * 1;
                float tentative_g_score = g_score[current] + movement_cost_to_neighbour;
                f_score[current] = g_score[current] + heuristic_cost_estimate(neighbourNode, goal);

                if (OpenSet.Contains(neighbourNode) && tentative_g_score >= g_score[current]) {
                    continue;
                }

                Came_From[neighbourNode] = current;
                g_score[neighbourNode] = tentative_g_score;
                f_score[neighbourNode] = g_score[neighbourNode] + heuristic_cost_estimate(neighbourNode, goal);

                if (OpenSet.Contains(neighbourNode) == false) {
                    OpenSet.Enqueue(neighbourNode, f_score[neighbourNode]);
                } else {
                    OpenSet.UpdatePriority(neighbourNode, f_score[neighbourNode]);
                }
            } // End of foreach neighbour
        } // End of While

        // If we got to here, there is no avaible path from start to goal
        return null;
    }

    float heuristic_cost_estimate(PathVertex< TD_Tile> start, PathVertex< TD_Tile> goal) {
        return Mathf.Sqrt(Mathf.Pow(start.data.x - goal.data.x, 2) + Mathf.Pow(start.data.y - goal.data.y, 2));
    }

    float DistBetween(PathVertex< TD_Tile> a, PathVertex< TD_Tile> b) {

        // Hori/Vert neighbours have a distance of 1.
        // Diag neighbours have a distance of 1.41421356237.

        if (Mathf.Abs(a.data.x - b.data.x) + Mathf.Abs(a.data.y - b.data.y) == 1)
            return 1f;
        if ((Mathf.Abs(a.data.x - b.data.x) == 1 && Mathf.Abs(a.data.y - b.data.y) == 1))
            return 1.41421356237f;
        else {
            //Debug.LogError("Path_AStar:: DistBetween -- Not a neighbour node");
            return Mathf.Sqrt(Mathf.Pow(a.data.x - b.data.x, 2) + Mathf.Pow(a.data.y - b.data.y, 2));
        }
    }

    Queue<TD_Tile> Repath(Dictionary<PathVertex< TD_Tile>, PathVertex< TD_Tile>> Came_From, PathVertex< TD_Tile> current) {
        // At this point, current is the goal, we will walk backwards through the Came_From map
        // until we reach the starting node.

        Queue< TD_Tile> total_path = new Queue< TD_Tile>();
        total_path.Enqueue(current.data); // Add goal to the queue

        while (Came_From.ContainsKey(current)) {
            current = Came_From[current];
            total_path.Enqueue(current.data); // Add every node in the path to queue
        }

        return new Queue<TD_Tile>(total_path.Reverse());
    }
}
