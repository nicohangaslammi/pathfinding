using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TilemapPathfinding
{
    [RequireComponent(typeof(AStarGrid))]
    public class Pathfinding : MonoBehaviour
    {
        private static Pathfinding _instance;
        public static Pathfinding Instance { get { return _instance; } }

        AStarGrid grid;

        private void Awake()
        {
            if (grid == null)
            {
                grid = GetComponent<AStarGrid>();
            }

            if (_instance == null)
            {
                _instance = this;
            }
        }

        private void Start()
        {

        }

        public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            if (!grid.Ready) return null;

            List<Node> path = new List<Node>();

            // Convert positions to nodes
            Node startNode = grid.GetNodeFromWorldPoint(startPos);
            Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

            Heap<Node> openSet = new Heap<Node>(grid.GridSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                
                closedSet.Add(currentNode);

                // Found path
                if (currentNode == targetNode)
                {
                    path = RetracePath(startNode, targetNode);
                    return path;
                }

                // Select best neighbour
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (neighbour.solid || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour);

                    if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            return path;
        }

        private bool IsBetter (Node a, Node b)
        {
            return a.fCost < b.fCost || a.fCost == b.fCost && a.hCost < b.hCost;
        }

        private int GetDistance(Node a, Node b)
        {
            int distX = Mathf.Abs(a.gridX - b.gridX);
            int distY = Mathf.Abs(a.gridY - b.gridY);

            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }
            else
            {
                return 14 * distX + 10 * (distY - distX);
            }
        }
    }
}
