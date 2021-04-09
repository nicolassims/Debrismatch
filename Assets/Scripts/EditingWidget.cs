using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditingWidget : MonoBehaviour {
    public GameObject redspot;//the location weapons will snap to
    public float snapDistance = 1;//the distance weapons can be from mounting locations before they'll snap to them.
    public GameObject closestMount;

    void Start() {
        redspot = GameObject.FindGameObjectWithTag("Mounting");
    }

    //when the mouse is dragging the weapon, update its location to the mouse's location.
    void OnMouseDrag() {
        if (closestMount != null) {//if a closest mount has been set, break the connection in both directions.
            closestMount.GetComponent<UpdatePosition>().servant = null;//remove the mount's reference to this
            closestMount = null;//remove this gameobject's reference to the mount
        }

        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousepos.x, mousepos.y);

        Vector2 tank = GameObject.Find("Tank").transform.position;
        float newy = transform.position.y - tank.y;
        float newx = transform.position.x - tank.x;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(newy, newx) + 180);
    }

    //when the mouse is let go--i.e., after you stop dragging--look for the nearest mount and snap to it.
    void OnMouseUp() {
        closestMount = GetClosestMount(GameObject.FindGameObjectsWithTag("Mounting"));

        if (closestMount != null) {
            transform.position = closestMount.transform.position;
            UpdatePosition spot = closestMount.GetComponent<UpdatePosition>();
            spot.servant = gameObject;
            spot.myRotation = transform.rotation.eulerAngles.z;
        }
    }

    //Return the nearest GameObject, from the given list. Returns null if none are closer than 50, or if the list is empty.
    GameObject GetClosestMount(GameObject[] mounts) {
        GameObject closestMount = null;
        float closestDistanceSqr = snapDistance;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in mounts) {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                closestMount = potentialTarget;
            }
        }

        return closestMount;
    }
}
