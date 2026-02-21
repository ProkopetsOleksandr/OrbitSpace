namespace OrbitSpace.Application.Email.Templates
{
    public class EmailVerificationTemplate : IEmailTemplate
    {
        public string TemplateName => "EmailVerificationTemplate";
        public required string FirstName { get; init; }
        public required string ConfirmationUrl { get; init; }

        public IReadOnlyDictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                ["FirstName"] = FirstName,
                ["ConfirmationUrl"] = ConfirmationUrl
            };
        }
    }
}
