namespace MikyM.Common.DataAccessLayer.Helpers;

internal static class SharedState
{
    internal static bool DisableOnBeforeSaveChanges { get; set; } = false;
}