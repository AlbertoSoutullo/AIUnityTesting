using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Image healthBarImage;
    public Player player;
    public Color fullColor;
    public Color lowColor;

    public Camera Camera;

    private void Update()
    {
        transform.parent.LookAt(transform.position + Camera.transform.rotation * Vector3.back, 
            Camera.transform.rotation * Vector3.down);
    }

    public void UpdateHealthBar()
    {
        // Debug.Log(fullColor.ToString()); Debug.Log(lowColor.ToString());
        // Debug.Log(healthBarImage.ToString()); Debug.Log(player.ToString());
        healthBarImage.fillAmount = Mathf.Clamp(player.health / player.maxHealth, 0, 1f);
        healthBarImage.color = Color.Lerp(lowColor, fullColor, healthBarImage.fillAmount);
        
    }

}
