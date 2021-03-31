using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour {
    public GameObject master;//the tank this mountingpoint is attached to
    public GameObject servant;//the weapon, if any, this mountingpoint is attached to

    private Vector2 localPos;

    // Start is called before the first frame update
    void Start() {
        localPos = transform.position;
    }

    // Update is called once per frame
    public void UpdateMountPosition() {
        Vector2 newpos = new Vector2(master.transform.position.x, master.transform.position.y) + localPos;
        transform.position = newpos;
        if (servant != null) {
            servant.transform.position = newpos;
        }
    }
}
