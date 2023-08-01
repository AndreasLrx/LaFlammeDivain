using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabManager : Singleton<PrefabManager>
{
    public GameObject[] outerWallTiles;
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;
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

    public static Type GetRandomEliteType()
    {
        return GetRandomElement(Instance.EliteTypes.ToArray());
    }

    public static int GetEnemyTypeId(Enemy enemy)
    {
        Type type = enemy.GetType();
        for (int i = 0; i < Instance.enemies.Length; i++)
            if (Instance.enemies[i].GetType() == type)
                return i;
        return -1;
    }
}
