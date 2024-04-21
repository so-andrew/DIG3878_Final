using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

// Things to track:
// - Total money collected
// - Total plants placed per type
// - Total enemies interacted with

public enum QuestType
{
    Plant,
    Enemy,
    Money
}

public abstract class Quest
{
    public string ID { get; set; }
    public string DisplayName { get; set; }
    public int Level { get; set; }
    public bool Active { get; set; }
    public bool Complete { get; set; }
    public int RewardAmount { get; set; }
    public int RequiredAmount { get; set; }
    public int PreviousAmount { get; set; }
    public int CurrentAmount { get; set; }
    public float CompletionPercentage
    {
        get
        {
            return (float)CurrentAmount / RequiredAmount;
        }
    }

    public void ActivateQuest()
    {
        Active = true;
    }

    public void SetCurrentProgress(int amount)
    {
        bool doUpdate = Active && amount > CurrentAmount;
        PreviousAmount = CurrentAmount;
        CurrentAmount = amount;
        if (doUpdate)
        {
            UpdateQuestPopup();
            GameManager.Instance.UpdateQuestDisplay();
        }
    }

    public void UpdateQuestPopup()
    {
        GameManager.Instance.questPopup.PushNewQuest(new QuestNotification(DisplayName, CurrentAmount, RequiredAmount));
    }

    public void CompleteQuest()
    {
        Complete = true;
        GameManager.Instance.ChangePlayerCurrency(RewardAmount);
        Debug.Log($"Quest {ID} completed");
        GameManager.Instance.UpdateQuestDisplay();
    }

    public override string ToString()
    {
        return $"{ID} - {DisplayName} - {CurrentAmount}/{RequiredAmount} - {RewardAmount} Gold";
    }
}

// Tracks how many of a specific plant is placed
public class PlaceItemQuest : Quest
{
    public Item.ItemType ItemType { get; set; }
    public PlaceItemQuest(string id, string displayName, int level, Item.ItemType type, int requiredAmount, int rewardAmount, bool active = true)
    {
        ID = id;
        DisplayName = displayName;
        Level = level;
        ItemType = type;
        RequiredAmount = requiredAmount;
        RewardAmount = rewardAmount;
        Active = active;
        Complete = false;
    }
}

// Tracks how many times a players heals any plant
public class HealQuest : Quest
{
    public HealQuest(string id, string displayName, int level, int requiredAmount, int rewardAmount, bool active = true)
    {
        ID = id;
        DisplayName = displayName;
        Level = level;
        RequiredAmount = requiredAmount;
        RewardAmount = rewardAmount;
        Active = active;
        Complete = false;
    }
}

public class CoinCollectQuest : Quest
{
    public CoinCollectQuest(string id, string displayName, int level, int requiredAmount, int rewardAmount, bool active = true)
    {
        ID = id;
        DisplayName = displayName;
        Level = level;
        RequiredAmount = requiredAmount;
        RewardAmount = rewardAmount;
        Active = active;
        Complete = false;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    private List<Quest> Quests = new List<Quest>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // Initialize quests in Awake

        // Quests.Add(new PlaceItemQuest("Place_Plant1_Lv1", "Place 5 hyacinths.", 1, Item.ItemType.Plant1, 5, 100));
        // Quests.Add(new PlaceItemQuest("Place_Plant1_Lv2", "Place 10 hyacinths.", 2, Item.ItemType.Plant1, 10, 200, false));
        // Quests.Add(new PlaceItemQuest("Place_Plant1_Lv3", "Place 15 hyacinths.", 3, Item.ItemType.Plant1, 15, 300, false));
        // Quests.Add(new PlaceItemQuest("Place_Plant2_Lv1", "Place 2 daffodils.", 1, Item.ItemType.Plant2, 2, 150));
        // Quests.Add(new PlaceItemQuest("Place_Plant2_Lv2", "Place 4 daffodils.", 2, Item.ItemType.Plant2, 4, 300, false));
        // Quests.Add(new PlaceItemQuest("Place_Plant2_Lv3", "Place 6 daffodils.", 3, Item.ItemType.Plant2, 6, 450, false));
        // Quests.Add(new PlaceItemQuest("Place_Plant3_Lv1", "Place 1 sunflower.", 1, Item.ItemType.Plant3, 1, 200));
        // Quests.Add(new PlaceItemQuest("Place_Plant3_Lv2", "Place 3 sunflowers.", 2, Item.ItemType.Plant3, 3, 400, false));
        // Quests.Add(new PlaceItemQuest("Place_Plant3_Lv3", "Place 5 sunflowers.", 3, Item.ItemType.Plant3, 5, 600, false));
        // Quests.Add(new HealQuest("Heal_Lv1", "Heal plants 1 time.", 1, 1, 150));
        // Quests.Add(new HealQuest("Heal_Lv2", "Heal plants 5 times.", 2, 5, 300, false));
        // Quests.Add(new HealQuest("Heal_Lv3", "Heal plants 10 times.", 3, 10, 450, false));
        // Quests.Add(new CoinCollectQuest("CoinCollect_Lv1", "Collect 5 coins.", 1, 5, 100));
        // Quests.Add(new CoinCollectQuest("CoinCollect_Lv2", "Collect 10 coins.", 2, 10, 200, false));
        // Quests.Add(new CoinCollectQuest("CoinCollect_Lv3", "Collect 20 coins.", 3, 20, 300, false));
    }

