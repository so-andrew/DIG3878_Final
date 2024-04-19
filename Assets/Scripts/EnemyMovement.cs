using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    /*bool randomMove = true;
    int randomMoveCount = 0;
    bool targetMove = false;
    GameObject targetPlant;*/

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centrePoint = GetComponent<Transform>();
    }


    void Update()
    {

        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            NextRandomMovement();
        }

        /*if (randomMove)
        {
            Debug.Log("Random movement");
            if (agent.remainingDistance <= agent.stoppingDistance) //done with path and still has paths to go
            {
                if (randomMoveCount <= 1000)
                {
                    NextRandomMovement();
                    randomMoveCount++;
                    Debug.Log("next random point");
                }
                else
                {
                    randomMove = false;
                    targetMove = true;
                    randomMoveCount = 0;
                    targetPlant = null;
                    Debug.Log("Random movement over");
                }
                NextRandomMovement();
            }
        }*/
        /*else if (GameObject.FindGameObjectsWithTag("plant").Length <= 0)
        {
            Debug.Log("No plants");
            if (agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                Debug.Log("Next Rand Move");
                NextRandomMovement();
            }
        }
        else if (targetMove)
        {
                NextTargetMovement();
                Debug.Log("New plant target");
                targetMove = false;
        }
        else
        {
        }*/
    }

    /*bool AllPlantsDead()
    {
        if (GameObject.FindGameObjectsWithTag("plant").Length <= 0)
        {
            return false;
        }
        else
        {
            foreach (GameObject plant in GameObject.FindGameObjectsWithTag("plant"))
            {
                if (!plant.GetComponentInChildren<Health>().isDead())
                {
                    return false;
                }
            }
            return true;
        }
    }*/
    /*public void HandleEnemyClicked()
    {
        //randomMove = true;
        //targetMove = false;
        Debug.Log("Enemy clicked");
        Destroy(gameObject);
    }*/

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

    /*bool PlantPoint(out Vector3 result)
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
    }*/
    /*IEnumerator DelayTheNextRandomMovement()
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
        }
    }*/

    void NextRandomMovement()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
        }
    }
    /*void NextTargetMovement()
    {
        Vector3 point;
        if (PlantPoint(out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
        }
    }*/

}
