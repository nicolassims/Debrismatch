using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditingWidget : MonoBehaviour {
    public GameObject redspot;//the location weapons will snap to
    public float snapDistance = 1;//the distance weapons can be from mounting locations before they'll snap to them.
    public GameObject closestMount;
    public SpriteRenderer fireFx;
    public Transform root; // the object normally above this in the scene hierarchy

    void Start() {
        redspot = GameObject.FindGameObjectWithTag("Mounting");
        ToggleFx(false);
        root = transform.parent;
    }

    //when the mouse is dragging the weapon, update its location to the mouse's location.
    void OnMouseDrag() {
        this.transform.SetParent(root); // reset parent to root
        if (closestMount != null) {//if a closest mount has been set, break the connection in both directions.
            closestMount.GetComponent<UpdatePosition>().servant = null;//remove the mount's reference to this
            closestMount = null;//remove this gameobject's reference to the mount
        }

        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousepos.x, mousepos.y);

        Transform tankTransform = GameObject.Find("Tank").transform;
        Vector2 tank = tankTransform.position;
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
            this.transform.SetParent(closestMount.transform); // now this will move when its mount does
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

    // Used with thrusters which display a flame effect while active.
    // Simply turns the fire on or off.
    public void ToggleFx(bool isOn)
    {
        if (fireFx != null)
        {
            fireFx.enabled = isOn;
        }
    }
}
