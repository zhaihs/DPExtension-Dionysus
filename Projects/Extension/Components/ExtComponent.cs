using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Coroutines;

namespace Extension.Components
{
    [Serializable]
    public class ExtComponent<TExt> : Component, IGameObject where TExt : class, IExtension
    {
        public ExtComponent(TExt owner, int id, string name) : base(id)
        {
            _owner = owner;
            Name = name;

            _unstartedComponents = new List<Component>();
            _unstartedComponents.Add(this);

            _coroutineSystem = new CoroutineSystem();
        }

        public TExt Owner => _owner;
        public event Action OnAwake;

        public override void Awake()
        {
            base.Awake();

            OnAwake?.Invoke();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_unstartedComponents.Count > 0)
            {
                Component.ForeachComponents(_unstartedComponents, c => c.EnsureStarted());
                _unstartedComponents.Clear();
            }

            _coroutineSystem.Update();
        }

        /// <summary>
        /// return myself and ensure awaked
        /// </summary>
        /// <returns></returns>
        public ExtComponent<TExt> GetAwaked()
        {
            EnsureAwaked();

            return this;
        }

        public void CreateScriptComponent<T>(int id, string description, params object[] parameters) where T : Scriptable<TExt>
        {
            this.CreateScriptComponent(typeof(T).Name, id, description, parameters);
        }

        public void CreateScriptComponent<T>(string description, params object[] parameters) where T : Scriptable<TExt>
        {
            CreateScriptComponent<T>(NO_ID, description, parameters);
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _coroutineSystem.StartCoroutine(coroutine);
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            _coroutineSystem.StopCoroutine(coroutine);
        }

        void IGameObject.AddComponent(Component component)
        {
            AddComponentEx(component, this);
        }

        void IGameObject.RemoveComponent(Component component)
        {
            component.DetachFromParent();
        }

        ExtensionReference<TExt> _owner;
        List<Component> _unstartedComponents;
        private CoroutineSystem _coroutineSystem;
    }
}
