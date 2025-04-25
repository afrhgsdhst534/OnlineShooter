using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class UpgradeButton : MonoBehaviour
{
    public string UpgradeName { get; private set; }
    private string myUpgrade;
    private bool isSelectedByMe = false;
    public void Setup(string upgrade)
    {
        myUpgrade = upgrade;
        UpgradeName = upgrade;
        isSelectedByMe = false;
        GetComponentInChildren<Text>().text = upgrade;
        ResetHighlight();
        gameObject.SetActive(true);
    }
    public void Upgrade()
    {
        if (isSelectedByMe) return;
        foreach (var b in transform.parent.GetComponentsInChildren<UpgradeButton>())
        {
            if (b.isSelectedByMe)
            {
                b.ResetButton();
            }
        }
        isSelectedByMe = true;
        GetComponent<Image>().color = Color.green;
        uint myNetId = NetworkClient.connection.identity.netId;
        ExpManager.Instance?.CmdSelectUpgrade(myNetId, myUpgrade);
    }
    public void HighlightAsOtherPlayer()
    {
        if (!isSelectedByMe)
        {
            GetComponent<Image>().color = Color.yellow;
        }
    }
    public void ResetHighlight()
    {
        if (!isSelectedByMe)
        {
            GetComponent<Image>().color = Color.white;
        }
    }
    public void ResetButton()
    {
        isSelectedByMe = false;
        GetComponent<Image>().color = Color.white;
    }
    public bool IsSelectedByMe() => isSelectedByMe;
}