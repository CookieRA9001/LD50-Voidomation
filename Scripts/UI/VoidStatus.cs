using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoidStatus : MonoBehaviour {
    public Text tm;
    public Color Doom_Color, Good_Color;

    void Update() {
        tm.text = "Void Status: " + (Ender.Colappsing ? "Collapsing!" : ((int)Ender.capasity == 0 ? "Inactive" : "Active")) + "\nStablity - " + Ender.time_remaining_for_death +"\nEnergy Consumtion - " + (int)Ender.EnergyCost + " KJ\nEnergy Reserves - " + Ender.capasity + " KJ";

        if (Ender.Colappsing && tm.color != Doom_Color) {
            tm.color = Doom_Color;
        }
        else if (!Ender.Colappsing && tm.color != Good_Color) {
            tm.color = Good_Color;
        }
    }
}
