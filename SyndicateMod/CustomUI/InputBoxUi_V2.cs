using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace SyndicateMod.CustomUI
{
    public class InputBoxUi_V2 : InputBoxUi
    {
        public Button[] buttons = new Button[] { };
        //private InputBoxUi inputBox;

        public InputBoxUi_V2(InputBoxUi inputBox)
        {
            //if (inputBox == Manager.GetUIManager().InputBoxUi)
            //{
            //    inputBox = UnityEngine.Object.Instantiate(Manager.GetUIManager().m_InputBoxUi);
            //    Manager.GetUIManager().InputBoxUi.Hide();
            //}
            transform.parent = inputBox.transform.parent;
            transform.position = inputBox.transform.position;
            transform.rotation = inputBox.transform.rotation;
            transform.eulerAngles = inputBox.transform.eulerAngles;
            transform.localScale = inputBox.transform.localScale;

            m_TitleText = inputBox.m_TitleText;
            m_InputFieldLabel = inputBox.m_InputFieldLabel;
            m_InputControlContainer = inputBox.m_InputControlContainer;
            m_OkButtonContainer = inputBox.m_OkButtonContainer;
            m_CancelButtonContainer = inputBox.m_CancelButtonContainer;
            m_InputBoxType = inputBox.InputBoxType;
            m_TimeScaler = TimeManager.AddTimeScaler(1f);
            InputText = inputBox.InputText;

            inputBox.transform.SetParent(this.transform);

            foreach (Transform t in inputBox.transform)
            {
                t.SetParent(this.transform);
            }
            //if(inputBox != Manager.GetUIManager().InputBoxUi)
            //    Destroy(inputBox);
            Show();
        }

        public void AddButton(string name = "NewButton", UnityAction action = null)
        {
            var layout = this.OkButton.transform.parent;
            var divider = layout.FindChild("Divider");
            var newDivider = UnityEngine.Object.Instantiate<Transform>(divider);
            newDivider.SetParent(layout);
            var newButton = UnityEngine.Object.Instantiate<Button>(this.CancelButton);
            newButton.name = name;
            var prefabButton = newButton.GetComponent<PrefabButton>();
            prefabButton.ButtonText.text = name;
            //newButton.onClick = new Button.ButtonClickedEvent();
            newButton.onClick.AddListener(action);

            var content = OkButton.transform.parent.parent.parent;
            divider = content.FindChild("Divider");
            newDivider = UnityEngine.Object.Instantiate<Transform>(divider);
            newDivider.SetParent(content);
            newDivider.SetAsLastSibling();
            var newButtons = UnityEngine.Object.Instantiate<Transform>(content.FindChild("Buttons"));
            newButtons.SetParent(content);
            newButtons.SetAsLastSibling();
            UpdateButtons();
        }

        public void UpdateButtons()
        {
            var allButtons = this.OkButton.transform.parent.GetComponentsInChildren<Button>();
            buttons = allButtons;
        }

        //public Coroutine DoModalMessageBox(string titleText, string messageText, InputBoxUi.InputBoxTypes messsageBoxType, string okText = null, string cancelText = null, Action<bool> ok = null, Func<string> messageTextFunc = null, string inputValue = "")
        //{
        //    return base.StartCoroutine(this.ModalMessageBoxRoutine(titleText, messageText, messsageBoxType, okText, cancelText, ok, messageTextFunc, inputValue));
        //}

        //private IEnumerator ModalMessageBoxRoutine(string titleText, string messageText, InputBoxUi.InputBoxTypes messsageBoxType, string okText = null, string cancelText = null, Action<bool> ok = null, Func<string> messageTextFunc = null, string inputText = "")
        //{
        //    if (Manager.GetUIManager().m_InputBoxUi.isActiveAndEnabled)
        //    {
        //        Manager.GetUIManager().m_InputBoxUi.Hide();
        //        yield return null;
        //    }
        //    bool inputControlEnabled = Manager.GetUIManager().InputControlUi.gameObject.activeSelf;
        //    Manager.GetUIManager().InputControlUi.gameObject.SetActive(false);
        //    Manager.ptr.DisableKeyCommands();
        //    Manager.GetUIManager().m_InputBoxUi.InputBoxType = messsageBoxType;
        //    Manager.GetUIManager().m_InputBoxUi.InputFieldLabelText = messageText;
        //    Manager.GetUIManager().m_InputBoxUi.InputFieldLabelTextFunc = messageTextFunc;
        //    Manager.GetUIManager().m_InputBoxUi.TitleText = titleText;
        //    Manager.GetUIManager().m_InputBoxUi.InputText = inputText;
        //    Manager.GetUIManager().m_InputBoxUi.OkButtonText.text = (okText ?? TextManager.GetLoc("BUTTON_OK", true, false));
        //    Manager.GetUIManager().m_InputBoxUi.CancelButtonText.text = (cancelText ?? TextManager.GetLoc("BUTTON_CANCEL", true, false));

        //    Manager.GetUIManager().m_InputBoxUi.transform.SetAsLastSibling();

        //    yield return Manager.GetUIManager().WaitForActive(Manager.GetUIManager().m_InputBoxUi.gameObject, false);
        //    Utils.SafeInvoke<bool>(ok, Manager.GetUIManager().m_InputBoxUi.IsOk());
        //    Manager.GetUIManager().InputControlUi.gameObject.SetActive(inputControlEnabled);

        //    Manager.ptr.EnableKeyCommands();
        //    yield break;
        //}
    }
}
