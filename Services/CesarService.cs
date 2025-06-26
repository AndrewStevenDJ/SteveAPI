namespace GeometriaAPI.Service
{
    public class CesarService
    {
        public string Encriptar(string mensaje, int desplazamiento)
        {
            return ProcesarMensaje(mensaje, desplazamiento);
        }

        public string Desencriptar(string mensaje, int desplazamiento)
        {
            return ProcesarMensaje(mensaje, -desplazamiento);
        }

        private string ProcesarMensaje(string mensaje, int desplazamiento)
        {
            char[] buffer = mensaje.ToCharArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];

                if (char.IsLetter(c))
                {
                    char d = char.IsUpper(c) ? 'A' : 'a';
                    buffer[i] = (char)(((c + desplazamiento - d + 26) % 26) + d);
                }
            }

            return new string(buffer);
        }
    }
}
