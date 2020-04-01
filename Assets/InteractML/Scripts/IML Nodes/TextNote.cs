﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    [NodeTint("#3A3B5B")]
    public class TextNote : Node
    {
        public string note;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            name = "TEXT NOTE";


        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteTextNoteNode(this);
            }
        }
    }
}
