using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessPanel : MonoBehaviour {
    void Start() {
        Vector2Int v2i = GenerateEvn.getIndexs(transform.position);
        float light_level = GenerateEvn.Environment[2, v2i.x, v2i.y]*GenerateEvn.Environment[2, v2i.x, v2i.y];
        PowerGen pg = GetComponent<PowerGen>();
        if (pg) {
            pg.gen = Mathf.RoundToInt(pg.gen * light_level);
        }
    }
}
