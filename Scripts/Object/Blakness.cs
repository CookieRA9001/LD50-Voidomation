using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blakness : MonoBehaviour {
    float time = 0.25f;
    private void Update() {
        if (time<3000) {
            time += Time.deltaTime*300;
            transform.localScale = new Vector3(time, time);
        }
    }
}
