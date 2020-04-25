﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InteractML
{
    /// <summary>
    /// Parent Class UI. Handles communication between an IML Component and the in-game UI
    /// </summary>
    public class IMLUI<T>: MonoBehaviour
    {
        #region Variables
        [Header("Setup")]
        public IMLComponent MLComponent;
        [SerializeField]
        protected int m_NodeIndex;

        /// <summary>
        /// Current Node we are controlling
        /// </summary>
        protected T m_CurrentNode;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a reference to a training examples node by index
        /// </summary>
        /// <param name="which"></param>
        /// <returns></returns>
        public T GetNode(int which, List<T> nodeList)
        {
            if (nodeList.Count > 0)               
                return nodeList[which];
            else
                return default(T);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the text of a UI Element
        /// </summary>
        /// <param name="UIText"></param>
        /// <param name="text"></param>
        protected void UpdateUIText(TextMeshProUGUI UIText, string text)
        {
            if (UIText)
            {
                UIText.text = text;
            }
        }

        public virtual void MoveToNextNode(List<T> nodeList)
        {
            // Don't run if the list is null
            if (nodeList == null)
                return;

            // Increase by one the index
            m_NodeIndex++;

            // If the index is above boundaries, we just get the last one
            if (m_NodeIndex >= nodeList.Count)
                m_NodeIndex = nodeList.Count - 1;

            // Get the node
            m_CurrentNode = GetNode(m_NodeIndex, nodeList);
        }       

        public virtual void MoveToPreviousNode(List<T> nodeList)
        {
            // Don't run if the node list is nul
            if (nodeList == null)
                return;

            // Decrease by one the index
            m_NodeIndex--;

            // If the index is below boundaries, we just get the first one
            if (m_NodeIndex < 0)
                m_NodeIndex = 0;                            

            // Get the node
            m_CurrentNode = GetNode(m_NodeIndex, nodeList);

        }
        #endregion
    }

}
