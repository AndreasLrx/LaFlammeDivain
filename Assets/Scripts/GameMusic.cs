using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip calm, intense;

    public AudioClip intenseToCalm;


    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = intense;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void IntenseMusic()
    {
        if (audioSource.clip != intense)
        {
            float progress = audioSource.time;
            audioSource.Stop();
            audioSource.clip = intense;
            audioSource.time = progress;
            audioSource.Play();
        }
    }

    public void CalmMusic()
    {
        if (audioSource.clip != calm)
        {
            float progress = audioSource.time;
            audioSource.Stop();
            audioSource.clip = calm;
            audioSource.time = progress;
            audioSource.Play();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.GetComponent<PlayerController>().currentRoom.remainingEnemies > 0)
        {
            IntenseMusic();
        }
        else
        {
            CalmMusic();
        }
    }
}
