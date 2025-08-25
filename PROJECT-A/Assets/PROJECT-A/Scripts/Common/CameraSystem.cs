using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TST
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance { get; private set; }

        //public bool IsActiveWorldCamera
        //{
        //    get => isActiveWorldCamera;
        //    set
        //    {
        //        isActiveWorldCamera = value;
        //        worldCamera.gameObject.SetActive(isActiveWorldCamera);
        //    }
        //}

        //public bool isActiveWorldCamera = false;

        //[field: SerializeField] public bool IsCameraZoom { get; set; } = false;
        //[field: SerializeField] public bool IsCameraSideOnRight { get; set; } = true;

        //public bool IsFpsMode
        //{
        //    get
        //    {
        //        return isFpsMode;
        //    }
        //    set
        //    {
        //        isFpsMode = value;
        //        TransitionToFpsCamera(isFpsMode);
        //    }
        //}

        private bool isFpsMode;
        public Camera mainCamera;


        public Vector2 cameraDistance = new Vector2(2f, 1.0f);

        //private Cinemachine3rdPersonFollow tpsCameraFollow;
        //private float blendCameraSide;
        //private float blendCameraDistance;

        //private CinemachineCameraOffset cameraCrouchOffset;
        Vector3 cameraoffSetTarget;

        //public void Initialize()
        //{
            
        //}

        //private void Awake()
        //{
        //    Instance = this;
        //    tpsCameraFollow = tpsCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        //    cameraCrouchOffset = tpsCamera.gameObject.GetComponent<CinemachineCameraOffset>();

        //    var cameraData = mainCamera.GetUniversalAdditionalCameraData();
        //    cameraData.cameraStack.Add(UIManager.Singleton.UICamera);
        //}

        //private void Update()
        //{
        //    blendCameraDistance = Mathf.Lerp(blendCameraDistance, IsCameraZoom ? cameraDistance.y : cameraDistance.x, Time.deltaTime * 10f);
        //    blendCameraSide = Mathf.Lerp(blendCameraSide, IsCameraSideOnRight ? 1 : 0, Time.deltaTime * 10f);

        //    tpsCameraFollow.CameraDistance = blendCameraDistance;
        //    tpsCameraFollow.CameraSide = blendCameraSide;

        //    // Ä«¸Þ¶ó offset
        //    cameraCrouchOffset.m_Offset = Vector3.Lerp(cameraCrouchOffset.m_Offset, cameraoffSetTarget, Time.deltaTime * 10.0f);
        //}
            
        //public void SetCrouchOffSet(Vector3 offSet)
        //{
        //    cameraoffSetTarget = offSet;
        //}

        //// Èì ¸¾¿¡ ¾Èµå´Âµ¥
        //public void TransitionToFpsCamera(bool isChange)
        //{
        //    if (isChange)
        //        fpsCamera.Priority = 11;

        //    if (isChange == false)
        //        fpsCamera.Priority = 9;
        //}
       
    }
}
