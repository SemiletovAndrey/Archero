using System;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public class ScreenEntry
    {
        public ScreenType Type;
        public GameObject Prefab;
    }
}