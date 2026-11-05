using Template.Features.ClassFactory;
using UnityEngine;
using Zenject;

namespace Template.Features.UIExtensions
{
    public class SpawnableElementsListView<TElement> : ElementsListView<TElement> where TElement : MonoBehaviour, IView
    {
        [SerializeField] private TElement _prefab;

        private IClassFactory _classFactory;

        [Inject]
        private void Construct(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }
 
        public TElement Create()
        {
            TElement element = Spawn();
            Add(element);
            return element;
        }
 
        public void Clear()
        {
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                TElement element = Elements[i];
                
                Remove(element);
                Destroy(element.gameObject);
            }
        }
        
        protected virtual TElement Spawn()
        {
            return _classFactory.CreateMonoBehaviour(_prefab, Parent);
        }
    }
}