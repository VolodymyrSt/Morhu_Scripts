using UnityEngine;

namespace Morhu.Infrustructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper _bootstrapperPrefab;

        private void Awake() => InitializeBootstrapper();

        private void InitializeBootstrapper()
        {
            var bootstrapper = FindFirstObjectByType<GameBootstrapper>();

            if (bootstrapper == null)
                Instantiate(_bootstrapperPrefab);
        }
    }
}
