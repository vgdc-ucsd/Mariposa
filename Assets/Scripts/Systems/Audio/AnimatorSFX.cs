using UnityEngine;
using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;

//this is for in-world sfx environment objects such as the crackling electiricty
//This should have doppler effect 
public class AnimatorSFX : MonoBehaviour
{
    [SerializeField] private EventReference sfxEventReference;
   
    public void PlaySFX()
    {
        RuntimeManager.PlayOneShotAttached(sfxEventReference,gameObject);
    }

}
