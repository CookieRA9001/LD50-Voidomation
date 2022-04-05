using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSprites : MonoBehaviour {
    public Sprite[] sprite_arr;
    static public Sprite[] belt_sprites;
    void Start() {
        belt_sprites = sprite_arr;
    }
}
