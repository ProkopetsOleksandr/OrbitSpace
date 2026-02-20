namespace OrbitSpace.Application.Common.Models;

public class EmailVerificationMessage(string email, string verificationUrl) : EmailMessage
{
    public override string To { get; } = email;
    public override string Subject => "Verify your email";
    public override string Body { get; } = $"Click to verify: {verificationUrl}";
}