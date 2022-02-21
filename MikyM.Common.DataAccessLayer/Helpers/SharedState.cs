namespace MikyM.Common.DataAccessLayer.Helpers;

internal static class SharedState
{
    internal static bool DisableOnBeforeChanges { get; set; } = false;
}