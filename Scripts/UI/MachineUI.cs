using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour {
    private float animation_time=0.25f, time_remaining=0;
    private TextMeshProUGUI TMP;
    private RectTransform rect_t;
    private Vector2 Pos;
    private new Camera camera;
    public bool hidden = true;
    private float unit_to_UI_pixels;

    private float max_width, min_height, extra_heigth;

    void Start() {
        rect_t = GetComponent<RectTransform>();
        TMP = GetComponentInChildren<TextMeshProUGUI>();
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update() {
        if (time_remaining > 0) {
            time_remaining -= Time.deltaTime;
            // update size
            var width = (animation_time - time_remaining) / animation_time * max_width;
            var height = min_height + (animation_time - time_remaining) / animation_time * extra_heigth;

            rect_t.sizeDelta = new Vector2(width, height);
        }
        if (!hidden) posistion();
    }
    public void Hide() {
        hidden = true;
        rect_t.sizeDelta = new Vector2(0, 0);
    }

    private void posistion() { 
        Vector2 screen_pos = camera.WorldToScreenPoint(Pos);
        Vector2 new_pos;
        if (screen_pos.x > Screen.width/2) {
            new_pos = camera.WorldToScreenPoint(new Vector2(Pos.x - 0.5f, Pos.y + 0.5f));
            rect_t.pivot = new Vector2(1,1);
        }
        else {
            new_pos = camera.WorldToScreenPoint(new Vector2(Pos.x + 0.5f, Pos.y + 0.5f));
            rect_t.pivot = new Vector2(0, 1);
        }
        unit_to_UI_pixels = Mathf.Abs((new_pos - screen_pos).x) * 2;
        rect_t.position = new_pos;
    }

    public void Init(Vector2 pos) {
        Pos = pos;
        posistion();

        // get date to place in TextMesh
        TMP.text = getStatsText(pos);
        
        max_width = 5 * unit_to_UI_pixels;
        min_height = unit_to_UI_pixels;
        int lineCount = TMP.GetTextInfo(TMP.text).lineCount;
        if (lineCount > 2) extra_heigth = (lineCount - 2) * unit_to_UI_pixels/3;
        rect_t.sizeDelta = new Vector2( 0, min_height);

        time_remaining += animation_time;
        hidden = false;
    }

    private string getStatsText(Vector2 pos) {
        string s= "";

        var Machine = P_action.placed_mashines[pos];
        if (!Machine) return "";

        if (Machine is PowerNode) {
            s = "Max power: " + Machine.battery + "KJ\nPower: "
               + Machine.charge + " KJ\nTransfer Speed: "
               + ((PowerNode)Machine).powerPerTick / ((PowerNode)Machine).TickLength + "KW";
        }
        else if (Machine is PowerGen) {
            s = "Max power: " + Machine.battery + "KJ\nPower: "
               + Machine.charge + " KJ\nGenerates: "
               + ((PowerGen)Machine).gen / (int)((PowerGen)Machine).time_for_resorce + "KW";
        }
        else if (Machine is Ender) {
            s = "Void Status: " + (Ender.Colappsing ? "Collapsing!" : ((int)Ender.capasity == 0 ? "Inactive" : "Active")) + "\nStablity - " + Ender.time_remaining_for_death + "\nEnergy Consumtion - " + (int)Ender.EnergyCost + " KJ\nEnergy Reserves - " + Ender.capasity + " KJ";
        }
        else if (Machine is Stablize) {
            s = "Max power: " + Machine.battery + "KJ\nPower: "
               + Machine.charge + " KJ\nPower Cost: "
               + ((Stablize)Machine).power_cost / (int)((Stablize)Machine).time_for_resorce + "KW";
        }
        else if (Machine is ResourceGen) {
            s = "Max power: " + Machine.battery + "KJ\nPower: "
               + Machine.charge + " KJ\nPower Cost: "
               + ((ResourceGen)Machine).power_cost / (int)((ResourceGen)Machine).time_for_resorce + "KW";
        }
        else {  // Machine_base
            s = "Max power: " + Machine.battery + "KJ\nPower: " + Machine.charge + " KJ";
        }

        return s;
    }
}
