using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MenuMusic : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }
}
