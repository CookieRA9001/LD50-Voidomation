using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_hover : MonoBehaviour {
    public MachineUI UI;
    private float hold_time=0.5f, time_remaining=0;
    private Vector2 lastMousePos;

    void Start() {
        lastMousePos = new Vector2(0,0);
    }

    void Update() {
        if (InventoryMenu.inMenu || P_action.pause) return;

        Vector2 pos = getPos();

        if (lastMousePos == pos) {
            if (P_action.placed_mashines.ContainsKey(pos)) {
                if (P_action.placed_mashines[pos].battery == 0) {
                    resetHover();
                    return;
                }

                if (time_remaining<=0) {
                    if (UI.hidden) {
                        UI.Init(pos);
                    }
                }
                else {
                    time_remaining -= Time.deltaTime;
                }
            }
            else {
                resetHover();
            }
        }
        else {
            resetHover();
            lastMousePos = pos;
        }
    }

    private void resetHover() { 
        UI.Hide();
        time_remaining = hold_time;
    }

    public Vector2 getPos() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)(Mathf.Round(worldPosition.x));
        int y = (int)(Mathf.Round(worldPosition.y));
        Vector2 pos = new Vector2(x, y);
        return pos;
    }
}
