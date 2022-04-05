using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour {

    void Update() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)(Mathf.Round(worldPosition.x));
        int y = (int)(Mathf.Round(worldPosition.y));
        Vector3 pos = new Vector3(x, y, -10);
        transform.position = pos;
    }
}
