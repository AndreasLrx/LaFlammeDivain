using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabManager : Singleton<PrefabManager>
{
    public GameObject outerWallPrefab;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public Sprite[] wallTiles;
    public Sprite[] floorTiles;
    public Enemy[] enemies;
    public Projectile boneProjectile;
    public Projectile ectoplasmProjectile;
    public Wisp[] wisps;
    public Player player;
    public AICompanion companion;
    public WispsGroup wispsGroup;

    public NavMeshSurface[] navMeshSurfaces;

    public List<Type> EliteTypes;

    public GameObject lightPoint;
    public GameObject fireTrail;

    protected override void Awake()
    {
        base.Awake();
        EliteTypes = new()
        {
            typeof(FireElite),
            typeof(HealthElite),
            typeof(SpeedElite),
            typeof(RegenElite)
        };
    }
    public GameObject doorTiles;

    private static T GetRandomElement<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    private static GameObject GetRandomeTiledElement(GameObject gameObject, Sprite[] tiles)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = GetRandomElement(tiles);
        return gameObject;
    }

    public static GameObject GetDoor()
    {
        return Instance.doorTiles;
    }

    public static GameObject GetRandomOuterWall()
    {
        return GetRandomeTiledElement(Instance.outerWallPrefab, Instance.wallTiles);
    }

    public static GameObject GetRandomWall()
    {
        return GetRandomeTiledElement(Instance.wallPrefab, Instance.wallTiles);
    }

    public static GameObject GetRandomFloor()
    {
        return GetRandomeTiledElement(Instance.floorPrefab, Instance.floorTiles);
    }

    public static Enemy GetRandomEnemy()
    {
        return GetRandomElement(Instance.enemies);
    }

    public static Wisp GetRandomWisp()
    {
        return GetRandomElement(Instance.wisps);
    }

    public static Type GetRandomEliteType()
    {
        return GetRandomElement(Instance.EliteTypes.ToArray());
    }
}
