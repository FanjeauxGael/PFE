using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreboardItem;

    [SerializeField]
    Transform playerScoreboardList;

    private void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();

        foreach (Player player  in players)
        {
            Debug.Log(player.name + " _ " + player.kills + " - " + player.deaths);
            GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item != null)
            {
                item.Setup(player);
            }
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}

