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
                    go.GetComponent<EditingWeapon>().enabled = true;
                    go.GetComponent<BoxCollider2D>().enabled = true;
                    go.GetComponent<ActiveWeapon>().enabled = false;
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = true;
                }
            } else {
                cam.orthographicSize *= 3;
                foreach (GameObject go in widgetlist) {
                    go.GetComponent<EditingWeapon>().enabled = false;
                    go.GetComponent<BoxCollider2D>().enabled = false;
                    go.GetComponent<ActiveWeapon>().enabled = true;
                    if (go.GetComponent<EditingWeapon>().closestMount == null) {
                        Destroy(go);
                    }
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            lastEditing = editing;
        }
        
    }
}
