using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct QuestNotification
{
    public QuestNotification(string text, int progressAmount, int requiredAmount)
    {
        Text = text;
        ProgressAmount = progressAmount;
        RequiredAmount = requiredAmount;
    }
    public string Text;
    public int ProgressAmount;
    public int RequiredAmount;
}

public class QuestPopup : MonoBehaviour
{
    private Queue<QuestNotification> questQueue = new Queue<QuestNotification>();
    private Animator animator;
    private Coroutine queueChecker;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void PushNewQuest(QuestNotification notif)
    {
        //Debug.Log($"Adding {notif.Text}, {notif.ProgressAmount}/{notif.RequiredAmount} to queue");
        questQueue.Enqueue(notif);
        queueChecker ??= StartCoroutine(CheckQueue());
    }

    private IEnumerator CheckQueue()
    {
        do
        {
            DisplayPopup(questQueue.Dequeue());
            do
            {
                yield return null;
            } while (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
        } while (questQueue.Count > 0);
        queueChecker = null;
    }

    private void DisplayPopup(QuestNotification notif)
    {
        SetQuestPopup(notif);
        animator.Play("ShowPopup");
    }

    // Set quest popup text
    public void SetQuestPopup(QuestNotification notif)
    {
        Transform questText = transform.Find("QuestText");
        questText.GetComponent<TMP_Text>().text = notif.Text;

        GameObject completionPercentageText = transform.Find("CompletionPercent").gameObject;
        GameObject completionImage = transform.Find("CompleteImage").gameObject;
        float completionPercentage = (float)notif.ProgressAmount / notif.RequiredAmount;

        completionPercentageText.GetComponent<TMP_Text>().text = $"{notif.ProgressAmount}/{notif.RequiredAmount}";

        if (completionPercentage >= 1.0f)
        {
            completionPercentageText.SetActive(false);
            completionImage.SetActive(true);
        }
        else
        {
            completionPercentageText.SetActive(true);
            completionImage.SetActive(false);
        }
    }
}
