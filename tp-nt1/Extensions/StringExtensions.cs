using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tp_nt1.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Función para encriptar un texto utilizando el algoritmo de hash <see cref="SHA256Managed"/>.
        /// </summary>
        /// <param name="texto">Texto a encriptar</param>
        /// <returns>Retorna un <see cref="byte[]"/> con el texto encriptado.</returns>
        public static byte[] Encriptar(this string texto)
        {
            return new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(texto));
        }

        public static void ValidarPassword(this string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("La contraseña es requerida.");
            }

            // evaluar criterios de longitud
            if (password.Length < 8)
            {
                throw new Exception("La contraseña debe tener al menos 8 caracteres.");
            }

            // evaluar criterios de seguridad
            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnNumero || !contieneUnaMinuscula || !contieneUnaMayuscula)
            {
                throw new Exception("La contraseña debe contener al menos un número, una minúscula y una mayúscula.");
            }
        }

    }
}
