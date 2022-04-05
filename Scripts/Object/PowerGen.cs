using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGen : ResourceGen {
    public int gen = 1;
    public int power_ticks = 10, ticks_remaining;

    void Awake() {
        time_remaining = time_for_resorce;
        ticks_remaining = 0;
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, p.y, -8);
    }

    public override void Update() {
        if (valid) {
            if (ticks_remaining > 0) {
                if (time_remaining > 0) {
                    time_remaining -= Time.deltaTime;
                }
                else {
                    Generate();
                    ticks_remaining -= 1;
                    time_remaining = time_for_resorce;
                }
            }
            else CanProduceEnergy();
        }
    }

    public virtual void CanProduceEnergy() {
        if (gen_requirments == null || gen_requirments()) {
            ticks_remaining = power_ticks;
        }
    }

    public override void Generate() {
        charge = Mathf.Min(charge + gen, battery);
    }
}
