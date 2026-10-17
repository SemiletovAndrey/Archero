using System;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public class ScreenEntry : ScreenEntryBase
    {
        public GameObject Prefab;

        public override bool HasSource => Prefab != null;
    }
}