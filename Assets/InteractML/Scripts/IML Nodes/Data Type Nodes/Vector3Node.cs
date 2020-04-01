﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeTint("#3A3B5B")]
    public class Vector3Node : Node
    {
        [SerializeField]
        public Vector3 Value;

        [Output]
        public Vector3 Vector3ToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            name = "VECTOR3";

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Vector3ToOutput = Value;
            return Vector3ToOutput;
        }
    }
}
