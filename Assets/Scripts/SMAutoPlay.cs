using System.Collections;
using System.Collections.Generic;
using Sounds;
using UnityEngine;

public class SMAutoPlay : MonoBehaviour
{
    [SerializeField] private SoundState soundStateToPlay;
    
    // Start is called before the first frame update
    void Start()
    {
        SoundManager soundManager = GetComponent<SoundManager>();
        soundManager.LoadSoundState(soundStateToPlay);
    }
}
