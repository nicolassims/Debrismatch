using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrag : MonoBehaviour {
    public GameObject redspot;//the location weapons will snap to
    public float snapDistance;//the distance weapons can be from mounting locations before they'll snap to them.

    private GameObject closestMount;

    //when the mouse is dragging the weapon, update its location to the mouse's location.
    private void OnMouseDrag() {
        if (closestMount != null) { 
            closestMount.GetComponent<UpdatePosition>().servant = null;
            closestMount = null;
        }

        transform.position = new Vector3(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(transform.position.y, transform.position.x) + 180);
    }

    //when the mouse is let go--i.e., after you stop dragging--look for the nearest mount and snap to it.
    private void OnMouseUp() {
        closestMount = GetClosestMount(GameObject.FindGameObjectsWithTag("Mounting"));

        if (closestMount != null) {
            transform.position = closestMount.transform.position;
            closestMount.GetComponent<UpdatePosition>().servant = gameObject;
        }
    }

    //Return the nearest GameObject, from the given list. Returns null if none are closer than 50, or if the list is empty.
    private GameObject GetClosestMount(GameObject[] mounts) {
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
