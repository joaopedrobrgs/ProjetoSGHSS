using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static SGHSS_Backend.Models.Enums;

namespace SGHSS_Backend.Models.Exceptions;

[Serializable]
public class CustomException : Exception
{
    public bool Rollback { get; set; }
    public bool Writelog { get; set; }
    public string Title { get; set; }
    public StatusEnum Status { get; set; }
    public long Code { get; set; }

    public const string DefaultMessage = "Não foi possível concluir sua ação. Caso persista, entre em contato com o suporte.";
    public const bool DefaultRollback = true;
    public const bool DefaultWritelog = true;
    public const string DefaultTitle = "Erro ao processar a requisição.";
    public const StatusEnum DefaultStatus = StatusEnum.Error;
    public const long DefaultCode = 400;

    public CustomException(string? message = null)
        : this(message, null, null, null, null, null, null) { }

    public CustomException(string? message = null, long? code = null)
        : this(message, code, null, null, null, null, null) { }

    public CustomException(string? message = null, long? code = null, bool? rollback = null)
    : this(message, code, rollback, null, null, null, null) { }

    public CustomException(string? message = null, long? code = null, bool? rollback = null, bool? writeLog = null)
: this(message, code, rollback, writeLog, null, null, null) { }

    public CustomException(string? message = null, long? code = null, bool? rollback = null, bool? writeLog = null, string? title = null)
    : this(message, code, rollback, writeLog, title, null, null) { }

    public CustomException(string? message = null, long? code = null, bool? rollback = null, bool? writeLog = null, string? title = null, StatusEnum? status = null)
: this(message, code, rollback, writeLog, title, status, null) { }

    public CustomException(string? message = null, long? code = null, bool? rollback = null, bool? writeLog = null, string? title = null, StatusEnum? status = null, Exception? innerException = null)
    : base(message ?? DefaultMessage, innerException)
    {
        Rollback = rollback ?? DefaultRollback;
        Writelog = writeLog ?? DefaultWritelog;
        Title = title ?? DefaultTitle;
        Status = status ?? DefaultStatus;
        Code = code ?? DefaultCode;
    }

    protected CustomException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Rollback = DefaultRollback;
        Writelog = DefaultWritelog;
        Title = DefaultTitle;
        Status = DefaultStatus;
        Code = DefaultCode;
    }
}

