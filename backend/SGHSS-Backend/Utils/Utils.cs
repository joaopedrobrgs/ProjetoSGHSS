namespace SGHSS_Backend.Utils;

public class Utils
{
    public static bool ValidarCpf(string cpf)
    {
        // 1. Remove caracteres não numéricos
        string cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());

        // 2. Verifica se tem 11 dígitos
        if (cpfLimpo.Length != 11)
        {
            return false;
        }

        // 3. Verifica se todos os dígitos são iguais
        if (cpfLimpo.All(c => c == cpfLimpo[0]))
        {
            return false;
        }

        // Arrays para os cálculos
        int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string cpfSemDigitos = cpfLimpo.Substring(0, 9);
        int soma = 0;
        int resto;

        // 4. Cálculo do primeiro dígito verificador
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpfSemDigitos[i].ToString()) * multiplicadores1[i];
        }

        resto = soma % 11;
        int digitoVerificador1 = (resto < 2) ? 0 : 11 - resto;

        // Verifica o primeiro dígito
        if (int.Parse(cpfLimpo[9].ToString()) != digitoVerificador1)
        {
            return false;
        }

        // 5. Cálculo do segundo dígito verificador
        soma = 0;
        string cpfComPrimeiroDigito = cpfSemDigitos + digitoVerificador1;

        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpfComPrimeiroDigito[i].ToString()) * multiplicadores2[i];
        }

        resto = soma % 11;
        int digitoVerificador2 = (resto < 2) ? 0 : 11 - resto;

        // Verifica o segundo dígito
        if (int.Parse(cpfLimpo[10].ToString()) != digitoVerificador2)
        {
            return false;
        }

        // Se passou por todas as verificações, o CPF é válido
        return true;
    }
    //public static async Task WriteLog(String message, string? _fileNameLog = null, Exception? ex = null, string? dir = null, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        Boolean isWebServer = IsWebServer();
    //        await semaphoreLog.WaitAsync(cancellationToken).ConfigureAwait(false);
    //        String
    //            _logPath = "",
    //            _date = DateTime.Now.ToStringBrl("yyyy.MM.dd");

    //        if (String.IsNullOrEmpty(dir))
    //        {
    //            if (IsdotnetCore())
    //                _logPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "log");
    //            else
    //                _logPath = isWebServer ? System.Configuration.ConfigurationManager.AppSettings["LogPath"] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QualityERP", "log");
    //        }
    //        else
    //            _logPath = Path.Combine(dir, "log");


    //        if (!System.IO.Directory.Exists(_logPath))
    //            try
    //            {
    //                System.IO.Directory.CreateDirectory(_logPath);
    //            }
    //            catch
    //            {
    //                _logPath = "";
    //            }

    //        if (!String.IsNullOrWhiteSpace(_logPath))
    //        {
    //            string _path = _logPath +
    //                (
    //                    (
    //                        (_logPath[_logPath.Length - 1] == '\\') ||
    //                        (_logPath[_logPath.Length - 1] == '/')
    //                    ) ?
    //                    "" :
    //                    "\\"
    //                ) +
    //                $"log{(!String.IsNullOrEmpty(_fileNameLog) ? $"-{_fileNameLog}" : null)}_" +
    //                _date +
    //                ".log";

    //            using System.IO.StreamWriter fs = new(
    //                _path,
    //                true
    //            );
    //            if (ex is not null)
    //            {
    //                try
    //                {
    //                    StringBuilder str = new();
    //                    str.AppendLine($"{DateTimeOffset.Now} - Error - {ex.Message}{(!String.IsNullOrEmpty(message) ? $" - {message}" : null)}");
    //                    str.AppendLine($"Stack:");
    //                    str.AppendLine(ex.ToString());
    //                    str.AppendLine("-----------------------------------");
    //                    await fs.WriteLineAsync(str, cancellationToken).ConfigureAwait(false);
    //                    str.Clear();
    //                }
    //                catch { }
    //            }
    //            else
    //                await fs.WriteLineAsync(new StringBuilder(String.Format("{0} - {1}", DateTime.Now.ToStringBrl("dd/MM/yyyy HH:mm:ss"), message)), cancellationToken).ConfigureAwait(false);

    //            await fs.FlushAsync().ConfigureAwait(false);
    //            fs.Close();
    //        }
    //        else { }
    //    }
    //    catch { }
    //    finally
    //    {
    //        semaphoreLog.Release();
    //    }
    //}
}
