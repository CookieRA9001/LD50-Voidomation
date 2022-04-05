using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGen : Machine_Base {
    public float time_for_resorce = 3, time_remaining;
    public bool valid = false;
    public Resorces resorce;
    public int power_cost = 0;

    // is it in a valid spot
    public delegate bool ValidSpotToGen();
    public ValidSpotToGen validation = null;

    public delegate bool GenRequirments();
    public GenRequirments gen_requirments = null;

    void Awake() {
        time_remaining = time_for_resorce;
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, p.y, -8);
    }

    public override void Start() {
        base.Start();
        ExecuteValidation();
    }

    public override void Update() {
        if (time_remaining > 0) {
            time_remaining -= Time.deltaTime;
        }
        else {
            Generate();
            time_remaining = time_for_resorce;
        }
    }

    public void ExecuteValidation() {
        if (validation != null) {
            valid = validation();
        }
        else valid = true;
    }

    public virtual void Generate() {
        if (resorce && valid) {
            if (gen_requirments == null || gen_requirments()) {
                if (gen_requirments == null) {
                    if (charge >= power_cost) {
                        charge -= power_cost;
                        Instantiate(resorce.item_object, new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f), new Quaternion(0, 0, 0, 0)).GetComponent<Resorces>().move_dir = dir;
                    }
                
                }
                else if (gen_requirments()) {
                    Instantiate(resorce.item_object, new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f), new Quaternion(0, 0, 0, 0)).GetComponent<Resorces>().move_dir = dir;
                }
            }
        }
    } 
}
