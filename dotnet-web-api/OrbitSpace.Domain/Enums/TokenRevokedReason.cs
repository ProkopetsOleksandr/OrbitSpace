namespace OrbitSpace.Domain.Enums
{
    public enum TokenRevokedReason : byte
    {
        UserLogout = 1,
        TokenReuse = 2,           // Reuse detection triggered
        Expired = 3,
        ManualRevocation = 4,     // Admin action
        CompromiseDetected = 5,   // Suspicious activity
        DeviceRemoved = 6,        // User removed trusted device
        PasswordChanged = 7       // Security: revoke all tokens
    }
}
