using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using Photon.Pun; // 1. Add Photon Namespace

namespace UnityStandardAssets.Characters.FirstPerson
{
    // 2. Inherit from MonoBehaviourPun
    public class HeadBob : MonoBehaviourPun
    {
        public Camera Camera;
        public CurveControlledBob motionBob = new CurveControlledBob();
        public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
        public RigidbodyFirstPersonController rigidbodyFirstPersonController;
        public float StrideInterval;
        [Range(0f, 1f)] public float RunningStrideLengthen;

        // private CameraRefocus m_CameraRefocus;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;

        private void Start()
        {
            // 3. THE FIX: The Gatekeeper
            // If this player belongs to someone else, we don't need to bob the camera.
            // (Their camera is disabled anyway, so let's save CPU).
            if (!photonView.IsMine)
            {
                enabled = false;
                return;
            }

            // Safety check: sometimes the camera isn't assigned in prefabs
            if (Camera == null) Camera = GetComponentInChildren<Camera>();

            motionBob.Setup(Camera, StrideInterval);
            m_OriginalCameraPosition = Camera.transform.localPosition;
            //   m_CameraRefocus = new CameraRefocus(Camera, transform.root.transform, Camera.transform.localPosition);
        }

        private void Update()
        {
            // Because we set 'enabled = false' in Start(), 
            // the code below ONLY runs for the Local Player.

            //  m_CameraRefocus.GetFocusPoint();
            Vector3 newCameraPosition;
            if (rigidbodyFirstPersonController.Velocity.magnitude > 0 && rigidbodyFirstPersonController.Grounded)
            {
                Camera.transform.localPosition = motionBob.DoHeadBob(rigidbodyFirstPersonController.Velocity.magnitude * (rigidbodyFirstPersonController.Running ? RunningStrideLengthen : 1f));
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = Camera.transform.localPosition.y - jumpAndLandingBob.Offset();
            }
            else
            {
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
            }
            Camera.transform.localPosition = newCameraPosition;

            if (!m_PreviouslyGrounded && rigidbodyFirstPersonController.Grounded)
            {
                StartCoroutine(jumpAndLandingBob.DoBobCycle());
            }

            m_PreviouslyGrounded = rigidbodyFirstPersonController.Grounded;
            //  m_CameraRefocus.SetFocusPoint();
        }
    }
}