using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Junksion : Machine_Base {
    public override void Start() {
        transform.position = new Vector3(transform.position.x, transform.position.y, -7);
    }
    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Resorces>();
        if (!r) return;
        Vector3 pos = transform.position;
        Vector3 col_pos = col.transform.position;

        bool x_axis = Mathf.RoundToInt(pos.x - col_pos.x) == 0;
        bool y_axis = Mathf.RoundToInt(pos.y - col_pos.y) == 0;
        Transform tr = col.transform;

        if (y_axis && !x_axis) {
            if (pos.x > col_pos.x) { // left
                MoveItem(3, tr, r);
            }
            else { // right
                MoveItem(1, tr, r);
            }
        }
        else if (x_axis && !y_axis) {
            if (pos.y > col_pos.y) { // top
                MoveItem(2, tr, r);
            }
            else { // bottom
                MoveItem(0, tr, r);
            }
        }
        else { /* Add fail particals */ }
    }

    void MoveItem(short side, Transform tr, Resorces r) {
        Vector2 pos = transform.position;
        tr.GetComponent<SpriteRenderer>().enabled = false;
        r.Timeout(1f, UnhideItem);
        tr.GetComponent<Rigidbody2D>().velocity *= 0;
        r.move_dir = side;
        switch (side) {
            case 0: {
                tr.position = new Vector3(pos.x, pos.y-0.5f, -5);
                break;
            }
            case 1: { 
                tr.position = new Vector3(pos.x-0.5f, pos.y, -5);
                break;
            }
            case 2: { 
                tr.position = new Vector3(pos.x, pos.y+0.5f, -5);
                break;
            }
            case 3: { 
                tr.position = new Vector3(pos.x+0.5f, pos.y, -5);
                break;
            }
        }
    }

    void UnhideItem(Transform t) {
        t.GetComponent<SpriteRenderer>().enabled = true;
    }
}
