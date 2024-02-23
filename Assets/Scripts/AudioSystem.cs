using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSystem : MonoBehaviour
{
    public AudioSource player;


    public AudioClip keyPress1;
    public AudioClip keyPress2;
    public AudioClip keyPress3;
    public AudioClip keyPressSpace;

    public AudioClip switchToggle;
    public AudioClip buttonClick;

    public AudioClip lineclearclip;

    public AudioSource gameMusic;

    public AudioClip pauseMenuUI;

    [SerializeField]
    bool isMute;

    public void PlayKeyPressed(KeyCode keyPressed)
    {
        if(keyPressed == KeyCode.Space  )
        {
            if (!isMute) player.PlayOneShot(keyPressSpace);
            
            return;
        }
        int randomIndex = Random.Range(0, 2);
        AudioClip[] clips = { keyPress1, keyPress2, keyPress3 };
        if( !isMute) player.PlayOneShot(clips[randomIndex]);
    }

    public void PlayClearLine()
    {
        if (!isMute) player.PlayOneShot(lineclearclip);

    }
    public void PlaySwitchToggle()
    {
        if (!isMute) player.PlayOneShot(switchToggle);
    }
    public void PlayButtonClick()
    {
        if (!isMute) player.PlayOneShot(buttonClick);
    }
    public bool IsAudioRunning()
    {
        if (player.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   
    public void PlayPauseMenu()
    {
        if (!isMute) player.PlayOneShot(pauseMenuUI);

    }
  


    public void ToggleMute()
    {
      player.mute = isMute;
      gameMusic.mute = isMute;
      isMute = !isMute;
    }

}
