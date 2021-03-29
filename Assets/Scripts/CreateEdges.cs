using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEdges : MonoBehaviour {
    public int numSides;//the number of sides the "placement object" has. This assumes the placement object is a parallelogram
    public GameObject redspot;

    // Start is called before the first frame update
    void Start() {
        if (numSides < 3) {
            throw new ArgumentException("Number of sides cannot be <3.");
        }

        //create function here that will modify PolygonCollider2D's points to be right place/location, if not already
        //NOTE: This script will only work on square objects until this function is implemented
        
        //find the midpoints of each edge and add them together.
        PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
        Vector2[] points = coll.points;
        ArrayList midpoints = new ArrayList();
        for (var i = 0; i < numSides - 1; i++) {
            float newfloatx = (points[i].x + points[i + 1].x) / 2;
            float newfloaty = (points[i].y + points[i + 1].y) / 2;
            midpoints.Add(new Vector2(newfloatx, newfloaty));
        }
        float lastfloatx = (points[0].x + points[points.Length - 1].x) / 2;
        float lastfloaty = (points[0].y + points[points.Length - 1].y) / 2;
        midpoints.Add(new Vector2(lastfloatx, lastfloaty));

        //add a red spot on each midpoint
        foreach (Vector2 pos in midpoints) {
            //converting Vector2 into Vector3
            Vector3 v3 = pos;
            //creating the red spot at the edge of the object. the empty Quaternion is mandatory.
            Instantiate(redspot, v3, new Quaternion());
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
