﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(IntegerNode))]
    public class IntegerNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private IntegerNode m_IntegerNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;
        private Rect m_InnerBodyRect;
        private Rect m_HelpRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_IntegerNode = (target as IntegerNode);

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));
            
            //Display Node name
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Space(10);
            GUILayout.Label("LIVE INTEGER DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowIntegerNodePorts();

            // Draw Body Section
            DrawBodyLayout();
            ShowIntegerValue();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton();
        }

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
            m_BodyRect.height = 90;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_HelpRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowIntegerNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Integer\nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_IntegerNode.GetInputPort("m_In"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Integer\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_IntegerNode.GetOutputPort("m_Out"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the integer value fields with attribute name
        /// </summary>
        private void ShowIntegerValue()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name: " + m_IntegerNode.ValueName, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Integer: " + System.Math.Round(m_IntegerNode.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndArea();

        }

        /// <summary>
        /// Display help button
        /// </summary>
        private void ShowHelpButton()
        {
            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 30;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Button("Help", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

