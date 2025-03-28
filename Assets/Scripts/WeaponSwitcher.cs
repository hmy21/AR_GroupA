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
    [Tooltip("ǹе�������ű�")]
    public GunfireController gunController;
    [Tooltip("���׷��ÿ������ű�")]
    public LandmineSpawner landmineSpawner;

    [Header("UI Elements")]
    [Tooltip("�л������İ�ť")]
    public Button switchButton;
    [Tooltip("��ʾ��ǰ����ͼ��� Image")]
    public Image weaponIcon;
    [Tooltip("ǹеͼ�� Sprite")]
    public Sprite gunIcon;
    [Tooltip("����ͼ�� Sprite")]
    public Sprite landmineIcon;

    [Header("Scene Objects")]
    [Tooltip("�����е�ǹеģ��")]
    public GameObject gunModel;
    [Tooltip("׼�� UI Ԫ�ص� RectTransform")]
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
