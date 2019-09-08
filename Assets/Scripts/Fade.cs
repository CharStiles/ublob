using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    // Start is called before the first frame update
    CanvasGroup cg;
    public bool isFadedOut;
    bool beingUsed = false;

    void Start()
    {
        isFadedOut = false;
        cg = gameObject.GetComponent<CanvasGroup>();
    }

    IEnumerator fadeInThread(){
        while(beingUsed == true){
            yield return null;
        }
        beingUsed = true;
        while(cg.alpha < 1){
            cg.alpha = cg.alpha +0.1f;
            yield return null;
        }
        beingUsed = false;
    }


    IEnumerator fadeOutThread(bool deactivate = false){
        while(beingUsed == true){
            yield return null;
        }
        beingUsed = true;
        while(cg.alpha > 0){
            cg.alpha = cg.alpha -0.1f;
            yield return null;
        }
        if (deactivate){

            cg.blocksRaycasts = false;
            //gameObject.SetActive(false);
        }
        if (cg.alpha > 0){
            cg.alpha = 0;
        }
         beingUsed = false;
    }

    public void FadeOut(){
        isFadedOut = true;
        StartCoroutine(fadeOutThread());

    }
    public void FadeIn(){
        isFadedOut = false;
        // gameObject.SetActive(true);
        cg.blocksRaycasts = true;
        cg.alpha = 0;

        StartCoroutine(fadeInThread());
    }

    public void FadeOutAndDeactivate(){
        isFadedOut = true;
        StartCoroutine(fadeOutThread(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
