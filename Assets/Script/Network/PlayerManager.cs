using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;
    public event Action<GameObject> onAdd;
    private readonly List<GameObject> players = new();
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    [Server]
    public List<GameObject> GetAllPlayers()
    {
            return new List<GameObject>(players);
    }
    [Server]
    public void AddPlayer(GameObject player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            onAdd?.Invoke(player);
        }
    }
    [Server]
    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
            players.Remove(player);
    }
    [Server]
    public GameObject RandomPlayer()
    {
        if (players.Count == 0) return null;
        return players[UnityEngine.Random.Range(0, players.Count)];
    }
}