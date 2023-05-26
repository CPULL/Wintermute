using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace OccaSoftware.Altos.Runtime
{
	[AddComponentMenu("Altos/Altos Sky Director")]
	[ExecuteAlways]
	public class AltosSkyDirector : MonoBehaviour
	{
		private static AltosSkyDirector instance;
		public static AltosSkyDirector Instance { get { return instance; } }


        [Header("Definitions")]
        //[Reload("Definitions/Skybox Definition.asset")]
		public SkyDefinition skyDefinition;

        //[Reload("Definitions/Atmosphere Definition.asset")]
        public AtmosphereDefinition atmosphereDefinition;

        //[Reload("Definitions/Star Definition.asset")]
        public StarDefinition starDefinition;

        //[Reload("Definitions/Cloud Definition.asset")]
        public CloudDefinition cloudDefinition;


        [Header("Resources")]
        [Reload("Scripts/Runtime/Data/AltosDataAsset.asset")]
        public AltosData data;


		private const float maxDistance = 10f;
		public static float _HOURS_TO_DEGREES = 15f;
		private bool isSetupCorrect = false;


		private List<SkyObject> _SkyObjects = new List<SkyObject>();
		public List<SkyObject> SkyObjects
		{
			get => _SkyObjects;
		}

		private List<SkyObject> _Sun = new List<SkyObject>();
		public SkyObject Sun
		{
			get => _Sun.Count > 0 ? _Sun[0] : null;
		}

		internal void RegisterSkyObject(SkyObject skyObject)
		{
			if (!_SkyObjects.Contains(skyObject))
			{
				_SkyObjects.Add(skyObject);
			}
			
			if(skyObject.type == SkyObject.ObjectType.Sun)
			{
				if (!_Sun.Contains(skyObject))
				{
					_Sun.Add(skyObject);
				}
			}

			_SkyObjects = _SkyObjects.OrderByDescending(o => o.sortOrder).ToList();
		}

		internal void DeregisterSkyObject(SkyObject skyObject)
		{
			_SkyObjects.Remove(skyObject);
			_Sun.Remove(skyObject);
		}

		private void OnEnable()
		{
			Setup();
			ReloadNullProperties();
            SetIcon();
		}

		private void Setup()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = this;
			}
		}

		#region Editor
		private void ReloadNullProperties()
		{
			#if UNITY_EDITOR
			ResourceReloader.TryReloadAllNullIn(this, "Assets/OccaSoftware/Altos/AssetResources");
			#endif
		}

		private void SetIcon()
		{
			#if UNITY_EDITOR
			string directory = AltosData.packagePath + "/Textures/Editor/";
            string id = "day-night-icon.png";
            Texture2D icon = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(directory + id, typeof(Texture2D));
            UnityEditor.EditorGUIUtility.SetIconForObject(gameObject, icon);
			#endif
		}
		#endregion


		void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			isSetupCorrect = ValidateSetup();

			if (!isSetupCorrect)
				return;

			skyDefinition.Initialize();
		}


		private bool ValidateSetup()
		{
			#if !UNITY_2021_3_OR_NEWER
                Debug.LogError("This version of Altos is designed for Unity 2021.3 or newer. Please upgrade your Unity Editor to ensure that Altos is compatible with your Unity version.");    
			#endif

			if (skyDefinition == null)
				return false;

			return true;
		}

		private void Update()
		{
            Profiler.BeginSample("TimeOfDayManager: Update Execution");

            if (!isSetupCorrect)
                return;

            if (skyDefinition == null)
                return;

			skyDefinition.Update();
            ResetScaleAndRotation();

            Profiler.EndSample();
		}


		private void ResetScaleAndRotation()
		{
			if(transform.localScale != Vector3.one || transform.rotation != Quaternion.identity)
			{
				transform.localScale = Vector3.one;
				transform.rotation = Quaternion.identity;
			}
		}


		#region Editor
		#if UNITY_EDITOR
		Transform[] selection;

		private void OnDrawGizmos()
		{
			selection = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.ExcludePrefab | UnityEditor.SelectionMode.Editable | UnityEditor.SelectionMode.TopLevel);
			foreach(Transform selectionItem in selection)
			{
				if(selectionItem.root == transform)
				{
					DrawVisuals();
					DrawText();
				}
			}
		}

		private void DrawVisuals()
		{
			float length = 1f;
			float thickness = 3f;
			UnityEditor.Handles.color = Color.blue;
			UnityEditor.Handles.DrawLine(transform.position - new Vector3(0, 0, length), transform.position + new Vector3(0, 0, length), thickness);

			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawLine(transform.position - new Vector3(length, 0, 0), transform.position + new Vector3(length, 0, 0), thickness);


			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawLine(transform.position - new Vector3(0, length, 0), transform.position + new Vector3(0, length, 0), thickness);

			// Draw Upper and Lower Hemispheres + Celestial Horizon
			UnityEditor.Handles.color = new Color(0, 0, 0, 0.1f);
			
			UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, maxDistance);

			for(int i = 1; i <= (int)maxDistance; ++i)
			{
				UnityEditor.Handles.color = new Color(1, 1, 1, 0.02f);
				UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, i);
			}
		}

		private void DrawText()
		{
			GUIStyle s = new GUIStyle();
			s.fontSize = 12;
			s.fontStyle = FontStyle.Bold;
			s.normal.textColor = Color.white;
			s.alignment = TextAnchor.UpperRight;

			float mm = 0;
			float hh = 0;

			if(skyDefinition != null)
			{
				mm = skyDefinition.CurrentTime - Mathf.Floor(skyDefinition.CurrentTime);
				mm /= 1.67f;
				mm *= 100f;
				hh = Mathf.Floor(skyDefinition.CurrentTime);
			}
			

			UnityEditor.Handles.Label(transform.position + new Vector3(0, 3f, 5f), new GUIContent($"time"), s);
			s.normal.textColor = new Color(1, 1, 1, 1);
			UnityEditor.Handles.Label(transform.position + new Vector3(0, 3f, 5f), new GUIContent($"\n{hh:00}:{mm:00}"), s);

			s.normal.textColor = new Color(1, 1, 1, 1);
			UnityEditor.Handles.Label(transform.position + Vector3.right * maxDistance, new GUIContent("east"), s);
			UnityEditor.Handles.Label(transform.position + Vector3.left * maxDistance, new GUIContent("west"), s);
			UnityEditor.Handles.Label(transform.position + Vector3.forward * maxDistance, new GUIContent("north"), s);
			UnityEditor.Handles.Label(transform.position + Vector3.back * maxDistance, new GUIContent("south"), s);
		}
        #endif
        #endregion
    }


}