using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOres : MonoBehaviour {
    public void Awake() {
        ResourceGen rg = GetComponent<ResourceGen>();
        if (rg) {
            rg.validation = validation; 
            rg.ExecuteValidation();
        }
    }

    public bool validation() {
        Vector2Int v2i = GenerateEvn.getIndexs(transform.position);
        int tile_index = GenerateEvn.PlacedTileIndexes[v2i.x, v2i.y];
        return tile_index > 7;
    }
}
