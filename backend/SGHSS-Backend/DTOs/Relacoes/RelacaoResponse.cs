namespace SGHSS_Backend.DTOs.Relacoes;

public class RelacaoResponse
{
    public int IdRelacao { get; set; }
    public int IdProfissional { get; set; }
    public int IdPaciente { get; set; }
    public string StatusRelacao { get; set; }
}
