using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using SGHSS_Backend.Models.Exceptions;
using static SGHSS_Backend.Models.Enums;

namespace SGHSS_Backend.Controllers;

public class ControllerSGHSS : ControllerBase
{
    public ControllerSGHSS()
    {
    }

    protected async ValueTask<ObjectResult> CustomErrorRequestAsync(Exception ex, IDbContextTransaction? transaction = null)
    {
        bool rollback = CustomException.DefaultRollback;
        bool writeLog = CustomException.DefaultWritelog;
        StatusEnum status = CustomException.DefaultStatus;
        long code = CustomException.DefaultCode;
        string message = CustomException.DefaultMessage;
        string title = CustomException.DefaultTitle;

        if (ex is CustomException _ex)
        {
            rollback = _ex.Rollback;
            writeLog = _ex.Writelog;
            status = _ex.Status;
            title = _ex.Title;
            message = _ex.Message;
            code = _ex.Code;
        }

        if (transaction is not null && rollback)
            await transaction.RollbackAsync().ConfigureAwait(false);

        //if (writeLog)
        //    Utils.WriteLog(ex.Message, null, ex);

        var obj = new
        {
            status,
            code,
            title,
            message
        };

        return base.BadRequest(obj);
    }
}
