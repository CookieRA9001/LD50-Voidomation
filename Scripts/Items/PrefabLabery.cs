using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLabery : MonoBehaviour {
    public GameObject GroupedPrefab;
    public List<Resorces> Resources;
    public List<Item> Items;

    public static GameObject GetGroupedPrefab(){
        return (GameObject.FindGameObjectWithTag("Game").GetComponent<PrefabLabery>()).GroupedPrefab;
    }

    public static List<Resorces> GetResources() {
        return (GameObject.FindGameObjectWithTag("Game").GetComponent<PrefabLabery>()).Resources;
    }

    public static List<Item> GetItems() {
        return (GameObject.FindGameObjectWithTag("Game").GetComponent<PrefabLabery>()).Items;
    }

    private void Start()
    {
        UpdateStringToResource();
    }

    public static void UpdateStringToResource() {
        MainStorage.StringToResource.Clear();
        foreach (Resorces res in GetResources()) {
            if (!MainStorage.StringToResource.ContainsKey(res.item_name)) {
                MainStorage.StringToResource.Add(res.item_name, res);
            }
        }
    }
}
