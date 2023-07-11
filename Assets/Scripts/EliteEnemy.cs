using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class EliteEnemy : MonoBehaviour
{   
    public GameObject[] wisps;
    private Enemy enemyClass;

    private void Awake()
    {
        enemyClass = gameObject.GetComponent<Enemy>();
       
    }
    private void Update() {
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
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.AddWisp(Instantiate(wisps[Random.Range(0, wisps.Length)], player.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        yield break;
    }
}
