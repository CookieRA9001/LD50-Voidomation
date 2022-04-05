using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvayerBelt : Machine_Base {
    private bool[] adjasonts; // index: 0 - down, 1 - left, 2 - up, 3 - right
    public short sprite_index = 0;
    public SpriteRenderer sr;

    public override void Start() {
        sr = transform.GetComponent<SpriteRenderer>();
        battery = 0;
    }

    public override void UpdateBelts () {
        Vector2Int pos_origin = new Vector2Int((int)transform.position.x,(int)transform.position.y);
        sprite_index = 0;
        adjasonts = new bool[] {false, false, false, false};

        UpdateAdjBelt(new Vector2Int(pos_origin.x+1, pos_origin.y), 3, 1, 1);
        UpdateAdjBelt(new Vector2Int(pos_origin.x-1, pos_origin.y), 1, 2, 3);
        UpdateAdjBelt(new Vector2Int(pos_origin.x, pos_origin.y+1), 2, 4, 0);
        UpdateAdjBelt(new Vector2Int(pos_origin.x, pos_origin.y-1), 0, 8, 2);

        sprite_index += (short)(dir*16);
        UpdateSprite();
    }

    public void UpdateAdjBelt(Vector2Int pos, int adj, short spriteInc, int dir) {
        if ( P_action.placed_mashines.ContainsKey(pos) ) {
            if (!P_action.placed_mashines[pos].connectsToBelts) {
                adjasonts[adj] = false;
                return;
            }
            adjasonts[adj] = true;
            
            sprite_index += spriteInc;
            if ( P_action.placed_mashines[pos] is ConvayerBelt ) {
                ((ConvayerBelt)P_action.placed_mashines[pos]).UpdateBeltDumb(dir, true);
            }
        }
    }

    public void UpdateBeltDumb (int update_dir, bool added) {
        Vector2 pos = transform.position;
        if (adjasonts[update_dir] != added) {
            adjasonts[update_dir] = added;
            int inv = -1;
            if (added) inv = 1;

            if ( update_dir == 0 ) {
                sprite_index += (short)(8*inv);
            }
            else if ( update_dir == 1 ) {
                sprite_index += (short)(2*inv);
            }
            else if ( update_dir == 2 ) {
                sprite_index += (short)(4*inv);
            }
            else {
                sprite_index += (short)(1*inv);
            }
        }
        UpdateSprite();
    }

    public override void UpdateSprite () {
        sprite_index = (short)(sprite_index % 64);
        sr.sprite = BeltSprites.belt_sprites[sprite_index];
    }

    public override void ClickOnMeOwO() {
        sprite_index -= (short)(dir*16);
        dir++;
        if (dir>3) dir = 0;
        sprite_index += (short)(dir*16);

        UpdateSprite();
    }
    public override void CallOnDestroy()
    {
        base.CallOnDestroy();
    }
}
