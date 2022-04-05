using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour {
    public Sprite sprite;
    public string item_name;
    public string item_description;
    public string item_category;
    public GameObject item_object;

    [Serializable]
    public struct Recipe {
        public string name;
        public int count;
    }
    public Recipe[] recipe;

    void Start() { }


    void Update() {}
}
