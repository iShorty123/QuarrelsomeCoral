using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{

    public Slider musicSlider = null;

    //Start is called before the first frame update
    void Start()
    {
        musicSlider.onValueChanged.AddListener(UpdateVolume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateVolume(float value){
        GameObject musicObject = GameObject.FindWithTag("Music");
        MusicPlayer musicPlayer = musicObject.GetComponent<MusicPlayer>();
        musicPlayer.UpdateVolume(value);
    }
}
