
namespace DimensionalTag
{

    public partial class PortalConnectionService
    {
#if ANDROID || WINDOWS

        public partial object? GetConnection();

#endif
    }
}
