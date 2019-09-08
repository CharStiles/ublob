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

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        lastNext = 0f;
        lastTouch = 0f;
        touches = 0;
        //screens = GameObject.FindGameObjectsWithTag("Welcome");
    }

    public void LogOut(){
        Fade f;
        for (int i = 0 ; i < screens.Length; i ++ ){
            
            f = screens[i].GetComponent<Fade>();
            if (f.isFadedOut){
                f.FadeIn();
            }
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

        if(Time.time - lastNext > 4 && ( (screens[screens.Length - 1].GetComponent<Fade>().isFadedOut) == true )){
            lastNext = Time.time;
            Fade f;
            for (int i = screens.Length-1 ; i >= 0 ; i --){
           
                f = screens[i].GetComponent<Fade>();
                if (f.isFadedOut == false){
                    f.FadeOut();
                    break;
                }
            }
        }
      
        if (Time.time - lastTouch > 120 && ( (screens[0].GetComponent<Fade>().isFadedOut) == false ) ){ // left the screen for 1 min
            lastTouch = Time.time;
            lastNext = Time.time;
                //CanvasGroup cg1 = screens[0].GetComponent<CanvasGroup>();
                //CanvasGroup cg2 = screens[screens.Length-1].GetComponent<CanvasGroup>();
            //if (cg1.blocksRaycasts == false && cg2.blocksRaycasts == true){
                LogOut();
            //}

        }
        
    }
}
