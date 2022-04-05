using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExportMenu : MonoBehaviour
{
    public static bool inMenu;
    public GameObject panel, dirpanel, resContent;
    public static List<Resorces> allResources;
    public static Dictionary<Resorces, GameObject> ResourcesToObject = new Dictionary<Resorces, GameObject>();

    public GameObject resSpotPref;
    public Exporter exporter;
    public Color selectedColor;

    void Start()
    {
        inMenu = false;
        allResources = PrefabLabery.GetResources();
        ResourcesToObject = new Dictionary<Resorces, GameObject>();
        PrefabLabery.UpdateStringToResource();
    }

    void Update()
    {
        UpdateResources();
        if (Input.GetKeyDown(KeyCode.E) && panel.activeSelf) {
            CloseMenu();
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
                resText.color = Color.black;
            }
            resText.text = amount.ToString();
        }
    }

    public void LoadResources() {
        foreach(GameObject obj in ResourcesToObject.Values) {
            obj.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(obj);
        }
        ResourcesToObject.Clear();

        foreach(Resorces res in allResources) {
            GameObject resObj = Instantiate(resSpotPref, Vector3.zero, Quaternion.identity);
            if (!ResourcesToObject.ContainsKey(res)) ResourcesToObject.Add(res, resObj);
            resObj.transform.SetParent(resContent.transform);

            foreach (Image i in resObj.GetComponentsInChildren<Image>()) {
                if (i.gameObject.name == "Image") {
                    i.sprite = res.sprite;
                } else if (i.gameObject.name == "BG") {
                    if (exporter.resorce == res) {
                        i.color = selectedColor;
                    }
                }
            }

            resObj.GetComponent<Button>().onClick.AddListener(delegate{OnClick(res);});

            TextMeshProUGUI resText = resObj.GetComponentInChildren<TextMeshProUGUI>();

            int amount;
            if (MainStorage.ResourcesInStorage.ContainsKey(res.item_name)) {
                amount = MainStorage.ResourcesInStorage[res.item_name];
                resText.color = Color.white;
            } else {
                amount = 0;
                resText.color = Color.black;
            }
            
            resText.text = amount.ToString();
        }
    }

    public void OnClick(Resorces res) {
        exporter.UpdateResource(res);
        dirpanel.SetActive(true);
    }

    public void CloseMenu() {
        dirpanel.SetActive(false);
        panel.SetActive(false);
        P_action.pause = false;
    }

    public void OpenMenu() {
        LoadResources();
        panel.SetActive(true);
        P_action.pause = true;
    }

    public void RotateExporter(int dir) {
        exporter.ChangeDir((short)dir);
        CloseMenu();
    }
}
