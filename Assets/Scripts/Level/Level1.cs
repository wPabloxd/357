using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] enemiesInFirstRoom;
    [SerializeField] GameObject[] enemiesInSecondRoom;
    [SerializeField] GameObject firstRoomEnemies;
    [SerializeField] GameObject secondRoomEnemies;
    [SerializeField] GameObject tutorialEnemies;
    [SerializeField] GameObject tutorialRed;
    [SerializeField] GameObject tutorialBlue;
    [SerializeField] bool[] enemiesDead;
    int killsNeededForFirstRoom;
    int killsInFirstRoom;
    int killsNeededForSecondRoom;
    int killsInSecondRoom;
    void Start()
    {
        firstRoomEnemies.SetActive(false);
        secondRoomEnemies.SetActive(false);
        tutorialEnemies.SetActive(false);
        for (int i = 0; i < enemiesInFirstRoom.Length; i++)
        {
            enemiesInFirstRoom[i].SetActive(false);
        }
        for (int i = 0; i < enemiesInSecondRoom.Length; i++)
        {
            enemiesInSecondRoom[i].SetActive(false);
        }
        killsInFirstRoom = 0;
        killsInSecondRoom = 0;
        switch (GameManager.Instance.difficulty)
        {
            case (0):
                killsNeededForFirstRoom = 5;
                killsNeededForSecondRoom = 6;
                break;
            case (1):
                killsNeededForFirstRoom = 7;
                killsNeededForSecondRoom = 9;
                break;
            case (2):
                killsNeededForFirstRoom = 10;
                killsNeededForSecondRoom = 11;
                break;
        }
        for (int i = 0; i < killsNeededForFirstRoom; i++)
        {
            enemiesInFirstRoom[i].SetActive(true);
        }
        for (int i = 0; i < killsNeededForSecondRoom; i++)
        {
            enemiesInSecondRoom[i].SetActive(true);
        }
    }
    public bool OpenInitialDoor()
    {
        tutorialEnemies.SetActive(true);
        return true;
    }
    public bool OpenFirstDoor()
    {
        if (!tutorialBlue.GetComponent<EnemyBlue>().alive && !tutorialRed.GetComponent<EnemyRed>().alive)
        {
            firstRoomEnemies.SetActive(true);
            return true;
        }
        return false;
    }
    public bool OpenSecondDoor()
    {
        killsInFirstRoom = 0;
        for (int i = 0; i < killsNeededForFirstRoom; i++)
        {
            if (enemiesInFirstRoom[i].GetComponent<EnemyBlue>())
            {
                if (!enemiesInFirstRoom[i].GetComponent<EnemyBlue>().alive)
                {
                    killsInFirstRoom++;
                }      
            }
            else if (enemiesInFirstRoom[i].GetComponent<EnemyRed>())
            {
                if (!enemiesInFirstRoom[i].GetComponent<EnemyRed>().alive)
                {
                    killsInFirstRoom++;
                }
            }
        }
        if (killsInFirstRoom == killsNeededForFirstRoom)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool OpenSecondRoomDoor()
    {
        secondRoomEnemies.SetActive(true);
        return true;
    }
    public bool OpenThirdDoor()
    {
        killsInSecondRoom = 0;
        for (int i = 0; i < killsNeededForSecondRoom; i++)
        {
            if (enemiesInSecondRoom[i].GetComponent<EnemyBlue>())
            {
                if (!enemiesInSecondRoom[i].GetComponent<EnemyBlue>().alive)
                {
                    killsInSecondRoom++;
                }
            }
            else if (enemiesInSecondRoom[i].GetComponent<EnemyRed>())
            {
                if (!enemiesInSecondRoom[i].GetComponent<EnemyRed>().alive)
                {
                    killsInSecondRoom++;
                }
            }
        }
        if (killsInSecondRoom == killsNeededForSecondRoom)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}