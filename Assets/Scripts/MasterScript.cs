using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterScript : MonoBehaviour {
    public bool editing = true;
    public GameObject vending;
    bool lastEditing = true;

    private void Start() {
        Physics.queriesHitTriggers = true; // Sets this global variable to ensure part dragging works
    }


    // Update is called once per frame
    void Update() {
        // Handle editor mode swap
        if (Input.GetKeyUp(KeyCode.Space)) {
            editing = !editing;
        }

        if (editing != lastEditing) {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            GameObject[] widgetlist = GameObject.FindGameObjectsWithTag("Widget");
            GameObject[] mountlist = GameObject.FindGameObjectsWithTag("Mounting");
            GameObject tank = GameObject.FindGameObjectWithTag("Tank");
            if (editing) {
                // Testing -> Editing
                cam.orthographicSize = 10;
                tank.GetComponent<TankScript>().Reset();
                foreach (GameObject go in widgetlist) {
                    EditingWidget ew = go.GetComponent<EditingWidget>();
                    ew.ToggleFx(false);

                    Transform root = go.GetComponent<EditingWidget>().root;
                    go.transform.SetParent(root); // now this will move when its mount does
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = true;
                }
                vending.SetActive(true);
            } else {
                // Editing -> Testing
                cam.orthographicSize = 30;
                foreach (GameObject go in widgetlist) {
                    ActiveWeapon aw = go.GetComponent<ActiveWeapon>();
                    if (aw != null) {
                        go.GetComponent<ActiveWeapon>().enabled = true;
                    }
                    if (!go.GetComponent<EditingWidget>().IsMounted()) {
                        Destroy(go);
                    } else {
                        Transform mount = go.GetComponent<EditingWidget>().closestMount.transform;
                        go.transform.SetParent(mount); // now this will move when its mount does
                    }
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = false;
                }
                vending.SetActive(false);
            }

            lastEditing = editing;
        }
        
    }
}
