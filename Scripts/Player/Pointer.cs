using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour{
    public static float angle;
    public static bool fliped = false;
    public SpriteRenderer[] reliantSpritesX;
    public SpriteRenderer[] reliantSpritesY;

    void Update(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = (mousePos - object_pos).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if ((Mathf.Abs(angle) > 90 && Mathf.Abs(angle) <= 180) != fliped) {
            fliped = !fliped;
            foreach(SpriteRenderer sprite in reliantSpritesY) {
                sprite.flipY = fliped;
            }
            foreach(SpriteRenderer sprite in reliantSpritesX) {
                sprite.flipX = fliped;
            }
        }
    }
}
