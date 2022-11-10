using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectsSource, musicSource;
    [SerializeField] private Vector2 pitchRange = Vector2.zero;
    public static SoundManager SharedInstance;
    private void Awake() {
        if(SharedInstance != null){
            Destroy(gameObject);
        }else{
            SharedInstance = this;
        }

        //No destruyas el gameobject al cargar el juego o otra escena
        DontDestroyOnLoad(gameObject);
    }
}
