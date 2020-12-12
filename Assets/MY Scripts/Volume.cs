using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volume : MonoBehaviour
{

    private Tuner tuner;

    public float volumeLevel;

    [FMODUnity.EventRef]
    public string music = "event:/Song";

    FMOD.Studio.EventInstance musicEv;

    private void Awake()
    {
        tuner = GetComponent<Tuner>();

        musicEv = FMODUnity.RuntimeManager.CreateInstance(music);

        musicEv.start();

        
    }

    public void AdjustVolume()
    {
        musicEv.setParameterByName("Volume", volumeLevel);
    }

    public void PlaySong()
    {
        musicEv.setParameterByName("Play", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (tuner.turningTuner)
        {
            /**
             * Converts the Angle value which is from 0 to the clamp to a value of 0 to 1 for FMOD
             * new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
             * valueFmodSees = ( (angle_value (which is gotten from tunerscript) - 0) / (clamp - 0 (The clamp is from 0 to whatever angle which is what clamp is)) ) * (FMODParameterMax( which is 1) - FMODParameterMin(which is 0)) + FMODParameterMin(which is 0)
             **/
            volumeLevel = ((tuner.angle - 0f) / (tuner.clamp.y - 0)) * (1f - 0f) + 0f;

            AdjustVolume();
        }

    }
}
