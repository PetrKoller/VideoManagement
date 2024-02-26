namespace VideoManagement.Features.Resource;

public class ResourceSigner : IResourceSigner
{
    private const string CustomPolicyTemplate =
        """
        {
           "Statement": [{
              "Resource":"RESOURCE",
              "Condition":{
                 "DateLessThan":{"AWS:EpochTime":END_TIME}
              }
           }]
        }
        """;

    private const string Sha1 = "SHA1";

    private readonly CloudFrontOptions _options;
    private readonly Lazy<XmlDocument> _xmlPrivateKey;

    public ResourceSigner(IOptions<CloudFrontOptions> options)
    {
        _options = options.Value;
        _xmlPrivateKey = new Lazy<XmlDocument>(LoadPrivateKey);
    }

    public SignedResource CreateSignedResource(string resource, string? file = null)
    {
        string resourceUrl = GetResourceUrl(resource);
        string policy = CreatePolicy(resourceUrl, _options.LinkLifetimeInMinutes);
        byte[] policyHash = HashPolicy(policy);
        byte[] signedHash = SignPolicyHash(policyHash);

        return new SignedResource(
            ToUrlSafeBase64String(Encoding.UTF8.GetBytes(policy)),
            ToUrlSafeBase64String(signedHash),
            _options.PublicKeyId,
            file is null ? resourceUrl : GetResourceUrl(file));
    }

    private static string CreatePolicy(string resourceUrl, int linkLifetimeInMinutes)
    {
        var intervalEnd =
            DateTime.UtcNow.Add(TimeSpan.FromMinutes(linkLifetimeInMinutes)) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int endTimestamp = (int)intervalEnd.TotalSeconds;

        return CustomPolicyTemplate
            .Replace("RESOURCE", resourceUrl)
            .Replace("END_TIME", endTimestamp.ToString());
    }

    private static string ToUrlSafeBase64String(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('=', '_')
            .Replace('/', '~');

    private static byte[] HashPolicy(string policy)
        => SHA1.HashData(Encoding.UTF8.GetBytes(policy));

    private byte[] SignPolicyHash(byte[] policyHash)
    {
        var providerRsa = new RSACryptoServiceProvider();
        providerRsa.FromXmlString(_xmlPrivateKey.Value.InnerXml);

        var rsaFormatter = new RSAPKCS1SignatureFormatter(providerRsa);
        rsaFormatter.SetHashAlgorithm(Sha1);

        return rsaFormatter.CreateSignature(policyHash);
    }

    private string GetResourceUrl(string resource) => $"{_options.Url}/{resource}";

    private XmlDocument LoadPrivateKey()
    {
        var xmlPrivateKey = new XmlDocument();
        xmlPrivateKey.Load(_options.PrivateKeyLocation);
        return xmlPrivateKey;
    }
}
