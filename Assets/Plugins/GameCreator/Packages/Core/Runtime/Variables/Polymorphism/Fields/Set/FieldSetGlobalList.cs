using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldSetGlobalList : TFieldSetVariable
    {
        [SerializeField] 
        protected GlobalListVariables m_Variable;

        [SerializeReference]
        protected IListSetPick m_Select = new SetPickFirst();
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldSetGlobalList(IdString typeID)
        {
            this.m_TypeID = typeID;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Set(object value)
        {
            if (this.m_Variable == null) return;
            m_Variable.Set(this.m_Select, value);
        }
        
        public override object Get()
        {
            if (this.m_Variable == null) return null;
            return m_Variable.Get(this.m_Select);
        }

        public override string ToString() => this.m_Variable != null
            ? $"{m_Variable.name}[{this.m_Select}]"
            : "(none)";
    }
}