    void Start()
    {
        InitializeQuests(GameManager.Instance.level);
    }

    void Update()
    {
        // Check all quests
        CheckQuestStatus();
    }

    public void InitializeQuests(int level)
    {

        // x = level1, y = plantType
        // ex plantSpawnByLevel[0,2] = multiplier for level 1, Plant3
        Item.ItemType[] plants = { Item.ItemType.Plant1, Item.ItemType.Plant2, Item.ItemType.Plant3 };
        int[,] plantSpawnByLevel = { { 5, 2, 1 }, { 10, 4, 2 } };
        int[] rewardMultiplierByPlant = { 100, 150, 200 };

        // Initialize place quests
        for (int i = 0; i < 3; i++) // Outer loop: plant type
        {
            for (int j = 0; j < 3; j++) // Inner loop: quest level
            {
                int requiredAmount = plantSpawnByLevel[level, i] * (j + 1);
                int rewardAmount = rewardMultiplierByPlant[i] * (j + 1);
                bool active = j < 1;
                string displayTypeString = Item.GetItemName(plants[i]).ToLower();
                if (requiredAmount > 1) displayTypeString += "s";
                Quest q = new PlaceItemQuest($"Place_Plant{i + 1}_Lv{j + 1}", $"Place {requiredAmount} {displayTypeString}.", j + 1, plants[i], requiredAmount, rewardAmount, active);
                Debug.Log(q.ToString());
                Quests.Add(q);
            }
        }

        int[] healAmounts = { 1, 5, 10 };
        int[] coinAmounts = { 5, 10, 20 };

        double levelMultiplier = level > 0 ? 1.5 : 1.0;

        // Initialize heal and collect quests
        for (int i = 0; i < 3; i++)
        {
            bool active = i < 1;

            int requiredHealAmount = (int)Math.Floor(healAmounts[i] * levelMultiplier);
            int rewardAmount = (int)Math.Floor(150 * (i + 1) * levelMultiplier);
            string endString = (requiredHealAmount > 1) ? "s." : ".";
            string healString = $"Heal plants {requiredHealAmount} time" + endString;
            Quest healQuest = new HealQuest($"Heal_Lv{i + 1}", healString, i + 1, requiredHealAmount, rewardAmount, active);
            Debug.Log(healQuest.ToString());
            Quests.Add(healQuest);

            int requiredCoinAmount = (int)Math.Floor(coinAmounts[i] * levelMultiplier);
            rewardAmount = (int)Math.Floor(100 * (i + 1) * levelMultiplier);
            endString = (requiredCoinAmount > 1) ? "s." : ".";
            string coinString = $"Collect {requiredCoinAmount} coin" + endString;
            Quest coinQuest = new CoinCollectQuest($"CoinCollect_Lv{i + 1}", coinString, i + 1, requiredCoinAmount, rewardAmount, active);
            Debug.Log(coinQuest.ToString());
            Quests.Add(coinQuest);
        }
    }

    // Returns list of quests; used to render quest list in QuestMenu
    public List<Quest> GetQuests()
    {
        return Quests;
    }

    public int GetCompletedQuestCount()
    {
        return Quests.Where(x => x.Complete).ToList().Count;
    }

    public int GetTotalQuestCount()
    {
        return Quests.Count;
    }

    // Loop through all quests and check for progress/completion
    private void CheckQuestStatus()
    {
        foreach (Quest quest in Quests)
        {
            if (quest.Complete) continue;
            // Place items quest
            if (quest is PlaceItemQuest quest1)
            {
                Item.ItemType type = quest1.ItemType;
                // Check if spawned amount >= required amount
                int currentSpawnTotal = GameManager.Instance.GetSpawnedTotal(type);
                quest1.SetCurrentProgress(currentSpawnTotal);
                if (quest1.CompletionPercentage >= 1.0)
                {
                    quest1.CompleteQuest();
                    ActivateNextQuestInLine(quest1);
                }
            }
            // Healing quest
            else if (quest is HealQuest quest2)
            {
                int healCount = GameManager.Instance.HealCount;
                quest2.SetCurrentProgress(healCount);
                if (quest2.CompletionPercentage >= 1.0)
                {
                    quest2.CompleteQuest();
                    ActivateNextQuestInLine(quest2);
                }
            }
            else if (quest is CoinCollectQuest quest3)
            {
                int coinCount = GameManager.Instance.CoinCollectedCount;
                quest3.SetCurrentProgress(coinCount);
                if (quest3.CompletionPercentage >= 1.0)
                {
                    quest3.CompleteQuest();
                    ActivateNextQuestInLine(quest3);
                }
            }
        }
    }

    private void ActivateNextQuestInLine(Quest quest)
    {
        var nextQuests = Quests.Where(nextQuest => nextQuest.ID == $"{quest.ID[..quest.ID.LastIndexOf("_")]}_Lv{quest.Level + 1}"); ;
        foreach (Quest q in nextQuests)
        {
            q.ActivateQuest();
        }
    }
}
