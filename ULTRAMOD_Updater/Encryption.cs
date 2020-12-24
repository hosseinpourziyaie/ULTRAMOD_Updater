/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : Encryption.cs Code
** - Description : Encryption Methud
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** https://stackoverflow.com/questions/11873878/c-sharp-encryption-to-php-decryption
**  
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace ULTRAMOD_Updater
{
    public static class Encryption
    {
        public static string Encrypt(string prm_text_to_encrypt, string prm_key, string prm_iv)
        {
            var sToEncrypt = prm_text_to_encrypt;

            var rj = new RijndaelManaged()
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256,
            };

            var encoding = new UTF8Encoding();
            var key = encoding.GetBytes(prm_key);
            var IV = encoding.GetBytes(prm_iv);

            var encryptor = rj.CreateEncryptor(key, IV);

            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            var toEncrypt = Encoding.ASCII.GetBytes(sToEncrypt);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encrypted = msEncrypt.ToArray();

            return (Convert.ToBase64String(encrypted));
        }
    }
}