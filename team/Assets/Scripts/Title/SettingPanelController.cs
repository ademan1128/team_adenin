using UnityEngine;

public class SettingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }
}
