namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class ObjectBlocker : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject);
    }
}