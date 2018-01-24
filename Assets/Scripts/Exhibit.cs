using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;

/* A custom editor for the Exhibit class so that we can control what variables are shown */
[CustomEditor(typeof(Exhibit))]
[CanEditMultipleObjects]
public class ExhibitEditor : Editor {
	GUIStyle style;

	/* Main settings */
	SerializedProperty exhibitType;
	SerializedProperty model;
	SerializedProperty image;


	/* Interaction settings */
	bool interactionFoldout = false;
	SerializedProperty interaction;
	SerializedProperty modelHasCollider;
	// Grab interaction
	SerializedProperty centerOnPickup;
	SerializedProperty tapToPickup;
	// Rotate interaction
	SerializedProperty autoRotate;
	SerializedProperty rotationalInertia;
	SerializedProperty headTiltRotation;
	SerializedProperty rotationSpeed;

	/* Textbox settings */
	bool textFoldout = false;
	// Basic
	SerializedProperty title;
	SerializedProperty enableDescriptionText;
	SerializedProperty description;
	Vector2 scrollPosition;
	Rect verticalGroup;
	// Advanced
	SerializedProperty showTextboxInVicinityOnly;
	SerializedProperty textboxPivotAngle;
	SerializedProperty textboxDisplacement;


	/* Stand settings */
	bool standFoldout = false;
	SerializedProperty standScale;
	SerializedProperty standRotation;


	/* Model settings */
	bool modelFoldout = false;
	// Basic
	SerializedProperty modelDisplacement;
	SerializedProperty modelRotation;
	SerializedProperty scale;
	SerializedProperty flipUpsideDown;
	SerializedProperty mirror;


	/* Picture settings */
	bool pictureFoldout = false;
	// Basic
	SerializedProperty usePictureFrame;
	SerializedProperty useExhibitStand;
	SerializedProperty imageBackOption;
	// Advanced
	SerializedProperty frameThickness;
	string[] imageBackOptions = new string[] {"Invisible",
											  "Black",
											  "Source image reversed"};
	int[] imageBackValues = { 0, 1, 2 };


	/* Teleport Point settings */
	bool teleportFoldout = false;
	// Basic
	SerializedProperty enableTeleportPoint;
	SerializedProperty teleportPointHeight;
	SerializedProperty teleportPointDistance;
	// Advanced
	SerializedProperty teleportPointOrbit;
	SerializedProperty teleportPointScale;


	/* Other settings */
	SerializedProperty advancedSettings;
	SerializedProperty useRoughBounding;
	SerializedProperty disableAutoUpdate;


	private void OnEnable() {
		exhibitType = serializedObject.FindProperty("exhibitType");

		model = serializedObject.FindProperty("model");
		image = serializedObject.FindProperty("image");

		interaction = serializedObject.FindProperty("interaction");
		modelHasCollider = serializedObject.FindProperty("modelHasCollider");
		centerOnPickup = serializedObject.FindProperty("centerOnPickup");
		tapToPickup = serializedObject.FindProperty("tapToPickup");
		autoRotate = serializedObject.FindProperty("autoRotate");
		rotationalInertia = serializedObject.FindProperty("rotationalInertia");
		headTiltRotation = serializedObject.FindProperty("headTiltRotation");
		rotationSpeed = serializedObject.FindProperty("rotationSpeed");

		title = serializedObject.FindProperty("title");
		enableDescriptionText = serializedObject.FindProperty("enableDescriptionText");
		description = serializedObject.FindProperty("description");
		showTextboxInVicinityOnly = serializedObject.FindProperty("showTextboxInVicinityOnly");
		textboxPivotAngle = serializedObject.FindProperty("textboxPivotAngle");
		textboxDisplacement = serializedObject.FindProperty("textboxDisplacement");
		
		standScale = serializedObject.FindProperty("standScale");
		standRotation = serializedObject.FindProperty("standRotation");

		modelDisplacement = serializedObject.FindProperty("modelDisplacement");
		modelRotation = serializedObject.FindProperty("modelRotation");
		scale = serializedObject.FindProperty("scale");
		flipUpsideDown = serializedObject.FindProperty("flipUpsideDown");
		mirror = serializedObject.FindProperty("mirror");

		usePictureFrame = serializedObject.FindProperty("usePictureFrame");
		useExhibitStand = serializedObject.FindProperty("useExhibitStand");
		imageBackOption = serializedObject.FindProperty("imageBackOption");
		frameThickness = serializedObject.FindProperty("frameThickness");

		advancedSettings = serializedObject.FindProperty("advancedSettings");
		useRoughBounding = serializedObject.FindProperty("useRoughBounding");
		disableAutoUpdate = serializedObject.FindProperty("disableAutoUpdate");

		enableTeleportPoint = serializedObject.FindProperty("enableTeleportPoint");
		teleportPointHeight = serializedObject.FindProperty("teleportPointHeight");
		teleportPointDistance = serializedObject.FindProperty("teleportPointDistance");
		teleportPointOrbit = serializedObject.FindProperty("teleportPointOrbit");
		teleportPointScale = serializedObject.FindProperty("teleportPointScale");
	}


