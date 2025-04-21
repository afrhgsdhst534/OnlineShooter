using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class ExpManager : NetworkBehaviour
{
    public static ExpManager Instance;

    public List<string> allUpgrades = new List<string> { "hp", "attack", "attackSpeed" };
    public GameObject upgradesUI;
    public Slider xpSlider;
    public int baseMaxXP = 100;

    [SyncVar(hook = nameof(OnXPChanged))] public int currentXP;
    [SyncVar(hook = nameof(OnXPChanged))] public int maxXP;
    [SyncVar] private bool isPaused = false;

    private List<string> currentUpgrades;
    private Dictionary<uint, string> playerChoices = new();

    void Awake() => Instance = this;

    public override void OnStartServer()
    {
        base.OnStartServer();
        int playerCount = NetworkServer.connections.Count;
        maxXP = baseMaxXP * playerCount;
        currentXP = 0;
    }

    void OnXPChanged(int _, int __)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = maxXP;
            xpSlider.value = currentXP;
        }
    }

    [Server]
    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= maxXP)
        {
            currentXP = 0;
            maxXP += baseMaxXP;

            Shuffle(allUpgrades);
            currentUpgrades = allUpgrades.GetRange(0, 3);

            playerChoices.Clear();
            isPaused = true;

            RpcPauseGame();
            RpcShowUpgradeUI(currentUpgrades.ToArray());
        }
    }

    [ClientRpc]
    void RpcPauseGame() => Time.timeScale = 0f;

    [ClientRpc]
    void RpcResumeGame() {
        Time.timeScale = 1f;
        upgradesUI.SetActive(false); // <--- ВАЖНО!

    }

    [ClientRpc]
    void RpcShowUpgradeUI(string[] upgrades)
    {
        upgradesUI.SetActive(true);

        for (int i = 0; i < upgradesUI.transform.childCount; i++)
        {
            var button = upgradesUI.transform.GetChild(i).GetComponent<UpgradeButton>();
            if (i < upgrades.Length)
            {
                button.gameObject.SetActive(true);
                button.Setup(upgrades[i]);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    [ClientRpc]
    public void RpcHighlightSelection(string upgrade, uint netId)
    {
        foreach (var btn in upgradesUI.GetComponentsInChildren<UpgradeButton>())
        {
            if (btn.IsUpgrade(upgrade))
                btn.Highlight(netId);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectUpgrade(uint playerNetId, string upgrade)
    {
        playerChoices[playerNetId] = upgrade;

        RpcHighlightSelection(upgrade, playerNetId);

        if (playerChoices.Count == NetworkServer.connections.Count)
        {
            bool allSame = playerChoices.Values.All(val => val == playerChoices.Values.First());

            if (allSame)
            {
                RpcResumeGame();
                upgradesUI.SetActive(false);

                Upgrade(playerChoices.Values.First());
            }
            else
            {
                // mismatch — just wait for re-selection or retry
                // could add UI feedback
            }
        }
    }

    void Upgrade(string upgrade)
    {
        Debug.Log($"All players chose upgrade: {upgrade}");
        // Применить выбранный апгрейд всем или по желанию
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
