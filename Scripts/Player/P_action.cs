using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_action : MonoBehaviour {
    public int inv_index = 0;
    public int inv_count;
    // interaction delay
    private float delay = 0.2f, time_remaining = 0;

    public Dictionary<KeyCode, int> KeyCodeToItemIndex = new Dictionary<KeyCode, int>();
    public List<Item> Inventory;
    public GameObject placing1x1, placing1x1Fail, destroying1x1, destroying1x1Hit;
    public InventoryBar inventoryBar;
    public static Dictionary<Vector2, Machine_Base> placed_mashines = new Dictionary<Vector2, Machine_Base>();
    private bool held, anyPlaced, pauseM0, delMode;
    private Vector2 startPos, lastPos;
    private int lastDiff, lastLead;
    private int[] dragDir = {1, -1};
    public static Dictionary<Vector2, GameObject> drag_objects = new Dictionary<Vector2, GameObject>();

    public GameObject EscScreen;
    public GameObject EndButton;
    public static GameObject EButton;
    public static bool pause = false;
    public static bool hasMined = false, hasPlacedT1 = false, hasPlacedConveyor = false, hasRotated = false;

    private void Awake() {
        drag_objects = new Dictionary<Vector2, GameObject>();
        placed_mashines = new Dictionary<Vector2, Machine_Base>();
        Debug.Log("cleared");
        EButton = EndButton;
        pause = false;
        hasMined = false;
        hasPlacedT1 = false;
        hasPlacedConveyor = false;
        hasRotated = false;
    }
    void Start() {
        inv_count = Inventory.Count;
        inventoryBar.UpdateInvItems(inv_index, Inventory, inv_count);
    }

    public static int NoramlizeIndex(int inv_index, int inv_count) {
        if (inv_index >= inv_count) {
                inv_index -= inv_count;
        }
        else if (inv_index <  0) {
            inv_index += inv_count;
        }
        return inv_index;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) MainMenu.OpenMenuPrompt(EscScreen);
        if (InventoryMenu.inMenu || pause) return;
        //if(!Game.isPaused){
            // INVENTORY SCROLLING 
            if (Input.mouseScrollDelta.y != 0) {
                if (Input.mouseScrollDelta.y > 0)
                {
                    inv_index += 1;
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    inv_index -= 1;
                }
                // NORMALIZE INDEX
                inv_index = NoramlizeIndex(inv_index, inv_count);

                inventoryBar.UpdateInvItems(inv_index, Inventory, inv_count);
            }
            
            // SHORT CUTS
            foreach(KeyValuePair<KeyCode, int> input in KeyCodeToItemIndex) {
                if(Input.GetKey(input.Key)){
                    inv_index = input.Value;
                    inventoryBar.UpdateInvItems(inv_index, Inventory, inv_count);
                }
            }

            // CLICK ACTION
            if(Input.GetKey(KeyCode.Mouse0)) {
                if (!pauseM0) {
                    Vector2 pos = getPos();
                    if (Input.GetKey(KeyCode.LeftShift) && !held) {
                        delMode = true;
                        FirstInDrag(pos);
                        held = true;
                    } else if (delMode) {
                        if (lastPos != pos) {
                            RenderDrag(pos);
                        }
                        held = true;
                    } else {
                        if (Inventory[inv_index].item_name != "Conveyor Belt") {
                            PlaceMachine(Inventory[inv_index], pos);
                        } else {
                            if (!held) {
                                FirstInDrag(pos);
                            } else {
                                if (lastPos != pos) {
                                    RenderDrag(pos);
                                }
                            }
                            held = true;
                        }
                    }
                    
                }
            } else {
                held = false;
                pauseM0 = false;
                if (anyPlaced && !delMode) {
                    foreach (Vector2 pos in drag_objects.Keys) {
                        Destroy(drag_objects[pos]);
                        if (!placed_mashines.ContainsKey(pos)) PlaceConvayerBelt(Inventory[inv_index], pos, dragDir);
                    }
                    drag_objects.Clear();
                    anyPlaced = false;
                } else if (anyPlaced) {
                    foreach (Vector2 pos in drag_objects.Keys) {
                        Destroy(drag_objects[pos]);
                        if (placed_mashines.ContainsKey(pos)) {
                            if (!placed_mashines[pos].indestructable) placed_mashines[pos].CallOnDestroy();
                        }
                    }
                    drag_objects.Clear();
                    anyPlaced = false;
                }
                delMode = false;
            }

            if (time_remaining>0) {
                time_remaining -= Time.deltaTime;
            } 
            else {
                if(Input.GetKey(KeyCode.Mouse1)) {
                    if (anyPlaced) {
                        foreach (Vector2 pos in drag_objects.Keys) {
                            Destroy(drag_objects[pos].gameObject);
                        }
                        drag_objects.Clear();
                        anyPlaced = false;
                        delMode = false;
                        pauseM0 = true;
                    } else {
                        ClickMachine();
                    }
                }
            }
        //}
    }

    private void RenderDrag(Vector2 pos) {
        float diffX = Mathf.Abs(startPos.x - pos.x);
        float diffY = Mathf.Abs(startPos.y - pos.y);
        int diff = (int)Mathf.Max(diffX, diffY);
        int curLead;
        if (diffX >= diffY) curLead = -1;
        else curLead = 1;

        if ((curLead == lastLead && diff != lastDiff) || (curLead != lastLead)) {

            dragDir = CalculateDir(startPos, pos, curLead);

            foreach (Vector2 posKey in drag_objects.Keys) {
                Destroy(drag_objects[posKey].gameObject);
            }

            drag_objects.Clear();

            for (int i = 0; i <= diff; i++) {
                Vector2 tPos = new Vector2((startPos.x+i*dragDir[1])-(dragDir[0]*i*dragDir[1]), startPos.y+dragDir[0]*i*dragDir[1]);
                GameObject temp;
                
                if (!placed_mashines.ContainsKey(tPos)) {
                    Vector2 player_pos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

                    if (delMode) temp = Instantiate(destroying1x1, new Vector3(tPos.x, tPos.y, -15), transform.rotation);
                    // test if it can be placed on this tile
                    else if (player_pos == tPos) temp = Instantiate(placing1x1Fail, new Vector3(tPos.x, tPos.y, -15), transform.rotation);
                    else if (!CanPlace(tPos)) temp = Instantiate(placing1x1Fail, new Vector3(tPos.x, tPos.y, -15), transform.rotation);

                    else temp = Instantiate(placing1x1, new Vector3(tPos.x, tPos.y, -15), transform.rotation);
                } else {
                    if (delMode) temp = Instantiate(destroying1x1Hit, new Vector3(tPos.x, tPos.y, -15), transform.rotation);
                    else temp = Instantiate(placing1x1Fail, new Vector3(tPos.x, tPos.y, -15), transform.rotation);
                }

                if (temp) {
                    drag_objects.Add(tPos, temp);
                    lastPos = tPos;
                    lastLead = curLead;
                    lastDiff = diff;
                }
            }
        }

    }

    private void FirstInDrag(Vector2 pos) {
        startPos = pos;
        GameObject temp;

        if (!placed_mashines.ContainsKey(pos)) {
            if (delMode) temp = Instantiate(destroying1x1, new Vector3(pos.x, pos.y, -15), transform.rotation);
            else temp = Instantiate(placing1x1, new Vector3(pos.x, pos.y, -15), transform.rotation);
        } else {
            if (delMode) temp = Instantiate(destroying1x1Hit, new Vector3(pos.x, pos.y, -15), transform.rotation);
            else temp = Instantiate(placing1x1Fail, new Vector3(pos.x, pos.y, -15), transform.rotation);
        }

        if (temp) {
            drag_objects.Add(pos, temp);
            anyPlaced = true;
            lastPos = pos;
            lastLead = 0;
            lastDiff = 0;
        }
    }

    private int[] CalculateDir(Vector2 sPos, Vector2 cPos, int cLead) {
        int[] dDir = { 1, -1 };
        if (cLead == -1) {
            if (sPos.x - cPos.x > 0) {
                dDir = new int[] {0, -1};
            } else if (sPos.x - cPos.x < 0) {
                dDir = new int[] {0, +1};
            }
        } else {
            if (sPos.y - cPos.y > 0) {
                dDir = new int[] {1, -1};
            } else if (sPos.y - cPos.y < 0) {
                dDir = new int[] {1, 1};
            }
        }
        return dDir;
    }

    private void ClickMachine() {
        Vector2 pos = getPos();

        if (placed_mashines.ContainsKey(pos)) {
            time_remaining = delay;
            placed_mashines[pos].ClickOnMeOwO();
            if (!(placed_mashines[pos] is MainStorage) && !(placed_mashines[pos] is Ender)) hasRotated = true;
        } else {
            Vector2Int v2i = GenerateEvn.getIndexs(pos);
            int tile_index = GenerateEvn.PlacedTileIndexes[v2i.x, v2i.y];
            switch (tile_index) {
                case 8:
                    if (MainStorage.ResourcesInStorage.ContainsKey("Copper")) MainStorage.ResourcesInStorage["Copper"] += 1;
                    else MainStorage.ResourcesInStorage.Add("Copper", 1);
                    InventoryMenu.prompt.SetPrompt("+1 Copper", 0.5f, Color.green);
                    time_remaining = 1f;
                    break;
                case 9:
                    if (MainStorage.ResourcesInStorage.ContainsKey("Lead")) MainStorage.ResourcesInStorage["Lead"] += 1;
                    else MainStorage.ResourcesInStorage.Add("Lead", 1);
                    InventoryMenu.prompt.SetPrompt("+1 Lead", 0.5f, Color.green);
                    time_remaining = 1f;
                    break;
                case 10:
                    if (MainStorage.ResourcesInStorage.ContainsKey("Iron")) MainStorage.ResourcesInStorage["Iron"] += 1;
                    else MainStorage.ResourcesInStorage.Add("Iron", 1);
                    InventoryMenu.prompt.SetPrompt("+1 Iron", 0.5f, Color.green);
                    time_remaining = 1f;
                    break;
                default:
                    if (tile_index > 7) InventoryMenu.prompt.SetPrompt("You can't mine this manually", 2f, Color.red);
                    break;
            }
            if (tile_index > 7) hasMined = true;
        }
    }

    public void PlaceMachine(Item Mashine, Vector2 pos) {
        if (placed_mashines.ContainsKey(pos)) {
            return;
        }

        Vector3 player = transform.position;
        int x = (int)(Mathf.Round(player.x));
        int y = (int)(Mathf.Round(player.y));
        Vector2 player_pos = new Vector2(x, y);

        // test if it can be placed
        if (player_pos == pos) return;
        if (!CanPlace(pos)) return;

        //#if (!UNITY_EDITOR)
        if (!CanAfford(Mashine))  {
            InventoryMenu.prompt.SetPrompt("Can't afford machine", 2f, Color.red);
            return;
        }
        //#endif

        BuyItem(Mashine);

        Machine_Base placed = Instantiate(Mashine.item_object, new Vector3(pos.x, pos.y, -15), transform.rotation).GetComponentInChildren<Machine_Base>();
        if (placed) {
            placed_mashines.Add(pos, placed);
            placed.UpdateBelts();
        }

        if (Mashine.item_name == "Miner T1") {
            hasPlacedT1 = true;
        }
    }

    public void PlaceConvayerBelt(Item Mashine, Vector2 pos, int[] dir) {

        Vector3 player = transform.position;
        int x = (int)(Mathf.Round(player.x));
        int y = (int)(Mathf.Round(player.y));
        Vector2 player_pos = new Vector2(x, y);

        // test if it can be placed
        if (player_pos == pos) return;
        if (!CanPlace(pos)) return;

        #if (!UNITY_EDITOR)
        if (!CanAfford(Mashine))  {
            InventoryMenu.prompt.SetPrompt("Can't afford machine", 2f, Color.red);
            return;
        }
        #endif

        BuyItem(Mashine);

        ConvayerBelt placed = Instantiate(Mashine.item_object, new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponentInChildren<ConvayerBelt>();
        if (placed) {
            placed_mashines.Add(pos, placed);
            placed.UpdateBelts();

            placed.sprite_index -= (short)(placed.dir*16);
            placed.dir = (short)(dir[1]+2-1*dir[0]);
            if (placed.dir > 3) placed.dir = 0;
            placed.sprite_index += (short)(placed.dir*16);
            placed.UpdateSprite();
        }

        hasPlacedConveyor = true;
    }

    public Vector2 getPos() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)(Mathf.Round(worldPosition.x));
        int y = (int)(Mathf.Round(worldPosition.y));
        Vector2 pos = new Vector2(x, y);
        return pos;
    }

    public bool CanAfford(Item item) {
        bool fail = false;
        for (int i = 0; i < item.recipe.Length; i++) {
            if (MainStorage.StringToResource.ContainsKey(item.recipe[i].name)) {
                Resorces res = MainStorage.StringToResource[item.recipe[i].name];
                int amount = item.recipe[i].count;

                if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                    if (amount > MainStorage.ResourcesInStorage[res.item_name]) {
                        fail = true;
                    }
                } else if (amount != 0) fail = true;
            }
        }
        if (fail) return false;
        else return true;
    }

    public bool CanPlace(Vector2 pos) {
        Vector2Int evnPos = GenerateEvn.getIndexs(pos);
        return evnPos.x >= 0;
    }

    public void BuyItem(Item item) {
        for (int i = 0; i < item.recipe.Length; i++) {
            if (MainStorage.StringToResource.ContainsKey(item.recipe[i].name)) {
                Resorces res = MainStorage.StringToResource[item.recipe[i].name];
                int amount = item.recipe[i].count;

                if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                    MainStorage.ResourcesInStorage[res.item_name] -= amount;
                }
            }
        }
    }

    public void SellItem(Item item) {
        for (int i = 0; i < item.recipe.Length; i++) {
            if (MainStorage.StringToResource.ContainsKey(item.recipe[i].name)) {
                Resorces res = MainStorage.StringToResource[item.recipe[i].name];
                int amount = item.recipe[i].count;

                if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                    MainStorage.ResourcesInStorage[res.item_name] += amount;
                } else {
                    MainStorage.ResourcesInStorage.Add(res.item_name, amount);
                }
            }
        }
    }
}
