using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilemapPathfinding
{
    public class AStarGrid : MonoBehaviour
    {
        [SerializeField]
        private bool drawGizmos = true;

        public bool Ready { get; private set; }

        [SerializeField]
        private Tilemap solidTilemap = null;

        public List<Node> path;

        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        private void OnEnable()
        {
            nodeDiameter = solidTilemap.cellSize.x;
            gridSizeX = solidTilemap.size.x;
            gridSizeY = solidTilemap.size.y;

            CreateGrid();
        }

        public int GridSize
        {
            get { return gridSizeX * gridSizeY; }
        }

        // Create a grid using referenced solid tilemap
        private void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector2 worldBottomLeft = (Vector2) transform.position - Vector2.right * gridSizeX / 2 - Vector2.up * gridSizeY/2;

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    Vector2 worldPoint = worldBottomLeft
                    + Vector2.right * (x * nodeDiameter + nodeDiameter / 2)
                    + Vector2.up * (y * nodeDiameter + nodeDiameter / 2);

                    Vector3Int tilePosition = new Vector3Int(
                        Mathf.FloorToInt(worldPoint.x),
                        Mathf.FloorToInt(worldPoint.y),
                        0
                    );

                    bool solid = solidTilemap.HasTile(tilePosition);

                    grid[x, y] = new Node(solid, worldPoint, x, y);
                }
            }

            Ready = true;
        }

        // Get a list of surrounding nodes
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (Mathf.Abs(x) - Mathf.Abs(y) == 0)
                        continue;

                    int nodeX = node.gridX + x;
                    int nodeY = node.gridY + y;

                    if (nodeX >= 0 && nodeX < gridSizeX && nodeY >= 0 && nodeY < gridSizeY)
                    {
                        neighbours.Add(grid[nodeX, nodeY]);
                    }
                }
            }

            return neighbours;
        }

        // Convert position to node
        public Node GetNodeFromWorldPoint(Vector2 worldPosition)
        {
            float percentX = (worldPosition.x + gridSizeX / 2) / gridSizeX;
            float percentY = (worldPosition.y + gridSizeY / 2) / gridSizeY;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, gridSizeY, 1));

            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    Gizmos.color = node.solid ? Color.red : Color.green;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * 0.5f);
                }
            }

            if (path != null)
            {
                Gizmos.color = Color.blue;
                foreach (Node node in path)
                {
                    if (node.parent != null)
                    {
                        Gizmos.DrawLine(node.worldPosition, node.parent.worldPosition);
                    }
                }
            }
        }
    }
}
