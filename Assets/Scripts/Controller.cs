using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] screens;
    int touches;
    float lastTouch;
    public float lastNext;
    public DataController dc;
    public AudioSource mainAudio;
    public AudioSource loadingAudio;
    public float timeToLogOut= 80;
    float timeToWaitBetween;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        lastNext = 0f;
        timeToLogOut = 120;
        lastTouch = 0f;
        touches = 0;
        timeToWaitBetween = 2;
       // dc = GameObject.FindWithTag("DataCont");
        //screens = GameObject.FindGameObjectsWithTag("Welcome");
    }

    public void LogOut(){
        Fade f;
        timeToWaitBetween = 2;
        for (int i = 0 ; i < screens.Length; i ++ ){
            
            f = screens[i].GetComponent<Fade>();
            if (f.isFadedOut){
                f.FadeIn();
            }
        }

    }
    IEnumerator fadeOutAudio(AudioSource a){
        while(a.volume > 0f ){
            a.volume -= 0.1f;
            yield return null;
        }
        a.Stop();
    }
    IEnumerator fadeInAudio(AudioSource a){
        a.Play();
        while(a.volume < 1f ){
            a.volume += 0.1f;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {

            // Make sure user is on Android platform
    if (Application.platform == RuntimePlatform.Android) {
        
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("esc");
        }
    }
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)){
            lastTouch = Time.time;
            lastNext = Time.time;
        }

        if(Time.time - lastNext > timeToWaitBetween && ( (screens[screens.Length - 1].GetComponent<Fade>().isFadedOut) == true ) && ( (screens[0].GetComponent<Fade>().isFadedOut) == false )  ){
            lastNext = Time.time;
            Fade f;
            for (int i = screens.Length-1 ; i >= 0 ; i --){
                if (i < screens.Length -1){
                    timeToWaitBetween = 5;
                }
                f = screens[i].GetComponent<Fade>();
                if (f.isFadedOut == false){
                    f.FadeOut();
                    if (i == 2){
                        StartCoroutine(fadeOutAudio(mainAudio));
                        StartCoroutine(fadeInAudio(loadingAudio));
                        timeToWaitBetween = 6;
                    }
                    if (i == 1){
                        StartCoroutine(fadeInAudio(mainAudio));
                        StartCoroutine(fadeOutAudio(loadingAudio));
                        timeToWaitBetween = 4;
                    }
                    break;
                }
            }
        }
      
        if (Mathf.Abs(Time.time - lastNext) > timeToLogOut){ // left the screen for 1 min
            lastTouch = Time.time;
            lastNext = Time.time;

        }
        
    }
}