	private void TextGUI() {
		EditorGUILayout.Space();
		textFoldout = EditorGUILayout.Foldout(textFoldout, "Text Settings", true, style);

		if (textFoldout) {
			title.stringValue = EditorGUILayout.TextField("Exhibit Title", title.stringValue);
			EditorGUILayout.PropertyField(enableDescriptionText);
			if (enableDescriptionText.boolValue) {
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(80));
				description.stringValue = EditorGUILayout.TextArea(description.stringValue, EditorStyles.textArea, GUILayout.MaxHeight(80));
				EditorGUILayout.EndScrollView();
			}

			// ADVANCED
			if (advancedSettings.boolValue) {
				if (enableTeleportPoint.boolValue) {
					showTextboxInVicinityOnly.boolValue = EditorGUILayout.Toggle
						(new GUIContent("Only Show Textbox At Teleport",
						"Only shows the textbox when you are at its teleport point."),
						showTextboxInVicinityOnly.boolValue);
				}
				else {
					showTextboxInVicinityOnly.boolValue = EditorGUILayout.Toggle
						(new GUIContent("Only Show Textbox Near Exhibit",
						"Only shows the textbox when you are near the exhibit."),
						showTextboxInVicinityOnly.boolValue);
				}
				EditorGUILayout.PropertyField(textboxPivotAngle);
				textboxDisplacement.vector3Value = EditorGUILayout.Vector3Field("Textbox Displacement", textboxDisplacement.vector3Value);
			}
		}
	}

	private void InteractionGUI() {
		EditorGUILayout.Space();
		interactionFoldout = EditorGUILayout.Foldout(interactionFoldout, "Interaction Settings", true, style);

		if (interactionFoldout) {
			EditorGUILayout.PropertyField(interaction);

			if (interaction.enumValueIndex != 0 &&
				!modelHasCollider.boolValue) {
				EditorGUILayout.HelpBox("In order to interact with an object, it must have a Collider component. " +
					"The model you have selected does not have a Collider on it, so a generic SphereCollider will be attached to " +
					"it. It is recommended to set your own Collider on the original model, as the default one will likely be inaccurate.",
					MessageType.Warning);
			}
			if (interaction.enumValueIndex == 1) {
				EditorGUILayout.PropertyField(centerOnPickup);
				EditorGUILayout.PropertyField(tapToPickup);
			}
			else if (interaction.enumValueIndex == 2) {
				EditorGUILayout.PropertyField(autoRotate);
				if (!autoRotate.boolValue)
					EditorGUILayout.PropertyField(rotationalInertia);
				EditorGUILayout.PropertyField(headTiltRotation);
				EditorGUILayout.PropertyField(rotationSpeed);
			}
		}
	}

	private void ModelGUI() {
		EditorGUILayout.Space();
		modelFoldout = EditorGUILayout.Foldout(modelFoldout, "Model Settings", true, style);

		if (modelFoldout) {
			// BASIC
			if (!advancedSettings.boolValue) {
				modelDisplacement.vector3Value = new Vector3(modelDisplacement.vector3Value.x,
													EditorGUILayout.FloatField("Height Displacement", modelDisplacement.vector3Value.y),
													modelDisplacement.vector3Value.z);
				modelRotation.vector3Value = new Vector3(modelRotation.vector3Value.x,
													EditorGUILayout.FloatField("Rotation About Y Axis", modelRotation.vector3Value.y),
													modelRotation.vector3Value.z);
				EditorGUILayout.PropertyField(flipUpsideDown);
			}
			// ADVANCED
			else {
				modelDisplacement.vector3Value = EditorGUILayout.Vector3Field("Model Displacement", modelDisplacement.vector3Value);
				modelRotation.vector3Value = EditorGUILayout.Vector3Field
					(new GUIContent("Model Rotation", "NOTE: Use with caution. Significant rotation on more than one " +
						"axis can make subsequent rotations difficult due to gimbal lock."), modelRotation.vector3Value);
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(mirror);
			EditorGUILayout.PropertyField(scale);
		}
	}

	private void ExhibitStandGUI() {
		EditorGUILayout.Space();
		standFoldout = EditorGUILayout.Foldout(standFoldout, "Exhibit Stand Settings", true, style);

		if (standFoldout) {
			// BASIC
			if (!advancedSettings.boolValue) {
				standScale.vector3Value = new Vector3(standScale.vector3Value.x,
												EditorGUILayout.FloatField("Stand Scale", standScale.vector3Value.y),
												standScale.vector3Value.z);
			}
			// ADVANCED
			else {
				standScale.vector3Value = EditorGUILayout.Vector3Field("Stand Scale", standScale.vector3Value);
			}
			EditorGUILayout.PropertyField(standRotation);
		}
	}

	private void PictureGUI() {
		EditorGUILayout.Space();
		pictureFoldout = EditorGUILayout.Foldout(pictureFoldout, "Picture Settings", true, style);

		if (pictureFoldout) {
			modelRotation.vector3Value = new Vector3(modelRotation.vector3Value.x,
												EditorGUILayout.FloatField("Rotation About Y Axis", modelRotation.vector3Value.y),
												modelRotation.vector3Value.z);
			EditorGUILayout.PropertyField(flipUpsideDown);
			EditorGUILayout.PropertyField(mirror);
			EditorGUILayout.PropertyField(scale);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(usePictureFrame);
			if (advancedSettings.boolValue && usePictureFrame.boolValue) {
				EditorGUILayout.PropertyField(frameThickness);
			}
			EditorGUILayout.PropertyField(useExhibitStand);
			this.imageBackOption.intValue = EditorGUILayout.IntPopup("Back of Image: ",
				this.imageBackOption.intValue, imageBackOptions, imageBackValues);
		}
	}

	private void TeleportPointGUI() {
		EditorGUILayout.Space();
		teleportFoldout = EditorGUILayout.Foldout(teleportFoldout, "Teleport Point Settings", true, style);

		if (teleportFoldout) {
			EditorGUILayout.PropertyField(enableTeleportPoint);

			if (enableTeleportPoint.boolValue) {
				EditorGUILayout.PropertyField(teleportPointHeight);
				EditorGUILayout.PropertyField(teleportPointDistance);

				// ADVANCED
				if (advancedSettings.boolValue) {
					EditorGUILayout.Space();
					if (exhibitType.enumValueIndex == 1) {
						EditorGUILayout.PropertyField(teleportPointOrbit);
					}
					EditorGUILayout.PropertyField(teleportPointScale);
				}
			}
		}
	}


	public override void OnInspectorGUI() {
		style = new GUIStyle(EditorStyles.foldout);
		style.fontStyle = FontStyle.Bold;

		serializedObject.Update();

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(exhibitType);

		switch (exhibitType.enumValueIndex) {
			// DISPLAY
			case (int)Exhibit.ExhibitType.Display:
				EditorGUILayout.PropertyField(model);

				TextGUI();
				InteractionGUI();
				ModelGUI();
				ExhibitStandGUI();
				break;

			// STRUCTURE
			case (int)Exhibit.ExhibitType.Structure:
				EditorGUILayout.PropertyField(model);

				TextGUI();
				InteractionGUI();
				ModelGUI();
				break;

			// PICTURE
			case (int)Exhibit.ExhibitType.Picture:
				EditorGUILayout.PropertyField(image);

				TextGUI();
				PictureGUI();
				if (useExhibitStand.boolValue) {
					ExhibitStandGUI();
				}
				break;
		}

		TeleportPointGUI();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(advancedSettings);

		if (advancedSettings.boolValue) {
			//EditorGUILayout.Space();
			//EditorGUILayout.PropertyField(useRoughBounding); // Disabled for non-utility
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(disableAutoUpdate);
			EditorGUILayout.HelpBox("Disables automatic object managment. \n" +
									"You will be on your own for all modifications when using this.", MessageType.Info);
		}

		serializedObject.ApplyModifiedProperties();
	}
}

#endif // UNITY_EDITOR

/* Exhibit class */
[ExecuteInEditMode]
public class Exhibit : MonoBehaviour {
	[Tooltip("Prevents this object from auto-updating to correct itself " +
			 "(which it may do at any point in making manual changes). \n" +
			 "This will also prevent the script from monitoring your changes, so you'll be on your " +
			 "own at that point!")]
	public bool disableAutoUpdate = false;
	
	public enum ExhibitType {
		Display = 0,
		Structure = 1,
		Picture = 2
	}
	
	private GameObject modelObject;
	private GameObject imageObject;
	private GameObject exhibitStand;

	public bool advancedSettings = false;

	// *********************** INTERACTION PROPERTIES *********************** //

	public enum Interaction {
		None = 0,
		Grab = 1,
		Rotate = 2
	}

