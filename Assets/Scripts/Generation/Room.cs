using System;
using System.Collections.Generic;
using NavMeshPlus.Components;
using NavMeshPlus.Extensions;
using UnityEngine;
using static RoomGenerator;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public enum CellType
    {
        None = -1,      // Out of room
        Empty = 0,      // Nothing on the cell
        Floor = 1,      // Floor
        Border = 2,     // Border wall
        Wall = 3,       // Inner wall
        Door = 4        // Door
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

    public struct DoorsDiff
    {
        //the diff between 2 doors
        public Vector2 offset;
        //door of this room 
        public Vector2 doorPossiblePos;
        //door of last otherRooms
        public Vector2 otherRoomDoor;

        //bounding box of the room base on parts
        public List<RectInt> partSizeBoundingBox;
    }

    private Vector2 _partSize;
    public Vector2 partSize { get { return _partSize; } }

    /// Generated room layout
    // Position of the room parts
    private List<Vector2> _partsPositions;
    public List<Vector2> partsPositions { get { return _partsPositions; } }

    // Room bounding box (minimum box where all parts fits in)
    private RectInt _roomBoundingBox;
    public RectInt roomBoundingBox { get { return _roomBoundingBox; } }
    // Boundaries of the camera (list of points)
    public List<Vector2> roomBoundaries;

    // Associative double dimension array representing the cells
    private GameObject tiles;
    private List<List<Cell>> grid;

    // Possible doors positions
    public List<Vector2> doorsPossiblePositions;

    // Room offset
    public Vector2 offset;

    //bounding box of the room base on parts
    public List<RectInt> partSizeBoundingBox = new();
    // List of empty positions
    public List<Vector2> emptyPositions;

    public List<Door> doors = new();

    public int remainingEnemies = 0;

    public Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, emptyPositions.Count);
        Vector2 RandomPosition = emptyPositions[randomIndex];
        emptyPositions.RemoveAt(randomIndex);
        return RandomPosition;
    }

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

    public Vector2Int GetPartPositionFromRoomPosition(Vector2 roomPosition)
    {
        Vector2 floatPos = roomPosition / partSize;
        return new Vector2Int((int)(floatPos.x - ((floatPos.x < 0) ? 1 : 0)), (int)(floatPos.y - ((floatPos.y < 0) ? 1 : 0)));
    }

    private void FillGrid()
    {
        _roomBoundingBox = new();
        grid = new();
        foreach (Vector2 partPos in partsPositions)
        {
            if (partPos.x < roomBoundingBox.xMin)
                _roomBoundingBox.xMin = (int)partPos.x;
            if (partPos.x + 1 > roomBoundingBox.xMax)
                _roomBoundingBox.xMax = (int)partPos.x + 1;
            if (partPos.y < roomBoundingBox.yMin)
                _roomBoundingBox.yMin = (int)partPos.y;
            if (partPos.y + 1 > roomBoundingBox.yMax)
                _roomBoundingBox.yMax = (int)partPos.y + 1;
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
                GameObject prefab = null;
                switch (cell.type)
                {
                    case CellType.Floor:
                        prefab = PrefabManager.GetRandomFloor();
                        break;
                    case CellType.Wall:
                        prefab = PrefabManager.GetRandomWall();
                        break;
                    case CellType.Border:
                        prefab = PrefabManager.GetRandomOuterWall();
                        break;
                }
                if (prefab != null)
                    cell.gameObject = Instantiate(prefab, pos, Quaternion.identity, tiles.transform);
            }
        }

        foreach (NavMeshSurface surface in PrefabManager.Instance.navMeshSurfaces)
        {
            NavMeshSurface s = Instantiate(surface, transform).GetComponent<NavMeshSurface>();
            s.GetComponent<RootSources2d>().RooySources.Add(gameObject);
            s.BuildNavMesh();
        }
    }

    public void Initialize(Vector2 partSize, List<Vector2> partsPositions)
    {
        tiles = new GameObject("Tiles");
        tiles.transform.SetParent(transform);

        this._partSize = partSize;
        this._partsPositions = partsPositions;

        FillGrid();
    }

    public Direction? getFreeDirectionFromDoor(Vector2 door)
    {
        foreach (Direction direction in (Direction[])Enum.GetValues(typeof(Direction)))
        {
            Cell cell = GetCellAt(MoveInDirection(door, direction));
            // if (cell == null || cell.type == CellType.Empty)
            if (cell == null)// && cell.type == CellType.Border)
                return direction;
        }
        return null;
    }

    public void ComputePartSizeBoundingBox()
    {
        partSizeBoundingBox.Clear();
        foreach (Vector2 partPos in partsPositions)
        {
            RectInt boundingBox = new()
            {
                x = (int)(partPos.x * partSize.x),
                y = (int)(partPos.y * partSize.y),
                width = (int)partSize.x,
                height = (int)partSize.y
            };

            partSizeBoundingBox.Add(boundingBox);
        }
    }

    public List<DoorsDiff> getPossibleConnections(List<Room> otherRooms, Room otherRoom)
    {
        List<DoorsDiff> possibleConnections = new();

        foreach (Vector2 doorPossiblePos in doorsPossiblePositions)
        {
            foreach (Vector2 otherRoomDoor in otherRoom.doorsPossiblePositions)
            {
                // Offset from the other room local pos to this room local pos ??
                offset = doorPossiblePos - otherRoomDoor;

                // Get free direction from door else it's not valid
                Direction? doorPossibleDir = getFreeDirectionFromDoor(doorPossiblePos);
                if (doorPossibleDir == null)
                    continue;

                // Get free direction from otherDoor else it's not valid
                Direction? otherRoomDoorDir = otherRoom.getFreeDirectionFromDoor(otherRoomDoor);
                if (otherRoomDoorDir == null)
                    continue;

                // If they are not facing each other it's not valid
                if (((int)doorPossibleDir % 2) != (int)otherRoomDoorDir % 2)
                    continue;


                List<RectInt> partSizeBoundingBox = new List<RectInt>();
                List<RectInt> otherPartSizeBoundingBox = new List<RectInt>();
                //Calculate bounding box of the room base on parts
                foreach (Vector2 partPos in partsPositions)
                {
                    RectInt boundingBox = new();
                    boundingBox.x = (int)(partPos.x * partSize.x);
                    boundingBox.y = (int)(partPos.y * partSize.y);
                    boundingBox.width = (int)(partSize.x);
                    boundingBox.height = (int)(partSize.y);
                    boundingBox.x += -(int)(offset.x - otherRoom.offset.x);
                    boundingBox.y += -(int)(offset.y - otherRoom.offset.y);

                    partSizeBoundingBox.Add(boundingBox);
                }

                bool isValid = true;
                //Check if they are colliding
                foreach (RectInt boundingBox in partSizeBoundingBox)
                {
                    foreach (Room room in otherRooms)
                    {
                        foreach (RectInt otherBoundingBox in room.partSizeBoundingBox)
                        {
                            if (boundingBox.Overlaps(otherBoundingBox))
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (!isValid)
                            break;
                    }
                    if (!isValid)
                        break;
                }

                if (isValid)
                    possibleConnections.Add(new() { offset = -offset, doorPossiblePos = doorPossiblePos, otherRoomDoor = otherRoomDoor, partSizeBoundingBox = partSizeBoundingBox });
            }
        }

        return possibleConnections;
    }
}
