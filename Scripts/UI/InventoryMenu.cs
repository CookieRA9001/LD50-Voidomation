using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryMenu : MonoBehaviour
{
    public static bool inMenu;
    public static PromptScript prompt;
    public PromptScript promptObj;
    public GameObject panel, resContent, itemContent;
    public static List<Resorces> allResources;
    public static List<Item> items;
    public static Dictionary<Resorces, GameObject> ResourcesToObject = new Dictionary<Resorces, GameObject>();
    public static Dictionary<Item, GameObject> ItemsToObject = new Dictionary<Item, GameObject>();
    public static Dictionary<GameObject, List<GameObject>> ItemObjToReqSpots = new Dictionary<GameObject, List<GameObject>>();

    public GameObject resSpotPref;
    public GameObject iSpotPref;
    public GameObject reqSpotPref;

    public P_action player;

    private List<GameObject> reqSpots;
    public static bool hasOpenedInv = false, hasEquippedT1 = false, hasEquippedConveyor = false;
    private float timerDef = 4f;
    private float timer;

    void Start()
    {
        hasEquippedConveyor = false;
        hasEquippedT1 = false;
        hasOpenedInv = false;
        inMenu = false;
        allResources = PrefabLabery.GetResources();
        items = PrefabLabery.GetItems();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<P_action>();
        prompt = promptObj;
        ResourcesToObject = new Dictionary<Resorces, GameObject>();
        ItemsToObject = new Dictionary<Item, GameObject>();
        ItemObjToReqSpots = new Dictionary<GameObject, List<GameObject>>();
        PrefabLabery.UpdateStringToResource();
        LoadResources();
        LoadItems("Transportation");
    }

    void Update()
    {
        UpdateItems();
        UpdateResources();
        if (Input.GetKeyDown(KeyCode.E) && !P_action.pause) {
            hasOpenedInv = true;
            if (panel.activeSelf) {
                panel.SetActive(false);
                inMenu = false;
            }
            else {
                panel.SetActive(true);
                inMenu = true;
            }
        } else {
            if (panel.activeSelf) inMenu = true;
        }

        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            timer = timerDef;

            if (!P_action.hasRotated) {
                if (!P_action.hasPlacedConveyor) {
                    if (!hasEquippedConveyor) {
                        if (!P_action.hasPlacedT1) {
                            if (!hasEquippedT1) {
                                if (!hasOpenedInv) {
                                    if (!P_action.hasMined) {
                                        InventoryMenu.prompt.SetPrompt("Try right-clicking on an ore (hint - try copper)", 2f, Color.white);
                                    } else InventoryMenu.prompt.SetPrompt("Try opening your inventory with E", 2f, Color.white);
                                } else InventoryMenu.prompt.SetPrompt("Try equipping a Miner T1 (inventory)", 2f, Color.white);
                            } else InventoryMenu.prompt.SetPrompt("Try placing down a Miner T1 (on ore)", 2f, Color.white);
                        } else InventoryMenu.prompt.SetPrompt("Try equipping a Conveyor Belt (inventory)", 2f, Color.white);
                    } else InventoryMenu.prompt.SetPrompt("Try placing down a Conveyor Belt (hold and drag left-click)", 2f, Color.white);
                } else InventoryMenu.prompt.SetPrompt("Try rotating machinery (right-click)", 2f, Color.white);
            }
        }
    }

    public void UpdateResources() {
        foreach(Resorces res in ResourcesToObject.Keys) {
            TextMeshProUGUI resText = ResourcesToObject[res].GetComponentInChildren<TextMeshProUGUI>();
            int amount;
            if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                amount = MainStorage.ResourcesInStorage[res.item_name];
                resText.color = Color.white;
            } else {
                amount = 0;
                resText.color = Color.grey;
            }
            resText.text = amount.ToString();
        }
    }

    public void LoadResources() {
        foreach(Resorces res in allResources) {
            GameObject resObj = Instantiate(resSpotPref, Vector3.zero, Quaternion.identity);
            if (!ResourcesToObject.ContainsKey(res)) ResourcesToObject.Add(res, resObj);
            resObj.transform.SetParent(resContent.transform);
            resObj.GetComponentInChildren<Image>().sprite = res.sprite;

            TextMeshProUGUI resText = resObj.GetComponentInChildren<TextMeshProUGUI>();

            int amount;
            if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                amount = MainStorage.ResourcesInStorage[res.item_name];
                resText.color = Color.white;
            } else {
                amount = 0;
                resText.color = Color.grey;
            }
            
            resText.text = amount.ToString();
        }
    }

    public void UpdateItems() {
        foreach(Item item in ItemsToObject.Keys) {
            if (ItemObjToReqSpots.ContainsKey(ItemsToObject[item])) {
                List<GameObject> reqs = ItemObjToReqSpots[ItemsToObject[item]];
                foreach(GameObject req in reqs) {
                    Destroy(req);
                }
                reqs.Clear();
                for(int i = 0; i < item.recipe.Length; i++) {
                    if(MainStorage.StringToResource.ContainsKey(item.recipe[i].name)) {
                        Resorces res = MainStorage.StringToResource[item.recipe[i].name];
                        int amount = item.recipe[i].count;

                        GameObject reqObj = Instantiate(reqSpotPref, Vector3.zero, Quaternion.identity);
                        GameObject content = ItemsToObject[item].GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
                        reqObj.transform.SetParent(content.transform);

                        reqObj.GetComponentInChildren<Image>().sprite = res.sprite;
                        TextMeshProUGUI reqText = reqObj.GetComponentInChildren<TextMeshProUGUI>();

                        reqText.text = amount.ToString();

                        if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                            if (amount > MainStorage.ResourcesInStorage[res.item_name]) {
                                reqText.color = Color.red;
                            } else {
                                reqText.color = Color.green;
                            }
                        } else {
                            if (amount == 0) reqText.color = Color.green;
                            else reqText.color = Color.red;
                        }
                        reqs.Add(reqObj);
                    }
                }
                ItemObjToReqSpots[ItemsToObject[item]] = reqs;
            }
        }
    }

    public void LoadItems(string category)
    {
        foreach (GameObject iObj in ItemObjToReqSpots.Keys) {
            foreach(GameObject rSpot in ItemObjToReqSpots[iObj]) {
                Destroy(rSpot);
            }
        }
        ItemObjToReqSpots.Clear();

        foreach(GameObject obj in ItemsToObject.Values) {
            obj.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(obj);
        }
        ItemsToObject.Clear();

        foreach (Item item in items) {
            if (item.item_category == category) {
                GameObject itemObj = Instantiate(iSpotPref, Vector3.zero, Quaternion.identity);
                if (!ItemsToObject.ContainsKey(item)) ItemsToObject.Add(item, itemObj);
                itemObj.transform.SetParent(itemContent.transform);
                itemObj.GetComponent<Button>().onClick.AddListener(delegate{OnClick(item);});

                foreach (Image i in itemObj.GetComponentsInChildren<Image>()) {
                    if (i.gameObject.name == "Image") {
                        i.sprite = item.sprite;
                    }
                }

                TextMeshProUGUI nameText, desText;

                foreach (TextMeshProUGUI text in itemObj.GetComponentsInChildren<TextMeshProUGUI>()) {
                    if (text.gameObject.name == "NameText") {
                        nameText = text;
                        nameText.text = item.item_name;
                    } else if (text.gameObject.name == "DescriptionText") {
                        desText = text;
                        desText.text = item.item_description;
                    }
                }

                List<GameObject> reqs = new List<GameObject>();
                for (int i = 0; i < item.recipe.Length; i++) {
                    if (MainStorage.StringToResource.ContainsKey(item.recipe[i].name)) {
                        Resorces res = MainStorage.StringToResource[item.recipe[i].name];
                        int amount = item.recipe[i].count;

                        GameObject reqObj = Instantiate(reqSpotPref, Vector3.zero, Quaternion.identity);
                        GameObject content = itemObj.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
                        reqObj.transform.SetParent(content.transform);

                        reqObj.GetComponentInChildren<Image>().sprite = res.sprite;
                        TextMeshProUGUI reqText = reqObj.GetComponentInChildren<TextMeshProUGUI>();

                        reqText.text = amount.ToString();

                        if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                            if (amount > MainStorage.ResourcesInStorage[res.item_name]) {
                                reqText.color = Color.red;
                            } else {
                                reqText.color = Color.green;
                            }
                        } else {
                            if (amount == 0) reqText.color = Color.green;
                            else reqText.color = Color.red;
                        }
                        reqs.Add(reqObj);
                    }
                }

                if (!ItemObjToReqSpots.ContainsKey(itemObj)) {
                    ItemObjToReqSpots.Add(itemObj, reqs);
                }
            }
        }
    }

    public void OnClick(Item item) {
        if (item.item_name == "Conveyor Belt") {
            hasEquippedConveyor = true;
        } else if (item.item_name == "Miner T1") {
            hasEquippedT1 = true;
        }
        player.Inventory[P_action.NoramlizeIndex(player.inv_index, player.inv_count)] = item;
        player.inventoryBar.UpdateInvItems(player.inv_index, player.Inventory, player.inv_count);
    }
}
