using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Imagine.WebAR.Editor
{
	public class ImageWebARMenu
	{
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Rocks", false, 1010)]
		public static void CreateFreeImageTargetRock()
        {
			CreateFreeImageTarget("rocks");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Rocks", true)]
		static bool ValidateCreateImageTargetRocks()
		{
			return CanCreateTarget("rocks");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Corals", false, 1011)]
		public static void CreateFreeImageTargetCoral()
		{
			CreateFreeImageTarget("corals");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Corals", true)]
		static bool ValidateCreateImageTargetCorals()
		{
			return CanCreateTarget("corals");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Leaves", false, 1012)]
		public static void CreateFreeImageTargetLeaves()
		{
			CreateFreeImageTarget("leaves");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Leaves", true)]
		static bool ValidateCreateImageTargetLeaves()
		{
			return CanCreateTarget("leaves");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Nebula", false, 1013)]
		public static void CreateFreeImageTargetNebula()
		{
			CreateFreeImageTarget("nebula");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Nebula", true)]
		static bool ValidateCreateImageTargetNebula()
		{
			return CanCreateTarget("nebula");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/SeaShells", false, 1014)]
		public static void CreateFreeImageTargetSeaShells()
		{
			CreateFreeImageTarget("seashells");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/SeaShells", true)]
		static bool ValidateCreateImageTargetSeaShells()
		{
			return CanCreateTarget("seashells");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Tree", false, 1014)]
		public static void CreateFreeImageTargetTree()
		{
			CreateFreeImageTarget("tree");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Tree", true)]
		static bool ValidateCreateImageTargetTree()
		{
			return CanCreateTarget("tree");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Castle", false, 1014)]
		public static void CreateFreeImageTargetCastle()
		{
			CreateFreeImageTarget("castle");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Castle", true)]
		static bool ValidateCreateImageTargetCastle()
		{
			return CanCreateTarget("castle");
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Ladybugs", false, 1014)]
		public static void CreateFreeImageTargetLadybugs()
		{
			CreateFreeImageTarget("ladybugs");

		}
		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Ladybugs", true)]
		static bool ValidateCreateImageTargetLadybugs()
		{
			return CanCreateTarget("ladybugs");
		}

		static bool CanCreateTarget(string targetId)
        {

			var tracker = GameObject.FindObjectOfType<ImageTracker>();
			if (tracker == null)
            {
				//Debug.LogError("ImageTracker not found in scene");
				return false;
			}

			//Debug.Log("Tracker is found");

			var so = new SerializedObject(tracker);
			var sp = so.FindProperty("imageTargets");

			var dots = sp.propertyPath.Count(c => c == '.');
			foreach (SerializedProperty inner in sp)
			{
				if(targetId == inner.FindPropertyRelative("id").stringValue)
                {
					//Debug.LogError("ImageTarget " + targetId + " is already present in your scene");
					return false;
                }
			}

			return true;
		}

		public static void CreateFreeImageTarget(string targetId)
		{
			var tracker = GameObject.FindObjectOfType<ImageTracker>();
			if (tracker != null)
			{
				var prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Imagine/ImageTracker/Imagetargets/Prefabs/" + targetId + ".prefab");
				var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				Selection.activeObject = prefab;
				PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
				Selection.activeGameObject = go;
				go.name = targetId;

				var so = new SerializedObject(tracker);
				var sp = so.FindProperty("imageTargets");

				go.transform.position = new Vector3((sp.arraySize), 0, 0);
				go.transform.parent = tracker.transform;

				sp.arraySize++;
				so.ApplyModifiedProperties();

				var obj = sp.GetArrayElementAtIndex(sp.arraySize - 1);
				obj.FindPropertyRelative("id").stringValue = targetId;
				obj.FindPropertyRelative("transform").objectReferenceValue = go.transform;

				so.ApplyModifiedProperties();
			}
            else {
				EditorUtility.DisplayDialog("No ImageTracker in your scene", "Please create an ImageTracker before adding and Imagetarget", "Ok");
			}

		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Target/Create From Custom Image", false, 1050)]
		public static void CreateCustomImage()
		{
			if(EditorUtility.DisplayDialog("Upgrade Required", "Custom Imagetargets are not available in the Free version of this asset", "Upgrade", "Maybe Later"))
            {
				EditorApplication.delayCall = () =>
				{
					Application.OpenURL("https://assetstore.unity.com/packages/tools/camera/imagine-webar-image-tracker-240128");
				};

			}
		}



		[MenuItem("Assets/Imagine WebAR/Create/Image Tracker", false, 1100)]
		public static void CreateImageTracker()
		{
			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Imagine/ImageTracker/Prefabs/Tracker.prefab");
			GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			Selection.activeGameObject = gameObject;
			gameObject.name = "Tracker";
		}

		[MenuItem("Assets/Imagine WebAR/Create/Image Tracker Camera", false, 1101)]
		public static void CreateImageTrackerCamera()
		{
			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Imagine/ImageTracker/Prefabs/TrackerCamera.prefab");
			GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			Selection.activeGameObject = gameObject;
			gameObject.name = "TrackerCamera";
		}

		[MenuItem("Assets/Imagine WebAR/Update Plugin to URP", false, 1200)]
		public static void SetURP()
		{
			if (EditorUtility.DisplayDialog(
				"Update Imagine WebAR Plugin to URP",
				"Please make sure that the Universal RP package is already installed before doing this step.",
				"Proceed",
				"Cancel"))
			{
				string[] files = Directory.GetFiles(Application.dataPath + "/Imagine/ImageTracker/Demos/Materials", "*.mat", SearchOption.TopDirectoryOnly);
				foreach (var file in files)
				{
					var path = file.Replace(Application.dataPath, "Assets");
					var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
					if (mat.shader.name == "Standard")
					{
						mat.shader = Shader.Find("Universal Render Pipeline/Lit");
					}
					else if (mat.shader.name == "Imagine/ARShadow")
                    {
						mat.shader = Shader.Find("Imagine/ARShadowURP");
					}
				}

				AddDefineSymbol("IMAGINE_URP");
				EditorUtility.DisplayDialog("Completed", "Imagine WebAR Plugin is now set to URP. \n\nSome URP features such as HDR and Post-Processing may be partially/fully unsupported.", "Ok");
			}
		}


		[MenuItem("Assets/Imagine WebAR/Roll-back Plugin to Built-In RP", false, 1201)]
		public static void SetBuiltInRP ()
		{
			if (EditorUtility.DisplayDialog(
				"Roll-back Imagine WebAR Plugin to Built-In RP",
				"Plese confirm.",
				"Proceed",
				"Cancel"))
			{
				string[] files = Directory.GetFiles(Application.dataPath + "/Imagine/ImageTracker/Demos/Materials", "*.mat", SearchOption.TopDirectoryOnly);
				foreach (var file in files)
				{
					var path = file.Replace(Application.dataPath, "Assets");
					var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
					if (mat.shader.name == "Universal Render Pipeline/Lit" || mat.shader.name == "Hidden/InternalErrorShader")
					{
						mat.shader = Shader.Find("Standard");
					}
					else if (mat.shader.name == "Imagine/ARShadowURP")
					{
						mat.shader = Shader.Find("Imagine/ARShadow");
					}
				}

				RemoveDefineSymbol("IMAGINE_URP");

				EditorUtility.DisplayDialog("Completed", "Imagine WebAR Plugin is now set to Built-In RP. Some edited materials may still require manual shader change","Ok");

			}
		}

		public static void AddDefineSymbol(string symbol)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = definesString.Split(';').ToList();
			if (!allDefines.Contains(symbol))
				allDefines.Add(symbol);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				 EditorUserBuildSettings.selectedBuildTargetGroup,
				 string.Join(";", allDefines.ToArray()));
			AssetDatabase.RefreshSettings();
		}

		public static void RemoveDefineSymbol(string symbol)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = definesString.Split(';').ToList();
			allDefines.RemoveAll(s => s == symbol);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				 EditorUserBuildSettings.selectedBuildTargetGroup,
				 string.Join(";", allDefines.ToArray()));
			AssetDatabase.RefreshSettings();

		}
	}
}

