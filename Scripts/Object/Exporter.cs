using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exporter : Machine_Base
{
    public float time_for_resorce = 3, time_remaining;
    public bool valid = false;
    public Resorces resorce;
    public int power_cost = 0;

    // is it in a valid spot
    public delegate bool ValidSpotToGen();
    public ValidSpotToGen validation = null;

    public delegate bool GenRequirments();
    public GenRequirments gen_requirments = null;

    public bool nextToStorage;
    public ExportMenu exporterMenu;
    private Vector2Int myPos;

    void Awake() {
        time_remaining = time_for_resorce;
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, p.y, -8);
    }

    public override void Start() {
        base.Start();
        ExecuteValidation();
        myPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        exporterMenu = GameObject.FindGameObjectWithTag("ExporterMenu").GetComponent<ExportMenu>();
        CheckForStorage();
    }

    public override void Update() {
        if (time_remaining > 0) {
            time_remaining -= Time.deltaTime;
        }
        else {
            CheckForStorage();
            if (resorce && nextToStorage) {
                if (MainStorage.ResourcesInStorage.ContainsKey(resorce.item_name)) {
                    if (MainStorage.ResourcesInStorage[resorce.item_name] > 0) {
                        MainStorage.ResourcesInStorage[resorce.item_name]--;
                        Generate();
                    }
                }
            }
            time_remaining = time_for_resorce;
        }
    }

    public void ExecuteValidation() {
        if (validation != null) {
            valid = validation();
        }
        else valid = true;
    }

    public void CheckForStorage() {
        void calc (Vector2Int p) {
            if (P_action.placed_mashines.ContainsKey(p)) {
                if (P_action.placed_mashines[p] is MainStorage) {
                    nextToStorage = true;
                }
            }
        }
        nextToStorage = false;
        calc(new Vector2Int(myPos.x+1, myPos.y));
        calc(new Vector2Int(myPos.x-1, myPos.y));
        calc(new Vector2Int(myPos.x, myPos.y+1));
        calc(new Vector2Int(myPos.x, myPos.y-1));
    }

    public void UpdateResource(Resorces res) {
        resorce = res;
    }

    public virtual void Generate() {
        if (resorce && valid) {
            if (gen_requirments == null || gen_requirments()) {
                }
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

    public override void ClickOnMeOwO()
    {
        if (nextToStorage) {
            exporterMenu.exporter = this;
            exporterMenu.OpenMenu();
        } else {
            InventoryMenu.prompt.SetPrompt("Place exporter next to the storage system!", 2f, Color.red);
        }
    }

    public void ChangeDir(short direction) {
        dir = direction;
        while (dir>3) dir -= 4;

        switch (dir) {
            case 0: {
                transform.rotation = Quaternion.Euler(0,0,0);
                break;
            }
            case 1: {
                transform.rotation = Quaternion.Euler(0,0,270);
                break;
            }
            case 2: {
                transform.rotation = Quaternion.Euler(0,0,180);
                break;
            }
            case 3: {
                transform.rotation = Quaternion.Euler(0,0,90);
                break;
            }
        }
    }
}
