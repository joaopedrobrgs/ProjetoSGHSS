# ProjetoSGHSS
Sistema de Gestão Hospitalar e de Serviços de Saúde (SGHSS) - Projeto Multidisciplinar UNINTER

## Como executar o Back-end

Pré-requisitos:
- .NET 8 SDK
- SQL Server local (connection string em `backend/SGHSS-Backend/appsettings.json`)

Passos (Windows):
1. Configure a chave JWT (recomendado via User Secrets). Em desenvolvimento, a aplicação falha se `Jwt:Key` não existir.
2. No terminal, na raiz do repositório, execute:
	- Restaurar e compilar: `dotnet build backend/SGHSS-Backend/SGHSS-Backend.csproj -c Debug`
	- Rodar com HTTPS (Development): `dotnet run --project backend/SGHSS-Backend/SGHSS-Backend.csproj --launch-profile https`
3. A API ficará disponível em:
	- Swagger UI: https://localhost:7098/swagger
	- Base URL: https://localhost:7098

Seed (usuário ADMIN):
- Email: `admin@sghss.local`
- Senha: `Admin@123`

## Testar rapidamente (Swagger)
1. Abra `https://localhost:7098/swagger`.
2. Faça login em `POST /api/Auth/login` com o ADMIN de seed e copie o `token`.
3. Clique em "Authorize" (canto superior direito), informe `Bearer SEU_TOKEN`.
4. Execute os endpoints desejados (vide regras de autorização no plano de testes).

## Testar com Insomnia
- Coleção completa: `assistiveFiles/Insomnia_SGHSS.json`
  - Ambiente já inclui: `base_url`, `admin_email`, `admin_senha`, `jwt_token`, `idPaciente`, `idProfissional`.
  - Dica: primeiro faça o Login (request Auth/Login), copie o `token` e atribua em `jwt_token` do ambiente.
  - Use `GET /api/Pacientes` e `GET /api/Profissionais` para descobrir IDs e, se necessário, ajuste `idPaciente`/`idProfissional`.

Guia de prints: `assistiveFiles/prints/README.md`
Plano de testes detalhado: `assistiveFiles/TestPlan_SGHSS.md`

## Regras de autorização (resumo)
- ADMIN: acesso total a CRUDs e gestão de relações.
- PROFISSIONAL: atualiza dados de paciente somente se houver RELAÇÃO ATIVA entre o par; agenda/atualiza consultas da própria agenda.
- PACIENTE: acessa e altera apenas os próprios dados (com restrições de campos) e suas consultas.

Relação Profissional–Paciente:
- Criada/reativada automaticamente ao criar uma Consulta entre o par.
- Pode ser ativada/inativada manualmente por ADMIN via `/api/Relacoes`.
- É o único critério para PROFISSIONAL alterar dados de um paciente.
