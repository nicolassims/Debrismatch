using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    private int lifetime = 60 * 5;

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.left * 50 * Time.deltaTime);

        lifetime--;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }
    }
}
