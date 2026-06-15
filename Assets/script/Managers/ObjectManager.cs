using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // 【新增】公开方法：设置 Gemma 的状态
    public void SetGemmaState(bool isActive)
    {
        if (!interactiveStateDict.ContainsKey("Gemma"))
            interactiveStateDict.Add("Gemma", !isActive);
        else
            interactiveStateDict["Gemma"] = !isActive;
    }

    public static ObjectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 【新增】供 PhoneInteraction 调用，强制修改物品可用状态
    public void ForceSetItemAvailable(ItemName itemName, bool available)
    {
        if (itemAvailableDict.ContainsKey(itemName))
        {
            itemAvailableDict[itemName] = available;
        }
        else
        {
            itemAvailableDict.Add(itemName, available);
        }
    }

    // 【新增】用于保存 UnlockableItem 的状态，Key 是物体名称

    // 【新增】用于保存留声机指针和唱片的状态，Key 格式为 "物体名_zhizhen" 或 "物体名_changpian"
    private Dictionary<string, bool> liushengjiStateDict = new Dictionary<string, bool>();

    private Dictionary<ItemName, bool> itemAvailableDict = new Dictionary<ItemName, bool>();
    private Dictionary<string, bool> interactiveStateDict = new Dictionary<string, bool>();

    // 【新增】用于保存 UnlockableItem 的状态，Key 是物体名称
    private Dictionary<string, bool> unlockableItemStateDict = new Dictionary<string, bool>();

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.UpdateUIEvent += OnUpdateUIEvent;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateUIEvent -= OnUpdateUIEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        // 保存 Gemma 的状态
        GameObject[] gemmas = GameObject.FindGameObjectsWithTag("Interactive");
        foreach (GameObject gemma in gemmas)
        {
            if (gemma.name == "Gemma")
            {
                if (!interactiveStateDict.ContainsKey("Gemma"))
                    interactiveStateDict.Add("Gemma", !gemma.activeInHierarchy);
                else
                    interactiveStateDict["Gemma"] = !gemma.activeInHierarchy;
                break;
            }
        }




        // 1. 保存普通 Item 状态 (查找禁用的)
        foreach (var item in FindObjectsOfType<Item>(true))
        {
            bool isAvailable = item.gameObject.activeInHierarchy;
            if (itemAvailableDict.ContainsKey(item.itemName))
                itemAvailableDict[item.itemName] = isAvailable;
            else
                itemAvailableDict.Add(item.itemName, isAvailable);
        }

        // 2. 保存 Interactive 状态 (查找禁用的)
        foreach (var item in FindObjectsOfType<Interactive>(true))
        {
            if (interactiveStateDict.ContainsKey(item.name))
                interactiveStateDict[item.name] = item.isDone;
            else
                interactiveStateDict.Add(item.name, item.isDone);
        }

        // 【新增】3. 保存 UnlockableItem 状态 (查找禁用的)
        foreach (var unlockable in FindObjectsOfType<UnlockableItem>(true))
        {
            if (unlockableItemStateDict.ContainsKey(unlockable.name))
                unlockableItemStateDict[unlockable.name] = unlockable.GetUnlockState();
            else
                unlockableItemStateDict.Add(unlockable.name, unlockable.GetUnlockState());

            // 【调试日志】保存时打印
            // Debug.Log($"[ObjMgr] Saved Unlockable: {unlockable.name} = {unlockable.GetUnlockState()}");
        }

        // 【新增】4. 保存留声机状态
        foreach (var liushengji in FindObjectsOfType<LiushengjiInteraction>(true))
        {
            string keyZhizhen = liushengji.name + "_zhizhen";
            string keyChangpian = liushengji.name + "_changpian";

            if (liushengjiStateDict.ContainsKey(keyZhizhen))
                liushengjiStateDict[keyZhizhen] = liushengji.GetZhizhenState();
            else
                liushengjiStateDict.Add(keyZhizhen, liushengji.GetZhizhenState());

            if (liushengjiStateDict.ContainsKey(keyChangpian))
                liushengjiStateDict[keyChangpian] = liushengji.GetChangpianState();
            else
                liushengjiStateDict.Add(keyChangpian, liushengji.GetChangpianState());
        }


    }

    private void OnAfterSceneLoadedEvent()
    {
        // 恢复 Gemma 的状态
        GameObject[] gemmas = GameObject.FindGameObjectsWithTag("Interactive");
        foreach (GameObject gemma in gemmas)
        {
            if (gemma.name == "Gemma")
            {
                // 如果 Gemma 已经永久消失，确保它保持隐藏
                if (QuestManager.Instance != null && QuestManager.Instance.isGemmaPermanentlyGone)
                {
                    gemma.SetActive(false);
                }
                // 否则，检查保存的状态
                else if (interactiveStateDict.ContainsKey("Gemma"))
                {
                    gemma.SetActive(!interactiveStateDict["Gemma"]);
                }
                break;
            }
        }




        // 1. 恢复普通 Item 状态
        foreach (var item in FindObjectsOfType<Item>(true))
        {
            if (!itemAvailableDict.ContainsKey(item.itemName))
                itemAvailableDict.Add(item.itemName, true);

            // 【关键修改】判断是否是小熊
            if (item.itemName == ItemName.Bear)
            {
                // 如果电话没打通，无论如何强制隐藏
                if (QuestManager.Instance == null || !QuestManager.Instance.isPhoneDone)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    // 电话打通了，根据字典状态恢复（拨通时已强行设为true，捡起后会被UpdateUIEvent改为false）
                    item.gameObject.SetActive(itemAvailableDict[item.itemName]);
                }
            }
            else
            {
                // 其他物品走原逻辑
                item.gameObject.SetActive(itemAvailableDict[item.itemName]);
            }
        }


        // 2. 恢复 Interactive 状态
        foreach (var item in FindObjectsOfType<Interactive>())
        {
            if (interactiveStateDict.ContainsKey(item.name))
                item.isDone = interactiveStateDict[item.name];
            else
                interactiveStateDict.Add(item.name, item.isDone);
        }

        // 3. 恢复 UnlockableItem 状态
        foreach (var unlockable in FindObjectsOfType<UnlockableItem>(true))
        {
            bool savedState = false;
            if (unlockableItemStateDict.ContainsKey(unlockable.name))
            {
                savedState = unlockableItemStateDict[unlockable.name];
            }
            else
            {
                unlockableItemStateDict.Add(unlockable.name, false);
            }


            unlockable.SetUnlockState(savedState);

            // 联动逻辑：如果它同时是 Item，且已经被捡走了，必须强制隐藏！
            var itemComponent = unlockable.GetComponent<Item>();
            if (itemComponent != null && itemAvailableDict.ContainsKey(itemComponent.itemName))
            {
                if (!itemAvailableDict[itemComponent.itemName])
                {
                    unlockable.gameObject.SetActive(false);
                }
            }
        }



        // 4. 恢复留声机状态
        foreach (var liushengji in FindObjectsOfType<LiushengjiInteraction>(true))
        {
            string keyZhizhen = liushengji.name + "_zhizhen";
            string keyChangpian = liushengji.name + "_changpian";

            if (liushengjiStateDict.ContainsKey(keyZhizhen))
            {
                liushengji.SetZhizhenState(liushengjiStateDict[keyZhizhen]);
            }

            if (liushengjiStateDict.ContainsKey(keyChangpian))
            {
                liushengji.SetChangpianState(liushengjiStateDict[keyChangpian]);
            }
        }

    }


    private void OnUpdateUIEvent(ItemDetails itemDetails, int arg2)
    {
        if (itemDetails != null)
        {
            itemAvailableDict[itemDetails.itemName] = false;
        }
    }
}