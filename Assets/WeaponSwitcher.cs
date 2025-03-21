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
        // 简单切换：若当前为枪械，则切换为地雷；否则切换为枪械
        currentWeapon = (currentWeapon == WeaponType.Gun) ? WeaponType.Landmine : WeaponType.Gun;
        SetWeapon(currentWeapon);
        Debug.Log("Switched weapon to: " + currentWeapon);
    }

    private void SetWeapon(WeaponType weapon)
    {
        // 若是枪械模式，启用枪械脚本；若是地雷模式，禁用枪械脚本
        if (gunController != null)
            gunController.enabled = (weapon == WeaponType.Gun);

        // 若是地雷模式，启用地雷脚本；否则禁用
        if (landmineSpawner != null)
            landmineSpawner.enabled = (weapon == WeaponType.Landmine);

        // 更新武器图标
        if (weaponIcon != null)
        {
            weaponIcon.sprite = (weapon == WeaponType.Gun) ? gunIcon : landmineIcon;
        }

        // 切换场景中显示的对象：枪械模型与准星
        if (gunModel != null)
        {
            gunModel.SetActive(weapon == WeaponType.Gun);
        }

        if (crosshairUI != null)
        {
            crosshairUI.gameObject.SetActive(weapon == WeaponType.Gun);
        }
        // 当切换到地雷武器时，清除枪口火光
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
