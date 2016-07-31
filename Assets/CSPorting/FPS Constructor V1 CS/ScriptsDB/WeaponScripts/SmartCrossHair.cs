using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class SmartCrossHair : MonoBehaviour
	{

		public float length1;
		public float width1;
		public bool scale = false;
		private Texture textu;
		private GUIStyle lineStyle;
		public bool debug = false;
		public static bool displayWhenAiming = false;
		public bool useTexture = false;
		public static bool ownTexture = false;
		public GameObject crosshairObj;
		public static GameObject cObj;
		public static bool scl;
		public static float cSize;
		public static float sclRef;
		public static bool draw = false;
		public float crosshairSize;
		public float minimumSize;
		public float maximumSize;
		public Texture2D crosshairTexture;
		public Texture2D friendTexture;
		public Texture2D foeTexture;
		public Texture2D otherTexture;
		public bool colorFoldout;
		public float colorDist = 40;

		private bool hitEffectOn;
		public Texture2D hitEffectTexture;
		private float hitEffectTime;
		public float hitLength;
		public float hitWidth;
		public Vector2 hitEffectOffset = Vector2.zero;
		public AudioClip hitSound;
		public bool hitEffectFoldout;


		public int crosshairRange = 200;

		public static bool crosshair = true;

		void Awake()
		{
			DefaultCrosshair();
			sclRef = 1;
			crosshair = true;
			lineStyle = new GUIStyle();
			lineStyle.normal.background = crosshairTexture;
		}

		//Right now this script fires a raycast every frame
		//This might impact performance, and is an area to consider when optimizing
		void Update()
		{
			if (!PlayerWeapons.playerActive)
			{
				if (cObj)
					cObj.GetComponent<Renderer>().enabled = false;
				return;
			}
			else if (cObj)
			{
				cObj.GetComponent<Renderer>().enabled = true;
			}
			if (cObj != null)
			{
				if (crosshair && ownTexture)
				{
					cObj.GetComponent<Renderer>().enabled = true;
				}
				else
				{
					cObj.GetComponent<Renderer>().enabled = false;
				}
			}
			float temp;
			float temp2;
			if (!scl)
			{
				temp = 1;
				temp2 = 1 / Screen.width;
			}
			else
			{
				temp = GunScript.crosshairSpread;
				temp = temp / 180;
				temp = temp * GunScript.weaponCam.GetComponent<Camera>().fieldOfView;
				temp = temp / Screen.height;
				temp = temp / sclRef;
				temp2 = cSize * temp;
			}
			if (cObj != null)
			{
				if (scl)
				{
					cObj.transform.localScale = new Vector3(Mathf.Clamp(temp2, minimumSize, maximumSize), 1, Mathf.Clamp(temp2, minimumSize, maximumSize));
				}
				else
				{
					cObj.transform.localScale = new Vector3(cSize, 1, cSize);
				}
			}

			RaycastHit hit;
			LayerMask layerMask = 1 << PlayerWeapons.playerLayer;
			layerMask = ~layerMask;
			Vector3 direction = transform.TransformDirection(new Vector3(0, 0, 1));
			if (Physics.Raycast(transform.position, direction, out hit, crosshairRange, layerMask))
			{
				if (hit.collider && hit.transform.gameObject.GetComponent<CrosshairColor>() != null && (hit.distance <= colorDist || colorDist < 0))
				{
					CrosshairColor colorScript = hit.transform.gameObject.GetComponent<CrosshairColor>();
					if (colorScript.crosshairType == crosshairTypes.Friend)
					{
						ChangeColor("Friend");
					}
					else if (colorScript.crosshairType == crosshairTypes.Foe)
					{
						ChangeColor("Foe");
					}
					else if (colorScript.crosshairType == crosshairTypes.Other)
					{
						ChangeColor("Other");
					}
				}
				else
				{
					ChangeColor(""); //Any string not recognized by ChangeColor is the default color
				}
			}
			else
			{
				ChangeColor("");
			}

			if (hitEffectTime <= 0)
			{
				hitEffectOn = false;
			}
		}

		void OnGUI()
		{
			if (!PlayerWeapons.playerActive)
			{
				return;
			}
			GUI.color = Color.white;
			if (!ownTexture)
			{
				float distance1 = GunScript.crosshairSpread;
				if (!(distance1 > (Screen.height / 2)) && (crosshair || debug || displayWhenAiming))
				{

					GUI.Box(new Rect((Screen.width - distance1) / 2 - length1, (Screen.height - width1) / 2, length1, width1), textu, lineStyle);
					GUI.Box(new Rect((Screen.width + distance1) / 2, (Screen.height - width1) / 2, length1, width1), textu, lineStyle);

					GUI.Box(new Rect((Screen.width - width1) / 2, (Screen.height - distance1) / 2 - length1, width1, length1), textu, lineStyle);
					GUI.Box(new Rect((Screen.width - width1) / 2, (Screen.height + distance1) / 2, width1, length1), textu, lineStyle);
				}
			}
			if (hitEffectOn)
			{
				hitEffectTime -= Time.deltaTime * .5f;
				GUI.color = new Color(1, 1, 1, hitEffectTime);
				GUI.DrawTexture(new Rect((Screen.width - hitEffectOffset.x) / 2 - hitLength / 2, (Screen.height - hitEffectOffset.y) / 2 - hitWidth / 2, hitLength, hitWidth), hitEffectTexture);
			}
		}

		public void ChangeColor(string targetStatus)
		{
			if (targetStatus == "Friend")
			{
				lineStyle.normal.background = friendTexture;
			}
			else if (targetStatus == "Foe")
			{
				lineStyle.normal.background = foeTexture;
			}
			else if (targetStatus == "Other")
			{
				lineStyle.normal.background = otherTexture;
			}
			else
			{
				lineStyle.normal.background = crosshairTexture;
			}
		}

		public void Aiming()
		{
			crosshair = false;
		}

		public void NormalSpeed()
		{
			crosshair = true;
		}

		public void Sprinting()
		{
			crosshair = false;
		}

		public void SetCrosshair()
		{
			if (cObj != null)
			{
				cObj.GetComponent<Renderer>().enabled = false;
			}
		}

		public void DefaultCrosshair()
		{
			if (cObj != null)
			{
				cObj.GetComponent<Renderer>().enabled = false;
			}
			ownTexture = useTexture;
			if (crosshairObj != null)
			{
				cObj = crosshairObj;
			}
			if (scale)
			{
				cSize = maximumSize;
			}
			else
			{
				cSize = crosshairSize;
			}
			scl = scale;
		}

		public void HitEffect()
		{
			hitEffectOn = true;
			hitEffectTime = 1;
			if (GetComponent<AudioSource>() && !GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().clip = hitSound;
				GetComponent<AudioSource>().Play();
			}
		}
	}
}