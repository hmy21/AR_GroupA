using BigRookGames.Weapons;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    Gun,
    Landmine
}

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponType currentWeapon = WeaponType.Gun;

    [Header("Weapon Controllers")]
    [Tooltip("枪械控制器脚本")]
    public GunfireController gunController;
    [Tooltip("地雷放置控制器脚本")]
    public LandmineSpawner landmineSpawner;

    [Header("UI Elements")]
    [Tooltip("切换武器的按钮")]
    public Button switchButton;
    [Tooltip("显示当前武器图标的 Image")]
    public Image weaponIcon;
    [Tooltip("枪械图标 Sprite")]
    public Sprite gunIcon;
    [Tooltip("地雷图标 Sprite")]
    public Sprite landmineIcon;

    [Header("Scene Objects")]
    [Tooltip("场景中的枪械模型")]
    public GameObject gunModel;
    [Tooltip("准星 UI 元素的 RectTransform")]
    public RectTransform crosshairUI;

    void Start()
    {
        SetWeapon(currentWeapon);

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchWeapon);
        }
    }

    public void SwitchWeapon()
    {
       
        currentWeapon = (currentWeapon == WeaponType.Gun) ? WeaponType.Landmine : WeaponType.Gun;
        SetWeapon(currentWeapon);
        Debug.Log("Switched weapon to: " + currentWeapon);
    }

    private void SetWeapon(WeaponType weapon)
    {
        if (gunController != null)
            gunController.gameObject.SetActive(weapon == WeaponType.Gun);

       
        if (landmineSpawner != null)
        {
            landmineSpawner.gameObject.SetActive(weapon == WeaponType.Landmine);
            landmineSpawner.isPlacementEnabled = (weapon == WeaponType.Landmine);
        }

        
        if (weaponIcon != null)
            weaponIcon.sprite = (weapon == WeaponType.Gun) ? gunIcon : landmineIcon;

        if (gunModel != null)
            gunModel.SetActive(weapon == WeaponType.Gun);
        if (crosshairUI != null)
            crosshairUI.gameObject.SetActive(weapon == WeaponType.Gun);

        Debug.Log($"Weapon set to: {weapon}. Gun active: {(gunController != null && gunController.gameObject.activeSelf)}, Landmine placement enabled: {(landmineSpawner != null && landmineSpawner.isPlacementEnabled)}");
       
        if (weapon == WeaponType.Landmine)
        {
            GameObject[] flashes = GameObject.FindGameObjectsWithTag("MuzzleFlash");
            foreach (GameObject flash in flashes)
            {
                Destroy(flash);
            }
        }
        
    }
}
