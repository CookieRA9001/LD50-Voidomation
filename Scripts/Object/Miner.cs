using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour {
    public Resorces[] ore;

    void Start() {
        Vector2Int v2i = GenerateEvn.getIndexs(transform.position);
        int tile_index = GenerateEvn.PlacedTileIndexes[v2i.x, v2i.y];
        ResourceGen rg = GetComponent<ResourceGen>();
        if (rg && tile_index > 7) {
            rg.resorce = ore[tile_index-8];
        }
    }
}
