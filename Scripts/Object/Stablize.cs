using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stablize : Machine_Base {
    public float time_for_resorce = 1, time_remaining;
    public bool working = false;
    public int power_cost = 100;
    public float stabalization = 2;
    public override void Start() {
        base.Start();
        connectsToBelts = false;
    }

    public override void Update() {
        if (time_remaining > 0) {
            time_remaining -= Time.deltaTime;
        }
        else {
            Stabalize();
            time_remaining = time_for_resorce;
        }
    }

    public virtual void Stabalize() {
        if (charge>=power_cost) {
            charge -= power_cost;
            if (!working) {
                Ender.Stablalize(stabalization);
                working = true;
            }
        }
        else {
            if (working) {
                Ender.Stablalize(-stabalization);
                working = false;
            }
        }
    }
}
