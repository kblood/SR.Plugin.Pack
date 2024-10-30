using SRModManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SRModManager.CustomUI
{
    public class SRModVerticalDualButtonsUI
    {
        public InputBoxUi InputBoxUi;
        public Dictionary<SRModButtonElement, SRModButtonElement> Buttons;
        public Transform Content;
        public Transform Divider;
        public Transform CancelButtonLayout;

        public SRModVerticalDualButtonsUI(InputBoxUi inputBoxUi)
        {
            InputBoxUi = inputBoxUi;
            Buttons = new Dictionary<SRModButtonElement, SRModButtonElement>();
            
            inputBoxUi.m_InputControlContainer.gameObject.SetActive(false);
            inputBoxUi.m_OkButtonContainer.gameObject.SetActive(false);
            inputBoxUi.m_CancelButtonContainer.gameObject.SetActive(true);
            var children = InputBoxUi.transform.GetComponentsInChildren<Transform>();
            Content = children.Where(t => t.transform.name == "content").First();
            var message = children.Where(t => t.transform.name == "Message").First();
            message.gameObject.SetActive(false);
            Divider = Content.GetChild(Content.childCount-2);
            CancelButtonLayout = Content.GetLastChild().GetFirstChild();
            var vlg = Content.GetComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = true;
        }

        public void SetButtons(Dictionary<SRModButtonElement, SRModButtonElement> newButtons, ref string info)
        {
            info += " Deactivating buttons";
            foreach (var oldButton in Buttons)
            {
                oldButton.Key.Button.onClick.RemoveAllListeners();
                oldButton.Key.Text.text = "";
                oldButton.Key.ButtonText = "";
                oldButton.Key.DescriptionText.text = "";
                oldButton.Key.Description = "";
                oldButton.Key.Button.onClick.RemoveAllListeners();
                oldButton.Key.Container.gameObject.SetActive(false);
                //oldButton.Key.Button.onClick.AddListener(newButton.Action);
                //oldButton.Key.Container.gameObject.SetActive(true);

                if (oldButton.Value != null)
                {
                    oldButton.Value.Button.onClick.RemoveAllListeners();
                    oldButton.Value.Text.text = "";
                    oldButton.Value.ButtonText = "";
                    //oldButton.Value.DescriptionText.text = "";
                    oldButton.Value.Description = "";
                    oldButton.Value.Button.onClick.RemoveAllListeners();
                    oldButton.Value.Container.gameObject.SetActive(false);
                }
            }

            foreach (var newButton in newButtons)
            {
                if (Buttons.Count() == 0)
                {
                    //info += " Adding first button " + i;
                    var children = InputBoxUi.transform.GetComponentsInChildren<Transform>(); //SRInfoHelper.GetAllChildren(InputBoxUi.transform);
                    var container = UnityEngine.Object.Instantiate(CancelButtonLayout);

                    newButton.Key.Container = container;
                    container.SetParent(Content);

                    var descriptiontext = children.Where(t => t.transform.name == "TitleText").First();
                    descriptiontext = UnityEngine.Object.Instantiate(descriptiontext);

                    //info += " child count "+container.childCount;

                    var OkButton = newButton.Key.Container.GetChild(0);
                    var Cancel = newButton.Key.Container.GetChild(2);

                    //GameObject.DestroyImmediate(OkButton.gameObject);


                    // Destroy gameobjects and components that will be in the way
                    GameObject.DestroyImmediate(Cancel.GetComponent<PrefabButton>());
                    GameObject.DestroyImmediate(Cancel.GetLastChild().gameObject);

                    GameObject.DestroyImmediate(OkButton.GetComponent<PrefabButton>());
                    GameObject.DestroyImmediate(OkButton.GetLastChild().gameObject);

                    //info += " test1 " ;

                    newButton.Key.DescriptionText = descriptiontext.GetComponent<Text>();
                    //info += " test2 ";
                    // Make sure the description textbox is on the left of the divider
                    descriptiontext.SetParent(newButton.Key.Container);
                    descriptiontext.SetSiblingIndex(0);
                    newButton.Value.Container = OkButton;

                    OkButton.SetAsLastSibling();

                    //var container2 = UnityEngine.Object.Instantiate(container);
                    //newButton.Value.Container = container2;
                    //var children2 = newButton.Value.Container.GetComponentInChildren<Transform>();
                    //var descriptiontext2 = UnityEngine.Object.Instantiate(descriptiontext);
                    //container2.SetParent(Content);

                    //info += " test3 ";
                }
                else
                {
                    info += " Copying button for newbutton: ";

                    var container = UnityEngine.Object.Instantiate(Buttons.First().Key.Container);
                    newButton.Key.Container = container;
                    newButton.Value.Container = newButton.Key.Container.GetLastChild();
                    //var container2 = UnityEngine.Object.Instantiate(Buttons.First().Key.Container);
                    //newButton.Value.Container = container2;
                }

                info += ". Child count for newbutton container: " + newButton.Key.Container.childCount;

                var child0 = newButton.Key.Container.GetChild(0);

                //info += " Child0 " + child0.name;
                var child1 = newButton.Key.Container.GetChild(1);
                //info += " Child1 " + child1.name;
                var child2 = newButton.Key.Container.GetChild(2);
                //info += " Child2 " + child2.name;
                var okbuttonchild = newButton.Key.Container.GetChild(3);

                //FileManager.Log("Setting key stuff");

                newButton.Key.Button = child2.GetComponentInChildren<Button>();
                newButton.Key.Text = child2.GetComponentInChildren<Text>();
                newButton.Key.DescriptionText = child0.GetComponent<Text>();

                //FileManager.Log("Setting value stuff");

                newButton.Value.Button = okbuttonchild.GetComponentInChildren<Button>();
                newButton.Value.Text = okbuttonchild.GetComponentInChildren<Text>();
                //newButton.Value.DescriptionText = child0.GetComponent<Text>();

                // Place the button on the content part of the UiBox
                newButton.Key.Container.SetParent(Content);
                int index = newButton.Key.Container.GetSiblingIndex();
                //FileManager.Log("Current index of container: " + index);
                //info += " Current index: " + index;

                // Place this button above the divider for the Ok and cancel buttons (and below all previously added buttons)
                newButton.Key.Container.SetSiblingIndex(Content.childCount - 3);
                index = newButton.Key.Container.GetSiblingIndex();
                //FileManager.Log("New index of container: " + index);

                newButton.Key.DescriptionText.text = newButton.Key.Description;
                newButton.Key.Text.text = newButton.Key.ButtonText;
                //info += " Z ";
                //newButton.Button.onClick.RemoveAllListeners();
                newButton.Key.Button.onClick.AddListener(newButton.Key.Action);

                //FileManager.Log("Value button setup:");
                {
                    info += ". Child count for newbutton(mod ui) container: " + newButton.Value.Container.childCount;
                    /*
                    child0 = newButton.Value.Container.GetChild(0);
                    //info += " Child0 " + child0.name;
                    child1 = newButton.Value.Container.GetChild(1);
                    //info += " Child1 " + child1.name;
                    child2 = newButton.Value.Container.GetChild(2);
                    //info += " Child2 " + child2.name;

                    newButton.Value.Button = child2.GetComponentInChildren<Button>();
                    newButton.Value.Text = child2.GetComponentInChildren<Text>();
                    newButton.Value.DescriptionText = child0.GetComponent<Text>();

                    // Place the button on the content part of the UiBox
                    index = newButton.Value.Container.GetSiblingIndex();
                    //info += " Current index: " + index;

                    // Place this button above the divider for the Ok and cancel buttons (and below all previously added buttons)
                    newButton.Value.Container.SetSiblingIndex(Content.childCount - 3);

                    newButton.Value.DescriptionText.text = newButton.Value.Description;
                    */
                    //info += " Value Text: " + newButton.Value.Text;
                    //info += " Value Text.text: " + newButton.Value.Text.text;
                    //info += " Value button: " + newButton.Value.Button;
                    //info += " Value buttontext: " + newButton.Value.ButtonText;
                    //FileManager.Log(info);

                    newButton.Value.Text.text = newButton.Value.ButtonText;
                    //info += " Z ";
                    //newButton.Button.onClick.RemoveAllListeners();
                    newButton.Value.Button.onClick.AddListener(newButton.Value.Action);
                }

                FileManager.Log("Adding buttons");

                Buttons.Add(newButton.Key, newButton.Value);

                newButton.Key.Container.gameObject.SetActive(true);

                if (newButton.Value.ButtonText != null && !string.IsNullOrEmpty(newButton.Value.ButtonText))
                    newButton.Value.Container.transform.gameObject.SetActive(true);
                else
                    newButton.Value.Container.transform.gameObject.SetActive(false);

                //newButton.Container.ActivateChildren();
            }
            /*
            for (int i = 0; i < newButtons.Count(); i++)
            {
                var newButton = newButtons[i];

                if (i+1 < Buttons.Count())
                {
                    info += " Updating existing button " + i;

                    var oldButton = Buttons[i];

                    oldButton.Text.text = newButton.ButtonText;
                    oldButton.ButtonText = newButton.ButtonText;
                    oldButton.DescriptionText.text = newButton.Description;
                    oldButton.Description = newButton.Description;
                    oldButton.Button.onClick.RemoveAllListeners();
                    oldButton.Button.onClick.AddListener(newButton.Action);
                    oldButton.Container.gameObject.SetActive(true);
                }
                else
                {
                    if(Buttons.Count() == 0)
                    {
                        //info += " Adding first button " + i;

                        var children = InputBoxUi.transform.GetComponentsInChildren<Transform>(); //SRInfoHelper.GetAllChildren(InputBoxUi.transform);
                        var container = UnityEngine.Object.Instantiate(CancelButtonLayout);
                        newButton.Container = container;
                        container.SetParent(Content);
                        var descriptiontext = children.Where(t => t.transform.name == "TitleText").First();
                        descriptiontext = UnityEngine.Object.Instantiate(descriptiontext);

                        //info += " child count "+container.childCount;

                        var OkButton = newButton.Container.GetChild(0);
                        var Cancel = newButton.Container.GetChild(2);

                        // Destroy gameobjects and components that will be in the way
                        GameObject.DestroyImmediate(OkButton.gameObject);
                        GameObject.DestroyImmediate(Cancel.GetComponent<PrefabButton>());
                        GameObject.DestroyImmediate(Cancel.GetLastChild().gameObject);

                        //info += " test1 " ;

                        newButton.DescriptionText = descriptiontext.GetComponent<Text>();
                        //info += " test2 ";
                        // Make sure the description textbox is on the left of the divider
                        descriptiontext.SetParent(newButton.Container);
                        descriptiontext.SetSiblingIndex(0);

                        //info += " test3 ";
                    }
                    else
                    {
                        info += " Copying button for newbutton number: " + i;

                        var container = UnityEngine.Object.Instantiate(Buttons.First().Key.Container);
                        newButton.Container = container;
                    }
                    info += ". Child count for newbutton container: " + newButton.Container.childCount;
                    var child0 = newButton.Container.GetChild(0);
                    //info += " Child0 " + child0.name;
                    var child1 = newButton.Container.GetChild(1);
                    //info += " Child1 " + child1.name;
                    var child2 = newButton.Container.GetChild(2);
                    //info += " Child2 " + child2.name;

                    newButton.Button = child2.GetComponentInChildren<Button>();
                    newButton.Text = child2.GetComponentInChildren<Text>();
                    newButton.DescriptionText = child0.GetComponent<Text>();

                    // Place the button on the content part of the UiBox
                    newButton.Container.SetParent(Content);
                    int index = newButton.Container.GetSiblingIndex();
                    //info += " Current index: " + index;

                    // Place this button above the divider for the Ok and cancel buttons (and below all previously added buttons)
                    newButton.Container.SetSiblingIndex(Content.childCount - 3);

                    newButton.DescriptionText.text = newButton.Description;
                    newButton.Text.text = newButton.ButtonText;
                    //info += " Z ";
                    //newButton.Button.onClick.RemoveAllListeners();
                    newButton.Button.onClick.AddListener(newButton.Action);
                    Buttons.Add(newButton);

                    newButton.Container.gameObject.SetActive(true);
                    //newButton.Container.ActivateChildren();
                }
            }
            */
        }

    }
}
