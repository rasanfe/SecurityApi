namespace SecurityApi.Services
{
    /// <summary>
    /// Contrato del generador/validador de claves robustas. Se registra como Singleton
    /// (ver Program.cs): una sola instancia para toda la app.
    /// </summary>
    public interface IKeyGeneratorService
    {
        /// <summary>Comprueba si <paramref name="password"/> cumple las reglas de complejidad.</summary>
        bool Validate(string password);

        /// <summary>Genera una clave nueva que cumple esas reglas.</summary>
        string Generate();

        /// <summary>Ajusta la longitud mínima exigida a las claves.</summary>
        void SetTotalChars(int totalCharacters);
    }
}
