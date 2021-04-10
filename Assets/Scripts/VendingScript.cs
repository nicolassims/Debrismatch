using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingScript : MonoBehaviour {
    public GameObject[] parts;

    List<string> vended = new List<string>();

    void Start() {
        for (var i = 0; i < parts.Length; i++) {
            GameObject newobj = Instantiate(parts[i], new Vector3(-13.5f, 10 - 20 / (parts.Length + 1) * (i + 1)), Quaternion.identity);
            vended.Add(newobj.name.Replace("(Clone)", ""));
        }
    }

    void OnMouseDown() {
        List<string> tempvended = new List<string>(vended);
        foreach (GameObject part in GameObject.FindGameObjectsWithTag("Widget")) {
            if (part.GetComponent<EditingWidget>().closestMount == null) {
                tempvended.Remove(part.name.Replace("(Clone)", ""));
            }
        }
        foreach (string name in tempvended) {
            for (var i = 0; i < parts.Length; i++) {
                if (parts[i].name.Replace("(Clone)", "") == name) { 
                    Instantiate(parts[i], new Vector3(-13.5f, 10 - 20 / (parts.Length + 1) * (i + 1)), Quaternion.identity);
                    break;
                }
            }
        }
    }
}
