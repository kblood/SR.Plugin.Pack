
using ItemEditorMod.UIHelper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ItemEditorMod.UIHelper
{
    public class UIHelper
    {
        public static SRModVerticalButtonsUI VerticalButtonsUi;

        static InputBoxUi new_InputBoxUi;

        public static void ShowMessage(string text, int seconds = 30, int messagetypes = 0)
        {
            if(messagetypes < 1)
            {
                Manager.GetUIManager().ShowMessagePopup(text, seconds);
            }
            if(messagetypes < 2)
            {
                Manager.GetUIManager().ShowSubtitle(text, seconds);
            }
            if(messagetypes == 2)
            {
                Manager.GetUIManager().ShowMessagePopup(text, seconds);
            }
            //Manager.GetUIManager().man (text, 10);
            //Get().setEntityInfo("Show Message", text);
        }

        public static IEnumerator ModalVerticalButtonsRoutine(string titleText, List<SRModButtonElement> buttons)
        {
            string info = "";

            if (VerticalButtonsUi == null)
            {
                try
                {
                    var newInputBoxUi = UnityEngine.Object.Instantiate(Manager.GetUIManager().m_InputBoxUi);
                    VerticalButtonsUi = new SRModVerticalButtonsUI(newInputBoxUi);
                    VerticalButtonsUi.InputBoxUi.InputBoxType = InputBoxUi.InputBoxTypes.MbOkcancel;
                }
                catch (Exception e)
                {
                    info += " error thrown " + e.Message + " when ";
                    ShowMessage(info);
                    //FileManager.SaveText(info, "errors.log");
                }
                info += "instantiated";
                yield return null;
            }

            if (Manager.GetUIManager().m_InputBoxUi.isActiveAndEnabled)
            {
                Manager.GetUIManager().m_InputBoxUi.Hide();
                yield return null;
            }
            if (new_InputBoxUi != null && new_InputBoxUi.isActiveAndEnabled)
            {
                new_InputBoxUi.Hide();
                yield return null;
            }
            if (VerticalButtonsUi != null && VerticalButtonsUi.InputBoxUi.isActiveAndEnabled)
            {
                VerticalButtonsUi.InputBoxUi.Hide();
                yield return null;
            }

            bool inputControlEnabled = Manager.GetUIManager().InputControlUi.gameObject.activeSelf;
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(false);
            Manager.ptr.DisableKeyCommands();

            var inputboxui = VerticalButtonsUi.InputBoxUi;

            inputboxui.Show(Manager.GameActive);
            inputboxui.transform.SetAsLastSibling();
            inputboxui.TitleText = titleText;
            inputboxui.OkButtonText.text = ("Ok" ?? TextManager.GetLoc("BUTTON_OK", true, false));
            inputboxui.CancelButtonText.text = ("Cancel" ?? TextManager.GetLoc("BUTTON_CANCEL", true, false));

            Manager.GetUIManager().ToggleEverything(true);
            yield return 0;

            try
            {
                VerticalButtonsUi.SetButtons(buttons, ref info);
            }
            catch (Exception e)
            {
                info += " Exception thrown: " + e.Message;
                ShowMessage(info);
                //FileManager.SaveText(info, "errors.log");
            }

            VerticalButtonsUi.InputBoxUi.gameObject.SetActive(true);

            yield return Manager.GetUIManager().WaitForActive(VerticalButtonsUi.InputBoxUi.gameObject, false);

            //Utils.SafeInvoke<bool>(ok, VerticalButtonsUi.InputBoxUi.IsOk());
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(inputControlEnabled);
            Manager.GetUIManager().ToggleEverything(true);
            Manager.ptr.EnableKeyCommands();
            //FileManager.SaveText(info, "errors.log");

            //ShowMessage(info + " all done");

            //SRInfoHelper.GetAllChildren(VerticalButtonsUi.InputBoxUi.transform);
            yield break;
        }
    }
}
