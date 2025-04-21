using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class UpgradeButton : MonoBehaviour
{
    private string upgradeName;
    private static Dictionary<uint, UpgradeButton> selectedButtons = new(); // какой игрок что выбрал
    private static List<UpgradeButton> allButtons = new();

    private HashSet<uint> selectedBy = new();
    private Image image;

    void Awake()
    {
        allButtons.Add(this);
        image = GetComponent<Image>();
    }

    void OnDestroy()
    {
        allButtons.Remove(this);
    }

    public void Setup(string upgrade)
    {
        upgradeName = upgrade;
        selectedBy.Clear();
        UpdateColor();
        GetComponentInChildren<Text>().text = upgrade;
    }

    public void Upgrade()
    {
        uint myNetId = NetworkClient.connection.identity.netId;

        // если уже выбрана другая кнопка, сбросим её
        if (selectedButtons.TryGetValue(myNetId, out var oldBtn) && oldBtn != this)
        {
            oldBtn.selectedBy.Remove(myNetId);
            oldBtn.UpdateColor();
        }

        selectedBy.Add(myNetId);
        selectedButtons[myNetId] = this;
        UpdateColor();

        ExpManager.Instance.CmdSelectUpgrade(myNetId, upgradeName);
    }

    public void Highlight(uint netId)
    {
        // сброс всех других
        foreach (var btn in allButtons)
        {
            if (btn != this && btn.selectedBy.Contains(netId))
            {
                btn.selectedBy.Remove(netId);
                btn.UpdateColor();
            }
        }

        selectedBy.Add(netId);
        selectedButtons[netId] = this;
        UpdateColor();
    }

    public bool IsUpgrade(string name) => upgradeName == name;

    private void UpdateColor()
    {
        if (selectedBy.Count == 0)
            image.color = Color.white;
        else if (selectedBy.Count == 1)
            image.color = Color.green;
        else
            image.color = Color.yellow; // несколько игроков выбрали одну кнопку
    }
}
