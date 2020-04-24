﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVelocity))]
    public class ExtractVelocityNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVelocity m_ExtractVelocity;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractVelocity = (target as ExtractVelocity);

            // Initialise header background Rects
            InitHeaderRects();

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.Label("LIVE VELOCITY DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(60));
        }

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractVelocityNodePorts();

            // Draw the body
            DrawBodyLayout();
            ShowExtractedVelocityValues();

        }

        #region Methods

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60;
            GUI.DrawTexture(m_PortRect, NodeColor);

            // Draw line below ports
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            m_BodyRect.x = 5;
            m_BodyRect.y = HeaderRect.height + m_PortRect.height;
            m_BodyRect.width = NodeWidth - 10;
            m_BodyRect.height = 80;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowExtractVelocityNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live\nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractVelocity.GetInputPort("FeatureToInput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\nOut");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractVelocity.GetOutputPort("VelocityExtracted"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowExtractedVelocityValues()
        {
            GUILayout.BeginArea(m_BodyRect);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" velocity: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();


            GUILayout.EndArea();

        }

        #endregion
    }

}
