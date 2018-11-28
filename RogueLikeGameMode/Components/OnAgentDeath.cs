using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RogueLikeGameMode.Components
{
    class OnAgentDeath : MonoBehaviour
    {
        //public Transform m_ExplosiveObj;

        private AIEntity m_AIEntity;

        //private Projectile m_ExplosiveInstance;
        private UserData user;


        private void Start()
        {
            this.SetAIEntity();
            this.RegisterCallback();
        }

        private void OnEnable()
        {
            this.RegisterCallback();
            user = SaveGame.CurrentUser;
        }

        private void RegisterCallback()
        {
            if (this.m_AIEntity != null && !this.m_AIEntity.IsEventCallbackRegistered(AIEventNotification.Killed, new AIEntityEvent(this.AIEntityEventCallback)))
            {
                this.m_AIEntity.RegisterEventCallback(AIEventNotification.Killed, new AIEntityEvent(this.AIEntityEventCallback));
            }
        }

        private void SetAIEntity()
        {
            if (this.m_AIEntity == null)
            {
                Transform transform = base.transform;
                while (transform != null && this.m_AIEntity == null)
                {
                    this.m_AIEntity = transform.GetComponent<AIEntity>();
                    transform = transform.parent;
                }
            }
        }

        void OnApplicationQuit()
        {
            Manager.Get().DoSaveGame(0);
            Manager.GetInputControl().DoSaveGame(0);
            Manager.GetInputControl().DoSaveAuto();
            Manager.Get().AutoSave();
            
            // Don't allow the user to exit until we got permission in
            if (!allowQuitting)
            {
                StartCoroutine("DelayedQuit");
                Application.CancelQuit();
            }

            Debug.Log("Application ending after " + Time.time + " seconds");
            //if (Manager.Get().GameInProgress)
            //{
            //    // Don't allow the user to exit until we got permission in
            //    if (!allowQuitting)
            //    {
            //        StartCoroutine("DelayedQuit");
            //        Application.CancelQuit();
            //    }

            //    Debug.Log("Application ending after " + Time.time + " seconds");
            //}
        }

        // Delays quitting for 2 seconds and
        // loads the finalsplash level during that time.

        float delayQuitTime = 10.0f;
        static bool allowQuitting = false;

        //void Awake()
        //{
        //    // This game object needs to survive multiple levels
        //    DontDestroyOnLoad(this);
        //}

        IEnumerator DelayedQuit()
        {
            DeleteAllSaves();
            Manager.Get().DoSaveGame(0);
            Manager.GetInputControl().DoSaveGame(0);
            Manager.GetInputControl().DoSaveAuto();
            Manager.Get().AutoSave();
            //Application.LoadLevel("finalsplash");

            Manager.GetUIManager().ShowMessagePopup("Quitting... but first ;) ");
            // Wait for showSplashTimeout
            yield return new WaitForSeconds(delayQuitTime);

            // then quit for real
            allowQuitting = true;
            Application.Quit();
        }

        IEnumerator ShowMessage()
        {
            //yield return new WaitForSeconds(1);
            var agent = (AgentAI)m_AIEntity;
            string aClass = agent.AgentClassName();
            Manager.GetUIManager().DoModalMessageBox($"{aClass} is dead and gone", "Take a minute to mourn your lost agent.", InputBoxUi.InputBoxTypes.MbOk);
            yield return new WaitForSeconds(1);
        }

        public void AIEntityEventCallback(AIEntity _target, AIEventNotification _event)
        {

            var agent = (AgentAI)_target;
            string aClass = agent.AgentClassName();
            agent.m_DidRespawn = false;
            this.m_AIEntity = null;

            DeleteAllSaves();
            Destroy(agent.gameObject);
            //Manager.Get().DoSaveGame(0);
            //Manager.GetInputControl().DoSaveGame(0);
            //Manager.GetInputControl().DoSaveAuto();
            //Manager.Get().AutoSave();
            //StartCoroutine("ShowMessage");
            
            
            Manager.Get().DoSaveGame(0);
            Manager.GetInputControl().DoSaveGame(0);
            Manager.GetInputControl().DoSaveAuto();
            Manager.Get().AutoSave();
            Manager.GetUIManager().DoModalMessageBox($"{aClass} is dead and gone", "Take a minute to mourn your lost agent.", InputBoxUi.InputBoxTypes.MbOk);
        }

        void DeleteAllSaves()
        {
            for (int i = 0; i < SaveGame.MaxSaveSlots; i++)
            {
                SaveGame.DeleteSave(i, user);
            }
        }
    }
}
