using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrag : MonoBehaviour {
    public GameObject redspot;//the location weapons will snap to
    public float snapDistance;//the distance weapons can be from mounting locations before they'll snap to them.

    //when the mouse is dragging the weapon, update its location to the mouse's location.
    private void OnMouseDrag() {
        transform.position = new Vector3(
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
    }

    //when the mouse is let go--i.e., after you stop dragging--look for the nearest mount and snap to it.
    private void OnMouseUp() {
        GameObject closestMount = GetClosestMount(GameObject.FindGameObjectsWithTag("Mounting"));

        if (closestMount != null) {
            transform.position = closestMount.transform.position;
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
