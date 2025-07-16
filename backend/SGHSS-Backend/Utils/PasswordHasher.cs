using BCrypt.Net;

namespace SGHSS_Backend.Utils;

public static class PasswordHasher
{
    /// <summary>
    /// Gera um hash seguro para uma senha utilizando o algoritmo BCrypt.
    /// </summary>
    /// <param name="password">A senha em texto claro a ser hasheada.</param>
    /// <returns>O hash da senha.</returns>
    public static string HashPassword(string password)
    {
        // BCrypt.Net.BCrypt.HashPassword(password, workFactor)
        // O 'workFactor' (custo) padrão é 10, o que é um bom ponto de partida.
        // Valores maiores aumentam a segurança, mas também o tempo de hash.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifica se uma senha em texto claro corresponde a um hash BCrypt.
    /// </summary>
    /// <param name="password">A senha em texto claro a ser verificada.</param>
    /// <param name="hashedPassword">O hash da senha armazenado.</param>
    /// <returns>True se a senha corresponder ao hash, False caso contrário.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // BCrypt.Net.BCrypt.Verify(password, hashedPassword)
        // Compara a senha fornecida com o hash armazenado.
        // BCrypt gerencia a lógica de salting e iterações internamente.
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
