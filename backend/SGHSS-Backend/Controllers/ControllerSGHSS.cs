using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Services;
using System.Security.Claims;
using static SGHSS_Backend.Models.Enums;

namespace SGHSS_Backend.Controllers;

public class ControllerSGHSS : ControllerBase
{

    protected readonly SGHSSDbContext _context;

    public ControllerSGHSS(SGHSSDbContext context)
    {
        _context = context;
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

        // Retorna o código HTTP apropriado (ex.: 401, 403, 404, 409, etc.)
        return base.StatusCode((int)code, obj);
    }

    protected async Task<Usuario> GetUserLoggedAsync()
    {
        if (User is null || User.Identity is null || !User.Identity.IsAuthenticated)
            throw new CustomException("Usuário não autenticado.", 401);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new CustomException("Id do usuário não encontradas no token.", 403);

        if (!int.TryParse(userIdClaim.Value, out int userId))
            throw new CustomException("Formato do ID do usuário inválido.", 400);

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId) ?? throw new CustomException(null, 404);  // Erro 404: Not Found (Usuário não encontrado) 

        return new Usuario()
        {
            IdUsuario = user.IdUsuario,
            Ativo = user.Ativo,
            Email = user.Email,
            Perfil = user.Perfil,
            Senha = user.Senha,
            DataCriacao = user.DataCriacao,
        };
    }
}
