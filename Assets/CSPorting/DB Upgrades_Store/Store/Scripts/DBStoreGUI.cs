using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class DBStoreGUI : MonoBehaviour
	{
		// Class to hold common GUI Panel Configuration Information
		public class DBStorePanel
		{
			public float x;
			public float y;
			public float width;
			public float height;
			public Rect r;
			public float buttonWidth;
			public float buttonHeight;
			public float navButtonWidth;
			public float navButtonHeight;
			public Texture nextIcon;
			public Texture prevIcon;
			public Texture checkedIcon;
			public Texture lockedIcon;
			public float titleHeight;
			public float xPad;
			public float yPad;
			public string title;
			public Texture titleIcon;
			public string[] content;
			public WeaponInfo[] wi;
			public Texture[] contentIcon;
			public int columns;
			public int rows;
			public int buttonsPerPage;
			public int page = 1;
			public int nPages = 1;
			public int selection;
			public DBStoreController store;
		}

		// Store State Variables
		public enum HeaderModes
		{
			Buy = 0,
			Equip = 1
		};

		public GUISkin skin;
		public Texture checkedIcon;
		public Texture lockedIcon;

		private WeaponClassIcons wcIcons;
		private Texture[] slotIcons = new Texture[10];
		private DBStoreController store;

		//Position of the upper left corner of the store, if negative the store will be centered
		public float sPosX = -1;
		public float sPosY = -1;

		//configuration parameters fpr the Header (H) GUI Component
		public float HWidth = 400;
		public float HHeight = 100;
		public float HYpad = 10;
		public float HButtonWidth = 120;
		public float HButtonHeight = 50;
		public float HTitleHeight = 40;
		public string HTitle = "WEAPONS DEPOT";
		public Texture HTitleImage;


		//Configuration for Left Selection (LS) GUI Component
		public float LSHeight = 300;
		public float LSWidth = 140;
		public float LSButtonHeight = 50.0f;
		public float LSButtonWidth = 120;
		public float LSxPad = 5;
		public float LSyPad = 5;
		public Texture LSNextIcon;
		public Texture LSPrevIcon;

		// Configuration for Main Display (MD) GUI Component
		public float MDButtonWidth = 120;
		public float MDButtonHeight = 50.0f;
		public float MDxPad = 5;
		public float MDyPad = 5;

		private DBStorePanel header = new DBStorePanel();
		private DBStorePanel lsBuy = new DBStorePanel(); // Buy Weapons Left Selection GUI (LS) Component
		private DBStorePanel lsEquip = new DBStorePanel(); // Equip Slots Left Selection (LS) GUI Componenent
		private DBStorePanel mdBuy = new DBStorePanel(); // Buy Weapons Main Display (MD)GUI Component
		private DBStorePanel mdEquip = new DBStorePanel(); // Equip Slot Main Display (MD) GUI Componenet

		private Rect popupRect;
		private string[] MDContent;
		private int MDSelection;

		private int clicked; // temp variable used to track selections
		private bool viewUpgrades = true;

		//variables used for the Weapon Info/Buy/Upgrade Popup Window
		public float popupUpgradeWidth = 500;
		public float popupUpgradeHeight = 275;
		private Rect popupUpgradeRect; //Expanded Rectangle for the popup Buy/Upgrade Window
		private bool popupActive = false;
		private bool popupBuyActive = false;
		private Vector2 popupBuyScroll1;
		private bool[] upgradeSelected = new bool[20]; //allows a maximum of 20 upgrades per weapon

		private Rect popupLockedRect;
		private bool popupLockedActive = false;

		public Texture maskTexture; //Texture that's drawn over the scene when the store is active
		public Texture maskTexturePopup; //Texture that's drawn behind the Buy/Sell/Upgrade window
		private bool upgradeDisplayBuy = true;
		private bool upgradeDisplayEquip = false;
		public Texture upgradeBuyIcon;
		public Texture upgradeInfoIcon;

		public void Start()
		{
			store = FindObjectOfType<DBStoreController>();
			store.Initialize();
			if (sPosX < 0 || sPosY < 0)
			{
				// center the store
				sPosX = Screen.width / 2 - (HWidth + LSWidth) / 2;
				sPosY = Screen.height / 2 - (HHeight + LSHeight) / 2;
			}
			popupUpgradeRect = new Rect(sPosX + 30, sPosY + 60, popupUpgradeWidth, popupUpgradeHeight);
			popupLockedRect = new Rect(sPosX + LSWidth + 50, sPosY + HHeight + 50, 200, 200);
			wcIcons = FindObjectOfType<WeaponClassIcons>();
			setupLS(lsBuy);
			setuplsBuyContent();
			setupLS(lsEquip);
			setuplsEquipContent();
			setupHeader();
			setupMD(mdBuy);
			setupMD(mdEquip);
		}

		public void OnGUI()
		{
			if (!DBStoreController.inStore)
				return;
			//GUI.skin = skin;
			if (DBStoreController.inStore)
			{
				if (maskTexture)
					GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), maskTexture);

				clicked = DisplayHeader(header); //Draw Header Area
				if (!popupActive && clicked != -1)
				{
					header.selection = clicked; // If Popup is Active Don't respond to clicks
					setmdBuyContent(lsBuy.selection);
					setmdEquipContent(lsEquip.selection);
				}
				if (header.selection == (int) HeaderModes.Buy)
				{
					clicked = DisplayLS(lsBuy); // Draw Left Selection Area
					if (clicked != -1 && !popupActive)
					{
						setmdBuyContent(lsBuy.selection);
					}
					clicked = DisplayMD(mdBuy, lsBuy.selection, (HeaderModes) header.selection);
					if (clicked != -1 && mdBuy.wi[clicked].locked)
					{
						mdBuy.selection = clicked;
						popupLockedActive = true;
						popupActive = true;
					}
					else if (clicked != -1 && !popupActive)
					{
						mdBuy.selection = clicked;
						popupActive = true;
						popupBuyActive = true;
					}
				}
				else if (header.selection == (int) HeaderModes.Equip)
				{
					clicked = DisplayLS(lsEquip);
					if (clicked != -1 && !popupActive)
					{
						mdEquip.selection = clicked;
						setmdEquipContent(lsEquip.selection);
					}
					clicked = DisplayMD(mdEquip, lsEquip.selection, (HeaderModes) header.selection);
					if (clicked != -1 && !popupActive)
					{
						mdEquip.selection = clicked;
						popupActive = true;
						popupBuyActive = true;
					}
				}
				if (popupBuyActive)
				{
					if (maskTexture)
						GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), maskTexturePopup);
					popupRect = popupUpgradeRect;
					if (maskTexture)
						GUI.DrawTexture(popupRect, maskTexture);
					GUI.Window(1, popupUpgradeRect, WeaponBuyWindow, "Weapon Info");
				}
				else if (popupLockedActive)
				{
					popupRect = popupLockedRect;
					if (maskTexturePopup)
					{
						GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), maskTexturePopup);
						GUI.DrawTexture(popupRect, maskTexture);
					}
					GUI.Window(0, popupRect, WeaponLockedWindow, "Sorry!!!");
				}
			}
		}

		public void setupMD(DBStorePanel md)
		{
			md.selection = -1;
			md.buttonHeight = MDButtonHeight;
			md.buttonWidth = MDButtonWidth;
			md.xPad = MDxPad;
			md.yPad = MDyPad;
			md.checkedIcon = checkedIcon;
			md.lockedIcon = lockedIcon;
			md.r = new Rect(sPosX + LSWidth, sPosY + HHeight, HWidth, LSHeight);
			md.columns = (int) Mathf.Floor((md.r.width - md.xPad) / (md.buttonWidth + md.xPad));
			md.rows = (int) Mathf.Floor((md.r.height - md.titleHeight - md.yPad) / (md.buttonHeight - md.yPad));
			md.content = null;
			md.store = store;
		}

		//Set the content for the Buy MD panel based on weapon class selected
		public void setmdBuyContent(int sel)
		{
			mdBuy.content = new string[store.WeaponInfoByClass[sel].WeaponInfoArray.Length];
			mdBuy.wi = new WeaponInfo[store.WeaponInfoByClass[sel].WeaponInfoArray.Length];
			for (int i = 0; i < mdBuy.content.Length; i++)
			{
				mdBuy.content[i] = store.WeaponInfoByClass[sel].WeaponInfoArray[i].gunName;
				mdBuy.wi[i] = store.WeaponInfoByClass[sel].WeaponInfoArray[i];
			}
		}

		//set the content for the Equip MD panel based on the slot selected
		//for now it's the same for all slots but we'll be adding the ability to restrict slots to different types of weapons

		public void setmdEquipContent(int slot)
		{
			mdEquip.content = store.getWeaponNamesOwned(slot);
			mdEquip.wi = store.getWeaponsOwned(slot);
		}

		public void setupLS(DBStorePanel ls)
		{
			ls.selection = 0;
			ls.buttonHeight = LSButtonHeight;
			ls.buttonWidth = LSButtonWidth;
			ls.xPad = LSxPad;
			ls.yPad = LSyPad;
			ls.navButtonWidth = ls.buttonWidth / 2.0f;
			ls.navButtonHeight = ls.buttonHeight / 2.0f;
			ls.nextIcon = LSNextIcon;
			ls.prevIcon = LSPrevIcon;
			ls.r = new Rect(sPosX, sPosY + HHeight, LSWidth, LSHeight);
			ls.buttonsPerPage = (int) Mathf.Floor((ls.r.height - ls.titleHeight - ls.navButtonHeight - 2 * ls.yPad) / (ls.buttonHeight + ls.yPad));
			ls.store = store;
		}

		public void setuplsBuyContent()
		{
			lsBuy.title = "Weapon Classes";
			lsBuy.content = new string[store.WeaponInfoByClass.Length];
			lsBuy.contentIcon = new Texture[store.WeaponInfoByClass.Length];

			System.Array.Copy(store.weaponClassNamesPopulated, lsBuy.content, store.WeaponInfoByClass.Length);
			for (int i = 0; i < lsBuy.content.Length; i++)
			{
				lsBuy.content[i] = lsBuy.content[i] + " (" + store.WeaponInfoByClass[i].WeaponInfoArray.Length + ")";
				int ic = (int) store.WeaponInfoByClass[i].WeaponInfoArray[0].weaponClass;
				lsBuy.contentIcon[i] = wcIcons.weaponClassTextures[ic];
			}
			// Why does it use ceil?
			lsBuy.nPages = (int) (Mathf.Ceil(lsBuy.content.Length) / lsBuy.buttonsPerPage);
		}

		public void setuplsEquipContent()
		{
			lsEquip.title = "Weapon Slots";
			lsEquip.content = new string[store.playerW.weapons.Length];
			lsEquip.contentIcon = new Texture[10];
			for (int i = 0; i < lsEquip.content.Length; i++)
			{
				lsEquip.content[i] = store.slotInfo.slotName[i];
				lsEquip.contentIcon[i] = slotIcons[i];
			}
			// Why does it use ceil?
			lsEquip.nPages = (int) (Mathf.Ceil(lsEquip.content.Length) / lsEquip.buttonsPerPage);
		}

		public void setupHeader()
		{
			header.r = new Rect(sPosX + LSWidth, sPosY, HWidth, HHeight);
			header.yPad = HYpad;
			header.content = new string[] {"Buy", "Equip"};
			header.title = HTitle;
			header.titleHeight = HTitleHeight;
			header.buttonHeight = HButtonHeight;
			header.buttonWidth = HButtonWidth;
			header.checkedIcon = checkedIcon;
			header.lockedIcon = lockedIcon;
			header.store = store;
			header.titleIcon = HTitleImage;
		}

		//Function to display the Header Panel for the store

		public static int DisplayHeader(DBStorePanel cfg)
		{
			int clicked = -1;
			Rect rect;
			GUIContent gc;
			GUILayout.BeginArea(cfg.r);
			GUI.Box(new Rect(0, 0, cfg.r.width, cfg.r.height), "");
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			gc = setGUIContent(cfg.titleIcon, cfg.title, "");
			GUILayout.Label(gc, GUILayout.Height(cfg.titleHeight), GUILayout.Width(cfg.titleHeight / cfg.titleIcon.height * cfg.titleIcon.width));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int i = 0; i < cfg.content.Length; i++)
			{
				if (GUILayout.Button(cfg.content[i], GUILayout.Width(cfg.buttonWidth), GUILayout.Height(cfg.buttonHeight)))
					clicked = i;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Balance: $" + cfg.store.getBalance());
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			string msg = "";
			if (cfg.selection == (int) HeaderModes.Buy)
				msg = "Owned";
			if (cfg.selection == (int) HeaderModes.Equip)
				msg = "Equipped";
			GUILayout.BeginHorizontal();
			GUILayout.Label(cfg.checkedIcon, GUILayout.Width(40), GUILayout.Height(20));
			Rect r = GUILayoutUtility.GetLastRect();
			r.x += 25;
			r.width += 15;
			GUI.Label(r, msg);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label(cfg.lockedIcon, GUILayout.Width(40), GUILayout.Height(20));
			r = GUILayoutUtility.GetLastRect();
			r.x += 25;
			r.width += 10;
			GUI.Label(r, "Locked");
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			return clicked;
		}

		public static int DisplayLS(DBStorePanel cfg)
		{
			// function to display Left Selection Box

			int clicked = -1; //local variable to keep track of selections
			int startInt = ((cfg.page - 1) * cfg.buttonsPerPage);
			int endInt = Mathf.Min(startInt + cfg.buttonsPerPage, cfg.content.Length);

			GUILayout.BeginArea(cfg.r);
			GUI.Box(new Rect(0, 0, cfg.r.width, cfg.r.height), "");
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(cfg.title, GUILayout.Height(cfg.titleHeight));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			for (int i = startInt; i < endInt; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (cfg.contentIcon[i])
				{
					if (GUILayout.Button(cfg.contentIcon[i], GUILayout.Width(cfg.buttonWidth), GUILayout.Height(cfg.buttonHeight)))
					{
						clicked = i;
						cfg.selection = i;
					}
				}
				else
				{
					if (GUILayout.Button(cfg.content[i], GUILayout.Width(cfg.buttonWidth), GUILayout.Height(cfg.buttonHeight)))
					{
						clicked = i;
						cfg.selection = i;
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			float bx = cfg.r.width - cfg.navButtonWidth - cfg.xPad;
			float by = cfg.r.height - cfg.navButtonHeight - cfg.yPad;
			if (cfg.page < cfg.nPages)
			{
				if (cfg.nextIcon)
				{
					if (GUI.Button(new Rect(bx, by, cfg.navButtonWidth, cfg.navButtonHeight), cfg.nextIcon))
					{
						cfg.page++;
					}
				}
				else
				{
					if (GUI.Button(new Rect(bx, by, cfg.navButtonWidth, cfg.navButtonHeight), "next"))
					{
						cfg.page++;
					}
				}
			}
			if (cfg.page > 1)
			{
				bx = cfg.xPad / 2.0f;
				if (cfg.prevIcon)
				{
					if (GUI.Button(new Rect(bx, by, cfg.navButtonWidth, cfg.navButtonHeight), cfg.prevIcon))
					{
						cfg.page--;
					}
				}
				else
				{
					if (GUI.Button(new Rect(bx, by, cfg.navButtonWidth, cfg.navButtonHeight), "Prev"))
					{
						cfg.page--;
					}
				}
			}
			GUILayout.EndArea();


			return clicked;
		}

		public static int DisplayMD(DBStorePanel cfg, int sel, HeaderModes mode)
		{
			int clicked = -1;
			string msg; //used to hold temporary string messages
			GUIContent gc;
			GUILayout.BeginArea(cfg.r);
			GUI.Box(new Rect(0, 0, cfg.r.width, cfg.r.height), "");
			if (cfg.content == null)
			{
				GUILayout.EndArea();
				return clicked;
			}
			GUILayout.BeginVertical();
			GUILayout.Space(cfg.yPad);
			if (cfg.content.Length == 0)
			{
				DrawLabelHCenter("Slot: " + cfg.store.slotInfo.slotName[sel]);
				GUILayout.Space(20);
				DrawLabelHCenter("You Don't Own any Weapons For This Slot");
				GUILayout.EndVertical();
				GUILayout.EndArea();
				return clicked;
			}
			else
			{
				if (mode == HeaderModes.Equip)
					DrawLabelHCenter("Slot: " + cfg.store.slotInfo.slotName[sel]);
			}
			int count = 0;
			int cl = -1;
			for (int i = 0; i < cfg.rows; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(cfg.xPad);
				for (int j = 0; j < cfg.columns; j++)
				{
					if (count >= cfg.content.Length) break;

					// TODO: Something is wrong. this is always false.
					if (HeaderModes.Buy != 0 && cfg.wi[count].owned)
					{
						msg = cfg.content[count];
					}
					else
					{
						msg = cfg.content[count] + "\n$" + cfg.wi[count].buyPrice;
					}
					gc = setGUIContent(cfg.wi[count].icon, msg, "");
					if (GUILayout.Button(gc, GUILayout.Width(cfg.buttonWidth), GUILayout.Height(cfg.buttonHeight)))
					{
						clicked = count;
					}
					// now draw the overlay icons if the weapon is owned or locked when in buy mode
					// and equipped in slot in equip mode
					Rect r = GUILayoutUtility.GetLastRect();
					if (mode == HeaderModes.Equip)
					{
						if ((cfg.store.playerW.weapons[sel] == cfg.wi[count].gameObject) && cfg.checkedIcon)
						{
							GUI.Label(r, cfg.checkedIcon);
						}
					}
					else
					{
						if (cfg.wi[count].owned && cfg.checkedIcon)
						{
							GUI.Label(r, cfg.checkedIcon);
						}
						if (cfg.wi[count].locked && cfg.lockedIcon)
						{
							GUI.Label(r, cfg.lockedIcon);
						}
					}
					count++;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
			return clicked;
		}

		// This function draws the popup window to buy or equip a weapon.
		public void WeaponBuyWindow(int windowID)
		{
			WeaponInfo g;
			string msg;
			Rect rLeft = new Rect(5, 20, popupRect.width / 2 - 7, popupRect.height - 25);
			Rect rRight = new Rect(popupRect.width / 2 + 2, 20, popupRect.width / 2 - 7, popupRect.height - 25);
			int slot = lsEquip.selection; // only used for equiping
			if (header.selection == (int) HeaderModes.Buy)
			{
				g = mdBuy.wi[mdBuy.selection];
			}
			else
			{
				g = mdEquip.wi[mdEquip.selection];
			}
			Upgrade[] upgrades = g.getUpgrades();
			//bool[ ] upgradeToggles = new bool[ upgrades.Length];

			GUI.Box(rLeft, "");
			GUILayout.BeginArea(rLeft);
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			if (header.selection == (int) HeaderModes.Buy)
			{
				msg = getBuyMsg(g);
			}
			else
			{
				msg = getEquipMsg(g, slot);
			}

			GUILayout.Label(msg);
			GUILayout.Label("Available Balance = $" + store.getBalance());

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (header.selection == (int) HeaderModes.Equip)
			{
				if (store.playerW.weapons[slot] != g.gameObject)
				{
					if (GUILayout.Button("Equip", GUILayout.Width(65)))
					{
						store.equipWeapon(g, slot);
					}
				}
				else
				{
					if (GUILayout.Button("Un-Equip", GUILayout.Width(65)))
					{
						store.unEquipWeapon(g, slot);
					}
				}
				if (!g.gun.infiniteAmmo)
				{
					if (g.gun.clips < g.gun.maxClips)
					{
						if (store.getBalance() >= g.ammoPrice)
						{
							if (GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Current Ammo: " + g.gun.clips), GUILayout.Width(85)))
							{
								store.BuyAmmo(g);
							}
						}
						else
						{
							GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Can't Afford"), GUILayout.Width(85));
						}
					}
					else
					{
						GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Ammo Full: " + g.gun.clips), GUILayout.Width(85));
					}
				}
			}
			else
			{
				if (g.owned)
				{
					if (g.canBeSold)
					{
						if (GUILayout.Button("Sell", GUILayout.Width(70)))
						{
							store.sellWeapon(g);
						}
					}
					else
					{
						GUILayout.Button("Can't Sell", GUILayout.Width(70));
					}
					if (!g.gun.infiniteAmmo)
					{
						if (g.gun.clips < g.gun.maxClips)
						{
							if (store.getBalance() >= g.ammoPrice)
							{
								if (GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Current Ammo: " + g.gun.clips), GUILayout.Width(85)))
								{
									store.BuyAmmo(g);
								}
							}
							else
							{
								GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Can't Afford"), GUILayout.Width(85));
							}
						}
						else
						{
							GUILayout.Button(new GUIContent("Ammo ($" + g.ammoPrice + ")", "Ammo Full: " + g.gun.clips), GUILayout.Width(85));
						}
					}
				}
				else
				{
					if (store.getBalance() >= g.buyPrice)
					{
						if (GUILayout.Button("Buy", GUILayout.Width(70)))
						{
							store.buyWeapon(g);
						}
					}
					else
					{
						GUILayout.Button(new GUIContent("Buy", "Insufficient Funds"), GUILayout.Width(70));
					}
				}
			}

			if (GUILayout.Button("Close", GUILayout.Width(70)))
			{
				MDSelection = -1;
				popupActive = false;
				popupBuyActive = false;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Label(GUI.tooltip);
			GUILayout.EndVertical();
			GUILayout.Space(5);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();


			// Display for upgrades in the right side of the window
			// The display changes for buying upgrades or equipping upgrades.

			GUI.Box(rRight, "");
			GUILayout.BeginArea(rRight);
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			if (upgrades == null)
			{
				GUILayout.Label("No Upgrades Available for this Weapon");
			}
			else if (upgrades.Length < 0)
			{
				GUILayout.Label("No Upgrades Available for this Weapon");
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				upgradeDisplayBuy = GUILayout.Toggle(upgradeDisplayBuy, "Buy Upgrades");
				if (upgradeDisplayBuy)
				{
					upgradeDisplayEquip = false;
				}
				upgradeDisplayEquip = GUILayout.Toggle(upgradeDisplayEquip, "Equip Upgrades");
				if (upgradeDisplayEquip)
				{
					upgradeDisplayBuy = false;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				if (upgradeDisplayBuy)
				{
					GUILayout.Space(5);
					string upgradeMsg;
					if (g.owned)
					{
						upgradeMsg = "View or Buy Upgrades";
					}
					else
					{
						upgradeMsg = "You must Purchase the Weapon before buying Upgrades";
					}
					GUILayout.Label(upgradeMsg);
					popupBuyScroll1 = GUILayout.BeginScrollView(popupBuyScroll1);
					if (upgrades != null)
					{
						for (int i = 0; i < upgrades.Length; i++)
						{
							if (!upgrades[i].owned)
							{
								GUILayout.BeginHorizontal();
								GUILayout.Label(upgrades[i].upgradeName);
								GUILayout.Space(5);
								if (GUILayout.Button(setGUIContent(upgradeInfoIcon, "View", ""), GUILayout.Width(40), GUILayout.Height(20)))
								{
									if (upgradeSelected[i])
									{
										upgradeSelected[i] = false;
									}
									else
									{
										upgradeSelected[i] = true;
									}
								}
								GUILayout.Space(5);

								GUIContent gc;

								if (upgrades[i].locked)
								{
									GUILayout.Label("(Locked)");
									upgradeMsg = upgrades[i].lockedDescription;
								}
								else
								{
									if (g.owned)
									{
										float balance = store.getBalance();
										if (balance > upgrades[i].buyPrice)
											gc = setGUIContent(upgradeBuyIcon, "Buy", "");
										else
											gc = new GUIContent(upgradeBuyIcon, "Insufficient Funds");

										if (GUILayout.Button(gc, GUILayout.Width(40), GUILayout.Height(20)))
										{
											if (balance > upgrades[i].buyPrice)
												store.buyUpgrade(g, upgrades[i]);
										}
									}
								}
								upgradeMsg = "Price :\t$" + upgrades[i].buyPrice + "\n";
								upgradeMsg += "Description:\t" + upgrades[i].description;
								GUILayout.FlexibleSpace();
								GUILayout.EndHorizontal();
								if (upgradeSelected[i])
								{
									closeOtherSelections(i);
									GUILayout.BeginHorizontal();
									GUILayout.Space(10);
									GUILayout.BeginVertical();
									GUILayout.Label(upgradeMsg);
									GUILayout.EndVertical();
									GUILayout.EndHorizontal();
								}
							} // upgrade not owned
						} // loop over upgrades
					}
					GUILayout.EndScrollView();
					GUILayout.Label(GUI.tooltip);
				}
				else
				{
					// Displaying Equip Window
					GUILayout.Space(5);
					GUILayout.Label("Select Upgrades To Apply - Note: Some upgrades disable others");
					bool before;
					//			bool[ ] upgradesApplied = g.getUpgradesApplied();
					popupBuyScroll1 = GUILayout.BeginScrollView(popupBuyScroll1);
					for (int j = 0; j < upgrades.Length; j++)
					{
						if (upgrades[j].owned)
						{
							before = g.upgradesApplied[j]; // keep track of current state
							g.upgradesApplied[j] = GUILayout.Toggle(g.upgradesApplied[j], upgrades[j].upgradeName);
							if (before != g.upgradesApplied[j])
							{
								if (before)
								{
									upgrades[j].RemoveUpgrade();
								}
								else
								{
									upgrades[j].ApplyUpgrade();
									PlayerWeapons.HideWeaponInstant(); //turn off graphics for applied upgrade
								}
								g.updateApplied();
							}
						}
					}
					GUILayout.EndScrollView();
					GUILayout.Space(8);
				} // Displaying Equip Window
			} //upgrades.Length !=0

			GUILayout.EndVertical();
			GUILayout.Space(5);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		//This function displays popup messages, including the weapon locked message.
		public void WeaponLockedWindow(int windowID)
		{
			WeaponInfo g = mdBuy.wi[mdBuy.selection];
			GUILayout.BeginArea(new Rect(5, 10, popupLockedRect.width - 10, popupLockedRect.height - 20));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Label(g.lockedDescription);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close"))
			{
				MDSelection = -1;
				popupActive = false;
				popupLockedActive = false;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		// This function is used to make the GUILayout.Toggle() function act like a togglegroup for the Buy/Equip popup window.
		public void closeOtherSelections(int sel)
		{
			for (int i = 0; i < upgradeSelected.Length; i++)
			{
				if (i != sel) upgradeSelected[i] = false;
			}
		}

		//the following two functions are used just to cleanup the logic in the Buy/Upgrade popup window
		public string getBuyMsg(WeaponInfo g)
		{
			string msg;
			msg = "Weapon Name: " + g.gunName + "\n\n";
			if (g.owned)
			{
				msg += "Weapon Not Owned\n";
			}
			else
			{
				msg += "Weapon Owned\n";
			}
			msg += "Description: " + g.gunDescription + "\n\n";
			if (g.owned)
			{
				msg += "Sell Price: $" + g.sellPriceUpgraded + "\n";
			}
			else
			{
				msg += "Price: $" + g.buyPrice;
			}
			return msg;
		}

		public string getEquipMsg(WeaponInfo g, int slot)
		{
			string msg;
			msg = "Equipping for Slot " + slot + " \n";
			msg += "Weapon Name " + g.gunName + "\n";
			if (store.playerW.weapons[slot] == g.gameObject)
			{
				msg += "Weapon Equiped in Slot\n";
			}
			else
			{
				msg += "Weapon not Equiped in Slot\n";
			}
			msg += "Description: " + g.gunDescription + "\n";
			if (g.owned)
			{
				msg += "Sell Price: $" + g.sellPriceUpgraded + "\n";
			}
			else
			{
				msg += "Price: $" + g.buyPrice;
			}
			return msg;
		}

		//This utility function just keeps the code a little simpler. It centers a GUILayout Label Horizontally
		public static void DrawLabelHCenter(string s)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(s);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		// Utility function to create GUIContent
		// If texture existing dont set the string value
		// If tool tip is not null set it
		public static GUIContent setGUIContent(Texture t, string label, string tip)
		{
			GUIContent gc;
			if (tip != "")
			{
				if (t)
				{
					gc = new GUIContent(t, tip);
				}
				else
				{
					gc = new GUIContent(label, tip);
				}
			}
			else
			{
				if (t)
				{
					gc = new GUIContent(t);
				}
				else
				{
					gc = new GUIContent(label);
				}
			}
			return gc;
		}
	}
}