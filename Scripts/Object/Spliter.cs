using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spliter : Machine_Base {
    public short[] dir_difrences; // the difrence (from zero) where the item will go. Size = outputs
    private int dd_index = 0; // where the spliter is going to output; ( 0 - (dir_difrences.size-1) )
    public int valid_input = 2; // from what dir can it input

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

        tr.GetComponent<Rigidbody2D>().velocity *= 0;
        if (y_axis && !x_axis) {
            if ( (pos.x > col_pos.x && (valid_input + dir) % 4 == 1) || (pos.x <= col_pos.x && (valid_input + dir) % 4 == 3) ) { // left orright
                MoveItem(tr, r);
            }
            else r.move_dir = -1;
        }
        else if (x_axis && !y_axis) {
            if ( (pos.y < col_pos.y && (valid_input + dir) % 4 == 2) || (pos.y > col_pos.y && (valid_input + dir) % 4 == 0) ) { // top or bottom
                MoveItem(tr, r);
            }
            else r.move_dir = -1;
        }
    }

    void MoveItem(Transform tr, Resorces r) {
        Vector2 pos = transform.position;
        tr.GetComponent<SpriteRenderer>().enabled = false;
        r.Timeout(1f, UnhideItem);
        r.move_dir = (short)((dir_difrences[dd_index]+dir)%4);
        switch (r.move_dir) {
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
        dd_index = (short)((dd_index + 1) % dir_difrences.Length);
    }

    void UnhideItem(Transform t) {
        t.GetComponent<SpriteRenderer>().enabled = true;
    }
}
