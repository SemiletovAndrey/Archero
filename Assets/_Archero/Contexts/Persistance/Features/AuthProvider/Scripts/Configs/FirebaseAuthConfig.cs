using Template.Features.Config.Local;
using UnityEngine;

namespace Persistence.Features.AuthProvider
{
    [CreateAssetMenu(fileName = "Config_Firebase_Auth_Feature", menuName = "SO/Persistence/AuthProvider/Config")]
    public class FirebaseAuthConfig : BaseLocalFeatureConfig
    {
        [field: TextArea][field: SerializeField] public string WebClientId {get; private set;}
    }
}