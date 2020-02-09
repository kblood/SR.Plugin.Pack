using SyndicateMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SyndicateMod.CustomUI
{
    public class SRModVerticalButtonsUI
    {
        public InputBoxUi InputBoxUi;
        public List<SRModButtonElement> Buttons;
        public Transform Content;
        public Transform Divider;
        public Transform CancelButtonLayout;

        public SRModVerticalButtonsUI(InputBoxUi inputBoxUi)
        {
            InputBoxUi = inputBoxUi;
            Buttons = new List<SRModButtonElement>();
            
            inputBoxUi.m_InputControlContainer.gameObject.SetActive(false);
            inputBoxUi.m_OkButtonContainer.gameObject.SetActive(false);
            inputBoxUi.m_CancelButtonContainer.gameObject.SetActive(true);
            var children = SRInfoHelper.GetAllChildren(InputBoxUi.transform);
            Content = children.Where(t => t.transform.name == "content").First();
            var message = children.Where(t => t.transform.name == "Message").First();
            message.gameObject.SetActive(false);
            Divider = Content.GetChild(Content.childCount-2);
            CancelButtonLayout = Content.GetLastChild().GetFirstChild();
            var vlg = Content.GetComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = true;
        }

        public void SetButtons(List<SRModButtonElement> buttons, ref string info)
        {
            info += " Deactivating buttons";
            foreach (var button in Buttons)
            {
                button.Container.gameObject.SetActive(false);
            }
            
            for(int i = 0; i < buttons.Count(); i++)
            {
                if(i+1 < Buttons.Count())
                {
                    info += " Updating existing button " + i;

                    var oldButton = Buttons[i];
                    var newButton = buttons[i];

                    oldButton.Text.text = newButton.ButtonText;
                    oldButton.DescriptionText.text = newButton.Description;
                    oldButton.Button.onClick.RemoveAllListeners();
                    oldButton.Button.onClick.AddListener(newButton.Action);
                    oldButton.Container.gameObject.SetActive(true);
                }
                else
                {
                    var newButton = buttons[i];

                    if(Buttons.Count() == 0)
                    {
                        info += " Adding first button " + i;

                        var children = SRInfoHelper.GetAllChildren(InputBoxUi.transform);
                        var container = UnityEngine.Object.Instantiate(CancelButtonLayout);
                        newButton.Container = container;
                        container.SetParent(Content);
                        var descriptiontext = children.Where(t => t.transform.name == "TitleText").First();
                        descriptiontext = UnityEngine.Object.Instantiate(descriptiontext);

                        info += " child count "+container.childCount;

                        var OkButton = newButton.Container.GetChild(0);
                        GameObject.DestroyImmediate(OkButton.gameObject);

                        info += " test1 " ;
                        
                        newButton.DescriptionText = descriptiontext.GetComponent<Text>();

                        info += " test2 ";

                        descriptiontext.SetParent(newButton.Container);
                        descriptiontext.SetSiblingIndex(0);

                        info += " test3 ";
                    }
                    else
                    {
                        info += " Copying button for newbutton number: " + i;

                        var container = UnityEngine.Object.Instantiate(Buttons.First().Container);
                        newButton.Container = container;
                    }
                    info += ". Child count for newbutton container: " + newButton.Container.childCount;
                    var child0 = newButton.Container.GetChild(0);
                    info += " Child0 " + child0.name;
                    var child1 = newButton.Container.GetChild(1);
                    info += " Child1 " + child1.name;
                    var child2 = newButton.Container.GetChild(2);
                    info += " Child2 " + child2.name;

                    newButton.Button = child2.GetComponent<PrefabButton>().Button;
                    newButton.Text = child2.GetComponent<PrefabButton>().ButtonText;
                    newButton.DescriptionText = child0.GetComponent<Text>();
                    info += " Last stuff for button: " + i;

                    newButton.Container.SetParent(Content);
                    int index = newButton.Container.GetSiblingIndex();
                    info += " Current index: " + index;

                    newButton.Container.SetSiblingIndex(Content.childCount - 3);

                    index = newButton.Container.GetSiblingIndex();
                    info += " Current index: " + index;
                    //container.SetSiblingIndex(4);

                    info += " X ";
                    newButton.Text.text = newButton.ButtonText;
                    info += " Y ";
                    newButton.DescriptionText.text = newButton.Description;
                    info += " Z ";

                    Buttons.Add(newButton);
                    newButton.Button.onClick.RemoveAllListeners();
                    newButton.Button.onClick.AddListener(newButton.Action);
                    newButton.Container.gameObject.SetActive(true);
                }
            }

        }
    }
}
