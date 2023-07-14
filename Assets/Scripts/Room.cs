using System.Collections.Generic;
using NavMeshPlus.Components;
using NavMeshPlus.Extensions;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum CellType
    {
        None = -1,      // Out of room
        Empty = 0,      // Nothing on the cell
        Floor = 1,      // Floor
        Border = 2,     // Border wall
        Wall = 3        // Inner wall
    }

    public class Cell
    {
        public CellType type;
        public GameObject gameObject;

        public Cell(CellType type = CellType.None, GameObject gameObject = null)
        {
            this.type = type;
            this.gameObject = gameObject;
        }
    }

    private Vector2 partSize;

    /// Generated room layout
    // Position of the room parts
    private List<Vector2> _partsPositions;
    public List<Vector2> partsPositions { get { return _partsPositions; } }

    // Room bounding box (minimum box where all parts fits in)
    private RectInt roomBoundingBox;
    // Boundaries of the camera (list of points)
    public List<Vector2> roomBoundaries;

    // Associative double dimension array representing the cells
    private List<List<Cell>> grid;


    public Vector2Int RoomToGrid(Vector2 roomPos)
    {
        if (roomBoundingBox.xMin < 0)
            roomPos.x += partSize.x * -roomBoundingBox.xMin;
        if (roomBoundingBox.yMin < 0)
            roomPos.y += partSize.y * -roomBoundingBox.yMin;
        return new((int)roomPos.x, (int)roomPos.y);
    }

    public Vector2 GridToRoom(Vector2Int roomPos)
    {
        if (roomBoundingBox.xMin < 0)
            roomPos.x -= (int)partSize.x * -roomBoundingBox.xMin;
        if (roomBoundingBox.yMin < 0)
            roomPos.y -= (int)partSize.y * -roomBoundingBox.yMin;
        return new(roomPos.x, roomPos.y);
    }

    public Cell GetCellAt(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= grid.Count || gridPos.y >= grid[gridPos.x].Count)
            return null;
        return grid[gridPos.x][gridPos.y];
    }

    public Cell GetCellAt(Vector2 roomPos)
    {
        return GetCellAt(RoomToGrid(roomPos));
    }

    public bool IsCellTypeAroundPosition(Vector2 roomPosition, CellType[] types)
    {
        Vector2Int gridPos = RoomToGrid(roomPosition);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                Cell cell = GetCellAt(gridPos + new Vector2Int(x, y));
                if (cell == null)
                    continue;

                foreach (CellType type in types)
                    if (cell.type == type)
                        return true;
            }
        }
        return false;
    }

    public bool IsCellNextToWall(Vector2 cellPosition)
    {
        return IsCellTypeAroundPosition(cellPosition, new CellType[] { CellType.Border, CellType.Wall });
    }

    public bool IsCellEmpty(Vector2 cellPosition)
    {
        return IsCellType(cellPosition, CellType.Empty);
    }

    public bool IsCellType(Vector2 cellPosition, CellType expectedType)
    {
        Cell cell = GetCellAt(cellPosition);
        return cell != null && cell.type == expectedType;
    }

    private void FillGrid()
    {
        roomBoundingBox = new();
        grid = new();
        foreach (Vector2 partPos in partsPositions)
        {
            if (partPos.x < roomBoundingBox.xMin)
                roomBoundingBox.xMin = (int)partPos.x;
            if (partPos.x + 1 > roomBoundingBox.xMax)
                roomBoundingBox.xMax = (int)partPos.x + 1;
            if (partPos.y < roomBoundingBox.yMin)
                roomBoundingBox.yMin = (int)partPos.y;
            if (partPos.y + 1 > roomBoundingBox.yMax)
                roomBoundingBox.yMax = (int)partPos.y + 1;
        }


        for (int x = 0; x < roomBoundingBox.width * partSize.x + 1; x++)
        {
            grid.Add(new());
            for (int y = 0; y < roomBoundingBox.height * partSize.y + 1; y++)
                grid[x].Add(new());
        }
    }

    public void InstantiateTiles()
    {
        for (int x = 0; x < grid.Count; x++)
        {
            for (int y = 0; y < grid[x].Count; y++)
            {
                Cell cell = grid[x][y];
                Vector2 pos = GridToRoom(new Vector2Int(x, y));
                switch (cell.type)
                {
                    case CellType.Floor:
                        cell.gameObject = Instantiate(PrefabManager.GetRandomFloor(), pos, Quaternion.identity, transform);
                        break;
                    case CellType.Wall:
                        cell.gameObject = Instantiate(PrefabManager.GetRandomWall(), pos, Quaternion.identity, transform);
                        break;
                    case CellType.Border:
                        cell.gameObject = Instantiate(PrefabManager.GetRandomOuterWall(), pos, Quaternion.identity, transform);
                        break;
                }
            }
        }

        NavMeshSurface navMeshInstance = Instantiate(PrefabManager.Instance.humanoidNavMesh, transform);
        navMeshInstance.GetComponent<RootSources2d>().RooySources.Add(gameObject);
        navMeshInstance.BuildNavMesh();
    }

    public void Initialize(Vector2 partSize, List<Vector2> partsPositions)
    {
        this.partSize = partSize;
        this._partsPositions = partsPositions;

        FillGrid();
    }
}
