using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInput : MonoBehaviour {
    public List<Resorces> filer = new List<Resorces>();
    public Dictionary<string, int> StoredResources = new Dictionary<string, int>();
    // index: 0 - down, 1 - left, 2 - up, 3 - right (if pointing down)
    public bool[] input_sides= {false,false,true,false};
    ResourceGen rg;
    public int DebugStoredResourcesCount;
    public List<Recipe> recipes = new List<Recipe>();

    void Start() {
        rg = GetComponent<ResourceGen>();
        if (rg) {
            rg.gen_requirments = validation;
        }
    }

    private void Update() {
        DebugStoredResourcesCount = 0;
        foreach (var r in StoredResources) DebugStoredResourcesCount += r.Value;
    }

    void OnTriggerEnter2D(Collider2D col) {
        Resorces r = col.transform.GetComponent<Resorces>();
        if (!r) return;
        if (filer.FindAll(x => x.item_name == r.item_name).Count>0) {
            Vector3 pos = transform.position;
            Vector3 col_pos = col.transform.position;

            bool x_axis = Mathf.RoundToInt(pos.x - col_pos.x) == 0;
            bool y_axis = Mathf.RoundToInt(pos.y - col_pos.y) == 0;

            if (y_axis && !x_axis) {
                if (pos.x > col_pos.x) { // right
                    AddItem(r.item_name, 1, col.transform.gameObject, r);
                }
                else { // left
                    AddItem(r.item_name, 3, col.transform.gameObject, r);
                }
            }
            else if (x_axis && !y_axis) { 
                if (pos.y > col_pos.y) { // top
                    AddItem(r.item_name, 0, col.transform.gameObject, r);
                }
                else { // bottom
                    AddItem(r.item_name, 2, col.transform.gameObject, r);
                }
            }
            else { /* Add fail particals */ }
        }
        else if (Vector2.Distance(col.transform.position, transform.position)>= 0.75f) {
            col.transform.GetComponent<Rigidbody2D>().velocity *= 0;
            r.move_dir = -1;
        }
    }

    void AddItem(string name, int side, GameObject go, Resorces r) {
        if (input_sides[Mathf.Abs(side-rg.dir)%4]) {
            if (StoredResources.ContainsKey(name)) {
                StoredResources[name]+=r.count;
            }
            else {
                StoredResources.Add(name, r.count);
            }
            Destroy(go);
        }
        else {
            r.move_dir = -1;
            go.transform.GetComponent<Rigidbody2D>().velocity *= 0;
        }
    }

    // does the machine have all the needed items to make its thing
    public virtual bool validation()  {
        foreach (Recipe r in recipes) {
            bool has_all_resources = true;
            for (int i = 0; i< r.needed_items.Length; i++) {
                if (!StoredResources.ContainsKey(r.needed_items[i].item_name)) {
                    has_all_resources = false;
                }
                else if (StoredResources[r.needed_items[i].item_name] < r.item_counts[i]) {
                    has_all_resources = false;
                }
            }
            if (has_all_resources && r.power_needed <= rg.charge) {
                spend_resources(r);
                rg.resorce = r.product;
                return true;
            }
        }
        return false;
    }

    // spend the needed resources if validation passes
    public virtual void spend_resources(Recipe r) {
        for (int i = 0; i<r.needed_items.Length; i++) {
            StoredResources[r.needed_items[i].item_name] -= r.item_counts[i];
        }
        rg.charge -= r.power_needed;
    }
}
