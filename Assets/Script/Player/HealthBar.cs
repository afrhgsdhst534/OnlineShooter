using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class HealthBar : NetworkBehaviour
{
    public Slider slider;
    public void SetHealth(int cur,int max)
    {
        slider.value = cur; 
        slider.maxValue = max;
    }
}
