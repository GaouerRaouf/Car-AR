using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Imagine.WebAR
{
    [RequireComponent(typeof(Camera))]
    public class ImageTrackerCamera : MonoBehaviour
    {
        [DllImport("__Internal")] private static extern void UnpauseWebGLCamera();
        [DllImport("__Internal")] private static extern void PauseWebGLCamera();
        [DllImport("__Internal")] private static extern string GetWebGLCameraFrame(string typeStr);
        [DllImport("__Internal")] private static extern string GetWebGLCameraName();


        [HideInInspector] public Camera cam;

        [Header("*Experimental")]
        [SerializeField] string editorCamera = "";
        [SerializeField] bool paused = false;

        public enum VideoPlaneMode {
            NONE,
            EXPERIMENTAL_WEBCAMTEXTURE,
            EXPERIMENTAL_DATAURLTEXTURE
        }

        [SerializeField] public VideoPlaneMode videoPlaneMode = VideoPlaneMode.NONE;
        [SerializeField] float videoDistance = 100;
        GameObject videoBackground;
        public bool webcamTextureInitializing = false;
        WebCamTexture webcamTexture;

        Texture2D dataUrlTexture;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Start()
        {
            if (videoPlaneMode == VideoPlaneMode.EXPERIMENTAL_WEBCAMTEXTURE)
            {
                InitWebcam();
            }

            else if (videoPlaneMode == VideoPlaneMode.EXPERIMENTAL_DATAURLTEXTURE)
            {
                InitVideoPlane();
            }
        }

        void InitWebcam()
        {
            StartCoroutine(InitializeWebcamRoutine());
        }

        IEnumerator InitializeWebcamRoutine()
        {
            Debug.Log("Init Webcam");

            var devices = WebCamTexture.devices;
            for (var i = 0; i < devices.Length; i++)
            {
                Debug.Log(devices[i].name);
#if !UNITY_EDITOR                
                if (!devices[i].isFrontFacing)
                {
                    editorCamera = devices[i].name;
                    Debug.Log("Selected " + editorCamera);

                    break;
                }
#endif
            }

#if !UNITY_EDITOR
            var camName = GetWebGLCameraName();
            if (!string.IsNullOrEmpty(camName))
            {
                Debug.Log("Got camName " + camName);
                editorCamera = camName;
            }
#endif

            videoBackground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            videoBackground.name = "VideoBackground";
            videoBackground.transform.parent = transform;
            videoBackground.transform.localPosition = new Vector3(0, 0, -1000);
            //videoBackground.transform.localEulerAngles = new Vector3(30, 0, 0);

            

            yield return new WaitForEndOfFrame();

            bool success = true;
            webcamTextureInitializing = true;

            if (!string.IsNullOrEmpty(editorCamera))
            {
                Debug.Log("Starting " + editorCamera);
                webcamTexture = new WebCamTexture(editorCamera);
            }

            else
            {
                Debug.LogWarning("No back facing camera found");
                webcamTexture = new WebCamTexture();
            }

            webcamTexture.Play();

            float startTime = Time.time;
            float timeoutDuration = 10;
            while (webcamTexture.width <= 16)
            {
                Debug.Log("Waiting for " + editorCamera);
                yield return new WaitForEndOfFrame();
                if (Time.time - startTime > timeoutDuration)
                {
                    success = false; break;
                }
            }

            webcamTextureInitializing = false;

            Debug.Log("Completed...");


            if (success)
            {
                Debug.Log("Webcam Texture Initialized");
                var material = new Material(Shader.Find("Unlit/Texture"));
                material.mainTexture = webcamTexture;
                videoBackground.GetComponent<Renderer>().material = material;

                SetVideoDimensions();

                videoBackground.transform.localPosition = new Vector3(0, 0, videoDistance);
            }

            else
            {
                Debug.LogError("Webcam Texture Initialization Failed");
                Destroy(videoBackground);
            }

        }

        void SetVideoDimensions()
        {
            if (videoBackground && webcamTexture)
            {
                var v_ar = (float)webcamTexture.width / webcamTexture.height; var ar = (float)Screen.width / (float)Screen.height;
                float height = 2 * videoDistance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2); float width = height * ar;

                if (v_ar > ar) //bleed horizontally
                {
                    Debug.Log("bleed horizontal");
                    videoBackground.transform.localScale = new Vector3(height * v_ar, height, 1);
                } 

                else //bleed vertically
                {
                    Debug.Log("bleed vertical"); 
                    videoBackground.transform.localScale = new Vector3(width, width / v_ar, 1);
                }
            }

        }

        void InitVideoPlane()
        {
            Debug.Log("Init Video Plane");

            videoBackground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            videoBackground.name = "VideoBackground";
            videoBackground.transform.parent = transform;
            //videoBackground.transform.localPosition = new Vector3(0, 0, -1000);
            //videoBackground.transform.localEulerAngles = new Vector3(45, 0, 0);

            var material = new Material(Shader.Find("Unlit/Texture"));
            material.mainTexture = dataUrlTexture;
            videoBackground.GetComponent<Renderer>().material = material;

            SetVideoDimensions(videoWidth, videoHeight);

            videoBackground.transform.localPosition = new Vector3(0, 0, videoDistance);
        }

        public void UnpauseCamera()
        {
            if (videoPlaneMode == VideoPlaneMode.EXPERIMENTAL_WEBCAMTEXTURE && webcamTexture != null)
            {
                webcamTexture.Play();
            }
            else
            {
                UnpauseWebGLCamera();
            }

            paused = false;
        }

        public void PauseCamera()
        {
            if (videoPlaneMode == VideoPlaneMode.EXPERIMENTAL_WEBCAMTEXTURE && webcamTexture != null)
            {
                webcamTexture.Pause();
            }
            else
            {
                PauseWebGLCamera();
            }

            paused = true;
        }

        private void Update()
        {
            if( videoPlaneMode == VideoPlaneMode.EXPERIMENTAL_DATAURLTEXTURE &&
                videoBackground != null &&
                videoWidth > 1 &&
                videoHeight > 1 &&
                !paused)
            {
                GetCameraFrame();
            }
        }

        void GetCameraFrame()
        {
            var dataUrlStr = GetWebGLCameraFrame("image/jpeg");
            dataUrlStr = dataUrlStr.Replace("data:image/jpeg;base64,", "");
            dataUrlTexture.LoadImage(System.Convert.FromBase64String(dataUrlStr));
            dataUrlTexture.Apply();
        }

        private int videoWidth = 1, videoHeight = 1;
        public void SetVideoDimensions(int w, int h)
        {
            Debug.Log("Set Video Dimensions " + w + ", " + h);

            videoWidth = w;
            videoHeight = h;

            if (videoBackground)
            {
                dataUrlTexture = new Texture2D(w, h);
                videoBackground.GetComponent<Renderer>().material.mainTexture = dataUrlTexture;

                var v_ar = (float) w / h;
                var ar = (float)Screen.width / (float)Screen.height;
                float height = 2 * videoDistance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2);
                float width = height * ar;

                if (v_ar > ar) //bleed horizontally
                {
                    Debug.Log("bleed horizontal");
                    videoBackground.transform.localScale = new Vector3(height * v_ar, height, 1);
                }

                else //bleed vertically
                {
                    Debug.Log("bleed vertical");
                    videoBackground.transform.localScale = new Vector3(width, width / v_ar, 1);
                }
            }

        }

    }
}