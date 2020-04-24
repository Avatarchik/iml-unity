﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(Vector3Node))]
    public class Vector3NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private Vector3Node m_Vector3Node;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_Vector3Node = (target as Vector3Node);

            // Initialise header background Rects
            InitHeaderRects();

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.Label("LIVE VECTOR3 DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(60));
        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowVector3NodePorts();

            // Draw Body Section
            DrawBodyLayout();
            ShowVector3Values();

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
            m_BodyRect.height = 140;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowVector3NodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Vector3\nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_Vector3Node.GetInputPort("m_In"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Vector3\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_Vector3Node.GetOutputPort("m_Out"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the serial value fields with attribute name
        /// </summary>
        private void ShowVector3Values()
        {
            GUILayout.BeginArea(m_BodyRect);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name: " + m_Vector3Node.ValueName, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[1], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[2], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));

            GUILayout.EndArea();

        }
    }
}

