# SGHSS – Instruções para Copilot (Resumo e Próximos Passos)

Este documento resume o estado atual do projeto SGHSS (Sistema de Gestão Hospitalar e de Serviços de Saúde) e lista, de forma priorizada, tudo que ainda deve ser feito. Use-o como referência rápida para continuação do desenvolvimento.

## Visão geral do projeto

- Objetivo: API REST para gestão de pacientes, profissionais de saúde e consultas, com autenticação/autorização via JWT.
- Stack:
  - .NET 8 (ASP.NET Core Web API)
  - C#
  - Entity Framework Core + SQL Server
  - Autenticação JWT
  - Hash de senha com BCrypt
- Estrutura principal (pasta `backend/SGHSS-Backend/`):
  - `Program.cs`: bootstrap do app, DI, DbContext, Auth JWT.
  - `Data/`:
    - `SGHSSDbContext.cs`: mapeamentos EF.
    - `Entities/`: `Usuario`, `Paciente`, `Profissional`, `Consulta`.
  - `Controllers/`: `AuthController`, `PacientesController`, base `ControllerSGHSS`.
  - `Services/`: `AuthService`, `PacienteService`.
  - `DTOs/`: Auth (Login/Register + payloads), Pacientes (Response/UpdateRequest).
  - `Utils/`: `JwtTokenGenerator`, `PasswordHasher`.
  - `Models/`: `ApiResponse`, `Enums`, `CustomExceptions`.

## Modelo de dados (resumo)

Entidades e relacionamentos (conforme `SGHSSDbContext`):
- `Usuario` (1) — (0..1) `Paciente` e (0..1) `Profissional`
  - Unique: `Email`
- `Paciente` (N) — (1) `Usuario`; (1) — (N) `Consulta`
  - Unique: `Cpf`
- `Profissional` (N) — (1) `Usuario`; (1) — (N) `Consulta`
  - Unique: `CrmOuConselho`
- `Consulta` — FK para `Paciente` e `Profissional`
- Regras de deleção:
  - `Usuario` -> `Paciente` e `Profissional`: Cascade
  - `Paciente`/`Profissional` -> `Consulta`: Restrict

Perfis de usuário (roles) atuais: `PACIENTE`, `PROFISSIONAL`, `ADMIN`.

## Superfície da API implementada hoje

- Autenticação (`/api/auth`):
  - POST `/login` – Autentica usuário; retorna JWT e dados básicos.
  - POST `/register` – Cria `Usuario` e, se perfil for `PACIENTE` ou `PROFISSIONAL`, cria o respectivo registro associado com validações de unicidade (`Email`, `Cpf`, `CrmOuConselho`). Retorna JWT.
- Pacientes (`/api/Pacientes`): [Requer JWT]
  - GET `/` – Lista todos os pacientes. Roles: `ADMIN, PROFISSIONAL`.
  - GET `/{id}` – Retorna paciente por ID. Roles: `ADMIN, PROFISSIONAL, PACIENTE` (paciente só pode acessar o próprio).
  - PUT `/{id}` – Atualiza campos do paciente (parcial). Roles: `ADMIN, PROFISSIONAL, PACIENTE`.
    - Restrições: `PACIENTE` não pode alterar `HistoricoClinico` nem `Convenio`.
  - DELETE `/{id}` – Exclui paciente (deleta `Usuario` associado, em cascata deleta o `Paciente`). Role: `ADMIN`.

Observações:
- Controlador base `ControllerSGHSS` fornece `GetUserLoggedAsync()` (busca usuário do token) e tratamento de erro padronizado via `CustomErrorRequestAsync`.
- `AuthService` usa `PasswordHasher` (BCrypt) e `JwtTokenGenerator`.

## Configuração atual (importante)

Arquivo `appsettings.json`:
- ConnectionStrings: usa `DefaultConnection` para SQL Server local.
- Jwt: possui `Issuer`, `Audience` e `ExpiresInMinutes`, mas FALTAM dois pontos cruciais:
  1) Chave `Jwt:Key` (secreta para assinar tokens) – obrigatória.
  2) Comentário em JSON: existe comentário ao lado de `ExpiresInMinutes` que invalida o JSON. Precisa ser removido.

Program.cs:
- DI para `AuthService`, `PacienteService`, `JwtTokenGenerator`.
- Autenticação JWT configurada exigindo `Issuer`, `Audience` e `Key`.
- Falta configuração de Swagger e CORS.

## Como rodar localmente (Windows + SQL Server)

1) Pré-requisitos:
- .NET SDK 8.x
- SQL Server rodando e database acessível.