	public Interaction interaction;
	private Interaction _interaction;
	private GazePickup gpScript;
	private GazeRotator grScript;
	[SerializeField]
	private bool modelHasCollider = false;
	private bool colliderExists = false;
	// GRAB
	public bool centerOnPickup = false;
	public bool tapToPickup = false;
	// ROTATE
	[Tooltip("The object's rotation speed depends on the gaze's distance from the object center. \n" +
		 "Setting this to false makes gaze \"grab\" the object to be pulled manually.")]
	public bool autoRotate = false;
	[Tooltip("Rotation persists after the object is released.")]
	public bool rotationalInertia = false;
	[Tooltip("Head tilting rotates the object along the Z-axis.")]
	public bool headTiltRotation = false;
	public float rotationSpeed = 1f;

	// *********************** TEXT PROPERTIES *********************** //

	private GameObject textbox;
	public string title;
	private Text titleText;

	public bool enableDescriptionText = false;
	public string description;
	private string _description;
	private int descriptionOffsetIdx;
	private List<int> descriptionOffsetList = new List<int>();
	private Text descriptionText;

	private GameObject nextButton;
	private GameObject prevButton;

	private GameObject textPlane;

	[Tooltip("Only shows the textbox when you are near the exhibit or at its teleport point.")]
	public bool showTextboxInVicinityOnly = false;
	[Tooltip("The angle at which the textbox is turned from its top.")]
	public float textboxPivotAngle = 45f;
	public Vector3 textboxDisplacement;
	 
	// *********************** TYPE PROPERTIES *********************** //

	[Tooltip("Determines the position/layout of the model. \n" +
			 "Display: Object is placed on a stand, like an artifact. \n" +
			 "Structure: Object occupies its own space, like a statue. \n" +
			 "Picture: Object is to be placed on a wall, like a picture.")]
	public ExhibitType exhibitType = ExhibitType.Display;
	private ExhibitType _exhibitType = ExhibitType.Display;

	[Tooltip("The model (GameObject) to be displayed.")]
	public GameObject model;
	private GameObject _model;
	private Bounds modelBounds;
	[Tooltip("Uses less memory to place object on exhibit stand, but is less accurate.")]
	public bool useRoughBounding = false;
	private GameObject _boundsRep;

	[Tooltip("Source image that will be used for the exhibit.")]
	public Texture2D image;
	private Texture2D _image;
	private Material imageMat;
	private float aspectRatio;

	// *********************** OBJECT PROPERTIES *********************** //
	
	public float scale = 1.0f;
	private bool _modelScaleChanged = false;
	public bool flipUpsideDown = false;
	public bool mirror = false;

	// *********************** EXHIBIT STAND PROPERTIES *********************** //
	
	public Vector3 standScale = new Vector3(1.5f, 2.5f, 1.5f);
	public float standRotation = 0.0f;


	// *********************** MODEL PROPERTIES *********************** //
	
	public Vector3 modelDisplacement;
	public Vector3 modelRotation;
	

	// *********************** PICTURE PROPERTIES *********************** //
	
	[Tooltip("Frame the image in a picture frame.")]
	public bool usePictureFrame = false;
	private bool _usePictureFrame = false; // used to only apply the setting once
	public float frameThickness = 1f;
	private float _frameThickness = .1f;
	private GameObject pictureFrame;
	private GameObject top;
	private GameObject bottom;
	private GameObject right;
	private GameObject left;
	[Tooltip("Prop the image on an exhibit stand similar to the Display type.")]
	public bool useExhibitStand = false;
	
	[Tooltip("Image will be displayed on the back of the canvas in reverse.")]
	public int imageBackOption = 0;
	private int _imageBackOption = 0; // used to only apply the setting once
	private GameObject backImage;

	// *********************** TELEPORTER PROPERTIES *********************** //

	private GameObject teleportPoint;

	[Tooltip("Places a teleport point in front of the exhibit.")]
	public bool enableTeleportPoint = true;
	[Tooltip("Height offset of the teleport point from its initial location.")]
	public float teleportPointHeight = 0f;
	[Tooltip("Distance offset of the teleport point from its initial location. This is scaled " +
		"from the largest of the x and z dimensions of the exhibit.")]
	public float teleportPointDistance = 0f;

	[Tooltip("Rotates the teleport point around the exhibit.")]
	public float teleportPointOrbit = 0f;
	public float teleportPointScale = 0f;

	// *********************** *********************** //

	private void VerifyTeleportPoint() {
		// Create the teleportPoint object
		if (!this.teleportPoint) {
			for (int i = 0; i < this.transform.childCount; i++) {
				if (this.transform.GetChild(i).gameObject.CompareTag("Teleport Point")) {
					this.teleportPoint = this.transform.GetChild(i).gameObject;
				}
			}
			if (!this.teleportPoint) {
				this.teleportPoint = Instantiate(Resources.Load<GameObject>("Teleport Point"));
				this.teleportPoint.name = "Teleport Point";
				this.teleportPoint.transform.parent = this.transform;
				this.teleportPoint.transform.localPosition = new Vector3(0f, this.teleportPointHeight, this.teleportPointDistance);
			}
		}

		this.teleportPoint.SetActive(this.enableTeleportPoint);
	}

