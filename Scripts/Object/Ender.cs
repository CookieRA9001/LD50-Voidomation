using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ender : Machine_Base {
    static public float EnergyCost = 0;
    public float time_till_tick = 1, time_remaining;
    static public float time_till_death = 100, time_remaining_for_death;
    static public bool Colappsing = false;
    static public int capasity;
    public ParticleSystem ps;
    public GameObject blackness;
    private static float time = 0;
    static public Vector2 Pos;

    public override void Start() {
        Pos = transform.position;
        time = -180;
        capasity = 0;
        Colappsing = false;
        time_till_death = 100;
        EnergyCost = 0;
        CheckPowerConnections(6);
        time_remaining_for_death = time_till_death;
        connectsToBelts = false;
        indestructable = true;

        // add self to grid
        Vector2 pos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        for (int i = -2; i<3; i++) { 
            for (int j = -2; j<3; j++) {
                P_action.placed_mashines.Add(new Vector2(pos.x+i,pos.y+j), this);
            }
        }
    }

    public override void Update() {
        capasity = charge;
        time += Time.deltaTime;
        if (Colappsing) { 
            if (time_remaining_for_death > 0) {
                time_remaining_for_death -= Time.deltaTime;
            }
            else {
                death();
            }
        }

        if (time_remaining > 0) {
            time_remaining -= Time.deltaTime;
        }
        else {
            tick();
        }
    }

    public void tick() { 
        time_remaining = time_till_tick;
        EnergyCost +=  Mathf.Max(( (1+EnergyCost)*2 + (time/(time_remaining_for_death/10)) ) / time_remaining_for_death, 0);
        if ( charge >= (int)EnergyCost ) {
            charge -= (int)EnergyCost;
            Colappsing = false;
        }
        else {
            Colappsing = true;
            ParticleSystem.EmissionModule emit = ps.emission;
            ParticleSystem.VelocityOverLifetimeModule vOverLife = ps.velocityOverLifetime;
            ParticleSystem.NoiseModule noise = ps.noise;
            float inv_death = 100 - time_remaining_for_death;
            emit.rateOverTime = new ParticleSystem.MinMaxCurve(20 + Mathf.Max(inv_death+20, 0)*2);
            vOverLife.radial = new ParticleSystem.MinMaxCurve((Mathf.Max(inv_death, 0)/-10), (Mathf.Max(inv_death, 0)/10) - 3);
            noise.strength = new ParticleSystem.MinMaxCurve(0 + Mathf.Max(inv_death/10, 0));
        }
    }

    public void death() {
        // end game
        Instantiate(blackness, new Vector3(transform.position.x, transform.position.y, -20), new Quaternion(0, 0, 0, 0));
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("end") ) {
            go.GetComponent<Text>().enabled = true;
            if (go.name == "Time") go.GetComponent<Text>().text = "You delayed the end for: " + (int)(time*100)/100f + " seconds!";
        }
        P_action.EButton.SetActive(true);
        P_action.pause = true;
        transform.GetComponent<Ender>().enabled = false;
    }

    static public void Stablalize(float stability) {
        time_remaining_for_death += stability;
    }
}
