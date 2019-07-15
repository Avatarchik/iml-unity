﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(TrainingExamplesNode))]
    public class TrainingExamplesNodeEditor : NodeEditor
    {
        #region Variables

        private TrainingExamplesNode m_TrainingExamplesNode;
        private int m_RequiredOutputListSlots = 0;

        // Ints used to keep track of how many feautures existed in the node to display warnings
        private int m_LastKnownNumInputFeatures;
        private int m_LastKnownNumOutputFeatures;

        /// <summary>
        /// Flag that will contorl the training examples dropdown
        /// </summary>
        private bool m_ShowTrainingExamplesDropdown;

        #endregion

        #region Unity Messages

        public override void OnBodyGUI()
        {
            // Override label width for this node (since some names are quite long)
            LabelWidth = 184;

            base.OnBodyGUI();

            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);

            // TOTAL NUMBER OF TRAINING EXAMPLES
            ShowTotalNumberOfTrainingExamples();

            // DESIRED OUTPUT CONFIG AND BUILD OUTPUT LIST
            EditorGUILayout.Space();
            ShowDesiredOutputFeaturesLogic();
            EditorGUILayout.Space();

            // RECORD EXAMPLES BUTTON 
            ShowRecordExamplesButton();

            // CLEAR ALL TRAINING EXAMPLES BUTTON
            ShowClearAllExamplesButton();

            // WARNINGS IF FEATURES CHANGE
            CheckInputFeaturesChanges();
            CheckOutputFeaturesChanges();

            // TRAINING EXAMPLES DROPDOWN
            ShowTrainingExamplesDropdown();

            // Error with output configuration
            var nodeTraining = target as TrainingExamplesNode;
            if (nodeTraining.DesiredOutputsConfig.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no Desired Outputs Configured!", MessageType.Error);
            }
        }

        #endregion

        #region Private Methods

        private void ShowTotalNumberOfTrainingExamples()
        {
            string numberExamplesText = m_TrainingExamplesNode.TotalNumberOfTrainingExamples.ToString();
            EditorGUILayout.LabelField("Total No. Training Examples: ", numberExamplesText);

            EditorGUILayout.LabelField("Total No. Desired Outputs: ", m_TrainingExamplesNode.DesiredOutputFeatures.Count.ToString());

        }

        private void ShowRecordExamplesButton()
        {
            
            // Only run button logic when there are features to extract from
            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.InputFeatures))
            {
                string nameButton = "";

                if (m_TrainingExamplesNode.CollectingData )
                    nameButton = "STOP Recording Examples";
                else
                    nameButton = "Record Examples";

                bool disableButton = false;

                // If there are any models connected we check some conditions
                if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    for (int i = 0; i < m_TrainingExamplesNode.IMLConfigurationNodesConnected.Count; i++)
                    {
                        var IMLConfigNodeConnected = m_TrainingExamplesNode.IMLConfigurationNodesConnected[i];
                        // Disable button if model(s) connected are runnning or training
                        if (IMLConfigNodeConnected.Running || IMLConfigNodeConnected.Training)
                        { 
                            disableButton = true;
                            break;
                        }

                    }
                }

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                // Always enable it back at the end
                GUI.enabled = true;


            }
            // If there are no features to extract from we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Record Examples"))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;

            }

        }

        private void ShowClearAllExamplesButton()
        {
            string nameButton = "Delete All Training Examples";

            // Only run button logic when there are training examples to delete
            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
            {
                bool disableButton = false;

                // If there are any models connected we check some conditions
                if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    foreach (var IMLConfigNode in m_TrainingExamplesNode.IMLConfigurationNodesConnected)
                    {
                        // Disable button if any of the models is runnning OR collecting data OR training
                        if (IMLConfigNode.Running || IMLConfigNode.Training || m_TrainingExamplesNode.CollectingData)
                        {
                            disableButton = true;
                            break;
                        }
                    }
                }

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ClearTrainingExamples();
                }
                // Always enable it back at the end
                GUI.enabled = true;



            }
            // If there are no training examples to delete we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }
        }

        private void ShowDesiredOutputFeaturesLogic ()
        {
            // SHOW DESIRED OUTPUT CONFIG AND BUILD OUTPUT LIST
            EditorGUILayout.LabelField("With Desired Output: ");
            // Go through the list of desired outputs and show the correct kind of config editor tool
            for (int i = 0; i < m_TrainingExamplesNode.DesiredOutputFeatures.Count; i++)
            {
                string labelOutput = "Output " + i.ToString() + ": ";
                var outputFeature = m_TrainingExamplesNode.DesiredOutputFeatures[i];
                // We make sure that the desired output feature list captures the value inputted by the user
                switch (outputFeature.DataType)
                {
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Float:
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLFloat).SetValue(EditorGUILayout.FloatField(labelOutput, m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0]) );
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Integer:
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLInteger).SetValue(EditorGUILayout.IntField(labelOutput, (int)m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector2:
                        var vector2ToShow = new Vector2(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1]);
                        var valueVector2 = EditorGUILayout.Vector2Field(labelOutput, vector2ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector2).SetValues(valueVector2);
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector3:
                        var vector3ToShow = new Vector3(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[2]);
                        var valueVector3 = EditorGUILayout.Vector3Field(labelOutput, vector3ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector3).SetValues(valueVector3);
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector4:
                        var vector4ToShow = new Vector4(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[2],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[3]);
                        var valueVector4 = EditorGUILayout.Vector4Field(labelOutput, vector4ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector4).SetValues(valueVector4);
                        break;
                    default:
                        break;
                }
            }


        }

        /// <summary>
        /// Checks if the num of input features differ from what we last know to show warnings
        /// </summary>
        private void CheckInputFeaturesChanges()
        {
            // Get current number of input features
            TrainingExamplesNode trainingExamplesNode = target as TrainingExamplesNode;
            int currentNumFeatures = trainingExamplesNode.DesiredInputsConfig.Count;

            // Show debug info
            //EditorGUILayout.LabelField("LastKnownInputFeatures: " + m_LastKnownNumInputFeatures);

            // Style for warning box
            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(24, 10, 10, 10);
            tableStyle.margin.left = 10;


            // If we have no knowledge of input features
            if (m_LastKnownNumInputFeatures == 0)
            {
                m_LastKnownNumInputFeatures = currentNumFeatures;
            }
            // Warning for feature removed
            else if (currentNumFeatures < m_LastKnownNumInputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Deleting a Input/Output Feature deletes all the data recorded with it in all the Examples in this node!", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumInputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();

            }
            // Warning for feature added
            else if (currentNumFeatures > m_LastKnownNumInputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Adding a new Input Feature deletes all the previous Examples recorded in this node.", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumInputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();
            }
            // If there is no change performed
            else
            {
                // Update value of lasknown input features
                m_LastKnownNumInputFeatures = currentNumFeatures;
            }

        }

        /// <summary>
        /// Checks if the num of output features differ from what we last know to show warnings
        /// </summary>
        private void CheckOutputFeaturesChanges()
        {
            // Get current number of input features
            TrainingExamplesNode trainingExamplesNode = target as TrainingExamplesNode;
            int currentNumFeatures = trainingExamplesNode.DesiredOutputsConfig.Count;

            // Show debug info
            //EditorGUILayout.LabelField("LastKnownOutputFeatures: " + m_LastKnownNumOutputFeatures);

            // Style for warning box
            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(24, 10, 10, 10);
            tableStyle.margin.left = 10;


            // If we have no knowledge of output features
            if (m_LastKnownNumOutputFeatures == 0)
            {
                m_LastKnownNumOutputFeatures = currentNumFeatures;
            }
            // Warning for feature removed
            else if (currentNumFeatures < m_LastKnownNumOutputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Deleting a Input/Output Feature deletes all the data recorded with it in all the Examples in this node!", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumOutputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();

            }
            // Warning for feature added
            else if (currentNumFeatures > m_LastKnownNumOutputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Adding a new Output Feature deletes all the previous Examples recorded in this node.", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumOutputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();
            }
            // If there is no change performed
            else
            {
                // Update value of lasknown input features
                m_LastKnownNumOutputFeatures = currentNumFeatures;
            }

        }

        /// <summary>
        /// Shows a dropdown with the training examples
        /// </summary>
        private void ShowTrainingExamplesDropdown()
        {
            EditorGUILayout.Space();

            m_ShowTrainingExamplesDropdown = EditorGUILayout.Foldout(m_ShowTrainingExamplesDropdown, "Inspect Training Examples ");
            if (m_ShowTrainingExamplesDropdown)
            {
                EditorGUI.indentLevel++;

                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
                {
                    EditorGUILayout.LabelField("Training Examples List is empty");
                }
                else
                {
                    for (int i = 0; i < m_TrainingExamplesNode.TrainingExamplesVector.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Example " + i);

                        EditorGUI.indentLevel++;

                        var inputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Inputs;
                        var outputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Outputs;

                        // If the input features are not null...
                        if (inputFeatures != null)
                        {
                            // Draw inputs
                            for (int j = 0; j < inputFeatures.Count; j++)
                            {

                                if (inputFeatures[j].InputData == null )
                                {
                                    EditorGUILayout.LabelField("Inputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input " + j + " (" + inputFeatures[j].InputData.DataType + ")");

                                for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                {
                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString());

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Inputs are null ");
                        }

                        // If the output features are not null...
                        if (outputFeatures != null)
                        {
                            // Draw outputs
                            for (int j = 0; j < outputFeatures.Count; j++)
                            {
                                if (outputFeatures[j].OutputData == null)
                                {
                                    EditorGUILayout.LabelField("Outputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Output " + j+ " (" + outputFeatures[j].OutputData.DataType + ")");


                                for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                {

                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString());

                                    EditorGUI.indentLevel--;

                                }

                                EditorGUI.indentLevel--;

                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Outputs are null ");
                        }

                        EditorGUI.indentLevel--;

                    }


                }

                EditorGUI.indentLevel--;
            }
        }

        #endregion

    }

}