namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class ObjectBlocker : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject);
    }
}