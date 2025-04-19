using UnityEngine;
using System;
using System.Collections.Generic;

public class MazeLogicGenerator : MonoBehaviour
{
    public int row = 10, column = 10; // 可调参数

    public class Cell
    {
        public bool Visited;
        public bool E, S, W, N; // 四个方向通路

        public Cell()
        {
            Visited = false;
            E = S = W = N = false;
        }
    }

    private Cell[] maze;
    private Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
    private System.Random rnd = new System.Random();

    public Tuple<int, int> start = Tuple.Create(0, 0);
    public Tuple<int, int> end;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        maze = new Cell[row * column];
        for (int i = 0; i < maze.Length; i++)
        {
            maze[i] = new Cell();
        }

        int x = start.Item1;
        int y = start.Item2;
        stack.Push(Tuple.Create(x, y));
        maze[GetIndex(x, y)].Visited = true;

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(current.Item1, current.Item2);

            if (neighbors.Count > 0)
            {
                var next = neighbors[rnd.Next(neighbors.Count)];

                int dx = next.Item1 - current.Item1;
                int dy = next.Item2 - current.Item2;

                if (dx == 1) { maze[GetIndex(current.Item1, current.Item2)].E = true; maze[GetIndex(next.Item1, next.Item2)].W = true; }
                else if (dx == -1) { maze[GetIndex(current.Item1, current.Item2)].W = true; maze[GetIndex(next.Item1, next.Item2)].E = true; }
                else if (dy == 1) { maze[GetIndex(current.Item1, current.Item2)].S = true; maze[GetIndex(next.Item1, next.Item2)].N = true; }
                else if (dy == -1) { maze[GetIndex(current.Item1, current.Item2)].N = true; maze[GetIndex(next.Item1, next.Item2)].S = true; }

                maze[GetIndex(next.Item1, next.Item2)].Visited = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        // 设置终点为离起点最远的点（简单的 BFS）
        end = FindFarthestCellFrom(start);
        Debug.Log("Maze generated. Start: " + start + " End: " + end);
    }

    int GetIndex(int x, int y)
    {
        return y * row + x;
    }

    List<Tuple<int, int>> GetUnvisitedNeighbors(int x, int y)
    {
        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();

        if (x > 0 && !maze[GetIndex(x - 1, y)].Visited) neighbors.Add(Tuple.Create(x - 1, y));
        if (x < row - 1 && !maze[GetIndex(x + 1, y)].Visited) neighbors.Add(Tuple.Create(x + 1, y));
        if (y > 0 && !maze[GetIndex(x, y - 1)].Visited) neighbors.Add(Tuple.Create(x, y - 1));
        if (y < column - 1 && !maze[GetIndex(x, y + 1)].Visited) neighbors.Add(Tuple.Create(x, y + 1));

        return neighbors;
    }

    Tuple<int, int> FindFarthestCellFrom(Tuple<int, int> startCell)
    {
        int[,] distances = new int[row, column];
        bool[,] visited = new bool[row, column];
        Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
        q.Enqueue(startCell);
        visited[startCell.Item1, startCell.Item2] = true;

        Tuple<int, int> farthest = startCell;
        int maxDist = 0;

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            int x = current.Item1;
            int y = current.Item2;

            foreach (var dir in GetConnectedNeighbors(x, y))
            {
                int nx = dir.Item1;
                int ny = dir.Item2;

                if (!visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    distances[nx, ny] = distances[x, y] + 1;
                    if (distances[nx, ny] > maxDist)
                    {
                        maxDist = distances[nx, ny];
                        farthest = Tuple.Create(nx, ny);
                    }
                    q.Enqueue(Tuple.Create(nx, ny));
                }
            }
        }

        return farthest;
    }

    List<Tuple<int, int>> GetConnectedNeighbors(int x, int y)
    {
        List<Tuple<int, int>> connected = new List<Tuple<int, int>>();
        Cell c = maze[GetIndex(x, y)];

        if (c.N && y > 0) connected.Add(Tuple.Create(x, y - 1));
        if (c.E && x < row - 1) connected.Add(Tuple.Create(x + 1, y));
        if (c.S && y < column - 1) connected.Add(Tuple.Create(x, y + 1));
        if (c.W && x > 0) connected.Add(Tuple.Create(x - 1, y));

        return connected;
    }

    // 你可以添加一个公开函数暴露 maze，用于生成自定义 prefab 可视化
    public Cell[] GetMazeData()
    {
        return maze;
    }
}
