using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Managers;
using UI;
using TMPro;
using UnityEngine;
using Statuses;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Characters
{
    //[RequireComponent(typeof(Canvas))]
    public abstract class CharacterCanvas : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Transform statusIconRoot;
        [SerializeField] protected StatusIconsData statusIconData;
        [SerializeField] protected TextMeshProUGUI currentHealthText;


        protected Dictionary<StatusType, StatusIconBase> StatusDict = new Dictionary<StatusType, StatusIconBase>();
        protected Canvas TargetCanvas;

        protected TurnManager TurnManager => TurnManager.instance;



        public void InitCanvas()
        {
            for (int i = 0; i < Enum.GetNames(typeof(StatusType)).Length; i++)
                StatusDict.Add((StatusType)i, null);
        }

        public void ApplyStatus(StatusType targetStatus, int value)
        {
            if (StatusDict[targetStatus] == null)
            {
                var targetData = statusIconData.StatusIconList.FirstOrDefault(x => x.IconStatus == targetStatus);

                if (targetData == null) return;

                var clone = Instantiate(statusIconData.StatusIconBasePrefab, statusIconRoot);
                clone.SetStatus(targetData);
                StatusDict[targetStatus] = clone;
                Debug.Log("Status applied");
            }

            StatusDict[targetStatus].SetStatusValue(value);
        }

        public void ClearStatus(StatusType targetStatus)
        {
            if (StatusDict[targetStatus])
            {
                Destroy(StatusDict[targetStatus].gameObject);
            }

            StatusDict[targetStatus] = null;
        }

        public void UpdateStatusText(StatusType targetStatus, int value)
        {
            if (StatusDict[targetStatus] == null) return;

            StatusDict[targetStatus].StatusValueText.text = $"{value}";
        }
        private void Update()
        {
            if (TargetCanvas == null)
            {
                TargetCanvas = GetComponentInParent<Canvas>();
            }
        }
        public void UpdateHealthText(int currentHealth, int maxHealth) => currentHealthText.text = $"{currentHealth}/{maxHealth}";
    }
}