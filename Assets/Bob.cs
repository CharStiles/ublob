using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    Vector3 thisPos;
        float b = -1f;
        float c = 0.01f;
        float d = 2.0f;
    float easeOut(float t) {
        // t: current time, b: begInnIng value, c: change In value, d: duration
		return c*((t=t/d-1)*t*t + 1) + b;
	}
    void Start()
    {
        thisPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        thisPos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(thisPos.x, 2f+ easeOut(Mathf.Sin(Time.time)),thisPos.z);
    }
}
