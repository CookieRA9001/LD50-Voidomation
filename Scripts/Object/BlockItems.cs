using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItems : MonoBehaviour {
    public float radius = 0.5f;
    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Resorces>();
        if (!r) return;
        if (Vector2.Distance(col.transform.position, transform.position) >= radius) {// .25+.4375 = .6875
            col.transform.GetComponent<Rigidbody2D>().velocity *= 0;
            r.move_dir = -1;
        }
    }
}
