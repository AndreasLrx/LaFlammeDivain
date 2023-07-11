using System;
using System.Collections.Generic;
using Cinemachine;
using NavMeshPlus.Components;
using NavMeshPlus.Extensions;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public Count columns = new(6, 15);
    public Count rows = new(6, 15);

    public Count numberOfRoomParts = new(1, 4);

    // Size of the room parts in format -> Vector2(columns, rows)
    private Vector2 roomPartSize;

    public int enemiesCount = 1;

    public Count innerWallCountRange = new(3, 7);
    public Count innerWallLengthRange = new(4, 10);
    private int innerWallCount = 3;
    private int innerWallLength = 5;
    public NavMeshSurface navMeshSurface;

    private Player player;
    private Transform boardHolder;
    private List<Vector2> gridPositions = new();
    private List<Vector2> outerWallGridPositions = new();
    private List<Vector2> innerWallGridPositions = new();

    private int numberOfRoom = 1;

    // Contains the room parts positions
    private List<Vector2> roomParts;
    private List<List<Vector2>> cameraBoundaries = new();

    private enum Direction
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3
    }

    Vector2 MoveInDirection(Vector2 position, Direction direction)
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


    void BoardSetup()
    {
        roomPartSize = new Vector2(Random.Range(columns.minimum, columns.maximum + 1), Random.Range(rows.minimum, rows.maximum + 1));
        numberOfRoom = Random.Range(numberOfRoomParts.minimum, numberOfRoomParts.maximum + 1) - 1;
        boardHolder = new GameObject("Board").transform;

        Vector2 currentRoomPartPosition = Vector2.zero;
        List<Direction> roomDirections = new List<Direction>();
        roomParts = new List<Vector2>() { currentRoomPartPosition };

        //Calculate layout of other parts of the room
        for (int i = 0; i < numberOfRoom; i++)
        {
            Direction? direction = GetValidRoomPartDirection((Direction)Random.Range(0, 4));
            if (direction != null)
            {
                currentRoomPartPosition = MoveInDirection(currentRoomPartPosition, (Direction)direction);
                roomDirections.Add((Direction)direction);
                roomParts.Add(currentRoomPartPosition);
            }
        }
        //Generate first room part
        currentRoomPartPosition = Vector2.zero;
        GenerateRoomPart(currentRoomPartPosition, 0);

        //Generate other room parts
        int index = 1;
        foreach (Direction dir in roomDirections)
        {
            currentRoomPartPosition = MoveInDirection(currentRoomPartPosition, dir);
            GenerateRoomPart(currentRoomPartPosition, index++);
        }
    }

    Direction NextDirection(Direction direction)
    {
        if (direction == Direction.left)
            return 0;
        else
            return direction + 1;
    }

    Direction? GetValidRoomPartDirection(Direction direction, int depth = 0)
    {
        // Avoid infinite recursive loop
        if (depth > 4)
            return null;
        // Don't need to check first room 
        if (roomParts.Count == 0)
            return direction;
        // Test if the direction is available
        if (roomParts.Contains(MoveInDirection(roomParts[^1], direction)))
            return GetValidRoomPartDirection(NextDirection(direction), depth + 1);
        return direction;
    }

    void GenerateRoomPart(Vector2 position, int roomPartId)
    {
        List<Direction> wallPositions = CheckWhereToInstantiateWall(roomParts, roomPartId);
        if (wallPositions.Contains(Direction.left))
            cameraBoundaries.Add(new List<Vector2> {
                position * roomPartSize,
                new Vector2(position.x, position.y + 1) * roomPartSize
            });
        if (wallPositions.Contains(Direction.right))
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(position.x + 1, position.y) * roomPartSize,
                new Vector2(position.x + 1, position.y + 1) * roomPartSize
            });
        if (wallPositions.Contains(Direction.top))
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(position.x, position.y + 1) * roomPartSize,
                new Vector2(position.x + 1, position.y + 1) * roomPartSize
            });
        if (wallPositions.Contains(Direction.bottom))
            cameraBoundaries.Add(new List<Vector2> {
                position * roomPartSize,
                new Vector2(position.x + 1, position.y) * roomPartSize
            });

        for (int x = 0; x < roomPartSize.x + 1; x++)
        {
            for (int y = 0; y < roomPartSize.y + 1; y++)
            {
                Vector2 wallPos = new Vector2(x, y) + position * roomPartSize;

                if ((x == 0 && wallPositions.Contains(Direction.left)) ||
                    (x == roomPartSize.x && wallPositions.Contains(Direction.right)) ||
                    (y == 0 && wallPositions.Contains(Direction.bottom)) ||
                    (y == roomPartSize.y && wallPositions.Contains(Direction.top)))
                {
                    //Don't add a wall if there is already one
                    if (!outerWallGridPositions.Contains(wallPos) && !gridPositions.Contains(wallPos))
                    {
                        Instantiate(PrefabManager.GetRandomOuterWall(), wallPos, Quaternion.identity, boardHolder);
                        outerWallGridPositions.Add(wallPos);
                    }
                }
                else
                {
                    //Don't add a floor tile if there is already one
                    if (!gridPositions.Contains(wallPos) && !outerWallGridPositions.Contains(wallPos))
                        gridPositions.Add(wallPos);
                }
            }
        }
    }

    List<Direction> CheckWhereToInstantiateWall(List<Vector2> map, int roomPos)
    {
        List<Direction> wallPositions = new List<Direction>();


        Vector2 actualRoom = map[roomPos];
        map.RemoveAt(roomPos);
        //Check if there is a room on the left
        if (!map.Contains(new Vector2(actualRoom.x - 1, actualRoom.y)))
            //Instantiate wall on the left
            wallPositions.Add(Direction.left);
        //Check if there is a room on the right
        if (!map.Contains(new Vector2(actualRoom.x + 1, actualRoom.y)))
            //Instantiate wall on the right
            wallPositions.Add(Direction.right);
        //Check if there is a room on the top
        if (!map.Contains(new Vector2(actualRoom.x, actualRoom.y + 1)))
            //Instantiate wall on the top
            wallPositions.Add(Direction.top);
        //Check if there is a room on the bottom
        if (!map.Contains(new Vector2(actualRoom.x, actualRoom.y - 1)))
            //Instantiate wall on the bottom
            wallPositions.Add(Direction.bottom);
        map.Insert(roomPos, actualRoom);
        return wallPositions;
    }

    void SetupCameraBoundaries()
    {
        // change polygon collider points to match the map boundaries
        List<Vector2> mapBoundaries = new List<Vector2> { };

        mapBoundaries.Add(cameraBoundaries[0][0]);
        mapBoundaries.Add(cameraBoundaries[0][1]);
        cameraBoundaries.RemoveAt(0);
        //Connect points to make a polygon

        while (cameraBoundaries.Count != 0)
        {
            List<Vector2> temp = null;
            foreach (var item in cameraBoundaries)
            {
                if (item[0] == mapBoundaries[^1] || item[1] == mapBoundaries[^1])
                {
                    temp = item;
                    cameraBoundaries.Remove(item);
                }
                if (temp != null)
                    break;
            }

            if (temp[0] == mapBoundaries[^1])
                mapBoundaries.Add(temp[1]);
            else
                mapBoundaries.Add(temp[0]);
        }

        GameObject.Find("MapBoundary").GetComponent<PolygonCollider2D>().points = mapBoundaries.ToArray();
    }

    Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 RandomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return RandomPosition;
    }

    Vector2 RandomPositionNotCloseToInnerWall(List<Vector2> innerWallGridPositions, int depth = 0)
    {
        if (depth > 40)
            throw new Exception("Room is too small for the amount of inner walls");

        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];

        foreach (var innerWall in innerWallGridPositions)
        {
            int diffX = (int)Math.Abs((float)(innerWall.x - randomPosition.x));
            int diffY = (int)Math.Abs((float)(innerWall.y - randomPosition.y));

            if (diffX + diffY > 5)
                break;

            randomIndex = Random.Range(0, gridPositions.Count);
            randomPosition = gridPositions[randomIndex];
        }

        if (!CheckIfPositionIsNextToPositions(randomPosition, innerWallGridPositions))
        {
            gridPositions.RemoveAt(randomIndex);
            return randomPosition;
        }
        return RandomPositionNotCloseToInnerWall(innerWallGridPositions, depth + 1);
    }

    bool CheckIfPositionIsNextToPositions(Vector2 position, List<Vector2> positions)
    {
        foreach (var pos in positions)
        {
            int diffX = (int)Math.Abs((float)(pos.x - position.x));
            int diffY = (int)Math.Abs((float)(pos.y - position.y));

            if (diffX + diffY > 0 && diffX + diffY <= 2 && diffX != 2 && diffY != 2)
                return true;
        }
        return false;
    }

    bool CheckIfPositionIsNextToWall(Vector2 position, List<Vector2> innerWalls)
    {
        return (CheckIfPositionIsNextToPositions(position, outerWallGridPositions)
        || CheckIfPositionIsNextToPositions(position, innerWallGridPositions))
        && !CheckIfPositionIsNextToPositions(position, innerWalls);
    }

    void ChangeNumberOfWall()
    {
        if (roomPartSize.x * roomPartSize.y * 0.8 < innerWallCount * innerWallLength)
        {
            if (innerWallCount > 1)
                innerWallCount--;
            if (innerWallLength > 1)
                innerWallLength--;
            ChangeNumberOfWall();
        }
    }

    void LayoutWallAtRandom()
    {
        innerWallCount = Random.Range(innerWallCountRange.minimum, innerWallCountRange.maximum + 1) * numberOfRoom;
        innerWallLength = Random.Range(innerWallLengthRange.minimum, innerWallLengthRange.maximum + 1) + numberOfRoom * 2;
        ChangeNumberOfWall();
        for (int i = 0; i < innerWallCount; i++)
        {
            List<Vector2> innerWalls = new List<Vector2>();
            bool isTouchingWall = false;
            Vector2 startingPosition = RandomPositionNotCloseToInnerWall(innerWallGridPositions);

            GameObject tileChoice = PrefabManager.GetRandomOuterWall();
            List<Vector2> wallPath = new List<Vector2>{
                startingPosition
            };
            innerWallGridPositions.Add(startingPosition);
            innerWalls.Add(startingPosition);
            if (CheckIfPositionIsNextToWall(startingPosition, innerWallGridPositions))
                isTouchingWall = true;

            Vector2 randomPosition = startingPosition;
            bool shouldGenerateWall = true;

            for (int a = 0; a < innerWallLength - 1; a++)
            {
                List<Vector2> possiblePositions = new List<Vector2>() {
                    new Vector2(randomPosition.x + 1, randomPosition.y),
                    new Vector2(randomPosition.x, randomPosition.y + 1),
                    new Vector2(randomPosition.x - 1, randomPosition.y),
                    new Vector2(randomPosition.x, randomPosition.y -1),
                    };
                foreach (var item in innerWalls)
                    possiblePositions.Remove(item);

                foreach (var item in outerWallGridPositions)
                    possiblePositions.Remove(item);

                if (possiblePositions.Count == 0)
                {
                    if (wallPath.Count < innerWallLength * 0.90)
                    {
                        wallPath.Add(randomPosition);
                        gridPositions.Remove(randomPosition);
                        shouldGenerateWall = false;
                    }
                    i--;
                    break;
                }
                randomPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

                if (!gridPositions.Contains(randomPosition) ||
                isTouchingWall ||
                CheckIfPositionIsNextToWall(randomPosition, innerWalls)
                )
                {
                    if (wallPath.Count < innerWallLength * 0.90)
                    {
                        wallPath.Add(randomPosition);
                        gridPositions.Remove(randomPosition);
                        shouldGenerateWall = false;
                    }
                    i--;
                    break;
                }

                innerWalls.Add(randomPosition);
                innerWallGridPositions.Add(randomPosition);

                wallPath.Add(randomPosition);
                gridPositions.Remove(randomPosition);
            }
            if (shouldGenerateWall)
            {
                foreach (var position in wallPath)
                {
                    Instantiate(tileChoice, position, Quaternion.identity, boardHolder);
                }
            }
            else
            {
                foreach (var position in wallPath)
                {
                    innerWallGridPositions.Remove(position);
                    gridPositions.Add(position);
                }
            }
        }
        //Flood fill to lay floor tiles
        Vector2 startingPoint = gridPositions[0];
        List<Vector2> oldGridPositions = gridPositions;
        gridPositions = new List<Vector2>();
        FloodFill(startingPoint);

        //Check if the room is big enough else remove the floor tiles and redo FloodFill
        int totalSizeOfRoom = (int)roomPartSize.x * (int)roomPartSize.y * (numberOfRoom + 1);

        if (gridPositions.Count < totalSizeOfRoom * 0.70)
        {
            foreach (var item in gridPositions)
            {
                oldGridPositions.Remove(item);
            }
            if (oldGridPositions.Count == 0)
            {
                gridPositions = new List<Vector2>();
                outerWallGridPositions = new List<Vector2>();
                innerWallGridPositions = new List<Vector2>();
                cameraBoundaries = new List<List<Vector2>>();
                DestroyImmediate(GameObject.Find("Board"));
                boardHolder = null;
                BoardSetup();
                LayoutWallAtRandom();
                return;
            }

            startingPoint = oldGridPositions[0];
            for (int i = 0; i < boardHolder.childCount; i++)
            {
                if (boardHolder.GetChild(i).tag == "Floor")
                {
                    DestroyImmediate(boardHolder.GetChild(i).gameObject);
                    i--;
                }
            }
            gridPositions = new List<Vector2>();
            FloodFill(startingPoint);

            //check if the room is big enough else regenerate the room
            if (gridPositions.Count < totalSizeOfRoom * 0.70)
            {
                gridPositions = new List<Vector2>();
                outerWallGridPositions = new List<Vector2>();
                innerWallGridPositions = new List<Vector2>();
                cameraBoundaries = new List<List<Vector2>>();
                DestroyImmediate(GameObject.Find("Board"));
                boardHolder = null;
                BoardSetup();
                LayoutWallAtRandom();
                return;
            }
        }
    }

    void FloodFill(Vector2 startingPoint)
    {
        if (outerWallGridPositions.Contains(startingPoint) || innerWallGridPositions.Contains(startingPoint))
            return;
        if (gridPositions.Contains(startingPoint))
            return;

        GameObject toInstantiate = PrefabManager.GetRandomFloor();
        Instantiate(toInstantiate, startingPoint, Quaternion.identity, boardHolder);
        gridPositions.Add(startingPoint);

        FloodFill(new Vector2(startingPoint.x, startingPoint.y - 1));
        FloodFill(new Vector2(startingPoint.x + 1, startingPoint.y));
        FloodFill(new Vector2(startingPoint.x, startingPoint.y + 1));
        FloodFill(new Vector2(startingPoint.x - 1, startingPoint.y));
    }

    void LayoutEnemiesAtRandom(GameObject enemy)
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            Vector2 randomPosition = RandomPosition();
            Instantiate(enemy, randomPosition, Quaternion.identity).GetComponent<Enemy>().target = player.gameObject;
        }
    }

    void LayoutPlayer()
    {
        Vector2 randomPosition = RandomPosition();
        player = Instantiate(PrefabManager.Instance.player, randomPosition, Quaternion.identity);

        //Set virtual camera to follow player
        var vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = player.transform;
    }

    void BuildNavMesh()
    {
        NavMeshSurface navMeshInstance = Instantiate(navMeshSurface, boardHolder);
        navMeshInstance.GetComponent<RootSources2d>().RooySources.Add(boardHolder.gameObject);
        navMeshInstance.BuildNavMesh();
    }

    public void SetupScene()
    {
        BoardSetup();
        LayoutWallAtRandom();
        LayoutPlayer();
        // LayoutEnemiesAtRandom(enemy);
        SetupCameraBoundaries();
        BuildNavMesh();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug new wisp
        if (Input.GetKeyDown(KeyCode.T))
            player.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), player.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        // Reset level
        if (Input.GetKeyDown(KeyCode.M))
        {
            DestroyImmediate(GameObject.Find("Board"));
            boardHolder = null;
            DestroyImmediate(player.gameObject);
            player = null;
            gridPositions = new List<Vector2>();
            outerWallGridPositions = new List<Vector2>();
            innerWallGridPositions = new List<Vector2>();
            SetupScene();
        }
    }
}
