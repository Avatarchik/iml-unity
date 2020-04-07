﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractFloat))]
    public class ExtractFloatNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractFloat m_ExtractFloat;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractFloat = (target as ExtractFloat);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            base.OnBodyGUI();
            EditorStyles.label.normal = editorLabelStyle.normal;

        }
    }

}
