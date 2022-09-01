using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts
{
    class Prefabs
    {
        public static readonly Pop Pop = AssetDatabase.LoadAssetAtPath<Pop>("Assets/Prefabs/Pop.prefab");
    }
}
