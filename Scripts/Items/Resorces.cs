using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resorces : MonoBehaviour {
    public Sprite sprite;
    public string item_name;
    public GameObject item_object;
    public short move_dir = 0;
    public float move_speed = 2;
    private bool enter_exit = true; // false - exit, true - enter
    public int count = 1;

    SpriteRenderer sr;
    Rigidbody2D rd2d;

    private float timeout = 0;
    public delegate void doAfterTimeOut(Transform t);
    public doAfterTimeOut do_after_time_out = null;

    void Start() {
        if (!MainStorage.StringToResource.ContainsKey(item_name)) MainStorage.StringToResource.Add(item_name, this);
        item_object = GetComponent<GameObject>();
        sr = GetComponent<SpriteRenderer>();
        rd2d = GetComponent<Rigidbody2D>();
        if (sr) {
            sr.sprite = sprite;
        }
    }

    public void Timeout(float time, doAfterTimeOut func) {
        timeout = time;
        do_after_time_out = func;
    }
    public void Timeout(float time) {
        timeout = time;
    }

    void Update() {
        if (timeout>0) {
            timeout -= Time.deltaTime;
            if (timeout <= 0) {
                if (do_after_time_out != null) do_after_time_out(transform);
            }
            else return;
        }

        Vector2 p = transform.position;
        int x = (int)(Mathf.Round(p.x));
        int y = (int)(Mathf.Round(p.y));
        Vector2 pos = new Vector2(x, y);

        if (!P_action.placed_mashines.ContainsKey(pos)) {
            rd2d.velocity *= 0;
            // Destroy(transform.gameObject);
        }
        else {
            // when to change directions
            if (enter_exit) {
                if (Mathf.Abs((pos-p).x)<0.08f && Mathf.Abs((pos-p).y)<0.08f) {
                    enter_exit = false;
                    move_dir = P_action.placed_mashines[pos].dir;
                    transform.position = new Vector3(pos.x, pos.y, -5);
                }
            }
            else {
                if (Mathf.Abs((pos-p).x)>0.48f || Mathf.Abs((pos-p).y)>0.48f) {
                    enter_exit = true;
                }
            }

            // move the resource item
            switch (move_dir) {
                case 0: {
                    rd2d.velocity = new Vector2(0, -move_speed);
                    break;
                }
                case 1: {
                    rd2d.velocity = new Vector2(-move_speed, 0);
                    break;
                }
                case 2: {
                    rd2d.velocity = new Vector2(0, move_speed);
                    break;
                }
                case 3: {
                    rd2d.velocity = new Vector2(move_speed, 0);
                    break;
                }
                default: {
                    rd2d.velocity *= 0;
                    Timeout(0.25f, CheckIfWeCanMove);
                    break;
                }
            }
        }
    }

    private void CheckIfWeCanMove(Transform t) {
        Vector2 p = t.position;
        int x = (int)(Mathf.Round(p.x));
        int y = (int)(Mathf.Round(p.y));
        Vector2 pos = new Vector2(x, y);

        if (!P_action.placed_mashines.ContainsKey(pos)) return;

        switch (P_action.placed_mashines[pos].dir) {
            case 0: {
                if (GetMachine(new Vector2(pos.x, pos.y-1)) is ConvayerBelt) { 
                    move_dir = P_action.placed_mashines[pos].dir;
                    t.position = pos;
                }
                break;
            }
            case 1: {
                if (GetMachine(new Vector2(pos.x-1, pos.y)) is ConvayerBelt) { 
                    move_dir = P_action.placed_mashines[pos].dir;
                    t.position = pos;
                }
                break;
            }
            case 2: {
                if (GetMachine(new Vector2(pos.x, pos.y+1)) is ConvayerBelt) { 
                    move_dir = P_action.placed_mashines[pos].dir;
                    t.position = pos;
                }
                break;
            }
            case 3: {
                if (GetMachine(new Vector2(pos.x+1, pos.y)) is ConvayerBelt) { 
                    move_dir = P_action.placed_mashines[pos].dir;
                    t.position = pos;
                }
                break;
            }
        }
    }

    private Machine_Base GetMachine(Vector2 pos) {
        if (!P_action.placed_mashines.ContainsKey(pos)) return null;
        return P_action.placed_mashines[pos];
    } 

    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Resorces>();
        if (!r) return;
        if (r.item_name != item_name) return;

        if (count > r.count || (count == r.count && gameObject.GetInstanceID() > col.gameObject.GetInstanceID())) {
            Grouped.InitNewGroup(transform.position, this, r);
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }
}
