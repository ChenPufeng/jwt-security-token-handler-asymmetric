using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace jwt_security_token_handler_asymmetric.Helpers
{
  public class JwtValidation
  {
    private readonly string jwt;
    private readonly string publicKey;
    public JwtValidation(string jwt, string publicKey)
    {
      this.publicKey = publicKey;
      this.jwt = jwt;
    }

    public static void ValidateToken(string jwt, string publicKey) => 
        new JwtValidation(jwt, publicKey).CheckTokenSignature();

    private void CheckTokenSignature()
    {
        var validationParametrs = BuildValidationParameters();
        try
        {
            IdentityModelEventSource.ShowPII = true;
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(jwt, validationParametrs, out _);
        }
        catch (SecurityTokenNoExpirationException ex)
        {
            throw new ArgumentException($"Token exprired. {ex.Message}");
        }
        catch (SecurityTokenInvalidSignatureException e)
        {
            throw new ArgumentException($"Token invalid. {e.Message}");
        }
    }

    private TokenValidationParameters BuildValidationParameters()
    {
        var rsaKey = BuildRsaKey();
        var validationParametrs = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = rsaKey,
        };
        return validationParametrs;
    }

    private RsaSecurityKey BuildRsaKey()
    {
      if (string.IsNullOrEmpty(publicKey))
        throw new ArgumentException("Publickey can not be null");

      var keyBytes = Convert.FromBase64String(publicKey);
      var asymmetricKeyParameter = (RsaKeyParameters)PublicKeyFactory.CreateKey(keyBytes);

      var rsaParameters = new RSAParameters
      {
        Modulus = asymmetricKeyParameter.Modulus.ToByteArrayUnsigned(),
        Exponent = asymmetricKeyParameter.Exponent.ToByteArrayUnsigned()
      };
      var rsa = new RSACryptoServiceProvider();
      rsa.ImportParameters(rsaParameters);
      return new RsaSecurityKey(rsa);
    }
  }
}