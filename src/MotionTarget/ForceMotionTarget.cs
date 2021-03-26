﻿using DebugUtils;
using Leap.Unity;
using Leap.Unity.Infix;
using System.Collections.Generic;
using System.Linq;
using BusDriver.UI;
using BusDriver.Utils;
using UnityEngine;

namespace BusDriver.MotionTarget
{
    public class ForceMotionTarget : AbstractMotionTarget
    {
        private Rigidbody _target;
        private Vector3 _originPosition;
        private Quaternion _originRotation;

        private JSONStorableStringChooser TargetChooser;
        private UIDynamicButton CaptureOriginButton;

        protected override void CreateCustomUI(IUIBuilder builder)
        {
            TargetChooser = builder.CreateScrollablePopup("MotionSource:Force:Target", "Select Target", null, null, TargetChooserCallback, true);
            CaptureOriginButton = builder.CreateButton("Capture Origin", CaptureOriginCallback, true);

            FindAtoms();
        }

        public override void DestroyUI(IUIBuilder builder)
        {
            base.DestroyUI(builder);

            builder.Destroy(TargetChooser);
            builder.Destroy(CaptureOriginButton);
        }

        public override void Apply(Vector3 offset, Quaternion rotation)
        {
            if (_target == null)
                return;

            var sourceRotation = _originRotation * rotation;
            var sourcePosition = _originPosition + _originRotation * offset;

            _target.AddForce((sourcePosition - _target.position) * _target.mass / Time.fixedDeltaTime, ForceMode.Impulse);

            ApplyTorque(_target.transform.up, sourceRotation.GetUp());
            ApplyTorque(_target.transform.right, sourceRotation.GetRight());
            ApplyTorque(_target.transform.forward, sourceRotation.GetForward());

            DebugDraw.DrawTransform(_target.transform, 3);
            DebugDraw.DrawTransform(sourcePosition, sourceRotation.GetUp(), sourceRotation.GetRight(), sourceRotation.GetForward(), 3);
        }

        private void ApplyTorque(Vector3 from, Vector3 to)
        {
            var axis = Vector3.Cross(from.normalized, to.normalized);
            var angle = Mathf.Asin(axis.magnitude);

            var angularVelocityDelta = axis.normalized * angle / Time.fixedDeltaTime;
            var intertiaRotation = _target.transform.rotation * _target.inertiaTensorRotation;
            var torque = intertiaRotation * Vector3.Scale(_target.inertiaTensor, Quaternion.Inverse(intertiaRotation) * angularVelocityDelta);
            _target.AddTorque(torque, ForceMode.Impulse);
        }

        private void CaptureOriginCallback()
        {
            if (_target == null)
                return;

            _originPosition = _target.transform.position;
            _originRotation = _target.transform.rotation;
        }

        protected override void AtomChooserCallback(string s)
        {
            base.AtomChooserCallback(s);
            FindTargets();
        }

        private void FindTargets()
        {
            if (Atom == null)
            {
                TargetChooser.choices = new List<string>() { "None" };
                TargetChooserCallback(null);
                return;
            }

            var targets = Atom.forceReceivers.Select(c => c.name).ToList();
            var defaultTarget = "None";
            targets.Insert(0, "None");

            TargetChooser.choices = targets;
            TargetChooserCallback(defaultTarget);
        }

        protected void TargetChooserCallback(string s)
        {
            _target = Atom?.forceReceivers?.FirstOrDefault(c => string.Equals(s, c.name, System.StringComparison.OrdinalIgnoreCase))?.GetComponent<Rigidbody>();
            CaptureOriginCallback();

            TargetChooser.valNoCallback = _target == null ? "None" : s;
        }
    }
}
