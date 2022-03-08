using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Characters
{
    [Title("Point & Click")]
    [Image(typeof(IconLocationDrop), ColorTheme.Type.Green)]

    [Category("Point & Click")]
    [Description(
        "Moves the Player where the pointer's position clicks from the Main Camera's perspective"
    )]
    
    [Serializable]
    public class UnitPlayerPointClick : TUnitPlayer
    {
        private const int BUFFER_SIZE = 32;
        private const int LAYER_UI = 1 << 5;

        // RAYCAST COMPARER: ----------------------------------------------------------------------
        
        private static readonly RaycastComparer RAYCAST_COMPARER = new RaycastComparer();
        
        private class RaycastComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b)
            {
                return a.distance.CompareTo(b.distance);
            }
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        // Changed on 29/11/2021
        [FormerlySerializedAs("m_MoveButton")] [SerializeField] 
        private InputPropertyButton m_InputMove; 
        
        [SerializeField]
        private LayerMask m_LayerMask;

        [SerializeField]
        private PropertyGetInstantiate m_Indicator;

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private RaycastHit[] m_HitBuffer;

        // INITIALIZERS: --------------------------------------------------------------------------

        public UnitPlayerPointClick()
        {
            this.m_LayerMask = -1;
            this.m_Indicator = new PropertyGetInstantiate
            {
                usePooling = true,
                size = 5,
                hasDuration = true,
                duration = 1f
            };

            this.m_InputMove = InputButtonMousePress.Create();
        }
        
        public override void OnStartup(Character character)
        {
            base.OnStartup(character);
            this.m_InputMove.OnStartup();
        }
        
        public override void OnDispose(Character character)
        {
            base.OnDispose(character);
            this.m_InputMove.OnDispose();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            this.m_HitBuffer = new RaycastHit[BUFFER_SIZE];
            
            this.m_InputMove.Enable();
            this.m_InputMove.RegisterPerform(this.OnPointClick);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_HitBuffer = Array.Empty<RaycastHit>();
            
            this.m_InputMove.ForgetPerform(this.OnPointClick);
            this.m_InputMove.Disable();
            
            this.Character.Motion?.MoveToDirection(Vector3.zero, Space.World, 0);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public override void OnUpdate()
        {
            base.OnUpdate();
            this.m_InputMove.OnUpdate();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnPointClick()
        {
            if (!this.Character.IsPlayer) return;
            if (!this.m_IsControllable) return;

            Camera camera = ShortcutMainCamera.Get<Camera>();
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            int hitCount = Physics.RaycastNonAlloc(
                ray, this.m_HitBuffer,
                Mathf.Infinity, this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
            
            Array.Sort(this.m_HitBuffer, 0, hitCount, RAYCAST_COMPARER);

            for (int i = 0; i < hitCount; ++i)
            {
                int colliderLayer = this.m_HitBuffer[i].transform.gameObject.layer; 
                if ((colliderLayer & LAYER_UI) > 0) return;
                
                if (this.m_HitBuffer[i].transform.IsChildOf(this.Transform)) continue;

                this.m_Indicator.Get(
                    this.Character.gameObject,
                    this.m_HitBuffer[i].point,
                    Quaternion.identity
                );

                Vector3 point = this.m_HitBuffer[i].point;
                Location location = new Location(point);
                
                this.InputDirection = Vector3.Scale(
                    point - this.Character.transform.position, 
                    Vector3Plane.NormalUp
                );

                GameObject user = this.Character.gameObject;
                Vector3 locationPosition = location.GetPosition(user);
                
                Debug.DrawLine(locationPosition, locationPosition + Vector3.up * 5f);
                this.Character.Motion?.MoveToLocation(location, 0.1f, null, 0);
                return;
            }
        }

        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Point & Click";
    }
}