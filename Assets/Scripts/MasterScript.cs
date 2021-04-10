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
                cam.orthographicSize = 10;
                foreach (GameObject go in widgetlist) {
                    go.GetComponent<EditingWidget>().enabled = true;
                    go.GetComponent<BoxCollider2D>().enabled = true;
                    go.GetComponent<ActiveWeapon>().enabled = false;
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = true;
                }
            } else {
                cam.orthographicSize = 30;
                foreach (GameObject go in widgetlist) {
                    go.GetComponent<EditingWidget>().enabled = false;
                    go.GetComponent<BoxCollider2D>().enabled = false;
                    ActiveWeapon aw = go.GetComponent<ActiveWeapon>();
                    if (aw != null) {
                        go.GetComponent<ActiveWeapon>().enabled = true;
                    }
                    if (go.GetComponent<EditingWidget>().closestMount == null) {
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