namespace SGHSS_Backend.Utils;

public class Utils
{
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
