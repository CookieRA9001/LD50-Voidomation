using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grouped : Resorces {
    public SpriteRenderer[] groupedObjectsSR;

    public static void InitNewGroup(Vector3 pos, Resorces r1, Resorces r2) { 
        Grouped g = Instantiate(PrefabLabery.GetGroupedPrefab(), pos, new Quaternion(0, 0, 0, 0)).GetComponent<Grouped>();
        g.count = r1.count + r2.count;
        g.item_name  = r1.item_name;
        g.sprite = r1.sprite;
        g.item_object = r1.gameObject;
        g.move_dir = r1.move_dir;
        g.move_speed = 1;
        g.updateSprites();
    }

    private void updateSprites() {
        short c = 0;
        foreach (SpriteRenderer sr in groupedObjectsSR) {
            if (c >= count) return;
            sr.sprite = sprite;
            c++;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Grouped>();
        if (!r) r = col.transform.GetComponent<Resorces>();

        if (r) {
            if (r.item_name != item_name) return;

            if (count > r.count || (count == r.count && gameObject.GetInstanceID() > col.gameObject.GetInstanceID())) {
                count += r.count;
                Destroy(col.gameObject);
                updateSprites();
            }
        }
        
    }
}
