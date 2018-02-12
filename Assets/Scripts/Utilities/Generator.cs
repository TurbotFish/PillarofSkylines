//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Game.Utilities
//{
//    public static class Generator
//    {
//        private static System.Random random = new System.Random();

//        /// <summary>
//        /// Generates a string filled with random characters.
//        /// </summary>
//        /// <param name="length">the length of the string to generate</param>
//        /// <returns></returns>
//        public static string GenerateRandomString(int length)
//        {
//            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

//            var result = new Char[length];

//            for (int i = 0; i < length; i++)
//            {
//                result[i] = chars[random.Next(length)];
//            }

//            return new string(result);
//        }
//    }
//}