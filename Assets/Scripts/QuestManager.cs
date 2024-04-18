using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool Active { get; set; }
    public bool Complete { get; set; }
    public int RewardAmount { get; set; }

    public void ActivateQuest()
    {
        Active = true;
    }
    public abstract void CompleteQuest();
}

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
    public PlaceItemQuest(string id, string displayName, Item.ItemType type, int requiredAmount, int rewardAmount)
    {
        ID = id;
        DisplayName = displayName;
        ItemType = type;
        RequiredAmount = requiredAmount;
        RewardAmount = rewardAmount;
        Active = true;
        Complete = false;
    }

    public PlaceItemQuest(string id, string displayName, Item.ItemType type, int requiredAmount, int rewardAmount, bool active)
    {
        ID = id;
        DisplayName = displayName;
        ItemType = type;
        RequiredAmount = requiredAmount;
        RewardAmount = rewardAmount;
        Active = active;
        Complete = false;
    }

    public override void CompleteQuest()
    {
        Complete = true;
        GameManager.Instance.playerCurrency += RewardAmount;
        Debug.Log($"Quest {ID} completed");
        GameManager.Instance.UpdateQuestDisplay();
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

        Quests.Add(new PlaceItemQuest("Place_Plant1_Lv1", "Place 5 hyacinths.", Item.ItemType.Plant1, 5, 250));
        Quests.Add(new PlaceItemQuest("Place_Plant2_Lv1", "Place 2 daffodils.", Item.ItemType.Plant2, 2, 250));
        Quests.Add(new PlaceItemQuest("Place_Plant3_Lv1", "Place 1 sunflower.", Item.ItemType.Plant3, 1, 250));
    }

    void Start()
    {
        // Quests.Add(new PlaceItemQuest("Place_Plant1_Lv1", "Place 5 hyacinths.", Item.ItemType.Plant1, 5, 250));
        // Quests.Add(new PlaceItemQuest("Place_Plant2_Lv1", "Place 2 daffodils.", Item.ItemType.Plant2, 2, 250));
        // Quests.Add(new PlaceItemQuest("Place_Plant3_Lv1", "Place 1 sunflower.", Item.ItemType.Plant3, 1, 250));
    }

    void Update()
    {
        // Check all quests
        CheckQuestStatus();
    }

    public List<Quest> GetQuests()
    {
        return Quests;
    }

    private void CheckQuestStatus()
    {
        foreach (Quest quest in Quests)
        {
            if (quest.Complete) continue;
            if (quest.Active && quest is PlaceItemQuest quest1)
            {
                Item.ItemType type = quest1.ItemType;
                // Check if spawned amount >= required amount
                int currentSpawnTotal = GameManager.Instance.GetSpawnedTotal(type);
                quest1.SetCurrentProgress(currentSpawnTotal);
                if (quest1.CompletionPercentage >= 1.0)
                {
                    quest.CompleteQuest();
                }
            }
            //else if (quest.Active && quest is )
        }
    }
}
