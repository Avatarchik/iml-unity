﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public abstract class BaseDataTypeNode<T> : Node, IFeatureIML
    {
        public string ValueName;

        // Data variables
        // Input
        public virtual T In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private T m_In;

        // Value itself contained in the node
        public virtual T Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private T m_Value;

        // Output
        public virtual T Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private T m_Out;

        // IMLFeature variables
        public abstract IMLBaseDataType FeatureValues { get; }

        /// <summary>
        /// Lets external classes know if they should call UpdateFeature (always true for a data type)
        /// </summary>
        bool IFeatureIML.isExternallyUpdatable { get { return true; } }

        /// <summary>
        /// Was tge featyre already updated?
        /// </summary>
        bool IFeatureIML.isUpdated { get ; set; }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Out = Value;
            return Out;
        }

        /// <summary>
        /// This update fetches the input value connected to this data type node
        /// </summary>
        /// <returns></returns>
        object IFeatureIML.UpdateFeature()
        {
            // Read input (if it returns default(T) there is no connection )
            var inputReceived = GetInputValue<T>("m_In");
            Debug.Log("Attempting to read input pin...");
            // Check if we have something connected to the input port
            if (!inputReceived.Equals(default(T)))
            {
                // Update the value of this data node
                Value = inputReceived;
                Debug.Log("Managed to read input pin...");

            }
            // Return entire node to satisfy IFeatureIML requirements
            return this;
        }
    }
}