	private void VerifyImage() {
		// Create the imageObject object
		if (!this.imageObject) {
			for (int i = 0; i < this.transform.childCount; i++) {
				if (this.transform.GetChild(i).gameObject.CompareTag("Exhibit Picture")) {
					this.imageObject = this.transform.GetChild(i).gameObject;
					this.imageMat = this.imageObject.GetComponent<MeshRenderer>().sharedMaterial;
					this.aspectRatio = this.imageObject.transform.localScale.x / this.imageObject.transform.localScale.y;
				}
				else if (this.transform.GetChild(i).gameObject.name == "Picture Frame") {
					this.pictureFrame = this.transform.GetChild(i).gameObject;
					this.usePictureFrame = true;
					this._usePictureFrame = true;
					for (int j = 0; j < this.pictureFrame.transform.childCount; j++) {
						if (this.pictureFrame.transform.GetChild(j).gameObject.name == "Top")
							top = this.pictureFrame.transform.GetChild(j).gameObject;
						if (this.pictureFrame.transform.GetChild(j).gameObject.name == "Bottom")
							bottom = this.pictureFrame.transform.GetChild(j).gameObject;
						if (this.pictureFrame.transform.GetChild(j).gameObject.name == "Right")
							right = this.pictureFrame.transform.GetChild(j).gameObject;
						if (this.pictureFrame.transform.GetChild(j).gameObject.name == "Left")
							left = this.pictureFrame.transform.GetChild(j).gameObject;
					}
				}
			}
			if (this.imageObject) {
				for (int i = 0; i < this.imageObject.transform.childCount; i++) {
					if (this.imageObject.transform.GetChild(i).gameObject.name ==
						this.imageObject.name + "(Back)") {
						this.backImage = this.imageObject.transform.GetChild(i).gameObject;
					}
				}
			}
		}

		if (!this.imageObject) {
			this.imageObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			this.imageObject.tag = "Exhibit Picture";
			this.imageObject.transform.parent = this.transform;
			this.imageObject.transform.localPosition = Vector3.zero;

			this.aspectRatio = 1;

			if (this.imageBackOption > 0) {
				GenerateBackImage();
			}

			if (this.exhibitType != ExhibitType.Picture) {
				this.imageObject.SetActive(false);
			}
		}

		if (!this.imageMat) {
			this.imageMat = new Material(Shader.Find("Standard"));
			this.imageObject.GetComponent<MeshRenderer>().material = this.imageMat;
		}

		// Verify that an image is selected
		if (!this.image) {
			this._image = null;
			this.imageMat.mainTexture = null;
			this.imageObject.name = "";
			this.aspectRatio = 1;
			if (this.backImage) {
				this.backImage.name = "(Back)";
			}
			return;
		}

		// Update the selected image
		if (this.image != this._image || !this.imageMat.mainTexture) {
			this._image = this.image;
			this.imageObject.name = this.image.name;

			// Get aspect ratio for quad scaling
			this.aspectRatio = (float) this.image.width / (float) this.image.height;
			if (this.aspectRatio == 0) {
				Debug.LogError("Image aspect ratio is zero!");
			}
			if (this.aspectRatio > 1) {
				this.imageObject.transform.localScale = new Vector3(this.aspectRatio, 1, 1);
			}
			else {
				this.imageObject.transform.localScale = new Vector3(1, 1/this.aspectRatio, 1);
			}

			// Set the image on the material
			this.imageMat = new Material(Shader.Find("Standard"));
			this.imageObject.GetComponent<MeshRenderer>().material = this.imageMat;
			this.imageMat.mainTexture = this.image;

			// Set the back image to the same material
			if (this.imageBackOption == 1) {
				Material mat = new Material(Shader.Find("Standard"));
				mat.color = Color.black;
				this.backImage.GetComponent<MeshRenderer>().material = mat;
			}
			if (this.imageBackOption == 2) {
				this.backImage.GetComponent<MeshRenderer>().material = this.imageMat;
			}

			// Generate the picture frame
			if (this.usePictureFrame) {
				GeneratePictureFrame();
			}
		}
	}

	private void GenerateBackImage() {
		if (this.backImage)
			DestroyImmediate(this.backImage);
		this.backImage = GameObject.CreatePrimitive(PrimitiveType.Quad);
		this.backImage.transform.parent = this.imageObject.transform;
		this.backImage.transform.localPosition = Vector3.zero;
		this.backImage.transform.localRotation = Quaternion.identity;
		this.backImage.transform.Rotate(0f, 180f, 0f);
		this.backImage.transform.localScale = new Vector3(-1f, 1f, 1f);
	}

	private void GeneratePictureFrame() {
		if (this.pictureFrame)
			DestroyImmediate(this.pictureFrame);
		this.pictureFrame = new GameObject();
		this.pictureFrame.name = "Picture Frame";
		this.pictureFrame.transform.parent = this.transform; // easier to deal with scaling
		this.pictureFrame.transform.localPosition = this.imageObject.transform.localPosition;
		Material mat = Resources.Load<Material>("Frame");

		top = GameObject.CreatePrimitive(PrimitiveType.Cube);
		top.name = "Top";
		top.GetComponent<MeshRenderer>().sharedMaterial = mat;
		top.transform.parent = this.pictureFrame.transform;

		bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
		bottom.name = "Bottom";
		bottom.GetComponent<MeshRenderer>().sharedMaterial = mat;
		bottom.transform.parent = this.pictureFrame.transform;

		right = GameObject.CreatePrimitive(PrimitiveType.Cube);
		right.name = "Right";
		right.GetComponent<MeshRenderer>().sharedMaterial = mat;
		right.transform.parent = this.pictureFrame.transform;

		left = GameObject.CreatePrimitive(PrimitiveType.Cube);
		left.name = "Left";
		left.GetComponent<MeshRenderer>().sharedMaterial = mat;
		left.transform.parent = this.pictureFrame.transform;

		UpdatePictureFrame();
	}

	private void UpdatePictureFrame() {
		float ft = (this.advancedSettings) ? this._frameThickness : .1f;
		if (this.aspectRatio > 1) {
			top.transform.localScale = new Vector3(this.aspectRatio + ft * 2, ft, .1f);
			bottom.transform.localScale = new Vector3(this.aspectRatio + ft * 2, ft, .1f);
			right.transform.localScale = new Vector3(ft, 1f, .1f);
			left.transform.localScale = new Vector3(ft, 1f, .1f);

			top.transform.localPosition = new Vector3(0f, .5f + ft / 2, 0f);
			bottom.transform.localPosition = new Vector3(0f, -.5f - ft / 2, 0f);
			right.transform.localPosition = new Vector3((ft + this.aspectRatio) / 2, 0f, 0f);
			left.transform.localPosition = new Vector3(-(ft + this.aspectRatio) / 2, 0f, 0f);
		}
		else {
			top.transform.localScale = new Vector3(1f + ft * 2, ft, .1f);
			bottom.transform.localScale = new Vector3(1f + ft * 2, ft, .1f);
			right.transform.localScale = new Vector3(ft, 1f / this.aspectRatio, .1f);
			left.transform.localScale = new Vector3(ft, 1f / this.aspectRatio, .1f);

			top.transform.localPosition = new Vector3(0f, (ft + this.aspectRatio) / 2, 0f);
			bottom.transform.localPosition = new Vector3(0f, -(ft + this.aspectRatio) / 2, 0f);
			right.transform.localPosition = new Vector3(.5f + ft / 2, 0f, 0f);
			left.transform.localPosition = new Vector3(-.5f - ft / 2, 0f, 0f);
		}
	}

	private void VerifyModel() {
		// Create the modelObject object
		if (!this.modelObject) {
			for (int i = 0; i < this.transform.childCount; i++) {
				if (this.transform.GetChild(i).gameObject.CompareTag("Exhibit Model")) {
					this.modelObject = this.transform.GetChild(i).gameObject;
					GetModelBounds();
				}
			}
		}

		// Verify that a model is selected
		if (!this.model) {
			if (!this.modelObject || this.modelObject.name != "") {
				if (this.modelObject)
					DestroyImmediate(this.modelObject);

				this.modelObject = new GameObject();
				this.modelObject.name = "";
				this.modelObject.tag = "Exhibit Model";
				this.modelObject.transform.parent = this.transform;
				this.modelObject.transform.localPosition = Vector3.zero;

				if (this.exhibitType != ExhibitType.Display && this.exhibitType != ExhibitType.Structure) {
					this.modelObject.SetActive(false);
				}

				this.modelBounds = new Bounds();
			}
			this._model = null;

			return;
		}

		// Update the selected model
		if ((this.model && !this.modelObject) || (this.model != this._model)) {
			this._model = this.model;

			if (this.modelObject)
				DestroyImmediate(this.modelObject);

			this.modelObject = Instantiate(this.model);

			this.modelObject.name = this.model.name;
			this.modelObject.tag = "Exhibit Model";
			this.modelObject.transform.parent = this.transform;
			this.modelObject.transform.localPosition = Vector3.zero;

			GetModelBounds();

			if (this.exhibitType == ExhibitType.Display) {
				// Reset the height so that the modelObject sits on top of the exhibit
				SetOnStand(this.modelObject.transform);
			}
		}
	}

