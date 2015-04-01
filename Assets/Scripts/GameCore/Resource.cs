using UnityEngine;
using System.Collections;

/* This class follows the Typesafe Enum design pattern */
public sealed class Resource {

	//Ranged weapons
	public static readonly Resource BANANA_RANGE = new Resource("BananaZooka", "Weapons/BananaZooka");
	public static readonly Resource CHILI_RANGE = new Resource("Chilling Chili", "Weapons/ChiliZooka");
	public static readonly Resource PHOENIX = new Resource("Phoenix", "Weapons/Phoenix");
	public static readonly Resource FIRE_RANGE = new Resource("Aaaaah!", "Weapons/FireStaff");

	//Melee weapons
	public static readonly Resource LAYBOMB_MELEE = new Resource("Frog Lover", "Weapons/BombLay");
	public static readonly Resource ICY_MELEE = new Resource("Ice Cream", "Weapons/IcyKatana");
	public static readonly Resource FLAME_MELEE = new Resource("Hot Teaspoon", "Weapons/FlameKatana");
	public static readonly Resource ELECTRIC_MELEE = new Resource("Electric Deer", "Weapons/ElectricKatana");

	//Heroes
	//public static readonly Resource CACTUAR_HERO = new Resource("Cactuar", "Heroes/Cactuar");
	public static readonly Resource MONK_HERO = new Resource("Monk", "Heroes/Monk");
	//public static readonly Resource POTATO_HERO = new Resource("Mr. Potato", "Heroes/MrPotato");

    //Different monks
    public static readonly Resource MONK_HERO_B = new Resource("Monk Blue", "Heroes/Monk Blue");
    public static readonly Resource MONK_HERO_R = new Resource("Monk Red", "Heroes/Monk Red");
    public static readonly Resource MONK_HERO_G = new Resource("Monk Green", "Heroes/Monk Green");
    public static readonly Resource MONK_HERO_Y = new Resource("Monk Yellow", "Heroes/Monk Yellow");


	//Teams
    public static readonly Resource TEAM_1 = new Resource("The Bastards", "");
    public static readonly Resource TEAM_2 = new Resource("team_name", "");
    public static readonly Resource TEAM_3 = new Resource("Curry Soup", "");
    public static readonly Resource TEAM_4 = new Resource("Dolly Blues", "");

	public string Name{get; private set;}
	public string Path{get; private set;}
	private GameObject _objCache;

	private Resource(string name, string path){
		Name = name;
		Path = path;
	}

	//An instance of a resource can conveniently load itself and cache for frequent access
	public GameObject Prefab{get{
			if(_objCache == null){
				_objCache = Resources.Load(Path) as GameObject;
			}
			return _objCache;
		}
	}

	public static Resource[] MeleeWepons = { Resource.LAYBOMB_MELEE, Resource.ICY_MELEE, Resource.FLAME_MELEE, Resource.ELECTRIC_MELEE };
	public static Resource[] RangedWepons = { Resource.BANANA_RANGE, Resource.CHILI_RANGE, Resource.PHOENIX, Resource.FIRE_RANGE };
	public static Resource[] Heroes = { Resource.MONK_HERO };
    public static Resource[] Teams = { Resource.TEAM_1, Resource.TEAM_2, Resource.TEAM_3, Resource.TEAM_4 };
}
