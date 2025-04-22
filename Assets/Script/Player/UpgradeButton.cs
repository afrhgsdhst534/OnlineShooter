using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using System.Linq;
public class UpgradeButton : MonoBehaviour
{
    public string UpgradeName { get; private set; }
    private string myUpgrade;
    private bool selected = false;

    public void Setup(string upgrade)
    {
        myUpgrade = upgrade;
        UpgradeName = upgrade;
        selected = false;
        GetComponentInChildren<Text>().text = upgrade;
        GetComponent<Image>().color = Color.white;
        gameObject.SetActive(true);
    }

    public void Upgrade()
    {
        if (selected) return;
        ResetOthers();
        selected = true;
        GetComponent<Image>().color = Color.green;
        uint myNetId = NetworkClient.connection.identity.netId;
        if (ExpManager.Instance != null) ExpManager.Instance.CmdSelectUpgrade(myNetId, myUpgrade);
    }

    public void HighlightIfOther(uint netId)
    {
        if (!selected) GetComponent<Image>().color = Color.yellow;
    }

    private void ResetOthers()
    {
        foreach (var b in transform.parent.GetComponentsInChildren<UpgradeButton>())
        {
            if (b != this)
            {
                b.selected = false;
                b.GetComponent<Image>().color = Color.white;
            }
        }
    }
}