	private void VerifyTextbox() {
		if (!this.textbox) {
			for (int i = 0; i < this.transform.childCount; i++) {
				if (this.transform.GetChild(i).gameObject.CompareTag("Textbox")) {
					this.textbox = this.transform.GetChild(i).gameObject;
				}
			}

			if (!this.textbox) {
				this.textbox = Instantiate(Resources.Load<GameObject>("Textbox"));
				this.textbox.name = "Textbox";
				this.textbox.transform.parent = this.transform;
				this.textbox.transform.localPosition = Vector3.zero;
			}
		}

		if (!this.titleText) {
			for (int i = 0; i < this.textbox.transform.childCount; i++) {
				if (this.textbox.transform.GetChild(i).gameObject.CompareTag("Title Text")) {
					this.titleText = this.textbox.transform.GetChild(i).gameObject.GetComponent<Text>();
				}
			}
		}
		if (!this.descriptionText) {
			for (int i = 0; i < this.textbox.transform.childCount; i++) {
				if (this.textbox.transform.GetChild(i).gameObject.CompareTag("Description Text")) {
					this.descriptionText = this.textbox.transform.GetChild(i).gameObject.GetComponent<Text>();
				}
			}
		}
		if (!this.nextButton) {
			for (int i = 0; i < this.textbox.transform.childCount; i++) {
				if (this.textbox.transform.GetChild(i).gameObject.CompareTag("Next Arrow")) {
					this.nextButton = this.textbox.transform.GetChild(i).gameObject;
				}
			}
		}
		if (!this.prevButton) {
			for (int i = 0; i < this.textbox.transform.childCount; i++) {
				if (this.textbox.transform.GetChild(i).gameObject.CompareTag("Previous Arrow")) {
					this.prevButton = this.textbox.transform.GetChild(i).gameObject;
				}
			}
		}
		if (!this.textPlane) {
			for (int i = 0; i < this.textbox.transform.childCount; i++) {
				if (this.textbox.transform.GetChild(i).gameObject.CompareTag("Text Plane")) {
					this.textPlane = this.textbox.transform.GetChild(i).gameObject;
				}
			}
		}
		if (!this.titleText || !this.descriptionText ||
			!this.nextButton || !this.prevButton ||
			!this.textPlane) {
			DestroyImmediate(this.textbox);

			this.textbox = Instantiate(Resources.Load<GameObject>("Textbox"));
			this.textbox.name = "Textbox";
			this.textbox.transform.parent = this.transform;
			this.textbox.transform.localPosition = Vector3.zero;
		}
	}

	private void VerifyExhibitStand() {
		if (!this.exhibitStand) {
			for (int i = 0; i < this.transform.childCount; i++) {
				if (this.transform.GetChild(i).gameObject.CompareTag("Exhibit Stand")) {
					this.exhibitStand = this.transform.GetChild(i).gameObject;
				}
			}

		if (!this.exhibitStand) {
				this.exhibitStand = GameObject.CreatePrimitive(PrimitiveType.Cube);
				this.exhibitStand.tag = "Exhibit Stand";

				this.exhibitStand.transform.parent = this.transform;
				this.exhibitStand.transform.localPosition = Vector3.zero;
				this.exhibitStand.transform.localScale = new Vector3(1.5f, 2.5f, 1.5f);

				this.exhibitStand.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Exhibit");
				this.exhibitStand.GetComponent<BoxCollider>().enabled = false;

				this.exhibitStand.name = "Exhibit Stand";

				if (this.exhibitType != ExhibitType.Display) {
					this.exhibitStand.SetActive(false);
				}
			}
		}
	}

	private Bounds GetTotalBounds(GameObject go, bool roughBounding, bool checkForSkinnedRenderers = true) {
		if (roughBounding) {
			return GetTotalBoundsRough(go);
		}
		else { 
			return GetTotalBoundsRec(go, Vector3.zero, Vector3.one);
		}
	}

