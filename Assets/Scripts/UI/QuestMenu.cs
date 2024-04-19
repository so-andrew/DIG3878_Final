using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour
{
    [SerializeField] private Transform activeQuests;
    [SerializeField] private Transform completedQuests;
    [SerializeField] private Transform activeQuestTemplate;
    [SerializeField] private Transform completedQuestTemplate;

    [SerializeField] private Sprite activeButtonImage;
    [SerializeField] private Sprite inactiveButtonImage;

    private GameObject activeQuestsMenu;
    private GameObject completedQuestsMenu;

    private GameObject activeQuestButton;
    private GameObject completedQuestButton;

    private bool activeQuestsActive = true;
    private bool completedQuestsActive = false;

    void Awake()
    {
        activeQuestTemplate.gameObject.SetActive(false);
        completedQuestTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        activeQuestsMenu = transform.Find("ActiveQuestsMenu").gameObject;
        completedQuestsMenu = transform.Find("CompletedQuestsMenu").gameObject;
        activeQuestsMenu.SetActive(activeQuestsActive);
        completedQuestsMenu.SetActive(completedQuestsActive);

        activeQuestButton = transform.Find("ActiveQuestButton").gameObject;
        completedQuestButton = transform.Find("CompletedQuestButton").gameObject;

        // Generating quests
        GenerateQuests();
    }

    public void GenerateQuests()
    {
        GenerateActiveQuests();
        GenerateCompletedQuests();
    }

    private void GenerateActiveQuests()
    {
        if (activeQuests == null) return;
        int i = 0;
        GameObject[] containerChildren = new GameObject[activeQuests.childCount];
        foreach (Transform child in activeQuests)
        {
            if (child.gameObject.name != "ActiveQuestTemplate")
            {
                containerChildren[i] = child.gameObject;
                i++;
            }
        }

        foreach (GameObject obj in containerChildren)
        {
            Destroy(obj);
        }

        List<Quest> quests = QuestManager.Instance.GetQuests();
        var active = quests.Where(x => x.Complete == false && x.Active == true);
        foreach (Quest quest in active)
        {
            CreateActiveQuestUIItem(quest);
        }
    }

    private void GenerateCompletedQuests()
    {
        if (completedQuests == null) return;
        int i = 0;
        GameObject[] containerChildren = new GameObject[completedQuests.childCount];
        foreach (Transform child in completedQuests)
        {
            if (child.gameObject.name != "CompletedQuestTemplate")
            {
                containerChildren[i] = child.gameObject;
                i++;
            }
        }

        foreach (GameObject obj in containerChildren)
        {
            Destroy(obj);
        }

        List<Quest> quests = QuestManager.Instance.GetQuests();
        var completed = quests.Where(x => x.Complete == true && x.Active == true);
        foreach (Quest quest in completed)
        {
            CreateCompletedQuestUIItem(quest);
        }
    }

    private void CreateActiveQuestUIItem(Quest quest)
    {
        Transform questTransform = Instantiate(activeQuestTemplate, activeQuests);

        Transform questName = questTransform.Find("QuestName");
        questName.GetComponent<TMP_Text>().text = quest.DisplayName;

        Transform questProgress = questTransform.Find("Progress");
        if (quest is PlaceItemQuest quest1)
        {
            questProgress.GetComponent<TMP_Text>().text = $"{quest1.CurrentAmount}/{quest1.RequiredAmount}";
        }
        else if (quest is HealQuest quest2)
        {
            questProgress.GetComponent<TMP_Text>().text = $"{quest2.CurrentAmount}/{quest2.RequiredAmount}";
        }
        else
        {
            questProgress.GetComponent<TMP_Text>().text = "WIP";
        }

        Transform questReward = questTransform.Find("Reward");
        questReward.GetComponent<TMP_Text>().text = quest.RewardAmount.ToString();

        questTransform.gameObject.SetActive(true);
    }

    private void CreateCompletedQuestUIItem(Quest quest)
    {
        Transform questTransform = Instantiate(completedQuestTemplate, completedQuests);

        Transform questName = questTransform.Find("QuestName");
        questName.GetComponent<TMP_Text>().text = quest.DisplayName;

        Transform questReward = questTransform.Find("Reward");
        questReward.GetComponent<TMP_Text>().text = quest.RewardAmount.ToString();

        questTransform.gameObject.SetActive(true);
    }

    public void ShowActiveQuests()
    {
        activeQuestsActive = true;
        completedQuestsActive = !activeQuestsActive;
        ToggleQuestDisplay();
    }

    public void ShowCompletedQuests()
    {
        completedQuestsActive = true;
        activeQuestsActive = !completedQuestsActive;
        ToggleQuestDisplay();
    }

    private void ToggleQuestDisplay()
    {
        activeQuestsMenu.SetActive(activeQuestsActive);
        completedQuestsMenu.SetActive(completedQuestsActive);

        activeQuestButton.GetComponent<Image>().sprite = activeQuestsActive ? activeButtonImage : inactiveButtonImage;
        completedQuestButton.GetComponent<Image>().sprite = completedQuestsActive ? activeButtonImage : inactiveButtonImage;
    }
}
