﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System.Linq;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(RIMLConfiguration))]
    public class RIMLConfigurationEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private RIMLConfiguration m_RIMLConfiguration;
        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_RIMLConfiguration = (target as RIMLConfiguration);
            nodeSpace = 380;
            NodeName = "MACHINE LEARNING SYSTEM";
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            NodeWidth = 300;
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_RIMLConfiguration.tooltips;
            m_BodyRect.height = 320;
            m_BodyRect.height = 320;
            base.OnBodyGUI();
            Debug.Log(HeaderRect.height);
            Debug.Log(m_PortRect.height);
            Debug.Log(m_BodyRect.height);
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon("Regression");
            ShowButtons(m_RIMLConfiguration);
        }

 
        protected override void TrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_RIMLConfiguration.Model != null && m_RIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_RIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                if (m_RIMLConfiguration.Trained)
                    nameButton = "Trained (" + numberOfExamplesTrained + " Examples)";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_RIMLConfiguration.Running || m_RIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    m_RIMLConfiguration.TrainModel();
                    numberOfExamplesTrained = m_RIMLConfiguration.TotalNumTrainingData;
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_RIMLConfiguration.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", m_NodeSkin.GetStyle("Train")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Error[0];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

        }

        protected override void RunModelButton()
        {
            bool enabled = false;
            if (m_RIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_RIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_RIMLConfiguration.Training || m_RIMLConfiguration.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                    
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                {
                    m_RIMLConfiguration.ToggleRunning();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Run", m_NodeSkin.GetStyle("Run")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_RIMLConfiguration.tooltips.BodyTooltip.Error[1];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

        }
    
    }
}

