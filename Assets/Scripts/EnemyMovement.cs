using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float range; //radius of sphere
    [SerializeField] GameObject point;
    [SerializeField] float damageInterval;
    [SerializeField] float randomInterval;

    Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area
    bool randomMove = false;
    bool targetMove = true;
    GameObject targetPlant;
    float timer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centrePoint = GetComponent<Transform>();
        // centrePoint = point.GetComponent<Transform>();
    }


    void Update()
    {

        if (randomMove)
        {
            Debug.Log("Random movement");

            if (agent.remainingDistance <= agent.stoppingDistance) //done with path and still has paths to go
            {

                timer += Time.deltaTime;

                if (timer >= randomInterval)
                {
                    //Debug.Log("Random Movement End");
                    timer = 0f;
                    randomMove = false;
                    targetMove = true;
                    targetPlant = null;
                }
                else
                {
                    //Debug.Log("Random Movement");
                    NextRandomMovement();
                }

            }
        }
        else if (GameObject.FindGameObjectsWithTag("plant").Length <= 0)
        {
            //Debug.Log("No plants");
            if (agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                NextRandomMovement();
            }
        }
        else if (targetMove)
        {
            //Debug.Log("New plant target");
            if (agent.remainingDistance <= agent.stoppingDistance + 2)
            {
                Vector3 point = GeneratePlantTarget();

                if (Vector3.Distance(gameObject.transform.position, targetPlant.transform.position) < 6)
                {
                    //Debug.Log("STOP AT plant target");
                    agent.isStopped = true;
                    targetMove = false;
                }
                else
                {
                    NextTargetMovement(point);
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= damageInterval)
            {
                //Debug.Log("STOP AT plant target ENDED");
                agent.isStopped = false;
                targetMove = true;
                agent.ResetPath();
                timer = 0f;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        agent.ResetPath();
        NextRandomMovement();
    }

    public void HandleEnemyClicked()
    {
        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
        agent.ResetPath();
        //NextRandomMovement();
        randomMove = true;
        targetMove = false;

        Debug.Log("Enemy clicked");
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    bool PlantPoint(out Vector3 result)
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("plant");
        if (plants.Length > 0)
        {
            int index = Random.Range(0, plants.Length);
            targetPlant = plants[index];

            result = targetPlant.transform.position;
            return true;
        }
        else
        {
            Debug.Log("No target plant position returned");
            result = Vector3.zero;
            return false;
        }
    }
    Vector3 GeneratePlantTarget()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("plant");
        if (plants.Length > 0)
        {
            int index = Random.Range(0, plants.Length);
            targetPlant = plants[index];

            return targetPlant.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    void NextRandomMovement()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
        }
    }
    void NextTargetMovement(Vector3 point)
    {
        Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
        agent.SetDestination(point);
    }

}
