
using UnityEngine;
using System;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Size")]
    public int row = 10;
    public int column = 10;
    [Header("Prefab & Size")]
    public GameObject WallPrefab;
    public GameObject StartPositionPrefab; 
    public GameObject EndMarker; 
    public float cellSize = 5f; 

    [Header("Debug")]
    public bool drawSolution = true; // 是否绘制路径

    #region DATA STRUCTURE
    public class Cell
    {
        public bool Visited;
        public bool N, E, S, W; // 四向通道标记
    }
    private Cell[] maze;
    private Tuple<int, int> start = Tuple.Create(0, 0);
    private Tuple<int, int> end;
    #endregion

    private System.Random rnd = new System.Random();

    #region UNITY
    void Start()
    {
        GenerateMaze();
        BuildVisual();
        if (drawSolution) DrawSolutionPath();
    }
    #endregion

    #region MAZE LOGIC （Recursive Backtracker）
    void GenerateMaze()
    {
        // 初始化单元格
        maze = new Cell[row * column];
        for (int i = 0; i < maze.Length; i++) maze[i] = new Cell();

        // 栈回溯生成
        Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
        stack.Push(start);
        maze[Index(start)].Visited = true;
        int visited = 1;

        while (visited < maze.Length)
        {
            var cur = stack.Peek();
            var neigh = GetUnvisitedNeighbors(cur);
            if (neigh.Count > 0)
            {
                var next = neigh[rnd.Next(neigh.Count)];
                CarvePassage(cur, next);
                maze[Index(next)].Visited = true;
                stack.Push(next);
                visited++;
            }
            else
            {
                stack.Pop();
            }
        }

        end = FindFarthestCell(start);
    }

    List<Tuple<int, int>> GetUnvisitedNeighbors(Tuple<int, int> c)
    {
        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        int x = c.Item1, y = c.Item2;
        if (y > 0 && !maze[Index(x, y - 1)].Visited) list.Add(Tuple.Create(x, y - 1));
        if (x < row - 1 && !maze[Index(x + 1, y)].Visited) list.Add(Tuple.Create(x + 1, y));
        if (y < column - 1 && !maze[Index(x, y + 1)].Visited) list.Add(Tuple.Create(x, y + 1));
        if (x > 0 && !maze[Index(x - 1, y)].Visited) list.Add(Tuple.Create(x - 1, y));
        return list;
    }

    void CarvePassage(Tuple<int, int> a, Tuple<int, int> b)
    {
        int ax = a.Item1, ay = a.Item2;
        int bx = b.Item1, by = b.Item2;
        if (bx == ax && by == ay - 1) { maze[Index(a)].N = maze[Index(b)].S = true; }
        else if (bx == ax + 1 && by == ay) { maze[Index(a)].E = maze[Index(b)].W = true; }
        else if (bx == ax && by == ay + 1) { maze[Index(a)].S = maze[Index(b)].N = true; }
        else if (bx == ax - 1 && by == ay) { maze[Index(a)].W = maze[Index(b)].E = true; }
    }

    Tuple<int, int> FindFarthestCell(Tuple<int, int> origin)
    {
        Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
        bool[,] vis = new bool[row, column];
        int[,] dist = new int[row, column];
        q.Enqueue(origin);
        vis[origin.Item1, origin.Item2] = true;
        Tuple<int, int> far = origin;
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            foreach (var n in GetConnectedNeighbors(cur))
            {
                if (vis[n.Item1, n.Item2]) continue;
                vis[n.Item1, n.Item2] = true;
                dist[n.Item1, n.Item2] = dist[cur.Item1, cur.Item2] + 1;
                if (dist[n.Item1, n.Item2] > dist[far.Item1, far.Item2]) far = n;
                q.Enqueue(n);
            }
        }
        return far;
    }

    List<Tuple<int, int>> GetConnectedNeighbors(Tuple<int, int> c)
    {
        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        int x = c.Item1, y = c.Item2;
        Cell cell = maze[Index(c)];
        if (cell.N) list.Add(Tuple.Create(x, y - 1));
        if (cell.E) list.Add(Tuple.Create(x + 1, y));
        if (cell.S) list.Add(Tuple.Create(x, y + 1));
        if (cell.W) list.Add(Tuple.Create(x - 1, y));
        return list;
    }

    int Index(int x, int y) => y * row + x;
    int Index(Tuple<int, int> t) => t.Item2 * row + t.Item1;
    #endregion

    #region VISUALISE
    void BuildVisual()
    {
        Vector3 mazeOffset = new Vector3(0, 0, -cellSize);  // ↓往下挪一个格
        foreach (Transform c in transform) Destroy(c.gameObject);

        // 内部格子：只画南 & 西
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                Vector3 basePos = new Vector3(x * cellSize, 0, y * cellSize) - mazeOffset;
                /*if (FloorPrefab)
                    Instantiate(FloorPrefab, basePos + new Vector3(cellSize / 2, 0, cellSize / 2),
                        Quaternion.identity, transform);
                        */

                Cell cell = maze[Index(x, y)];

                // 只画西 & 南（防止重复）
                if (!cell.W)
                    Instantiate(WallPrefab, basePos, Quaternion.Euler(0, 90, 0), transform);
                if (!cell.S)
                    Instantiate(WallPrefab, basePos, Quaternion.identity, transform);
                if (y == 0)
                    Instantiate(WallPrefab, basePos - new Vector3(0, 0, cellSize), Quaternion.identity, transform);
                // 最外圈：东 & 北 另外补
                if (x == row - 1 && !cell.E)
                    Instantiate(WallPrefab, basePos + new Vector3(cellSize, 0, 0),
                        Quaternion.Euler(0, 90, 0), transform);
                if (y == column - 1 && !cell.N)
                    Instantiate(WallPrefab, basePos + new Vector3(0, 0, cellSize),
                        Quaternion.identity, transform);
            }
        }

        // 起止标记
        //StartPositionPrefab = Resources.Load<GameObject>("Prefabs/Player"); 
        StartPositionPrefab = GameObject.Find("Player");
        StartPositionPrefab.transform.position = new Vector3(start.Item1 * cellSize + cellSize / 2, 0.2f,
            start.Item2 * cellSize + cellSize / 2);
        /*Instantiate(StartPositionPrefab, new Vector3(start.Item1 * cellSize + cellSize / 2, 0.2f,
            start.Item2 * cellSize + cellSize / 2), Quaternion.identity, transform);*/
        if (EndMarker)
            Instantiate(EndMarker, new Vector3(end.Item1 * cellSize + cellSize / 2, 0.2f,
                end.Item2 * cellSize + cellSize / 2), Quaternion.identity, transform);
        
        
    }

    #endregion

    #region DEBUG PATH
    void DrawSolutionPath()
    {
        var path = FindPath(start, end);
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 a = new Vector3(path[i].Item1 * cellSize + cellSize / 2, 1f, path[i].Item2 * cellSize + cellSize / 2);
            Vector3 b = new Vector3(path[i + 1].Item1 * cellSize + cellSize / 2, 1f, path[i + 1].Item2 * cellSize + cellSize / 2);
            Debug.DrawLine(a, b, Color.red, 60f); // 显示 60 秒
        }
    }

    List<Tuple<int, int>> FindPath(Tuple<int, int> a, Tuple<int, int> b)
    {
        // 简单 BFS 路径返回
        Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
        Dictionary<Tuple<int, int>, Tuple<int, int>> prev = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
        q.Enqueue(a);
        prev[a] = a;
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur.Equals(b)) break;
            foreach (var n in GetConnectedNeighbors(cur))
            {
                if (!prev.ContainsKey(n)) { prev[n] = cur; q.Enqueue(n); }
            }
        }
        List<Tuple<int, int>> path = new List<Tuple<int, int>>();
        var step = b;
        while (!step.Equals(a)) { path.Add(step); step = prev[step]; }
        path.Add(a);
        path.Reverse();
        return path;
    }
    #endregion
}
