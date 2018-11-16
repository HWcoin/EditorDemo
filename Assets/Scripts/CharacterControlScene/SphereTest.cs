using UnityEngine;
using System.Collections;

public class SphereTest : MonoBehaviour {
    public CharacterController ccSphere;
	// Use this for initialization
	void Start () {
        ccSphere = transform.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.W)) {
            ccSphere.SimpleMove(Vector3.forward  * 10f);
        }
        if (Input.GetKey(KeyCode.S)) {
            ccSphere.SimpleMove(Vector3.forward  * -10f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ccSphere.SimpleMove(Vector3.left  * 10f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ccSphere.SimpleMove(Vector3.right * 10f);
        }

	}
    /*
    void OnControllerColliderHit(ControllerColliderHit hit){
        if (hit.gameObject.tag == "Background") return;
        Debug.LogError(hit.gameObject.name);
    }
    */
    void OnCollisionEnter(Collision collision)
    {
        Debug.LogError("OnCollisionEnter");
        if (collision == null) return;
        if (collision.gameObject.tag == "Background") return;
        Debug.LogError(collision.gameObject.name);
    }
    void OnCollisionExit(Collision collision){
        Debug.LogError("OnCollisionExit");
        if (collision == null) return;
        if (collision.gameObject.tag == "Background") return;
        Debug.LogError(collision.gameObject.name);
    }
    void OnCollisionStay(Collision collision){
        Debug.LogError("OnCollisionStay");
        if (collision == null) return;
        if (collision.gameObject.tag == "Background") return;
        Debug.LogError(collision.gameObject.name);
    }
    /*
    void OnTriggerEnter(Collision collision){
        if (collision == null) return;
        Debug.LogError(collision.gameObject.name);
    }
    void OnTriggerExit(Collision collision){
        if (collision == null) return;
        Debug.LogError(collision.gameObject.name);
    }
    void OnTriggerStay(Collision collision){
        if (collision == null) return;
        Debug.LogError(collision.gameObject.name);
    }
 */
    public void OnProgress(float progress) { 
    
    }
    
}