    private Bounds GetTotalBoundsRough(GameObject go, bool checkForSkinnedRenderers = true)
    {
        bool foundRenderer = false;
        Bounds bounds = new Bounds();
        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            Vector3 centerRelativeToParent = mr.bounds.center - mr.transform.position;
            Vector3 parentTranslation = (mr.transform.parent != this.transform.parent) ? mr.transform.parent.rotation * Vector3.Scale(mr.transform.lossyScale, mr.transform.localPosition) : Vector3.zero;
            Vector3 size = mr.bounds.extents * 2f;

            bounds.Encapsulate(new Bounds(centerRelativeToParent +
                               parentTranslation,
                               size));
            foundRenderer = true;
        }
        if (checkForSkinnedRenderers)
        {
            SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                Vector3 centerRelativeToParent = smr.bounds.center - smr.transform.position;
                Vector3 parentTranslation = (smr.transform.parent != this.transform.parent) ? smr.transform.parent.rotation * Vector3.Scale(smr.transform.lossyScale, smr.transform.localPosition) : Vector3.zero;
                Vector3 size = smr.bounds.extents * 2f;

                bounds.Encapsulate(new Bounds(centerRelativeToParent +
                                   parentTranslation,
                                   size));
                foundRenderer = true;
            }
        }

        if (!foundRenderer)
        {
            Debug.Log("Warning! " + go.name + " has no MeshRenderers in its heirarchy!");
        }

        return bounds;
    }

    // Recursive version of GetTotalBounds
    // More accurate but uses a new stack frame per child of each consecutive game object
    private Bounds GetTotalBoundsRec(GameObject go, Vector3 center, Vector3 scale, bool checkForSkinnedRenderers = true) {
		Bounds bounds = new Bounds();

		MeshRenderer mr = go.transform.GetComponent<MeshRenderer>();
		if (mr) {
			bounds.Encapsulate(new Bounds((mr.bounds.center - mr.transform.position) + center,
								mr.bounds.extents * 2));
		}
		if (checkForSkinnedRenderers) {
			SkinnedMeshRenderer smr = go.transform.GetComponent<SkinnedMeshRenderer>();
			if (smr) {
				bounds.Encapsulate(new Bounds((smr.bounds.center - smr.transform.position) + center,
									smr.bounds.extents * 2));
			}
		}

		Vector3 adjustedScale = Vector3.Scale(go.transform.localScale, scale);

		for (int i = 0; i < go.transform.childCount; i++) {
			bounds.Encapsulate(GetTotalBoundsRec(go.transform.GetChild(i).gameObject,
				go.transform.localRotation * Vector3.Scale(go.transform.GetChild(i).localPosition, adjustedScale) + center,
				adjustedScale));
		}

		return bounds;
	}

	private void ResetTransforms() {
		this.modelObject.transform.localPosition = Vector3.zero;
		if (this.model) {
			this.modelObject.transform.localRotation = this.model.transform.localRotation;
			this.modelObject.transform.localScale = this.model.transform.localScale;
		}
		else {
			this.modelObject.transform.localRotation = Quaternion.identity;
			this.modelObject.transform.localScale = Vector3.one;
		}

		this.imageObject.transform.localPosition = Vector3.zero;
		this.imageObject.transform.localRotation = Quaternion.identity;
		if (this.aspectRatio > 1) {
			this.imageObject.transform.localScale = new Vector3(this.aspectRatio, 1, 1);
		}
		else {
			this.imageObject.transform.localScale = new Vector3(1, 1 / this.aspectRatio, 1);
		}

		if (this.backImage) {
			this.backImage.transform.localPosition = Vector3.zero;
			this.backImage.transform.localScale = new Vector3(-1f, 1f, 1f);
			this.backImage.transform.localRotation = Quaternion.identity;
			this.backImage.transform.Rotate(0f, 180f, 0f);
		}
	}

	private void GetModelBounds() {
		bool rotated = false;
		if (this.advancedSettings && this.model && this.modelObject.transform.localRotation == this.model.transform.localRotation) {
			this.modelObject.transform.Rotate(this.modelRotation);
			rotated = true;
		}
		this.modelBounds = GetTotalBounds(this.modelObject, this.useRoughBounding);
		if (rotated) {
			this.modelObject.transform.localRotation = this.model.transform.localRotation;
		}
	}

	private void SetOnStand(Transform t) {
		float height = this.exhibitStand.transform.localScale.y / 2f + this.exhibitStand.transform.localPosition.y;
		t.localPosition += new Vector3(0f, height, 0f);
	}

	private void ApplyExhibitStandSettings() {
		// *********** OBJECT SETTINGS *********** //
		// Position adjustments
		this.exhibitStand.transform.localPosition = new Vector3(0f, this.standScale.y / 2f, 0f);

		// Rotation adjustments
		this.exhibitStand.transform.localRotation = Quaternion.identity;
		this.exhibitStand.transform.Rotate(0f, this.standRotation, 0f);

		// *********** BASIC SETTINGS ONLY *********** //
		if (!this.advancedSettings) {
			// Scale adjustments
			this.exhibitStand.transform.localScale = new Vector3(1.5f, this.standScale.y, 1.5f);
		}
		// *********** ADVANCED SETTINGS ONLY *********** //
		else {
			// Scale adjustments
			this.exhibitStand.transform.localScale = this.standScale;
		}
	}

	private void ApplyModelSettings() {
		// *********** OBJECT SETTINGS *********** //
		// Position adjustments
		float height = this.modelBounds.extents.y * this.scale;
		Vector3 centerOffset = this.modelBounds.center * this.scale;
		centerOffset = (!this.advancedSettings && this.flipUpsideDown) ? centerOffset * -1f : centerOffset;
		this.modelObject.transform.localPosition += new Vector3(0f, height, 0f) - centerOffset;

		// Scale adjustments
		this.modelObject.transform.localScale *= this.scale;
		if (this.mirror) {
			this.modelObject.transform.localScale = Vector3.Scale(this.modelObject.transform.localScale,
				new Vector3(-1f, 1f, 1f));
		}

		float extraRotation = (this.exhibitType == ExhibitType.Display ||
			(this.exhibitType == ExhibitType.Picture && this.useExhibitStand)) ? this.standRotation : 0f;

		// *********** BASIC SETTINGS ONLY *********** //
		if (!this.advancedSettings) {
			// Position adjustments
			this.modelObject.transform.localPosition += new Vector3(0f, this.modelDisplacement.y, 0f);
			// Rotation adjustments
			this.modelObject.transform.Rotate(0f, this.modelRotation.y + extraRotation, 0f, Space.World);
			if (this.flipUpsideDown) {
				this.modelObject.transform.Rotate(180f, 0f, 0f);
			}
		}
		// *********** ADVANCED SETTINGS ONLY *********** //
		else {
			// Position adjustments
			this.modelObject.transform.localPosition += this.modelDisplacement;
			// Rotation adjustments
			this.modelObject.transform.Rotate(0f, extraRotation, 0f, Space.World);
			this.modelObject.transform.Rotate(this.modelRotation, Space.World);
		}
	}

	private void ApplyPictureSettings() {
		// *********** OBJECT SETTINGS *********** //
		// Position adjustments
		this.imageObject.transform.localPosition += new Vector3(0f, this.imageObject.transform.localScale.y * this.scale / 2f, 0f);
		if (this.usePictureFrame) {
			if (!this.advancedSettings)
				this.imageObject.transform.localPosition += new Vector3(0f, .1f * this.scale, 0f);
			else
				this.imageObject.transform.localPosition += new Vector3(0f, this._frameThickness * this.scale, 0f);
		}

		// Scale adjustments
		this.imageObject.transform.localScale *= this.scale;
		if (this.mirror) {
			this.imageObject.transform.localScale = Vector3.Scale(this.imageObject.transform.localScale,
				new Vector3(-1f, 1f, 1f));
		}

		// Rotation adjustments
		float rotation = this.modelRotation.y;
		rotation += (this.useExhibitStand) ? this.standRotation : 0f;
		this.imageObject.transform.Rotate(0f, rotation, 0f);
		if (this.flipUpsideDown) {
			this.imageObject.transform.Rotate(0f, 0f, 180f, Space.Self);
		}

		// *********** OTHER SETTINGS *********** //
		// Picture frame settings
		if (this.usePictureFrame != this._usePictureFrame) {
			if (this.usePictureFrame) {
				GeneratePictureFrame();
			}
			else {
				if (this.pictureFrame)
					DestroyImmediate(this.pictureFrame);
			}
			this._usePictureFrame = this.usePictureFrame;
		}
		if (this.pictureFrame) {
			this.pictureFrame.transform.localScale = Vector3.one * this.scale;
			this.pictureFrame.transform.localPosition = this.imageObject.transform.localPosition;
			this.pictureFrame.transform.localRotation = this.imageObject.transform.localRotation;

			if (this.frameThickness != this._frameThickness * .1f) {
				this._frameThickness = this.frameThickness * .1f;
				UpdatePictureFrame();
			}
		}

		// Picture back settings
		if (this.imageBackOption != this._imageBackOption) {
			if (this.imageBackOption == 0) {
				if (this.backImage)
					DestroyImmediate(this.backImage);
			}
			else {
				if (!this.backImage) {
					GenerateBackImage();
				}
				if (this.imageBackOption == 1) {
					Material mat = new Material(Shader.Find("Standard"));
					mat.color = Color.black;
					this.backImage.GetComponent<MeshRenderer>().material = mat;
				}
				else {
					this.backImage.GetComponent<MeshRenderer>().material = this.imageMat;
				}
			}
			this._imageBackOption = this.imageBackOption;
		}
	}

	private void ApplyTeleportPointSettings() {
		Vector3 pos = new Vector3();

		switch (this.exhibitType) {
			case ExhibitType.Display:
				pos.y += this.standScale.y + 1f;
				pos.z -= Mathf.Max(this.standScale.z, this.standScale.x) + 1f;

				pos.y += this.teleportPointHeight;
				pos.z -= this.teleportPointDistance;
				break;

			case ExhibitType.Structure:
				pos.y += this.modelBounds.extents.y * this.scale;
				pos.z -= Mathf.Max(this.modelBounds.extents.z, this.modelBounds.extents.x) * this.scale + 1f;

				pos.y += this.teleportPointHeight;
				pos.z -= this.teleportPointDistance;
				break;

			case ExhibitType.Picture:
				if (this.useExhibitStand) {
					pos.y += this.standScale.y + 1f;
					pos.z -= Mathf.Max(this.standScale.z, this.standScale.x) + 1f;
				}
				else {
					pos.y += this.imageObject.transform.localScale.y / 2;
					pos.z -= 1f;
				}

				pos.y += this.teleportPointHeight;
				pos.z -= this.teleportPointDistance;
				break;
		}

		this.teleportPoint.transform.localPosition = pos;
		this.teleportPoint.transform.localRotation = Quaternion.identity;

		float rotation = (this.exhibitType == ExhibitType.Display ||
			(this.exhibitType == ExhibitType.Picture && this.useExhibitStand)) ?
			this.standRotation : 0f;
		rotation += (this.exhibitType == ExhibitType.Picture && !this.useExhibitStand) ?
			this.modelRotation.y : 0f;

		if (!this.advancedSettings) {
			this.teleportPoint.transform.localScale = Vector3.one;
		}
		// *********** ADVANCED SETTINGS ONLY *********** //
		else {
			rotation += (this.exhibitType == ExhibitType.Structure) ? this.teleportPointOrbit : 0f;
			this.teleportPoint.transform.localScale = Vector3.one * this.teleportPointScale;
		}

		this.teleportPoint.transform.RotateAround(this.transform.position, Vector3.up, rotation);
	}

	private void ApplyTextboxSettings() {
		// Reset transform
		this.textbox.transform.localPosition = Vector3.zero;
		this.textbox.transform.localRotation = Quaternion.identity;

		// Fix plane
		RectTransform rt = this.textbox.GetComponent<RectTransform>();
		float planeLength = this.titleText.rectTransform.rect.height + 50;
		planeLength += (this.enableDescriptionText) ? this.descriptionText.rectTransform.rect.height + 100 : 0f;
		this.textPlane.transform.localScale = new Vector3(rt.rect.width/10f, 1f, planeLength/10f);

		this.textPlane.transform.localPosition = Vector3.zero;
		this.textPlane.transform.localPosition += new Vector3(0f, -planeLength/2f, 0f);

		// Adjust transform
		switch (this.exhibitType) {
			case ExhibitType.Display:
				this.textbox.transform.localPosition = new Vector3(0f,
					this.standScale.y, -this.standScale.z / 2f);
				if (this.advancedSettings) {
					this.textbox.transform.localPosition += this.textboxDisplacement;
					this.textbox.transform.Rotate(this.textboxPivotAngle, 0f, 0f);
				}
				else {
					this.textbox.transform.Rotate(45f, 0f, 0f);
				}
				this.textbox.transform.RotateAround(this.exhibitStand.transform.position,
					Vector3.up, this.standRotation);
				break;

			case ExhibitType.Structure:
				//RectTransform rt = this.textbox.GetComponent<RectTransform>();
				this.textbox.transform.localPosition = new Vector3(0f,
					rt.localScale.y * this.textPlane.transform.localScale.z * 10,
					-Mathf.Max(this.modelBounds.extents.z, this.modelBounds.extents.x) * this.scale - 1f);
				if (this.advancedSettings) {
					this.textbox.transform.localPosition += this.textboxDisplacement;
					this.textbox.transform.Rotate(this.textboxPivotAngle, 0f, 0f);
					this.textbox.transform.RotateAround(this.modelObject.transform.position,
						Vector3.up, this.teleportPointOrbit);
				}
				break;

			case ExhibitType.Picture:
				if (this.useExhibitStand) {
					this.textbox.transform.localPosition = new Vector3(0f,
						this.standScale.y, -this.standScale.z / 2f);
					if (this.advancedSettings) {
						this.textbox.transform.localPosition += this.textboxDisplacement;
						this.textbox.transform.Rotate(this.textboxPivotAngle, 0f, 0f);
					}
					else {
						this.textbox.transform.Rotate(45f, 0f, 0f);
					}
					this.textbox.transform.RotateAround(this.exhibitStand.transform.position,
						Vector3.up, this.standRotation);
				}
				else {
					float yPos = (this.aspectRatio > 1) ? this.scale / 2f : this.scale / this.aspectRatio / 2f;
					if (this.usePictureFrame) {
						this.textbox.transform.localPosition = new Vector3(0f,
							-yPos + this.imageObject.transform.localPosition.y, -.05f * this.scale - .0001f);
					}
					else {
						this.textbox.transform.localPosition = new Vector3(0f,
							-yPos + this.imageObject.transform.localPosition.y, 0f);
					}
					if (this.advancedSettings) {
						this.textbox.transform.localPosition += this.textboxDisplacement;
						this.textbox.transform.Rotate(this.textboxPivotAngle, 0f, 0f);
					}
					else {
						this.textbox.transform.Rotate(0f, 0f, 0f);
					}
					this.textbox.transform.RotateAround(this.imageObject.transform.position,
						Vector3.up, this.modelRotation.y);
				}
				break;
		}

		// Apply the changes of the text
		this.titleText.text = this.title;

		if (this.enableDescriptionText) {
			this.descriptionText.gameObject.SetActive(true);
			// Section off parts of the description to fit into the text box
			if (this._description != this.description) {
				this._description = this.description;
				this.descriptionText.text = this.description;
				this.descriptionOffsetList.Clear();

				TextGenerator tg = this.descriptionText.cachedTextGenerator;
				RectTransform drt = this.descriptionText.rectTransform;
				tg.Populate(this.descriptionText.text,
					this.descriptionText.GetGenerationSettings(drt.rect.size));

				this.descriptionOffsetList.Add(0);
				while (this.descriptionText.text.Length > tg.characterCountVisible) {
					this.descriptionOffsetList.Add(this.descriptionOffsetList[this.descriptionOffsetList.Count - 1] +
						tg.characterCountVisible);
					this.descriptionText.text = this.descriptionText.text.Substring(tg.characterCountVisible);
					tg.Populate(this.descriptionText.text,
						this.descriptionText.GetGenerationSettings(drt.rect.size));
				}

				this.descriptionText.text = this.description;
				this.descriptionOffsetIdx = 0;
			}

			if (this.descriptionOffsetList.Count > 1) {
				if (this.descriptionOffsetIdx == this.descriptionOffsetList.Count - 1) {
					this.nextButton.SetActive(false);
					this.prevButton.SetActive(true);
				}
				else if (this.descriptionOffsetIdx == 0) {
					this.nextButton.SetActive(true);
					this.prevButton.SetActive(false);
				}
				else {
					this.nextButton.SetActive(true);
					this.prevButton.SetActive(true);
				}
			}
			else {
				this.nextButton.SetActive(false);
				this.prevButton.SetActive(false);
			}
		}
		else {
			this.descriptionText.gameObject.SetActive(false);
			this.nextButton.SetActive(false);
			this.prevButton.SetActive(false);
		}
	}

	public void NextDescriptionOffset() {
		if (this.descriptionOffsetIdx < this.descriptionOffsetList.Count - 1) {
			this.descriptionOffsetIdx++;
			this.descriptionText.text = this.description.Substring(this.descriptionOffsetList[this.descriptionOffsetIdx]);

			if (this.descriptionOffsetIdx == this.descriptionOffsetList.Count - 1) {
				this.nextButton.SetActive(false);
			}
			this.prevButton.SetActive(true);
		}
		else {
			Debug.LogWarning("The description does not have any text further than this!");
		}
	}

	public void PreviousDescriptionOffset() {
		if (this.descriptionOffsetIdx > 0) {
			this.descriptionOffsetIdx--;
			this.descriptionText.text = this.description.Substring(this.descriptionOffsetList[this.descriptionOffsetIdx]);

			if (this.descriptionOffsetIdx == 0) {
				this.prevButton.SetActive(false);
			}
			this.nextButton.SetActive(true);
		}
		else {
			Debug.LogWarning("The description does not have any text before this!");
		}
	}

	private void ApplyInteractionSettings() {
		switch (this.interaction) {
			case Interaction.None:
				break;

			case Interaction.Grab:
				this.gpScript.centerOnPickup = this.centerOnPickup;
				this.gpScript.tapToPickup = this.tapToPickup;
				break;

			case Interaction.Rotate:
				this.grScript.autoRotate = this.autoRotate;
				this.grScript.rotationalInertia = this.rotationalInertia;
				this.grScript.headTiltRotation = this.headTiltRotation;
				this.grScript.rotationSpeed = this.rotationSpeed;
				break;
		}

		if (this.interaction != 0 && !this.modelObject.GetComponent<Collider>()) {
			this.modelObject.AddComponent<SphereCollider>();
		}
	}


	private void UpdateSettings() {
		if (this.scale < 0)
			this.scale = 0;
		if (this.standScale.x < 0)
			this.standScale.x = 0;
		if (this.standScale.y < 0)
			this.standScale.y = 0;
		if (this.standScale.z < 0)
			this.standScale.z = 0;

		if (this.frameThickness < 0)
			this.frameThickness = 0;

		if (this.textboxPivotAngle < 0.01f)
			this.textboxPivotAngle = 0.01f;
		else if (this.textboxPivotAngle > 90)
			this.textboxPivotAngle = 90;

		this.modelHasCollider = (this.interaction != 0 && this.model.GetComponent<Collider>());
	}

	void Start () {
		UpdateInEditor(); // Need to run once for initialization
	}

	void Update() {
		if (Application.isPlaying)
			UpdateDuringRuntime();
		else
			UpdateInEditor();
	}

	// Run during the regular update cycles
	private void UpdateDuringRuntime() {
		// Determine if we show the textbox
		if (this.advancedSettings && this.showTextboxInVicinityOnly) {
			if (this.enableTeleportPoint) {
				if (TeleportPoint.currentPoint == this.teleportPoint) {
					this.textbox.SetActive(true);
				}
				else {
					this.textbox.SetActive(false);
					return;
				}
			}
			else {
				if ((Camera.main.transform.position - this.transform.position).magnitude < 8) { // arbitrary distance...
					this.textbox.SetActive(true);
				}
				else {
					this.textbox.SetActive(false);
					return;
				}
			}
		}
		else {
			this.textbox.SetActive(true);
		}
	}

	// Run on any change within the editor
	private void UpdateInEditor() {
		if (this.disableAutoUpdate && this.advancedSettings)
			return;

		VerifyImage();
		VerifyExhibitStand();
		VerifyModel();
		VerifyTeleportPoint();
		VerifyTextbox();

		UpdateSettings();

		// Check only if we switched types
		if (this.exhibitType != this._exhibitType) {
			this._exhibitType = this.exhibitType;

			switch (this.exhibitType) {
				case ExhibitType.Display:
					this.exhibitStand.SetActive(true);
					this.modelObject.SetActive(true);
					this.imageObject.SetActive(false);
					if (this.pictureFrame) this.pictureFrame.SetActive(false);
					break;

				case ExhibitType.Structure:
					this.exhibitStand.SetActive(false);
					this.modelObject.SetActive(true);
					this.imageObject.SetActive(false);
					if (this.pictureFrame) this.pictureFrame.SetActive(false);
					break;

				case ExhibitType.Picture:
					this.exhibitStand.SetActive(false);
					this.modelObject.SetActive(false);
					this.imageObject.SetActive(true);
					if (this.pictureFrame) this.pictureFrame.SetActive(true);
					break;
			}
		}

		// Check only if we switched interactions
		if (this.interaction != this._interaction) {
			this._interaction = this.interaction;
			if (this.gpScript) {
				DestroyImmediate(this.gpScript);
			}
			if (this.grScript) {
				DestroyImmediate(this.grScript);
			}

			switch (this.interaction) {
				case Interaction.None:
					break;

				case Interaction.Grab:
					this.modelObject.AddComponent<GazePickup>();
					this.gpScript = this.modelObject.GetComponent<GazePickup>();
					break;

				case Interaction.Rotate:
					this.modelObject.AddComponent<GazeRotator>();
					this.grScript = this.modelObject.GetComponent<GazeRotator>();
					break;
			}
		}

		// Check every time
		ResetTransforms();
		switch (this.exhibitType) {
			case ExhibitType.Display:
				GetModelBounds();
				ApplyExhibitStandSettings();
				ApplyModelSettings();
				SetOnStand(this.modelObject.transform);

				ApplyInteractionSettings();
				break;

			case ExhibitType.Structure:
				GetModelBounds();
				ApplyModelSettings();

				ApplyInteractionSettings();
				break;

			case ExhibitType.Picture:
				if (this.useExhibitStand) {
					this.exhibitStand.SetActive(true);
					ApplyExhibitStandSettings();
					SetOnStand(this.imageObject.transform);
				}
				else {
					this.exhibitStand.SetActive(false);
				}
				ApplyPictureSettings();
				break;
		}
		if (this.enableTeleportPoint) {
			this.ApplyTeleportPointSettings();
		}
		ApplyTextboxSettings();
	}
}
