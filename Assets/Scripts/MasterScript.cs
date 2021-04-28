using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterScript : MonoBehaviour {
    public bool editing = true;
    public GameObject vending;
    bool lastEditing = true;

    private void Start()
    {
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
                foreach (GameObject go in widgetlist) {
                    go.GetComponent<EditingWidget>().enabled = true;
                    go.GetComponent<BoxCollider2D>().enabled = true;
                    go.GetComponent<ActiveWeapon>().enabled = false;
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<SpriteRenderer>().enabled = true;
                }
                vending.SetActive(true);
            } else {
                // Editing -> Testing
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
                vending.SetActive(false);
            }

            lastEditing = editing;
        }
        
    }
}
