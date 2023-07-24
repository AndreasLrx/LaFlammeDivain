using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public abstract class Elite : MonoBehaviour
{

    protected Enemy enemyClass;

    protected virtual void Awake()
    {
        enemyClass = gameObject.GetComponent<Enemy>();
        enemyClass.onDeath += OnDeath;
        Instantiate(PrefabManager.Instance.lightPoint, transform).transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private IEnumerator OnDeath()
    {
        PlayerController.Instance.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), PlayerController.Instance.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        yield break;
    }
}
