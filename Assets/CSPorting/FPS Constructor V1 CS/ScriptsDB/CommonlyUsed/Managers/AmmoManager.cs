using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AmmoManager : MonoBehaviour
	{
		public string[] tempNamesArray = {"Ammo Set 1", "Ammo Set 2", "Ammo Set 3", "Ammo Set 4", "Ammo Set 5", "Ammo Set 6", "Ammo Set 7", "Ammo Set 8", "Ammo Set 9", "Ammo Set 10"};
		public string[] namesArray = new string[10];
		public int[] clipsArray = new int[10];
		public int[] maxClipsArray = new int[10];
		public bool[] infiniteArray = new bool[10];
	}
}