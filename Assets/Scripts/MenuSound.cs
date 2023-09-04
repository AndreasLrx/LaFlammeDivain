using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSound : MonoBehaviour
{

    public AudioSource audioSource;

    public void SelectOption(){
        audioSource.Play();
    }



}
