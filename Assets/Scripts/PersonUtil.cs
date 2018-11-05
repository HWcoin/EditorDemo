using UnityEngine;
using System.Collections;

public class PersonUtil : MonoBehaviour {
    private int count = 0;
    private float one_sec = 0;
	// Use this for initialization
	void Start () {
	
	}

    public void OnEnable() {
        count = 0;
        one_sec = 0;
    }
	
	// Update is called once per frame
	void Update () {

        one_sec += Time.deltaTime;

        if (one_sec > 1.0f) {
            one_sec = 0;
            count++;
            Debug.LogError("Call Per Frame: " + count + "s");
        }
	}
}
