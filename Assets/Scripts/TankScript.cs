using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankScript : MonoBehaviour {
    public int numSides;//the number of sides the "placement object" has. This assumes the placement object is a parallelogram
    public float rotationSpeed; // how many degrees per frame the tank can rotate
    public float friction; // 0-1. The larger this is, the faster the tank slows down.
    public float baseThrust; // the strength of one thruster.
    public GameObject redspot;
    

    List<GameObject> mounts = new List<GameObject>();
    private MasterScript master;
    private new Rigidbody2D rigidbody;
    // Variables to store the tank's original position
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    List<Vector2> GetCorners(float radius) {
        List<Vector2> points = new List<Vector2>();

        for (var i = 0; i < numSides; i++) {
            float radians = (2 * Mathf.PI) / numSides * i;
            float newx = radius * Mathf.Cos(radians) / 100;
            float newy = radius * Mathf.Sin(radians) / 100;
            points.Add(new Vector2(newx, newy));
        }

        return points;
    }

    // Start is called before the first frame update
    void Start() {
        // Assign rigidbody
        rigidbody = GetComponent<Rigidbody2D>();

        if (numSides < 3) {
            throw new ArgumentException("Number of sides cannot be <3.");
        }
        
        // Store the tank's original position
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        master = GameObject.FindGameObjectWithTag("Master").GetComponent<MasterScript>();
        List<Vector2> points = GetCorners(GetComponent<SpriteRenderer>().sprite.rect.width / 2);
        
        //find the midpoints of each edge and add them together.
        ArrayList midpoints = new ArrayList();
        for (var i = 0; i < numSides - 1; i++) {
            float newfloatx = (points[i].x + points[i + 1].x) / 2;
            float newfloaty = (points[i].y + points[i + 1].y) / 2;
            midpoints.Add(new Vector2(newfloatx, newfloaty));
        }
        float lastfloatx = (points[0].x + points[points.Count - 1].x) / 2;
        float lastfloaty = (points[0].y + points[points.Count - 1].y) / 2;
        midpoints.Add(new Vector2(lastfloatx, lastfloaty));

        //add a red spot on each midpoint
        foreach (Vector2 pos in midpoints) {
            //converting Vector2 into Vector3
            Vector3 v3 = pos + originalPosition;
            //creating the red spot at the edge of the object. the empty Quaternion is mandatory.
            // Mount spots should have this tank as their parent.
            redspot = Instantiate(redspot, v3, Quaternion.identity, transform);
            redspot.GetComponent<UpdatePosition>().master = gameObject;
            mounts.Add(redspot);
        }
    }

    void FixedUpdate() {
        if (!master.editing) {
            // Adjust rotation
            transform.Rotate(0, 0, rotationSpeed * -Input.GetAxis("Horizontal"));
            // Add engine thrust
            if (Input.GetKey(KeyCode.Z)) {
                ApplyEngineThrust();
            } else {
                // toggle all fire fx off
                foreach (GameObject spot in mounts) {
                    GameObject tankPart = spot.GetComponent<UpdatePosition>().servant;
                    if (tankPart != null && tankPart.GetComponent<ActiveWeapon>() == null) {
                        tankPart.GetComponent<EditingWidget>().ToggleFx(false);
                    }
                }
            }
            // Apply friction
            ApplyFriction(friction);
        }
    }

    private void ApplyFriction(float f) {
        var currentVelocity = rigidbody.velocity;
        var oppositeForce = -1 * f * currentVelocity;
        rigidbody.AddForce(oppositeForce);
    }

    /*
     * Applies a force to the tank based on the number of thrusters attached to it
     * and which directions they're facing.
     */
    void ApplyEngineThrust() {
        foreach (GameObject spot in mounts) {
            GameObject tankPart = spot.GetComponent<UpdatePosition>().servant;
            if (tankPart != null && tankPart.GetComponent<ActiveWeapon>() == null) {
                rigidbody.AddForce(tankPart.transform.right * baseThrust);
                tankPart.GetComponent<EditingWidget>().ToggleFx(true);
            }
        }
    }

    // Used when transitioning to Edit mode to move the tank back into frame.
    public void Reset() {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rigidbody.velocity = Vector2.zero; // kill movement
        rigidbody.angularVelocity = 0f; // kill spin
    }
}
