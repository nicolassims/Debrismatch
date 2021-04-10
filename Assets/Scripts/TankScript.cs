using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour {
    public int numSides;//the number of sides the "placement object" has. This assumes the placement object is a parallelogram
    public float rotationSpeed; // how many degrees per frame the tank can rotate
    public GameObject redspot;

    List<GameObject> mounts = new List<GameObject>();
    MasterScript master;

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
        if (numSides < 3) {
            throw new ArgumentException("Number of sides cannot be <3.");
        }

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
            Vector3 v3 = pos;
            //creating the red spot at the edge of the object. the empty Quaternion is mandatory.
            // Mount spots should have this tank as their parent.
            redspot = Instantiate(redspot, v3, Quaternion.identity, this.transform);
            redspot.GetComponent<UpdatePosition>().master = gameObject;
            mounts.Add(redspot);
        }
    }

    void Update() {
        foreach (GameObject mount in mounts) {
            mount.GetComponent<UpdatePosition>().UpdateMountPosition();
        }

        if (!master.editing) {
            float x = 0.1f * (Convert.ToInt32(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) - Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)));
            float y = 0.1f * (Convert.ToInt32(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) - Convert.ToInt32(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)));
            transform.Translate(x, y, 0);           
        }
        /*if (!master.editing)
        {
            // Adjust rotation
            transform.Rotate(0, 0, rotationSpeed * Input.GetAxis("Horizontal"));
            if (Input.GetKey(KeyCode.Z))
            {
                ApplyEngineThrust();
            }
        }*/
    }

    /*
     * Applies a force to the tank based on the number of thrusters attached to it
     * and which directions they're facing.
     */
    void ApplyEngineThrust()
    {
        Vector2 totalThrust = new Vector2(0, 0);
        float baseThrust = 0.1f;
        foreach (GameObject spot in mounts)
        {
            GameObject tankPart = spot.GetComponent<UpdatePosition>().servant;
            if (tankPart != null && tankPart.GetComponent<ActiveWeapon>() == null)
            {
                float partRotation = tankPart.transform.rotation.eulerAngles.z + 180f;
                totalThrust.x -= baseThrust * (float)Math.Cos(partRotation);
                totalThrust.y -= baseThrust * (float)Math.Sin(partRotation);
            }
        }
        transform.Translate(totalThrust.x, totalThrust.y, 0);
    }
}
