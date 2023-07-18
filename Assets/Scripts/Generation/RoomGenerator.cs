using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    private enum Direction
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3
    }

    [Serializable]
    public class Range
    {
        public int minimum;
        public int maximum;

        public Range(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    private class Vertice
    {
        public Vector2 p1;
        public Vector2 p2;

        public Vertice(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    // Room generation parameters
    public Range columns = new(6, 15);
    public Range rows = new(6, 15);
    public Range numberOfParts = new(1, 4);
    public Range innerWallCountRange = new(3, 7);
    public Range innerWallLengthRange = new(4, 10);
    public Range enemiesCountRange = new(2, 4);

    /// Generated room caracteristics
    private Vector2 partSize;
    private int innerWallCount;
    private int innerWallLength;
    private int enemiesCount;

    /// Generated room layout
    // Boundaries of the camera (list of vertices)
    private List<Vertice> cameraBoundaries;
    // List of empty positions
    private List<Vector2> emptyPositions;

    // Generated room object
    public Room room;

    // Helper methods
    private Vector2 MoveInDirection(Vector2 position, Direction direction)
    {
        return direction switch
        {
            Direction.top => new Vector2(position.x, position.y + 1),
            Direction.right => new Vector2(position.x + 1, position.y),
            Direction.bottom => new Vector2(position.x, position.y - 1),
            Direction.left => new Vector2(position.x - 1, position.y),
            _ => position,
        };
    }

    private Direction NextDirection(Direction direction)
    {
        if (direction == Direction.left)
            return 0;
        else
            return direction + 1;
    }

    private Vertice BorderDirectionToVertice(Direction direction, Vector2 partPos)
    {
        return direction switch
        {
            Direction.top => new Vertice(
                    new Vector2(partPos.x, partPos.y + 1) * partSize,
                    new Vector2(partPos.x + 1, partPos.y + 1) * partSize),
            Direction.right => new Vertice(
                    new Vector2(partPos.x + 1, partPos.y) * partSize,
                    new Vector2(partPos.x + 1, partPos.y + 1) * partSize),
            Direction.bottom => new Vertice(
                    partPos * partSize,
                    new Vector2(partPos.x + 1, partPos.y) * partSize),
            Direction.left => new Vertice(
                    partPos * partSize,
                    new Vector2(partPos.x, partPos.y + 1) * partSize),
            _ => null,
        };
    }

    private void RoomSetup()
    {
        room = new GameObject("Room", typeof(Room)).GetComponent<Room>();
        partSize = new Vector2(Random.Range(columns.minimum, columns.maximum + 1), Random.Range(rows.minimum, rows.maximum + 1));
        List<Vector2> partsPositions = GeneratePartsPositions();
        emptyPositions = new();

        // Define where we'll place our parts
        room.Initialize(partSize, partsPositions);

        // Generate our parts borders and the camera limits
        cameraBoundaries = new();
        for (int i = 0; i < partsPositions.Count; i++)
            GeneratePart(i);
    }


    private List<Vector2> GeneratePartsPositions()
    {
        // Path from the first room part to the last one
        Vector2 currentPartPosition = Vector2.zero;
        int roomPartsCount = Random.Range(numberOfParts.minimum, numberOfParts.maximum + 1) - 1;

        List<Vector2> partsPositions = new() { currentPartPosition };
        //Calculate layout of other parts of the room
        for (int i = 0; i < roomPartsCount; i++)
        {
            Direction? direction = GetValidPartDirection(partsPositions, (Direction)Random.Range(0, 4));
            if (direction != null)
            {
                currentPartPosition = MoveInDirection(currentPartPosition, (Direction)direction);
                partsPositions.Add(currentPartPosition);
            }
        }
        return partsPositions;
    }

    private Direction? GetValidPartDirection(List<Vector2> partsPositions, Direction direction, int depth = 0)
    {
        // Avoid infinite recursive loop
        if (depth > 4)
            return null;
        // Don't need to check first room 
        if (partsPositions.Count == 0)
            return direction;
        // Test if there is no part at the target position
        if (partsPositions.Contains(MoveInDirection(partsPositions[^1], direction)))
            return GetValidPartDirection(partsPositions, NextDirection(direction), depth + 1);
        return direction;
    }

    private void GeneratePart(int partIndex)
    {
        Vector2 partPos = room.partsPositions[partIndex];
        List<Direction> borderDirections = GetPartBorderDirections(partPos);

        foreach (Direction dir in borderDirections)
            cameraBoundaries.Add(BorderDirectionToVertice(dir, partPos));

        for (int x = 0; x < partSize.x + 1; x++)
        {
            for (int y = 0; y < partSize.y + 1; y++)
            {
                Vector2 cellPos = new Vector2(x, y) + partPos * partSize;
                Room.Cell cell = room.GetCellAt(cellPos);

                /// We are on a border cell and we need to place a border on this direction
                if ((x == 0 && borderDirections.Contains(Direction.left)) ||
                    (x == partSize.x && borderDirections.Contains(Direction.right)) ||
                    (y == 0 && borderDirections.Contains(Direction.bottom)) ||
                    (y == partSize.y && borderDirections.Contains(Direction.top)))
                    cell.type = Room.CellType.Border;
                else
                {
                    cell.type = Room.CellType.Empty;
                    emptyPositions.Add(cellPos);
                }
            }
        }
    }

    // Return the direction in which the part must have border (ie is not linked to another part)
    private List<Direction> GetPartBorderDirections(Vector2 partPos)
    {
        List<Direction> borderDirections = new();

        foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            // If there is no room in the direction, we need to set borders
            if (room.partsPositions.IndexOf(MoveInDirection(partPos, dir)) == -1)
                borderDirections.Add(dir);
        return borderDirections;
    }

    private bool GenerateInnerWalls()
    {
        innerWallCount = Random.Range(innerWallCountRange.minimum, innerWallCountRange.maximum + 1) * room.partsPositions.Count;
        innerWallLength = Random.Range(innerWallLengthRange.minimum, innerWallLengthRange.maximum + 1) + room.partsPositions.Count * 2;
        EnsureNumberOfWallIsValid();
        for (int i = 0; i < innerWallCount; i++)
        {
            // Try up to three times to generate a wall (avoid infinite loop)
            int retry = 3;

            while (!GenerateInnerWall() && retry > 0)
                --retry;
        }
        return FloodFill();
    }

    private bool GenerateInnerWall()
    {
        Vector2? startingPosition = GetInnerWallStartPosition();
        // Couldn't find a valid starting position
        if (startingPosition == null)
        {
            innerWallCount--;
            return false;
        }

        Vector2 currentPosition = (Vector2)startingPosition;
        List<Vector2> wallPositions = new List<Vector2>{
                currentPosition
            };
        int wallLength = (int)Random.Range(innerWallLength * 0.7f, innerWallLength * 1.3f);
        for (int i = 0; i < wallLength; i++)
        {
            List<Vector2> possiblePositions = new();
            foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                Vector2 pos = MoveInDirection(currentPosition, dir);
                if (room.IsCellEmpty(pos) && !wallPositions.Contains(pos) && !room.IsCellNextToWall(pos))
                    possiblePositions.Add(pos);
            }

            // If there is no possibility to place a wall we stop there
            if (possiblePositions.Count == 0)
                break;
            // Else if the position is valid, we add it to the wall positions
            currentPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];
            wallPositions.Add(currentPosition);
        }

        // The wall is valid if it is long enough
        bool isWallValid = wallPositions.Count > wallLength * 0.70;
        if (isWallValid)
            foreach (Vector2 position in wallPositions)
            {
                emptyPositions.Remove(position);
                room.GetCellAt(position).type = Room.CellType.Wall;
            }
        return isWallValid;
    }

    private void EnsureNumberOfWallIsValid()
    {
        while (innerWallCount * innerWallLength > partSize.x * partSize.y * 0.8)
        {
            if (innerWallCount > 1)
                innerWallCount--;
            if (innerWallLength > 1)
                innerWallLength--;
        }
    }

    private Vector2? GetInnerWallStartPosition(int depth = 0)
    {
        if (depth > 40)
            return null;

        int randomIndex = Random.Range(0, emptyPositions.Count);
        Vector2 randomPosition = emptyPositions[randomIndex];

        if (!room.IsCellTypeAroundPosition(randomPosition, new Room.CellType[] { Room.CellType.Wall }))
            return randomPosition;
        return GetInnerWallStartPosition(depth + 1);
    }

    private bool FloodFill()
    {
        int totalSizeOfRoom = (int)partSize.x * (int)partSize.y * room.partsPositions.Count;
        Vector2 startingPoint = emptyPositions[0];
        List<Vector2> visitedPositions = new();

        FloodFill(startingPoint, visitedPositions);
        if (visitedPositions.Count < totalSizeOfRoom * 0.70)
        {
            // We may have flood filled a small part of the room
            foreach (Vector2 visitedPosition in visitedPositions)
                emptyPositions.Remove(visitedPosition);
            if (emptyPositions.Count == 0)
                return false;

            startingPoint = emptyPositions[0];
            visitedPositions = new();
            FloodFill(startingPoint, visitedPositions);

            //check if the room is big enough else regenerate the room
            if (visitedPositions.Count < totalSizeOfRoom * 0.70)
                return false;
        }

        emptyPositions = visitedPositions;
        foreach (Vector2 floor in visitedPositions)
            room.GetCellAt(floor).type = Room.CellType.Floor;
        return true;
    }

    private void FloodFill(Vector2 evaluatedPos, List<Vector2> visited)
    {
        if (!room.IsCellEmpty(evaluatedPos) || visited.Contains(evaluatedPos))
            return;
        visited.Add(evaluatedPos);

        foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            FloodFill(MoveInDirection(evaluatedPos, dir), visited);
    }

    private bool GenerateRoomTiles()
    {
        RoomSetup();
        // This might fail and calls a room regeneration
        if (!GenerateInnerWalls())
            return false;
        room.InstantiateTiles();
        return true;
    }

    private void SetupCameraBoundaries()
    {
        // change polygon collider points to match the map boundaries
        List<Vector2> mapBoundaries = new List<Vector2> { };

        // if (cameraBoundaries.Count == 0)
        //     return;
        mapBoundaries.Add(cameraBoundaries[0].p1);
        mapBoundaries.Add(cameraBoundaries[0].p2);
        cameraBoundaries.RemoveAt(0);
        //Connect points to make a polygon

        while (cameraBoundaries.Count != 0)
        {
            Vertice temp = null;
            foreach (Vertice vertice in cameraBoundaries)
            {
                if (vertice.p1 == mapBoundaries[^1] || vertice.p2 == mapBoundaries[^1])
                {
                    temp = vertice;
                    cameraBoundaries.Remove(vertice);
                }
                if (temp != null)
                    break;
            }

            if (temp.p1 == mapBoundaries[^1])
                mapBoundaries.Add(temp.p2);
            else
                mapBoundaries.Add(temp.p1);
        }
        room.roomBoundaries = mapBoundaries;
        GameObject.Find("MapBoundary").GetComponent<PolygonCollider2D>().points = room.roomBoundaries.ToArray();

    }

    public Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, emptyPositions.Count);
        Vector2 RandomPosition = emptyPositions[randomIndex];
        emptyPositions.RemoveAt(randomIndex);
        return RandomPosition;
    }

    private void PlacePlayer()
    {
        Vector2 randomPosition = RandomPosition();
        if (!PlayerController.Instantiated())
            Instantiate(PrefabManager.Instance.player, randomPosition, Quaternion.identity);
        else
            PlayerController.Instance.transform.position = randomPosition;

        //Set virtual camera to follow player
        var vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = PlayerController.Instance.transform;

        if (AICompanion.instance == null)
            Instantiate(PrefabManager.Instance.companion, randomPosition, Quaternion.identity);
        else
            AICompanion.instance.transform.position = randomPosition;
        AICompanion.instance.roomGenerator = this;
    }

    private void PlaceEnemies()
    {
        enemiesCount = Random.Range(enemiesCountRange.minimum, enemiesCountRange.maximum + 1);

        for (int i = 0; i < enemiesCount; i++)
        {
            Vector2 randomPosition = RandomPosition();
            Instantiate(PrefabManager.GetRandomEnemy(), randomPosition, Quaternion.identity, room.transform).GetComponent<Enemy>().target = PlayerController.Instance.gameObject;
        }
    }

    private void GenerateRoomEntities()
    {
        PlacePlayer();
        PlaceEnemies();
    }

    public Room GenerateRoom()
    {
        if (!GenerateRoomTiles())
            return Regenerate();
        GenerateRoomEntities();
        SetupCameraBoundaries();
        return room;
    }

    public Room Regenerate()
    {
        DestroyImmediate(room.gameObject);
        return GenerateRoom();
    }
}
