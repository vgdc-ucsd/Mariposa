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

    private EventInstance sfxEventInstance;

    bool hasPlay = false;
    void Start()
    {
        sfxEventInstance = RuntimeManager.CreateInstance(sfxEventReference);
        RuntimeManager.AttachInstanceToGameObject(sfxEventInstance,gameObject);

    }

    void Update()
    {
        if(gameObject.activeInHierarchy && !hasPlay)
        {
            sfxEventInstance.start();
            hasPlay = true;
            Debug.Log("Playing SFX");
        }
    }

    public void Stop()
    {
        sfxEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        sfxEventInstance.release();
        Debug.Log("Stop Playing SFX");
    }

}
