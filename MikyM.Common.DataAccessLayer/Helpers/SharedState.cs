namespace MikyM.Common.DataAccessLayer.Helpers;

internal static class SharedState
{
    internal static bool DisableAuditEntries { get; set; } = false;
}