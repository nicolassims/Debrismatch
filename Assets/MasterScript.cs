using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterScript : MonoBehaviour {
    public bool editing = true;
    bool lastEditing = true;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            editing = !editing;
        }

        if (editing != lastEditing) {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            GameObject[] widgetlist = GameObject.FindGameObjectsWithTag("Widget");
            GameObject[] mountlist = GameObject.FindGameObjectsWithTag("Mounting");
            GameObject tank = GameObject.FindGameObjectWithTag("Tank");
            if (editing) {
                cam.orthographicSize /= 3;
                foreach (GameObject go in widgetlist) {
                    go.AddComponent<EditingWeapon>();
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = true;
                    mount.GetComponent<UpdatePosition>().servant = null;
                }
            } else {
                cam.orthographicSize *= 3;
                foreach (GameObject go in widgetlist) {
                    Destroy(go.GetComponent<EditingWeapon>());
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            lastEditing = editing;
        }
        
    }
}
