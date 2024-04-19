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

    public void ActivateQuest()
    {
        Active = true;
    }
    public void CompleteQuest()
    {
        Complete = true;
        GameManager.Instance.playerCurrency += RewardAmount;
        Debug.Log($"Quest {ID} completed");
        GameManager.Instance.UpdateQuestDisplay();
    }
}

// Tracks how many of a specific plant is placed
public class PlaceItemQuest : Quest
{
    public Item.ItemType ItemType { get; set; }
    public int RequiredAmount { get; set; }
    public int PreviousAmount { get; set; }
    public int CurrentAmount { get; set; }
    public float CompletionPercentage
    {
        get
        {
            return CurrentAmount / RequiredAmount;
        }
    }
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

    public void UpdateQuestPopup()
    {
        GameManager.Instance.DisplayQuestPopup(DisplayName, CurrentAmount, RequiredAmount);
    }

    public void SetCurrentProgress(int amount)
    {
        bool doUpdate = false;
        if (amount > PreviousAmount) doUpdate = true;

        PreviousAmount = CurrentAmount;
        CurrentAmount = amount;
        if (doUpdate)
        {
            UpdateQuestPopup();
            GameManager.Instance.UpdateQuestDisplay();
        }
    }
}

// Tracks how many times a players heals any plant
public class HealQuest : Quest
{
    public int RequiredAmount { get; set; }
    public int PreviousAmount { get; set; }
    public int CurrentAmount { get; set; }
    public float CompletionPercentage
    {
        get
        {
            return CurrentAmount / RequiredAmount;
        }
    }

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

    public void UpdateQuestPopup()
    {
        GameManager.Instance.DisplayQuestPopup(DisplayName, CurrentAmount, RequiredAmount);
    }

    public void SetCurrentProgress(int amount)
    {
        bool doUpdate = false;
        if (amount > PreviousAmount) doUpdate = true;

        PreviousAmount = CurrentAmount;
        CurrentAmount = amount;
        if (doUpdate)
        {
            UpdateQuestPopup();
            GameManager.Instance.UpdateQuestDisplay();
        }
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
        Quests.Add(new PlaceItemQuest("Place_Plant1_Lv1", "Place 5 hyacinths.", 1, Item.ItemType.Plant1, 5, 250));
        Quests.Add(new PlaceItemQuest("Place_Plant2_Lv1", "Place 2 daffodils.", 1, Item.ItemType.Plant2, 2, 250));
        Quests.Add(new PlaceItemQuest("Place_Plant3_Lv1", "Place 1 sunflower.", 1, Item.ItemType.Plant3, 1, 250));
        Quests.Add(new HealQuest("Heal_Lv1", "Heal plants 1 time.", 1, 1, 100));
        Quests.Add(new HealQuest("Heal_Lv2", "Heal plants 5 times.", 2, 5, 250, false));
        Quests.Add(new HealQuest("Heal_Lv3", "Heal plants 10 times.", 3, 10, 500, false));
    }

    void Update()
    {
        // Check all quests
        CheckQuestStatus();
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
            if (quest.Active && quest is PlaceItemQuest quest1)
            {
                Item.ItemType type = quest1.ItemType;
                // Check if spawned amount >= required amount
                int currentSpawnTotal = GameManager.Instance.GetSpawnedTotal(type);
                quest1.SetCurrentProgress(currentSpawnTotal);
                if (quest1.CompletionPercentage >= 1.0)
                {
                    quest1.CompleteQuest();
                }
            }
            // Healing quest
            else if (quest.Active && quest is HealQuest quest2)
            {
                int healCount = GameManager.Instance.HealCount;
                quest2.SetCurrentProgress(healCount);
                if (quest2.CompletionPercentage >= 1.0)
                {
                    quest2.CompleteQuest();
                    var nextQuests = Quests.Where(nextQuest => nextQuest.ID == $"Heal_Lv{quest2.Level + 1}");
                    Debug.Log(nextQuests);
                    foreach (Quest q in nextQuests)
                    {
                        q.ActivateQuest();
                    }
                }
            }
        }
    }
}
