using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if IMAGINE_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#endif
using System.Runtime.InteropServices;


namespace Imagine.WebAR
{
    [System.Serializable]
    public class ImageTarget
    {
        public string id;
        public Transform transform;
    }

    public class ImageTracker : MonoBehaviour
    {
        [DllImport("__Internal")] private static extern void StartWebGLTracker(string ids, string name);
        [DllImport("__Internal")] private static extern void StopWebGLTracker();
        [DllImport("__Internal")] private static extern float SetWebGLTrackerSettings(string settings);
        [DllImport("__Internal")] private static extern bool IsWebGLTrackerReady();
        [DllImport("__Internal")] private static extern float GetTrackerFov();
        [DllImport("__Internal")] private static extern float DebugImageTarget(string id);
        [DllImport("__Internal")] private static extern bool IsWebGLImageTracked(string id);
        

        [SerializeField] private ImageTrackerCamera trackerCam;
        [SerializeField] private List<ImageTarget> imageTargets;
        private Dictionary<string, ImageTarget> targets = new Dictionary<string, ImageTarget>();

        private enum TrackerOrigin { CAMERA_ORIGIN, FIRST_TARGET_ORIGIN }
        [SerializeField] private TrackerOrigin trackerOrigin;
        [SerializeField] private List<string> trackedIds = new List<string>();
        private string serializedIds = "";

        [SerializeField] private bool overrideTrackerSettings = false;
        [SerializeField] private TrackerSettings trackerSettings;

        [SerializeField] private UnityEvent<string> OnImageFound, OnImageLost;

        [SerializeField] [Range(.025f, 0.100f)] private float debugCamMoveSensitivity = 0.1f;
        [SerializeField] [Range(0.5f, 1.0f)] private float debugCamTiltSensitivity = 1f;

        IEnumerator Start()
        {
            if(transform.parent != null) {
                Debug.LogError("ImageTracker should be a root transform to receive Javascript messages");
            }

            if(trackerCam == null)
            {
                trackerCam = GameObject.FindObjectOfType<ImageTrackerCamera>();
            }

            foreach (var i in imageTargets)
            {
                targets.Add(i.id, i);
                i.transform.GetComponent<Renderer>().enabled = false;
                i.transform.gameObject.SetActive(false);

                serializedIds += i.id;
                if (i != imageTargets[imageTargets.Count - 1])
                {
                    serializedIds += ",";
                }
            }
            Debug.Log(serializedIds);


            Application.targetFrameRate = overrideTrackerSettings ?
                (int)this.trackerSettings.targetFrameRate :
                (int)ImageTrackerGlobalSettings.Instance.defaultTrackerSettings.targetFrameRate;
            
#if IMAGINE_URP
            //Debug.Log(GraphicsSettings.defaultRenderPipeline.GetType());
            if (GraphicsSettings.currentRenderPipeline != null &&
                 GraphicsSettings.defaultRenderPipeline.GetType().ToString().EndsWith("UniversalRenderPipelineAsset") &&
                 trackerCam.videoPlaneMode == ImageTrackerCamera.VideoPlaneMode.NONE
                 )
            {
                Debug.Log("URP detected");
                trackerCam.cam.clearFlags = CameraClearFlags.Depth;
                trackerCam.cam.allowHDR = false;
                var camData = trackerCam.GetComponent<UniversalAdditionalCameraData>();
                camData.renderPostProcessing = false;

                Debug.Log(trackerCam.cam.clearFlags + " " + camData.renderPostProcessing);
            }
#endif

#if !UNITY_EDITOR && UNITY_WEBGL
            while (!IsWebGLTrackerReady())
            {
                yield return new WaitForSeconds(0.1f);
            }

            StartWebGLTracker(serializedIds, name);
            if (overrideTrackerSettings) {
                Debug.Log(trackerSettings.Serialize());
                SetWebGLTrackerSettings(trackerSettings.Serialize());
            }
            trackerCam.cam.fieldOfView = GetTrackerFov();
#endif

            yield break;
        }

        private void OnDestroy()
        {


#if !UNITY_EDITOR && UNITY_WEBGL
            if (IsWebGLTrackerReady())
            {
                StopWebGLTracker();
            }

            if (overrideTrackerSettings)
            {
                //reset settings
                var settings = ImageTrackerGlobalSettings.Instance.defaultTrackerSettings;
                SetWebGLTrackerSettings(settings.Serialize());
            }

#endif
        }

        public void StartTracker()
        {
            if (IsWebGLTrackerReady())
            {
                StartWebGLTracker(serializedIds, name);
            }
        }

        public void StopTracker()
        {
            if (IsWebGLTrackerReady())
            {
                StopWebGLTracker();
            }
        }

        public bool IsImageTracked(string id)
        {
            return IsWebGLImageTracked(id);
        }

        void SetCameraFov(float fov)
        {
            trackerCam.cam.fieldOfView = fov;
        }

        void OnTrackingFound(string id)
        {
            if (!targets.ContainsKey(id))
                return;

            targets[id].transform.gameObject.SetActive(true);
            
            if(!trackedIds.Contains(id))
                trackedIds.Add(id);
            else
                Debug.LogError("Found an already tracked id - " + id);

            OnImageFound?.Invoke(id);
        }

        void OnTrackingLost(string id)
        {
            if (!targets.ContainsKey(id))
                return;

            targets[id].transform.gameObject.SetActive(false);

            var index = trackedIds.FindIndex(t => t == id);
            if (index > -1)
            {
                trackedIds.RemoveAt(index);
            }
            else{
                Debug.LogError("Lost an untracked id - " + id);
            }


            //HACK:clear to fix ghosting issue
            //cam.clearFlags = CameraClearFlags.SolidColor;
            //cam.Render();
            //cam.clearFlags = CameraClearFlags.Depth;
            //cam.Render();

            //GL.Clear(true, true, Color.black);
            //cam.Render();

            OnImageLost?.Invoke(id);
        }

