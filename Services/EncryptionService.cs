﻿using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace JwtCookiesScheme.Services
{
    public class EncryptionService:IEncryptionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _dataProtector;

        public EncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _dataProtector = _dataProtectionProvider.CreateProtector("MyApp.Purpose");
        }

        public string DecryptData(string cipherText)
        {
            return _dataProtector.Unprotect(cipherText);
        }

        public string EncryptData(string plainText)
        {
            return _dataProtector.Protect(plainText);

        }
    }
}
