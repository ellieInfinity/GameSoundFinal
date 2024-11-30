using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private DoorController _linkedDoor;
    [SerializeField] public GameObject _target;
    
    public static bool InFight;

    public Transform LinkedDoor => _linkedDoor._target.transform;
}
