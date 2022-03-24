using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerDropdown : MonoBehaviour
    {
        [Header("Settings")]
        public UIManager UIManagerAsset;
        public bool webglMode = false;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        public Image background;
        public Image contentBackground;
        public Image mainIcon;
        public TextMeshProUGUI mainText;
        public Image expandIcon;
        public Image itemBackground;
        public Image itemIcon;
        public TextMeshProUGUI itemText;
        CustomDropdown dropdownMain;
        DropdownMultiSelect dropdownMulti;

        void Awake()
        {
            if (Application.isPlaying && webglMode == true)
                return;
   
            try
            {
                dropdownMain = gameObject.GetComponent<CustomDropdown>();

                if (dropdownMain == null) { dropdownMulti = gameObject.GetComponent<DropdownMultiSelect>(); }
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateDropdown();
                    this.enabled = false;
                }
            }

            catch { Debug.Log("<b>[Modern UI Pack]</b> No UI Manager found, assign it manually.", this); }
        }

        void LateUpdate()
        {
            if (UIManagerAsset == null)
                return;

            if (UIManagerAsset.enableDynamicUpdate == true)
                UpdateDropdown();
        }

        void UpdateDropdown()
        {
            if (Application.isPlaying && webglMode == true)
                return;

            try
            {
                if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Basic)
                {
                    if (overrideColors == false)
                    {
                        background.color = UIManagerAsset.dropdownColor;
                        contentBackground.color = UIManagerAsset.dropdownColor;
                        mainIcon.color = UIManagerAsset.dropdownTextColor;
                        mainText.color = UIManagerAsset.dropdownTextColor;
                        expandIcon.color = UIManagerAsset.dropdownTextColor;
                        itemBackground.color = UIManagerAsset.dropdownItemColor;
                        itemIcon.color = UIManagerAsset.dropdownTextColor;
                        itemText.color = UIManagerAsset.dropdownTextColor;
                    }

                    if (overrideFonts == false)
                    {
                        mainText.font = UIManagerAsset.dropdownFont;
                        mainText.fontSize = UIManagerAsset.dropdownFontSize;
                        itemText.font = UIManagerAsset.dropdownFont;
                        itemText.fontSize = UIManagerAsset.dropdownFontSize;
                    }
                }

                else if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Custom)
                {
                    if (overrideColors == false)
                    {
                        background.color = UIManagerAsset.dropdownColor;
                        contentBackground.color = UIManagerAsset.dropdownColor;
                        mainIcon.color = UIManagerAsset.dropdownIconColor;
                        mainText.color = UIManagerAsset.dropdownTextColor;
                        expandIcon.color = UIManagerAsset.dropdownIconColor;
                        itemBackground.color = UIManagerAsset.dropdownItemColor;
                        itemIcon.color = UIManagerAsset.dropdownItemIconColor;
                        itemText.color = UIManagerAsset.dropdownItemTextColor;
                    }

                    if (overrideFonts == false)
                    {
                        mainText.font = UIManagerAsset.dropdownFont;
                        mainText.fontSize = UIManagerAsset.dropdownFontSize;
                        itemText.font = UIManagerAsset.dropdownItemFont;
                        itemText.fontSize = UIManagerAsset.dropdownItemFontSize;
                    }
                }
            }

            catch { }
        }
    }
}