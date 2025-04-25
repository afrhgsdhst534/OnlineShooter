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
    public Text hostChoiceText;
    public int baseMaxXP = 100;
    [SyncVar(hook = nameof(OnXPChanged))] public int currentXP;
    [SyncVar(hook = nameof(OnXPChanged))] public int maxXP;
    [SyncVar(hook = nameof(OnHostUpgradeChanged))] public string hostSelectedUpgrade;
    private List<string> currentUpgrades;
    private Dictionary<uint, string> selectedUpgrades = new();
    private double countdownStartTime;
    private float countdownDuration = 10f;
    private bool countdownActive = false;
    private PlayerManager playerManager;
    private void Awake()
    {
        Instance = this;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        int playerCount = NetworkServer.connections.Count;
        if (playerCount == 0) playerCount = 1;
        maxXP = baseMaxXP * playerCount;
        currentXP = 0;
        playerManager = PlayerManager.instance;
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
            currentUpgrades = allUpgrades.Take(3).ToList();
            RpcPauseGame();
            RpcShowUpgradeUI(currentUpgrades);
        }
    }
    void OnXPChanged(int _, int __)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = maxXP;
            xpSlider.value = currentXP;
        }
    }
    void OnHostUpgradeChanged(string _, string newValue)
    {
        Debug.Log($"[Client] Host selected: {newValue}");
    }
    [Server]
    public void Upgrade(string upgrade)
    {
        if (!isServer) return;
        Debug.Log($"[Upgrade Applied] {upgrade}");
        foreach (GameObject playerObj in playerManager.GetAllPlayers())
        {
            Player player = playerObj.GetComponent<Player>();
            if (player == null) continue;

            switch (upgrade)
            {
                case "hp":
                    player.maxHealth += 10;
                    break;
                case "attack":
                    player.attack += 2;
                    break;
                case "attackSpeed":
                    player.reloadTime -=100;
                    break;
            }
        }
    }
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    [ClientRpc]
    void RpcPauseGame()
    {
        Time.timeScale = 0f;
    }
    [ClientRpc]
    void RpcResumeGame()
    {
        Time.timeScale = 1f;
        upgradesUI.SetActive(false);
        hostChoiceText.text = "";
        foreach (var button in upgradesUI.GetComponentsInChildren<UpgradeButton>())
        {
            button.ResetButton();
        }
    }
    [ClientRpc]
    void RpcShowUpgradeUI(List<string> upgrades)
    {
        upgradesUI.SetActive(true);
        for (int i = 0; i < upgradesUI.transform.childCount; i++)
        {
            var button = upgradesUI.transform.GetChild(i).GetComponent<UpgradeButton>();
            if (i < upgrades.Count)
            {
                button.Setup(upgrades[i]);
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }
    [ClientRpc]
    void RpcUpdateSelections(uint playerNetId, string upgrade)
    {
        foreach (var button in upgradesUI.GetComponentsInChildren<UpgradeButton>())
        {
            if (!button.IsSelectedByMe()) 
            {
                button.ResetHighlight(); 
            }
        }
        if (playerNetId != NetworkClient.connection.identity.netId)
        {
            foreach (var button in upgradesUI.GetComponentsInChildren<UpgradeButton>())
            {
                if (button.UpgradeName == upgrade)
                {
                    button.HighlightAsOtherPlayer();
                }
            }
        }
    }
    [ClientRpc]
    void RpcUpdateTimerText(string text)
    {
        if (hostChoiceText != null)
        {
            hostChoiceText.text = text;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSelectUpgrade(uint playerNetId, string upgrade)
    {
        selectedUpgrades[playerNetId] = upgrade;
        RpcUpdateSelections(playerNetId, upgrade);
        if (playerNetId == NetworkServer.localConnection.identity.netId)
        {
            Debug.Log($"[Server] Хост выбрал: {upgrade}");
            hostSelectedUpgrade = upgrade;
            countdownStartTime = NetworkTime.time;
            countdownActive = true;
        }
        CheckConsensus();
    }
    void CheckConsensus()
    {
        if (AllPlayersSelectedSameUpgrade(out string sameUpgrade))
        {
            Debug.Log($"[Server] Все игроки выбрали {sameUpgrade}. Применяем!");
            ApplyUpgrade(sameUpgrade);
        }
    }
    bool AllPlayersSelectedSameUpgrade(out string upgrade)
    {
        upgrade = null;
        if (selectedUpgrades.Count < NetworkServer.connections.Count)
            return false;
        var distinct = selectedUpgrades.Values.Distinct().ToList();
        if (distinct.Count == 1)
        {
            upgrade = distinct[0];
            return true;
        }
        return false;
    }
    [ServerCallback]
    void Update()
    {
        if (!countdownActive || Time.timeScale != 0f) return;
        double elapsed = NetworkTime.time - countdownStartTime;
        double remaining = countdownDuration - elapsed;
        if (remaining <= 0)
        {
            Debug.Log($"[Server] Время вышло! Автовыбор: {hostSelectedUpgrade}");
            ApplyUpgrade(hostSelectedUpgrade);
            countdownActive = false;
        }
        else
        {
            RpcUpdateTimerText($"Хост выбрал: {hostSelectedUpgrade} (автовыбор через {Mathf.CeilToInt((float)remaining)} сек)");
        }
    }
    void ApplyUpgrade(string upgrade)
    {
        Debug.Log($"[Server] Применяем апгрейд: {upgrade}");
        RpcResumeGame();
        Upgrade(upgrade);
        selectedUpgrades.Clear();
        countdownActive = false;
    }
}