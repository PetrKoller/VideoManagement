namespace VideoManagement.Contracts.Api.V1;

public record SignedResource(
    string Policy,
    string Signature,
    string KeyPairId,
    string Url);
