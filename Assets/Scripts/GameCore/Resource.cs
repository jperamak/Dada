using UnityEngine;
using System.Collections;

/* This class follows the Typesafe Enum design pattern */
public sealed class Resource {

	//Ranged weapons
	public static readonly Resource BANANA_RANGE = new Resource("BananaZooka", "Weapons/BananaZooka");
	public static readonly Resource CHILI_RANGE = new Resource("Chilling Chili", "Weapons/ChiliZooka");
	public static readonly Resource PHOENIX = new Resource("Phoenix", "Weapons/Phoenix");

	//Melee weapons
	public static readonly Resource CHICKEN_MELEE = new Resource("Chicken Sword", "Weapons/ChickenSword");
	public static readonly Resource KATANA_MELEE = new Resource("Boring Katana", "Weapons/Katana");

	//Heroes
	public static readonly Resource CACTUAR_HERO = new Resource("Cactuar", "Heroes/Cactuar");
	public static readonly Resource MONK_HERO = new Resource("Monk", "Heroes/Monk");
	public static readonly Resource POTATO_HERO = new Resource("Mr. Potato", "Heroes/MrPotato");


	public string Name{get; private set;}
	public string Path{get; private set;}
	private GameObject _objCache;

	private Resource(string name, string path){
		Name = name;
		Path = path;
	}

	//An instance of a resource can conveniently load itself and cache for frequent access
	public GameObject Prefab{get{
			Debug.Log("Getting prefab "+Name+": "+_objCache);
			if(_objCache == null){
				_objCache = Resources.Load(Path) as GameObject;
			}
			return _objCache;
		}
	}

	public static Resource[] MeleeWepons = { Resource.CHICKEN_MELEE, Resource.KATANA_MELEE };
	public static Resource[] RangedWepons = { Resource.BANANA_RANGE, Resource.CHILI_RANGE, Resource.PHOENIX };
	public static Resource[] Heroes = { Resource.POTATO_HERO, Resource.CACTUAR_HERO, Resource.MONK_HERO };
}
