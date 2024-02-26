namespace VideoManagement.Features.Resource;

/// <summary>
/// Creates a signed resource for an access to a file via CloudFront CDN.
/// </summary>
public interface IResourceSigner
{
    SignedResource CreateSignedResource(string resource, string? file = null);
}
