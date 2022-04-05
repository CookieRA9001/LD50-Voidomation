using UnityEngine;
using System.Collections.Generic;

public class Cammera_follow : MonoBehaviour{
    //public Transform target;
    public float smoothing = 0.25f;
    public Transform player;

    void Start() {
        // to lazy to add a new script: this resets P_action statics even when the player is not active (like in the main menu)
        P_action.drag_objects = new Dictionary<Vector2, GameObject>();
        P_action.placed_mashines = new Dictionary<Vector2, Machine_Base>();
        P_action.pause = false;
    }

    void FixedUpdate(){
        Vector3 desPosito = new Vector3(player.position.x,player.position.y,transform.position.z);
        transform.position = Vector3.Lerp(transform.position,desPosito,smoothing);
    }
}
