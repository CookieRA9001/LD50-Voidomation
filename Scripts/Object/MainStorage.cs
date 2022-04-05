using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStorage : Machine_Base
{
    public static Dictionary<string, int> ResourcesInStorage = new Dictionary<string, int>();
    public static Dictionary<string, Resorces> StringToResource = new Dictionary<string, Resorces>();
    Vector2Int[] positions = new Vector2Int[4];
    public override void Start()
    {
        ResourcesInStorage = new Dictionary<string, int>();
        StringToResource = new Dictionary<string, Resorces>();
        PrefabLabery.UpdateStringToResource();
        ResourcesInStorage.Add("Copper", 100);
        BlockBuilding();
        base.Start();
    }

    private void Awake()
    {
        indestructable = true;
    }

    public void BlockBuilding() {
        Vector2 parPos = new Vector2(transform.parent.transform.position.x, transform.parent.transform.position.y);
        positions[0] = new Vector2Int((int)(parPos.x-0.5f), (int)(parPos.y-0.5f));
        positions[1] = new Vector2Int((int)(parPos.x-0.5f), (int)(parPos.y+0.5f));
        positions[2] = new Vector2Int((int)(parPos.x+0.5f), (int)(parPos.y-0.5f));
        positions[3] = new Vector2Int((int)(parPos.x+0.5f), (int)(parPos.y+0.5f));
        for (int i = 0; i < 4; i++) {
            P_action.placed_mashines.Add(positions[i], this);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void CallOnDestroy()
    {
        base.CallOnDestroy();
    }

    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Resorces>();
        if (r) AddItem(r.item_name, r.gameObject, r);
    }

    void AddItem(string name, GameObject go, Resorces r) {
        if (ResourcesInStorage.ContainsKey(name)) {
            ResourcesInStorage[name]+=r.count;
        } else {
            ResourcesInStorage.Add(name, r.count);
        }
        Destroy(go);
    }
}
