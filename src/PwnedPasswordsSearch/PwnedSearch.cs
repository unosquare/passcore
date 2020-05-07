using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace PwnedPasswordsSearch
{
    // Based on https://github.com/mikepound/pwned-search/blob/master/csharp/pwned-search.cs

    public static class PwnedSearch
    {
        /// <summary>
        /// Makes a call to Pwned Passwords API, asking for a set of hashes of publicly known passwords that match a partial hash of a given password.
        /// If any of the hashes returned by the API call fully matches the hash of the plaintext, it would mean that the password has been exposed 
        /// in publicly known data breaches and thus is not safe to use.
        /// See https://haveibeenpwned.com/API/v2#PwnedPasswords
        /// </summary>
        /// <param name="plaintext">Password to check against Pwned Passwords API</param>
        /// <returns>True when the password has been Pwned</returns>
        public static bool IsPwnedPassword(string plaintext)
        {
            try
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(plaintext));

                // Loop through each byte of the hashed data and format each one as a hexadecimal string.
                var sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                string result = sBuilder.ToString().ToUpper();
                //System.Diagnostics.Debug.Print($"The SHA-1 hash of {plaintext} is: {result}");

                // Get a list of all the possible password hashes where the first 5 bytes of the hash are the same
                string url = "https://api.pwnedpasswords.com/range/" + result.Substring(0, 5);
                WebRequest request = WebRequest.Create(url);
                using (Stream response = request.GetResponse().GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(response))
                    {
                        // Iterate through all possible matches and compare the rest of the hash to see if there is a full match
                        string hashToCheck = result.Substring(5);
                        string line = null;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null)
                            {
                                string[] parts = line.Split(':');
                                if (parts[0] == hashToCheck) // This is a full match: plaintext compromised!!!!
                                {
                                    System.Diagnostics.Debug.Print("The password '{plaintext}' is publicly known and can be used in dictionary attacks");
                                    return true;
                                }
                            }
                        } while (line != null);

                        // We've run through all the candidates and none of them is a full match
                        return false; // This plaintext is not publicly known
                    }
                }
            }
            catch (Exception ex)
            {
                // If any weird things happens, it is safer to suppose this plaintext is compromised (hence not to be used).
                return true; // Better safe than sorry.
            }
        }
    }
}