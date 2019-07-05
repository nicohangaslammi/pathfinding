using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TilemapPathfinding
{
    public class Node : IHeapItem<Node>
    {
        public bool solid;
        public Vector2 worldPosition;

        public int gridX;
        public int gridY;
        
        public int gCost; // Cost from start to node
        public int hCost; // Cost from target to node

        public Node parent;

        public int fCost
        {
            get {return gCost + hCost; }
        }

        public Node(bool solid, Vector2 worldPosition, int gridX, int gridY)
        {
            this.solid = solid;
            this.worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY;
        }

        private int heapIndex;
        public int HeapIndex
        {
            get { return heapIndex; }
            set { heapIndex = value; }
        }

        public int CompareTo(Node compareNode)
        {
            int compare = fCost.CompareTo(compareNode.fCost);

            if (compare == 0)
            {
                compare = hCost.CompareTo(compareNode.hCost);
            }

            return -compare;
        }
    }
}
