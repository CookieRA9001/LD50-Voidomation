using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerNode : Machine_Base
{
    public Dictionary<Vector2Int, Machine_Base> connections = new Dictionary<Vector2Int, Machine_Base>();
    public Dictionary<Vector2Int, GameObject> lines = new Dictionary<Vector2Int, GameObject>();
    public List<PowerGen> generators;

    public GameObject linePrefab;

    public int powerPerTick;
    public float timer, TickLength;

    public override void Start() {
        base.Start();
        connectsToBelts = false;
    }

    public override void Update()
    {
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            if (connections.Count > 0 && generators.Count != connections.Count) GivePower();
            TakePower();
            timer = TickLength;
        }
    }

    public void GivePower() {
        int amount = Mathf.FloorToInt(charge / (connections.Count-generators.Count));
        if (amount < 0) amount = 0;
        if (amount == 0 && charge > powerPerTick) amount = powerPerTick;
        if (amount > powerPerTick) amount = powerPerTick;
        foreach (Machine_Base machine in connections.Values) {
            if (!(machine is PowerGen) && machine.charge + amount <= machine.battery && charge >= amount) {
                machine.GainPower(amount);
                LosePower(amount);
            }
        }
    }

    public void TakePower() {
        if (charge <= battery-powerPerTick) {
            foreach(PowerGen gen in generators) {
                if(gen.charge >= powerPerTick && charge+powerPerTick <= battery) {
                    gen.LosePower(powerPerTick);
                    GainPower(powerPerTick);
                }
            }
        }
    }

    public void PowerConnect(Machine_Base from, Vector2Int fromPos) {
        if (!connections.ContainsKey(fromPos)) {
            connections.Add(fromPos, from);
        }
        if (from is PowerGen) {
            PowerGen pGen = from.gameObject.GetComponent<PowerGen>();
            if (!generators.Contains(pGen)) generators.Add(pGen);
        }
        CreateLine(fromPos);
        from.connectedNodes.Add(this);
    }

    public void PowerDisconnect(Machine_Base from, Vector2Int fromPos) {
        if (connections.ContainsKey(fromPos)) {
            connections.Remove(fromPos);
        }
        if (from is PowerGen) {
            PowerGen pGen = from.gameObject.GetComponent<PowerGen>();
            if (generators.Contains(pGen)) generators.Remove(pGen);
        }
        RemoveLine(fromPos);
        from.connectedNodes.Remove(this);
    }

    public void CreateLine(Vector2Int targetPos) {
        Vector2 selfPos = new Vector2Int((int)transform.position.x,(int)transform.position.y);

        Vector2 middlePos = new Vector2(selfPos.x + (targetPos.x - selfPos.x)/2, selfPos.y + (targetPos.y - selfPos.y)/2);

        Vector2 toOther = (selfPos - targetPos).normalized;
        float angle = Mathf.Atan2(toOther.y, toOther.x) * Mathf.Rad2Deg + 90;

        float distance = Vector2.Distance(selfPos, targetPos);

        GameObject line = Instantiate(linePrefab, new Vector3(middlePos.x, middlePos.y, -16), Quaternion.Euler(0, 0, angle)) as GameObject;
        line.transform.localScale = new Vector3(0.05f, distance - 0.025f, line.transform.localScale.z);
        if (!lines.ContainsKey(targetPos)) {
            lines.Add(targetPos, line);
        }
    }

    public void RemoveLine(Vector2Int pos) {
        Destroy(lines[pos]);
        if (lines.ContainsKey(pos)) {
            lines.Remove(pos);
        }
    }

    public override void CallOnDestroy()
    {
        List<Vector2Int> temp = new List<Vector2Int>();

        if (connections.Count > 0) {
            foreach(Vector2Int con in connections.Keys) {
                temp.Add(con);
            }
            foreach(Vector2Int con in temp) {
                PowerDisconnect(connections[con], con);
            }
        }

        temp.Clear();

        if (lines.Count > 0) {
            foreach(Vector2Int pos in lines.Keys) {
                temp.Add(pos);
            }
            foreach(Vector2Int pos in temp) {
                RemoveLine(pos);
            }
        }
        
        base.CallOnDestroy();
    }
}
