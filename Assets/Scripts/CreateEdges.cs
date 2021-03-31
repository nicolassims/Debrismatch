using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEdges : MonoBehaviour {
    public int numSides;//the number of sides the "placement object" has. This assumes the placement object is a parallelogram
    public GameObject redspot;

    private List<GameObject> mounts = new List<GameObject>();

    private List<Vector2> GetCorners(float radius) {
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
            redspot = Instantiate(redspot, v3, new Quaternion());
            redspot.GetComponent<UpdatePosition>().master = gameObject;
            mounts.Add(redspot);
        }
    }

    private void Update() {
        foreach (GameObject mount in mounts) {
            mount.GetComponent<UpdatePosition>().UpdateMountPosition();
        }
    }

}
