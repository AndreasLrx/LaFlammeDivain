using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public abstract class EliteEnemy : MonoBehaviour
{

    protected Enemy enemyClass;

    protected virtual void Awake()
    {
        enemyClass = gameObject.GetComponent<Enemy>();
        enemyClass.onDeath += OnDeath;
    }

    private IEnumerator OnDeath()
    {
        PlayerController.Instance.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), PlayerController.Instance.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        yield break;
    }
}
