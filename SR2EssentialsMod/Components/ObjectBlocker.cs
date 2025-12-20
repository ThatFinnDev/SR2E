using SR2E.Storage;

namespace SR2E.Components;

[InjectClass]
internal class ObjectBlocker : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject);
    }
}