using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    public bool Team1 = false;
    public bool Team2 = false;
    public bool Team3 = false;
    public bool Team4 = false;
    private bool[] _teams = new bool[4];

    void Start()
    {
        _teams[0] = Team1;
        _teams[1] = Team2;
        _teams[2] = Team3;
        _teams[3] = Team4;

        if (!Team1 && !Team2 && !Team3 && !Team4)
        {
            _teams[0] = true;
            _teams[1] = true;
            _teams[2] = true;
            _teams[3] = true;
        }
    }


    public bool IsValidForTeam(int team)
    {
        return _teams[team];
    }

}
