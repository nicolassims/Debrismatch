using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour {
    public GameObject bullet;
    public int maxcooldown;

    private int cooldown = 0;
    private MasterScript master;

    void Start() {
        master = GameObject.FindWithTag("Master").GetComponent<MasterScript>();
    }

    void Update() {
        if (!master.editing) {
            Vector3 mouse_pos = Input.mousePosition;
            Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
            mouse_pos.x -= object_pos.x;
            mouse_pos.y -= object_pos.y;
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg + 180;

            cooldown--;
            if (cooldown <= 0 && Input.GetKey(KeyCode.X)) {
                cooldown = maxcooldown;
                Instantiate(bullet, transform.position - 951 * transform.right / 200, transform.rotation);
            }
        }
    }
}
