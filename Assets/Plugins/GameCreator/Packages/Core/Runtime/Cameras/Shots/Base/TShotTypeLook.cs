using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public abstract class TShotTypeLook : TShotType
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected ShotSystemLook m_ShotSystemLook;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Transform[] m_Ignore = new Transform[1];

        // PROPERTIES: ----------------------------------------------------------------------------

        public ShotSystemLook Look => this.m_ShotSystemLook;

        public override Transform Target => this.m_ShotSystemLook.GetLookTarget(this);

        public override Args Args
        {
            get
            {
                this.m_Args ??= new Args(this.m_ShotCamera, null);
                this.m_Args.ChangeTarget(this.Look.GetLookTarget(this));
        
                return this.m_Args;
            }
        }

        public override Transform[] Ignore
        {
            get
            {
                this.m_Ignore[0] = this.Look.GetLookTarget(this);
                return this.m_Ignore;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TShotTypeLook() : base()
        {
            this.m_ShotSystemLook = new ShotSystemLook();
        }
        
        // MAIN METHODS: --------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            this.Look.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            this.Look.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            this.Look.OnDestroy(this);
        }

        protected override void OnAfterUpdate()
        {
            base.OnAfterUpdate();
            this.Look.OnUpdate(this);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            this.Look.OnDisable(this, camera);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            this.Look.OnEnable(this, camera);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void DrawGizmos(Transform transform)
        {
            this.Look.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            this.Look.OnDrawGizmosSelected(this, transform);
        }
    }
}