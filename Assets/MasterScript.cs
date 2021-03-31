using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterScript : MonoBehaviour {
    bool editing = true;
    bool lastEditing = true;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            editing = !editing;
        }

        if (editing != lastEditing) {
            GameObject[] widgetlist = GameObject.FindGameObjectsWithTag("Widget");
            GameObject[] mountlist = GameObject.FindGameObjectsWithTag("Mounting");
            if (editing) {
                foreach (GameObject go in widgetlist) {
                    go.AddComponent<EditingWeapon>();
                }
                foreach (GameObject mount in mountlist) {
                    mount.GetComponent<UpdatePosition>().servant = null;
                }
            } else {
                foreach (GameObject go in widgetlist) {
                    Destroy(go.GetComponent<EditingWeapon>());
                }
            }

            lastEditing = editing;
        }
        
    }
}
