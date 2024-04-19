using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5.0f * Time.deltaTime);
        }
        else
        {
            targetPosition = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));
            Debug.Log($"targetPosition = {targetPosition}");
        }
    }
}
