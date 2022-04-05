using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine_Base : MonoBehaviour {
    public short dir = 0; // 0 - down, 1 - left, 2 - up, 3 - right
    public int battery = 100, charge = 0;
    public bool indestructable = false;
    public bool connectsToBelts = true;
    public List<PowerNode> connectedNodes;

    public virtual void Start() {
        if (battery > 0) CheckPowerConnections(4);
    }
    public virtual void Update() { }

    public virtual void UpdateBelts () {
        Vector2Int pos_origin = new Vector2Int((int)transform.position.x,(int)transform.position.y);

        if (!connectsToBelts) {
            return;
        }

        UpdateAdjBelt(new Vector2Int(pos_origin.x+1, pos_origin.y), 1);
        UpdateAdjBelt(new Vector2Int(pos_origin.x-1, pos_origin.y), 3);
        UpdateAdjBelt(new Vector2Int(pos_origin.x, pos_origin.y+1), 0);
        UpdateAdjBelt(new Vector2Int(pos_origin.x, pos_origin.y-1), 2);
    }

    public virtual void UpdateAdjBelt(Vector2Int pos, int dir) {
        if ( P_action.placed_mashines.ContainsKey(pos) ) { // right
            if ( P_action.placed_mashines[pos] is ConvayerBelt ) {
                ((ConvayerBelt)P_action.placed_mashines[pos]).UpdateBeltDumb(dir, true);
            }
        }
    }
    public virtual void UpdateSprite () { }

    public virtual void ClickOnMeOwO() {
        dir++;
        if (dir>3) dir -= 4;

        switch (dir) {
            case 0: {
                transform.rotation = Quaternion.Euler(0,0,0);
                break;
            }
            case 1: {
                transform.rotation = Quaternion.Euler(0,0,270);
                break;
            }
            case 2: {
                transform.rotation = Quaternion.Euler(0,0,180);
                break;
            }
            case 3: {
                transform.rotation = Quaternion.Euler(0,0,90);
                break;
            }
        }
    }

    public virtual void UpdateSurrounding() {
        Vector2Int p = new Vector2Int((int)transform.position.x,(int)transform.position.y);
        
        void CheckForBelt(Vector2Int checkPos) {
            if (P_action.placed_mashines.ContainsKey(checkPos)) {
                if (P_action.placed_mashines[checkPos] is ConvayerBelt) P_action.placed_mashines[checkPos].UpdateBelts();
            }
        }
        
        CheckForBelt(new Vector2Int(p.x+1, p.y));
        CheckForBelt(new Vector2Int(p.x-1, p.y));
        CheckForBelt(new Vector2Int(p.x, p.y+1));
        CheckForBelt(new Vector2Int(p.x, p.y-1));
    }

    public virtual void CallOnDestroy() {
        Vector2Int p = new Vector2Int((int)transform.position.x,(int)transform.position.y);
        if (P_action.placed_mashines.ContainsKey(p)) P_action.placed_mashines.Remove(p);
        UpdateSurrounding();
        if (connectedNodes.Count > 0) {

            List<PowerNode> temp = new List<PowerNode>();

            foreach(PowerNode node in connectedNodes) {
                temp.Add(node);
            }
            foreach(PowerNode node in temp) {
                node.PowerDisconnect(this, p);
            }
        }
        Destroy(this.transform.parent.gameObject);
    }

    public virtual void CheckPowerConnections(int radius) {
        Vector2Int center = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        for (int i = center.x - radius; i <= center.x + radius; i++) {
            for (int j = center.y - radius; j <= center.y + radius; j++) {
                Vector2Int cPos = new Vector2Int(i, j);
                if (Vector2Int.Distance(center, cPos) <= radius) {
                    if (P_action.placed_mashines.ContainsKey(cPos) && cPos != center) {
                        if (P_action.placed_mashines[cPos] is PowerNode) {
                            P_action.placed_mashines[cPos].gameObject.GetComponent<PowerNode>().PowerConnect(this, center); 
                        }
                        if (P_action.placed_mashines[cPos].battery > 0 && this is PowerNode) {
                            this.gameObject.GetComponent<PowerNode>().PowerConnect(P_action.placed_mashines[cPos], cPos);
                        }
                    }
                }
            }
        }
    }

    public virtual void GainPower(int power) {
        charge += power;
    }

    public virtual void LosePower(int power) {
        charge -= power;
    }
}
