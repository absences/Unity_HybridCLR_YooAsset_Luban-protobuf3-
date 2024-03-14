using System;
using UnityEngine;


public sealed partial class UIComponent : BaseGameComponent
{
    [Serializable]
    private sealed class UIGroup
    {
        [SerializeField]
        private string m_Name = null;

        [SerializeField]
        private int m_Depth = 0;

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public int Depth
        {
            get
            {
                return m_Depth;
            }
        }
    }
}

