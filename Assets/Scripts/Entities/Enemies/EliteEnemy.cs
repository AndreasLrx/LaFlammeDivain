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
    }

    protected virtual void Update()
    {
        if (enemyClass.hp <= 0)
            StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return StartCoroutine(OnDeath());
        Destroy(gameObject);
    }

    protected virtual IEnumerator OnDeath()
    {
        Player.Instance.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), Player.Instance.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        yield break;
    }
}
