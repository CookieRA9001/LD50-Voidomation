using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_movment : MonoBehaviour{
    public float speed; // max_speed
    // private SpriteRenderer spR; 
    private Rigidbody2D rb2d;
    public GameObject Pointer;
    
    void Start() {
        // spR = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (P_action.pause) {
            rb2d.velocity = Vector2.zero;
            return;
        }
        Vector2 move = new Vector2(0,0);
        if (Input.GetKey(KeyCode.W)) {
            move.y += 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            move.x -= 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            move.y -= 1;
        }
        if (move.x != 0 && move.y != 0) {
            move /= Mathf.Sqrt(2);
        }

        // if (move.x<0 != spR.flipX && move.x!=0) spR.flipX = move.x<0;
        rb2d.velocity = move*speed;

        Vector3 void_pos = Ender.Pos;
        Vector3 this_pos = transform.position;
        Vector3 dir = (void_pos - this_pos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Pointer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