        void OnTrack(string data)
        {
            ParseData(data);
        }

        void ParseData(string data)
        {
            string[] values = data.Split(new char[] { ',' });

            string id = values[0];
            if (!targets.ContainsKey(id))
                return;

            Vector3 forward;
            forward.x = float.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture);
            forward.y = float.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture);
            forward.z = float.Parse(values[6], System.Globalization.CultureInfo.InvariantCulture);

            Vector3 up;
            up.x = float.Parse(values[7], System.Globalization.CultureInfo.InvariantCulture);
            up.y = float.Parse(values[8], System.Globalization.CultureInfo.InvariantCulture);
            up.z = float.Parse(values[9], System.Globalization.CultureInfo.InvariantCulture);

            Vector3 right;
            right.x = float.Parse(values[10], System.Globalization.CultureInfo.InvariantCulture);
            right.y = float.Parse(values[11], System.Globalization.CultureInfo.InvariantCulture);
            right.z = float.Parse(values[12], System.Globalization.CultureInfo.InvariantCulture);

            var rot = Quaternion.LookRotation(forward, up);


            Vector3 pos;
            pos.x = float.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
            pos.y = float.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
            pos.z = float.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);

            var target = targets[id].transform;

            if (trackerOrigin == TrackerOrigin.CAMERA_ORIGIN)
            {
                target.position = pos;
                target.rotation = rot;
            }

            else if (trackerOrigin == TrackerOrigin.FIRST_TARGET_ORIGIN)
            {
                if (trackedIds[0] == id)
                {
                    //first target in origin
                    target.position = Vector3.zero;
                    target.rotation = Quaternion.identity;

                    trackerCam.transform.position = Quaternion.Inverse(rot) * -pos;
                    trackerCam.transform.rotation = Quaternion.Inverse(rot);
                }
                else
                {
                    //succeeding targets relative to camera
                    target.position = trackerCam.transform.TransformPoint(pos);
                    target.rotation = trackerCam.transform.rotation * rot;
                }


            }

        }

        void ResizeVideo(string dims)
        {
            var vals = dims.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            var width = int.Parse(vals[0]);
            var height = int.Parse(vals[1]);
            trackerCam.SetVideoDimensions(width, height);
        }

        private int debugImageTargetIndex = 0;
        private void Update()
        {
            if (trackerSettings.debugMode)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    if(debugImageTargetIndex >= imageTargets.Count)
                    {
                        debugImageTargetIndex = 0;
                        DebugImageTarget("");
                    }
                    else
                    {
                        DebugImageTarget(imageTargets[debugImageTargetIndex].id);
                        debugImageTargetIndex++;
                    }
                }
            }
#if UNITY_EDITOR
            Update_Debug();
#endif
        }

        private void Update_Debug()
        {
            var x_left = Input.GetKey(KeyCode.A);
            var x_right = Input.GetKey(KeyCode.D);
            var z_forward = Input.GetKey(KeyCode.W); ;
            var z_back = Input.GetKey(KeyCode.S); ;
            var y_up = Input.GetKey(KeyCode.R);
            var y_down = Input.GetKey(KeyCode.F);

            float speed = debugCamMoveSensitivity;
            float dx = (x_right ? speed : 0) + (x_left ? -speed : 0);
            float dy = (y_up ? speed : 0) + (y_down ? -speed : 0);
            //float dsca = 1 + (z_forward ? speed : 0) + (z_back ? -speed : 0);
            float dz = (z_forward ? speed : 0) + (z_back ? -speed : 0);


            var y_rot_left = Input.GetKey(KeyCode.LeftArrow);
            var y_rot_right = Input.GetKey(KeyCode.RightArrow);
            var x_rot_up = Input.GetKey(KeyCode.UpArrow);
            var x_rot_down = Input.GetKey(KeyCode.DownArrow);
            var z_rot_cw = Input.GetKey(KeyCode.Comma);
            var z_rot_ccw = Input.GetKey(KeyCode.Period);

            var angularSpeed = debugCamTiltSensitivity; //degrees per frame
            var d_rotx = (x_rot_up ? angularSpeed : 0) + (x_rot_down ? -angularSpeed : 0);
            var d_roty = (y_rot_right ? angularSpeed : 0) + (y_rot_left ? -angularSpeed : 0);
            var d_rotz = (z_rot_ccw ? angularSpeed : 0) + (z_rot_cw ? -angularSpeed : 0);

            var w = trackerCam.transform.rotation.w;
            var i = trackerCam.transform.rotation.x;
            var j = trackerCam.transform.rotation.y;
            var k = trackerCam.transform.rotation.z;

            var rot = new Quaternion(i, j, k, w);

            var dq = Quaternion.Euler(d_rotx, d_roty, d_rotz);
            rot *= dq;
            //rot *= Quaternion.AngleAxis(d_rotz, trackerCamera.transform.forward);
            //rot *= Quaternion.AngleAxis(d_roty, trackerCamera.transform.up);
            //rot *= Quaternion.AngleAxis(d_rotx, trackerCamera.transform.right);

            //Debug.Log(dx + "," + dy + "," + dsca);
            var dp = Vector3.right * dx + Vector3.up * dy + Vector3.forward * dz;
            trackerCam.transform.Translate(dp);
            trackerCam.transform.rotation = rot;
        }
    }
}

