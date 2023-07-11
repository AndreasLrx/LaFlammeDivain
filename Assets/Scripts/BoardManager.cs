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

    public Count columns = new Count(6, 15);
    public Count rows = new Count(6, 15);

    public Count numberOfRoomParts = new Count(1, 4);

    private int columnsCount;
    private int rowsCount;

    public int enemiesCount = 1;

    public Count innerWallCountRange = new Count(3, 7);
    public Count innerWallLengthRange = new Count(4, 10);
    private int innerWallCount = 3;
    private int innerWallLength = 5;

    public GameObject[] outerWallTiles;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject enemy;
    public GameObject[] wisps;
    public Player playerPrefab;
    public NavMeshSurface navMeshSurface;

    private Player player;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    private List<Vector2> outerWallGridPositions = new List<Vector2>();
    private List<Vector2> innerWallGridPositions = new List<Vector2>();

    private int numberOfRoom = 1;

    private int depth = 0;

    private List<Vector2> map;

    private List<List<Vector2>> cameraBoundaries = new List<List<Vector2>>();

    private enum Direction
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3
    }
    void BoardSetup()
    {
        columnsCount = Random.Range(columns.minimum, columns.maximum + 1);
        rowsCount = Random.Range(rows.minimum, rows.maximum + 1);

        numberOfRoom = Random.Range(numberOfRoomParts.minimum, numberOfRoomParts.maximum + 1) - 1;

        boardHolder = new GameObject("Board").transform;

        int x = 0;
        int y = 0;
        List<Direction> roomPosition = new List<Direction>();
        map = new List<Vector2>() { new Vector2(x, y) };

        //Calculate layout of other parts of the room
        for (int i = 0; i < numberOfRoom; i++)
        {
            Direction dir = (Direction)Random.Range(0, 4);
            depth = 0;
            Direction? direction = CalculateValidPositions(map, dir);
            if (direction != null)
            {
                if (direction == Direction.top)
                {
                    roomPosition.Add(Direction.top);
                    y++;
                    map.Add(new Vector2(x, y));
                }
                else if (direction == Direction.right)
                {
                    roomPosition.Add(Direction.right);
                    x++;
                    map.Add(new Vector2(x, y));
                }
                else if (direction == Direction.bottom)
                {
                    roomPosition.Add(Direction.bottom);
                    y--;
                    map.Add(new Vector2(x, y));
                }
                else if (direction == Direction.left)
                {
                    roomPosition.Add(Direction.left);
                    x--;
                    map.Add(new Vector2(x, y));
                }
            }
        }
        //Generate first room part
        GenerateRoomPart(x, y, map, 0);

        //Generate other room parts
        int index = 1;
        foreach (var pos in roomPosition)
        {
            if (pos == Direction.top)
            {
                y += rowsCount;
                GenerateRoomPart(x, y, map, index); //top
            }
            else if (pos == Direction.right)
            {
                x += columnsCount;
                GenerateRoomPart(x, y, map, index); //right
            }
            else if (pos == Direction.bottom)
            {
                y -= rowsCount;
                GenerateRoomPart(x, y, map, index); //bottom
            }
            else if (pos == Direction.left)
            {
                x -= columnsCount;
                GenerateRoomPart(x, y, map, index); //left
            }
            index++;
        }
    }

    Direction NextDirection(Direction direction)
    {
        if (direction == Direction.left)
        {
            return 0;
        }
        else
        {
            return direction + 1;
        }
    }

    Direction? CalculateValidPositions(List<Vector2> map, Direction direction)
    {
        depth++;
        if (depth > 10)
        {
            return null;
        }
        if (map.Count == 0)
        {
            return direction;
        }

        Vector2 actualRoom = map[map.Count - 1];

        //Check if there is a room on the left
        if (map.Contains(new Vector2(actualRoom.x - 1, actualRoom.y)) && direction == Direction.left)
        {
            direction = NextDirection(direction);
            return CalculateValidPositions(map, direction);
        }
        //Check if there is a room on the right
        if (map.Contains(new Vector2(actualRoom.x + 1, actualRoom.y)) && direction == Direction.right)
        {
            direction = NextDirection(direction);
            return CalculateValidPositions(map, direction);
        }
        //Check if there is a room on the top
        if (map.Contains(new Vector2(actualRoom.x, actualRoom.y + 1)) && direction == Direction.top)
        {
            direction = NextDirection(direction);
            return CalculateValidPositions(map, direction);
        }
        //Check if there is a room on the bottom
        if (map.Contains(new Vector2(actualRoom.x, actualRoom.y - 1)) && direction == Direction.bottom)
        {
            direction = NextDirection(direction);
            return CalculateValidPositions(map, direction);
        }

        return direction;
    }

    void GenerateRoomPart(int xStart, int yStart, List<Vector2> map, int roomPos)
    {
        List<Direction> wallPositions = CheckWhereToInstantiateWall(map, roomPos);
        if (wallPositions.Contains(Direction.left))
        {
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(xStart, yStart),
                new Vector2(xStart, yStart + rowsCount)
            });
        }
        if (wallPositions.Contains(Direction.right))
        {
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(xStart + columnsCount, yStart),
                new Vector2(xStart + columnsCount, yStart + rowsCount)
            });
        }
        if (wallPositions.Contains(Direction.top))
        {
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(xStart, yStart + rowsCount),
                new Vector2(xStart + columnsCount, yStart + rowsCount)
            });
        }
        if (wallPositions.Contains(Direction.bottom))
        {
            cameraBoundaries.Add(new List<Vector2> {
                new Vector2(xStart, yStart),
                new Vector2(xStart + columnsCount, yStart)
            });
        }

        for (int x = xStart; x < columnsCount + xStart + 1; x++)
        {
            for (int y = yStart; y < rowsCount + yStart + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                Vector2 wallPos = new Vector2(x, y);

                if ((x == xStart && wallPositions.Contains(Direction.left)) || (x == (columnsCount + xStart) && wallPositions.Contains(Direction.right)) || (y == yStart && wallPositions.Contains(Direction.bottom)) || (y == (rowsCount + yStart) && wallPositions.Contains(Direction.top)))
                {
                    //Don't add a wall if there is already one
                    if (!outerWallGridPositions.Contains(wallPos) && !gridPositions.Contains(wallPos))
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        Instantiate(toInstantiate, wallPos, Quaternion.identity, boardHolder);
                        outerWallGridPositions.Add(wallPos);
                    }
                }
                else
                {
                    //Don't add a floor tile if there is already one
                    if (!gridPositions.Contains(wallPos) && !outerWallGridPositions.Contains(wallPos))
                    {
                        gridPositions.Add(wallPos);
                    }
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
        {
            //Instantiate wall on the left
            wallPositions.Add(Direction.left);
        }
        //Check if there is a room on the right
        if (!map.Contains(new Vector2(actualRoom.x + 1, actualRoom.y)))
        {
            //Instantiate wall on the right
            wallPositions.Add(Direction.right);
        }
        //Check if there is a room on the top
        if (!map.Contains(new Vector2(actualRoom.x, actualRoom.y + 1)))
        {
            //Instantiate wall on the top
            wallPositions.Add(Direction.top);
        }
        //Check if there is a room on the bottom
        if (!map.Contains(new Vector2(actualRoom.x, actualRoom.y - 1)))
        {
            //Instantiate wall on the bottom
            wallPositions.Add(Direction.bottom);
        }
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
                if (item[0] == mapBoundaries[mapBoundaries.Count - 1] || item[1] == mapBoundaries[mapBoundaries.Count - 1])
                {
                    temp = item;
                    cameraBoundaries.Remove(item);
                }
                if (temp != null)
                {
                    break;
                }
            }

            if (temp[0] == mapBoundaries[mapBoundaries.Count - 1])
            {
                mapBoundaries.Add(temp[1]);
            }
            else
            {
                mapBoundaries.Add(temp[0]);
            }
        }

        GameObject.Find("MapBoundary").GetComponent<PolygonCollider2D>().points = mapBoundaries.ToArray();
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 RandomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return RandomPosition;
    }

    Vector2 RandomPositionNotCloseToInnerWall(List<Vector2> innerWallGridPositions)
    {
        depth++;
        if (depth > 40)
        {
            throw new Exception("Room is too small for the amount of inner walls");
        }

        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];

        foreach (var innerWall in innerWallGridPositions)
        {
            int diffX = (int)Math.Abs((float)(innerWall.x - randomPosition.x));
            int diffY = (int)Math.Abs((float)(innerWall.y - randomPosition.y));

            if (diffX + diffY > 5)
            {
                break;
            }

            randomIndex = Random.Range(0, gridPositions.Count);
            randomPosition = gridPositions[randomIndex];
        }

        if (!CheckIfPositionIsNextToPositions(randomPosition, innerWallGridPositions))
        {
            gridPositions.RemoveAt(randomIndex);
            return randomPosition;
        }
        return RandomPositionNotCloseToInnerWall(innerWallGridPositions);
    }

    bool CheckIfPositionIsNextToPositions(Vector3 position, List<Vector2> positions)
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
        if ((columnsCount * rowsCount) * 0.8 < innerWallCount * innerWallLength)
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
            depth = 0;
            Vector2 startingPosition = RandomPositionNotCloseToInnerWall(innerWallGridPositions);

            GameObject tileChoice = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
            List<Vector2> wallPath = new List<Vector2>{
                startingPosition
            };
            innerWallGridPositions.Add(startingPosition);
            innerWalls.Add(startingPosition);
            if (CheckIfPositionIsNextToWall(startingPosition, innerWallGridPositions))
            {
                isTouchingWall = true;
            }

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
        Vector3 startingPoint = gridPositions[0];
        List<Vector3> oldGridPositions = gridPositions;
        gridPositions = new List<Vector3>();
        FloodFill(startingPoint);

        //Check if the room is big enough else remove the floor tiles and redo FloodFill
        int totalSizeOfRoom = columnsCount * rowsCount * (numberOfRoom + 1);

        if (gridPositions.Count < totalSizeOfRoom * 0.70)
        {
            foreach (var item in gridPositions)
            {
                oldGridPositions.Remove(item);
            }
            if (oldGridPositions.Count == 0)
            {
                gridPositions = new List<Vector3>();
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
            gridPositions = new List<Vector3>();
            FloodFill(startingPoint);

            //check if the room is big enough else regenerate the room
            if (gridPositions.Count < totalSizeOfRoom * 0.70)
            {
                gridPositions = new List<Vector3>();
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

    void FloodFill(Vector3 startingPoint)
    {
        if (outerWallGridPositions.Contains(startingPoint) || innerWallGridPositions.Contains(startingPoint))
            return;
        if (gridPositions.Contains(startingPoint))
            return;

        GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
        GameObject instance = Instantiate(toInstantiate, startingPoint, Quaternion.identity, boardHolder);
        gridPositions.Add(startingPoint);

        FloodFill(new Vector3(startingPoint.x, startingPoint.y - 1, 0));
        FloodFill(new Vector3(startingPoint.x + 1, startingPoint.y, 0));
        FloodFill(new Vector3(startingPoint.x, startingPoint.y + 1, 0));
        FloodFill(new Vector3(startingPoint.x - 1, startingPoint.y, 0));
    }

    void LayoutEnemiesAtRandom(GameObject enemy)
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(enemy, randomPosition, Quaternion.identity).GetComponent<Enemy>().target = player.gameObject;
        }
    }

    void LayoutPlayer(Player playerPrefab)
    {
        Vector3 randomPosition = RandomPosition();
        player = Instantiate(playerPrefab, randomPosition, Quaternion.identity);

        //Set virtual camera to follow player
        var vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = player.transform;
    }

    void BuildNavMesh()
    {
        NavMeshSurface navMeshInstance = Instantiate(navMeshSurface);
        navMeshInstance.GetComponent<RootSources2d>().RooySources.Add(boardHolder.gameObject);
        navMeshInstance.BuildNavMesh();
    }

    public void SetupScene()
    {
        BoardSetup();
        LayoutWallAtRandom();
        //LayoutEnemiesAtRandom(enemy);
        LayoutPlayer(playerPrefab);
        LayoutEnemiesAtRandom(enemy);
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
            player.AddWisp(Instantiate(wisps[Random.Range(0, wisps.Length)], player.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        // Reset level
        if (Input.GetKeyDown(KeyCode.M))
        {
            DestroyImmediate(GameObject.Find("Board"));
            boardHolder = null;
            DestroyImmediate(player.gameObject);
            player = null;
            gridPositions = new List<Vector3>();
            outerWallGridPositions = new List<Vector2>();
            innerWallGridPositions = new List<Vector2>();
            SetupScene();
        }
    }
}
