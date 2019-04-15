﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_ass1
{
    public class RBFSAgent : NavAgent
    {
        public RBFSAgent(GridMap map) : base(map)
        {

        }

        /* Get the manhattan distance to the closest goal
         * @param nodeCoords The coords of the node in question
         * @return The distance
         */
        private int Heuristic(Coords nodeCoords)
        {
            List<Coords> greenCells = _map.GetGreenCells();
            int manhattanDist = Math.Abs(nodeCoords.x - greenCells.First().x) + Math.Abs(nodeCoords.y - greenCells.First().y);
            int dist;

            foreach (Coords c in greenCells)
            {
                dist = Math.Abs(nodeCoords.x - c.x) + Math.Abs(nodeCoords.y - c.y);
                if (dist < manhattanDist)
                {
                    manhattanDist = dist;
                }
            }

            return manhattanDist;
        }

        private Node FindMinNode(List<Node> nodes)
        {
            Node minFNode = nodes.First();   // Min frontier node

            foreach (Node n in nodes)
            {
                if (n.F < minFNode.F)
                {
                    minFNode = n;
                }
                else if (n.F == minFNode.F)
                {
                    if (n.Move < minFNode.Move)
                    {
                        minFNode = n;
                    }
                }
            }
            return minFNode;
        }

        private Node FindMinNode(List<Node> nodes, int depth)
        {
            List<Node> eqDepthNodes = new List<Node>();
            foreach(Node n in nodes)
            {
                if (n.Depth == depth)
                {
                    eqDepthNodes.Add(n);
                }
            }
            return FindMinNode(eqDepthNodes);
        }

        private void SortFrontier(List<Node> frontier)
        {
            int depth = frontier.Last().Depth; //Last element always has max depth
            // No need to sort entire frontier
            Node minFNode = FindMinNode(frontier);
            frontier.Remove(minFNode);

            if (minFNode.Depth < depth)
            {
                //Roll back
                List<Node> rollBackNodes = new List<Node>();
                Node deepMinNode = FindMinNode(frontier, depth);
                int f = deepMinNode.F;
                while (deepMinNode.Depth > minFNode.Depth)
                {
                    deepMinNode = deepMinNode.ParentNode;
                    deepMinNode.F = f;
                }
                foreach (Node n in frontier)
                {
                    if (n.Depth > minFNode.Depth)
                    {
                        rollBackNodes.Add(n);
                    }
                }
                foreach (Node n in rollBackNodes)
                {
                    frontier.Remove(n);
                    n.Dispose();
                }
            }

            frontier.Insert(0, minFNode);

            Console.WriteLine("{0},{1} ", minFNode.Coords.x, minFNode.Coords.y);
        }

        public override List<Node> Expand(Node node)
        {
            List<Node> expandedNode = new List<Node>();
            expandedNode.AddRange(_map.GetNodes(node));

            foreach (Node n in expandedNode)
            {
                n.F = n.Depth + Heuristic(n.Coords);
            }

            return expandedNode;
        }

        public override List<Node> TreeSearch()
        {
            List<Node> moves = new List<Node>();
            List<Node> frontier = new List<Node>();
            Node node = new Node(_redCoords, Move.NOOP, null);
            bool reachedGoal = false;

            frontier.Add(node);   // Add initial state to frontier

            do
            {
                if (frontier.Count == 0) { return null; }
                
                SortFrontier(frontier);

                node = frontier.First();
                frontier.RemoveAt(0);

                if (_map.IsInGreenCell(node.Coords))
                {
                    reachedGoal = true;
                }
                frontier.AddRange(Expand(node));

            } while (!reachedGoal);

            while (node.ParentNode != null)
            {
                moves.Add(node);
                node = node.ParentNode;
            }
            moves.Reverse();

            return moves;
        }
    }
}
