using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{

    public int columns = 8;
    public int rows = 8;
    public int wallCount = 10;
    public int enemiesCount = 1;

    public GameObject[] outerWallTiles;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject enemy;
    public Player player;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
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
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    void LayoutEnemiesAtRandom(GameObject enemy)
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(enemy, randomPosition, Quaternion.identity);
        }
    }

    void LayoutPlayer(Player player)
    {
        Vector3 randomPosition = RandomPosition();
        Instantiate(player, randomPosition, Quaternion.identity);
    }

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles);
        LayoutEnemiesAtRandom(enemy);
        LayoutPlayer(player);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
