using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectPool
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        #region constructors

        public GameObjectPool(GameObject prefab, int preloadCount
        ) : base(() => Preload(prefab), GetAction, ReturnAction, preloadCount)
        { }

        #endregion
        
        #region public Methods

        public static GameObject Preload(GameObject prefab) => Object.Instantiate(prefab);
        public static void GetAction(GameObject @object) => @object.gameObject.SetActive(true);
        public static void ReturnAction(GameObject @object) => @object.gameObject.SetActive(false);

        #endregion
    }
}