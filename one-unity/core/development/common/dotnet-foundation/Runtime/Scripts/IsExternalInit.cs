#if CSHARP_9_COMPLETE
#else
using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class IsExternalInit
    {
    }
}
#endif