2) Configurar `appsettings.json`:
- Ajuste a connection string conforme seu SQL Server.
- Adicione a chave JWT e remova comentários:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SGHSS_DB;Integrated Security=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "SGHSS_API",
    "Audience": "SGHSS_Users",
    "Key": "SUA_CHAVE_SECRETA_LONGA_E_ALEATORIA",
    "ExpiresInMinutes": 60
  }
}
```

3) Criar banco e aplicar migrações (se não existir):
- Adicionar migração inicial (se não houver pasta `Migrations/`).
- Atualizar banco com `dotnet ef database update` (requer pacote de ferramentas EF instalado globalmente).

4) Executar API:
- Inicie o projeto `backend/SGHSS-Backend` (perfil Development). A API deve escutar em HTTPS.

5) Testar rapidamente:
- POST `/api/auth/register` – crie um usuário.
- POST `/api/auth/login` – obtenha um JWT.
- Use o JWT no header Authorization: `Bearer {token}` para chamar `/api/Pacientes`.

## Lacunas e backlog (priorizado)

Alta prioridade (corrigir para rodar):
1) appsettings.json inválido por comentário; remover. Adicionar `Jwt:Key` (obrigatório).
2) Habilitar Swagger para inspeção e testes rápidos:
   - `builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();`
   - `app.UseSwagger(); app.UseSwaggerUI();`
3) Criar e aplicar migrações EF Core (ausentes no repo) e validar schema no SQL Server.

Média prioridade (entregar escopo mínimo do trabalho):
4) Endpoints de Profissionais:
   - CRUD básico (lista, detalhe, update com regras, delete com checagem de `Consultas`).
   - DTOs: `ProfissionalResponse`, `ProfissionalUpdateRequest`, filtros.
5) Endpoints de Consultas:
   - Criar consulta (vincular `Paciente` e `Profissional`, checar disponibilidade), listar (por paciente, por profissional, e geral/ADMIN), atualizar status (`Agendada`, `Concluída`, `Cancelada`), cancelar.
   - DTOs: `ConsultaCreateRequest`, `ConsultaUpdateRequest`, `ConsultaResponse`.
   - Regras: impedir sobreposição de horários para o mesmo profissional; respeitar `DisponibilidadeAgenda`.
6) Correção do tratamento de erros HTTP:
   - `CustomErrorRequestAsync` sempre retorna 400; ajustar para retornar o `Code` correto (401/403/404/409 etc.).
   - Adotar `ProblemDetails` opcionalmente.
7) Melhorar autorização por perfil:
   - Clarificar nomenclatura (comentários mencionam `PROFISSIONAL_SAUDE`, código usa `PROFISSIONAL`). Unificar.
   - Validar escopos de alteração de `Paciente` (confirmar se ADMIN/PROFISSIONAL podem alterar todos os campos e se PACIENTE pode alterar apenas dados pessoais).

Boa prioridade (qualidade e UX):
8) CORS: habilitar para o(s) host(s) do frontend.
9) Paginação e filtros em listagens (ex.: pacientes por nome/CPF, profissionais por especialidade).
10) Validações de update com unicidade:
    - Ex.: ao atualizar `Cpf` de `Paciente` ou `CrmOuConselho` de `Profissional`, garantir uniqueness.
11) Seeds de dados (usuário ADMIN) para facilitar testes.
12) Logs: integrar `ILogger<T>` (em vez de utilitário comentado) + correlação.
13) Documentação: expandir `README.md` com instruções de build/run, exemplos e políticas de roles.

Opcional / extensão (se exigido pela disciplina):
14) Recuperação de senha/alteração de senha e verificação de e-mail.
15) Soft-delete e trilhas de auditoria (quem alterou o quê, quando).
16) Exportação/relatórios (ex.: consultas por período/profissional).
17) Testes automatizados (unit/integration) cobrindo serviços e controllers.

## Contratos e exemplos de payload

- Login
  - POST `/api/auth/login`
  - Body:
    ```json
    { "email": "user@email.com", "senha": "123456" }
    ```
  - 200 OK:
    ```json
    { "token": "...", "idUsuario": 1, "email": "user@email.com", "perfil": "ADMIN" }
    ```

- Registro
  - POST `/api/auth/register`
  - Body (PACIENTE):
    ```json
    {
      "email": "paciente@dominio.com",
      "senha": "123456",
      "perfil": "PACIENTE",
      "pacienteData": {
        "nomeCompleto": "Fulano da Silva",
        "dataNascimento": "1990-01-01",
        "cpf": "00000000000",
        "telefone": "(41) 99999-9999",
        "endereco": "Rua X, 123",
        "historicoClinico": "...",
        "rg": "MG-00.000.000",
        "sexo": "M",
        "convenio": "Plano Y"
      }
    }
    ```

## Decisões e observações importantes

- Segurança:
  - Não versionar `Jwt:Key` real; usar secret manager/variáveis de ambiente em produção.
  - BCrypt já é usado para senhas (ok).
- Exclusão de paciente: o serviço apaga o `Usuario` (cascade remove o `Paciente`). Alinhar essa decisão com regras de negócio (soft-delete pode ser preferível).
- Restrições de atualização: pacientes não podem alterar histórico clínico e convênio (já implementado).
- Índices/uniqueness: definidos em model builder (Email, Cpf, CrmOuConselho).

## Qualidade (gates rápidos)

- Build: não verificado aqui. Atenção: `appsettings.json` inválido por comentário; em runtime a configuração vai falhar até corrigir.
- Lint/Typecheck: não aplicável diretamente.
- Testes: inexistentes no repo no momento.

---

Se continuar deste ponto, comece corrigindo `appsettings.json`, habilite Swagger e crie os módulos de Profissionais e Consultas com DTOs, validações e regras de autorização. Em seguida, adicione migrações EF, seeds e documentação.
