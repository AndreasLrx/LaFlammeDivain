using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    public GameObject[] outerWallTiles;
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;
    public Enemy[] enemies;
    public Wisp[] wisps;
    public Player player;

    private static T GetRandomElement<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static GameObject GetRandomOuterWall()
    {
        return GetRandomElement(Instance.outerWallTiles);
    }

    public static GameObject GetRandomWall()
    {
        return GetRandomElement(Instance.wallTiles);
    }

    public static GameObject GetRandomFloor()
    {
        return GetRandomElement(Instance.floorTiles);
    }

    public static Enemy GetRandomEnemy()
    {
        return GetRandomElement(Instance.enemies);
    }

    public static Wisp GetRandomWisp()
    {
        return GetRandomElement(Instance.wisps);
    }
}