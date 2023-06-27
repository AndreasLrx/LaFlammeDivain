using System;
using System.Collections.Generic;
using Cinemachine;
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

    public int wallCount = 10;
    public int enemiesCount = 1;

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
    private List<Vector3> wallGridPositions = new List<Vector3>();

    void BoardSetup()
    {
        columnsCount = Random.Range(columns.minimum, columns.maximum + 1);
        rowsCount = Random.Range(rows.minimum, rows.maximum + 1);

        int numberOfRoom = Random.Range(numberOfRoomParts.minimum, numberOfRoomParts.maximum + 1) - 1;

        boardHolder = new GameObject("Board").transform;

        int x = 0;
        int y = 0;
        List<string> roomPosition = new List<string>();
        List<Vector2> m = new List<Vector2>() { new Vector2(x, y) };
        List<string> validPosition = new List<string>() { "top", "right", "bottom" };

        //Calculate layout of other parts of the room
        for (int i = 0; i < numberOfRoom; i++)
        {
            int randomIndex = Random.Range(0, 3);
            if (validPosition[randomIndex] == "top")
            {
                roomPosition.Add("top");
                y++;
                m.Add(new Vector2(x, y));
                validPosition = new List<string>() { "top", "right", "left" };
            }
            else if (validPosition[randomIndex] == "right")
            {
                roomPosition.Add("right");
                x++;
                m.Add(new Vector2(x, y));
                validPosition = new List<string>() { "top", "right", "bottom" };
            }
            else if (validPosition[randomIndex] == "bottom")
            {
                roomPosition.Add("bottom");
                y--;
                m.Add(new Vector2(x, y));
                validPosition = new List<string>() { "right", "bottom", "left" };
            }
            else if (validPosition[randomIndex] == "left")
            {
                roomPosition.Add("left");
                x--;
                m.Add(new Vector2(x, y));
                validPosition = new List<string>() { "top", "bottom", "left" };
            }
        }

        //Generate first room part
        GenerateRoomPart(x, y, m, 0);

        //Generate other room parts
        int index = 1;
        foreach (var pos in roomPosition)
        {
            if (pos == "top")
            {
                y += rowsCount;
                GenerateRoomPart(x, y, m, index); //top
            }
            else if (pos == "right")
            {
                x += columnsCount;
                GenerateRoomPart(x, y, m, index); //right
            }
            else if (pos == "bottom")
            {
                y -= rowsCount;
                GenerateRoomPart(x, y, m, index); //bottom
            }
            else if (pos == "left")
            {
                x -= columnsCount;
                GenerateRoomPart(x, y, m, index); //left
            }
            index++;
        }
    }

    void GenerateRoomPart(int xNumber, int yNumber, List<Vector2> m, int roomPos)
    {
        List<string> wallPositions = CheckWhereToInstantiateWall(m, roomPos);

        for (int x = xNumber; x < columnsCount + xNumber + 1; x++)
        {
            for (int y = yNumber; y < rowsCount + yNumber + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                Vector3 vector3 = new Vector3(x, y, 0f);

                if ((x == xNumber && wallPositions.Contains("left")) || (x == (columnsCount + xNumber) && wallPositions.Contains("right")) || (y == yNumber && wallPositions.Contains("bottom")) || (y == (rowsCount + yNumber) && wallPositions.Contains("top")))
                {
                    //Don't add a wall if there is already one
                    if (!wallGridPositions.Contains(vector3) && !gridPositions.Contains(vector3))
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        GameObject instance = Instantiate(toInstantiate, vector3, Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        wallGridPositions.Add(vector3);
                    }
                }
                else
                {
                    //Don't add a tile if there is already one
                    if (!gridPositions.Contains(vector3) && !wallGridPositions.Contains(vector3))
                    {
                        GameObject instance = Instantiate(toInstantiate, vector3, Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        gridPositions.Add(vector3);
                    }
                }
            }
        }
    }

    List<string> CheckWhereToInstantiateWall(List<Vector2> m, int roomPos)
    {
        List<string> wallPositions = new List<string>();


        Vector2 actualRoom = m[roomPos];
        m.RemoveAt(roomPos);
        //Check if there is a room on the left
        if (!m.Contains(new Vector2(actualRoom.x - 1, actualRoom.y)))
        {
            //Instantiate wall on the left
            wallPositions.Add("left");
        }
        //Check if there is a room on the right
        if (!m.Contains(new Vector2(actualRoom.x + 1, actualRoom.y)))
        {
            //Instantiate wall on the right
            wallPositions.Add("right");
        }
        //Check if there is a room on the top
        if (!m.Contains(new Vector2(actualRoom.x, actualRoom.y + 1)))
        {
            //Instantiate wall on the top
            wallPositions.Add("top");
        }
        //Check if there is a room on the bottom
        if (!m.Contains(new Vector2(actualRoom.x, actualRoom.y - 1)))
        {
            //Instantiate wall on the bottom
            wallPositions.Add("bottom");
        }
        m.Insert(roomPos, actualRoom);
        return wallPositions;
    }

    void SetupCameraBoundaries()
    {
        // change polygon collider points to match the map boundaries
        GameObject.Find("MapBoundary").GetComponent<PolygonCollider2D>().points = new Vector2[] {
            new Vector2(-1, rowsCount+0.5f),
            new Vector2(columnsCount, rowsCount+0.5f),
            new Vector2(columnsCount, -1),
            new Vector2(-1, -1)
        };
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 RandomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return RandomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray)
    {
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity, boardHolder);
        }
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
        //LayoutObjectAtRandom(wallTiles);
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
    }
}
