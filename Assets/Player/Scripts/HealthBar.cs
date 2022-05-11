using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;
    public Player player;
    public Color fullColor;
    public Color lowColor;

    private Camera _cameraToPoint;

    private void Start()
    {
        _cameraToPoint = Camera.main;
    }

    private void Update()
    {
        Quaternion rotation = _cameraToPoint.transform.rotation;
        transform.parent.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.down);
    }

    public void UpdateHealthBar()
    {
        healthBarImage.fillAmount = Mathf.Clamp(player.health / player.maxHealth, 0, 1f);
        healthBarImage.color = Color.Lerp(lowColor, fullColor, healthBarImage.fillAmount);
        
    }
}
