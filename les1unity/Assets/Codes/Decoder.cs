// using UnityEngine;
// using System;
// using System.Text;
// using System.Collections;
// public class Decoder : MonoBehaviour
// {
//     public static string GetUserIdFromToken(string token)
//     {
//         try
//         {
//             string[] parts = token.Split('.'); // Splits token in 3 delen
//             if (parts.Length < 2)
//             {
//                 Debug.LogError("❌ Ongeldig JWT-token!");
//                 return null;
//             }
//
//             string payload = parts[1]; // Pak het middelste deel (Payload)
//             payload = payload.Replace('-', '+').Replace('_', '/'); // Base64 correctie
//
//             switch (payload.Length % 4) // Fix padding issues
//             {
//                 case 2: payload += "=="; break;
//                 case 3: payload += "="; break;
//             }
//
//             string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload)); // Decode Base64
//             Debug.Log($"✅ JWT Payload: {json}"); // Debug de payload
//
//             JwtPayload jwtData = JsonUtility.FromJson<JwtPayload>(json);
//             return jwtData.sub; // De "sub" claim is meestal de User ID
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("❌ Kan de User ID niet uit token halen! " + ex.Message);
//             return null;
//         }
//     }
//
//     [Serializable]
//     private class JwtPayload
//     {
//         public string sub; // De User ID
//     }
// }
//
