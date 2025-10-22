# SGHSS - Plano de Testes (Back-end)

Data: 2025-10-21
Escopo: API Back-end (.NET 8)
Ferramenta primária: Insomnia (coleção incluída). Opcionais: Postman Runner, JMeter/Locust (carga), OWASP ZAP (segurança).

## Papéis e regra de autorização
- ADMIN: acesso total a CRUDs e gestão de relações.
- PROFISSIONAL: acesso a listagens e operações na própria agenda; só pode atualizar dados de paciente se houver RELAÇÃO ATIVA com o paciente.
- PACIENTE: acesso apenas aos próprios dados e consultas.

Relação Profissional–Paciente:
- Criada/reativada automaticamente ao criar uma Consulta entre o par.
- Pode ser ativada/inativada manualmente por ADMIN via /api/Relacoes.
- É o único critério para PROFISSIONAL alterar dados de um paciente (sem fallback por consultas).

## Endpoints principais
Base URL (Dev):
- HTTPS: https://localhost:7098
- HTTP: http://localhost:5054 (redireciona para HTTPS)

Cabeçalhos comuns:
- Authorization: Bearer <TOKEN>
- Content-Type: application/json

### Auth
- POST /api/auth/login
- POST /api/auth/register

Exemplo (login ADMIN):
Request:
POST /api/auth/login
{
	"email": "admin@sghss.local",
	"senha": "Admin@123"
}
Response 200:
{
	"token": "<jwt>",
	"idUsuario": 1,
	"email": "admin@sghss.local",
	"perfil": "ADMIN"
}

### Pacientes
- GET /api/Pacientes (ADMIN, PROFISSIONAL)
- GET /api/Pacientes/{id} (ADMIN, PROFISSIONAL, PACIENTE-próprio)
- PUT /api/Pacientes/{id} (ADMIN, PROFISSIONAL [relação ativa], PACIENTE-próprio com restrições)
- DELETE /api/Pacientes/{id} (ADMIN)

Validações:
- CPF válido e único (409), RG único (409)
- PROFISSIONAL precisa de relação ATIVA para atualizar (403 se ausente/inativa)

### Consultas
- POST /api/Consultas (cria/reativa relação automaticamente)
- GET /api/Consultas (ADMIN todas; PROFISSIONAL as suas; PACIENTE as suas)
- GET /api/Consultas/{id}
- PUT /api/Consultas/{id}
- DELETE /api/Consultas/{id} (ADMIN)

### Relações
- POST /api/Relacoes/ativar (ADMIN)
- PUT /api/Relacoes/inativar (ADMIN)

## Casos de Teste (CT)
| ID | Descrição | Perfil | Endpoint | Entrada | Resultado Esperado |
|----|-----------|--------|----------|---------|--------------------|
| CT-LOGIN-ADMIN-200 | Login como ADMIN | — | POST /api/auth/login | email/senha válidos | 200 + JWT |
| CT-UNAUTH-401 | Acesso sem token | — | GET /api/Pacientes | — | 401 Unauthorized |
| CT-LIST-PAC-200 | Listar pacientes | ADMIN | GET /api/Pacientes | — | 200 OK (lista) |
| CT-REG-PAC-201 | Registrar paciente | — | POST /api/auth/register | Perfil=PACIENTE + dados válidos | 201 Created + JWT |
| CT-REG-PAC-409 | CPF duplicado | — | POST /api/auth/register | CPF já usado | 409 Conflict |
| CT-REG-PROF-201 | Registrar profissional | — | POST /api/auth/register | Perfil=PROFISSIONAL + dados válidos | 201 Created + JWT |
| CT-PUT-PAC-403-NR | Profissional sem relação atualiza paciente | PROFISSIONAL | PUT /api/Pacientes/{id} | Update simples | 403 Forbidden |
| CT-CONSULTA-201 | Criar consulta (cria relação) | ADMIN/PROF/PAC | POST /api/Consultas | par prof-paciente | 201 Created + relação ativa |
| CT-PUT-PAC-200-R | Atualizar com relação ativa | PROFISSIONAL | PUT /api/Pacientes/{id} | Update simples | 200 OK |
| CT-REL-INAT-200 | Inativar relação | ADMIN | PUT /api/Relacoes/inativar | ids do par | 200 OK |
| CT-PUT-PAC-403-INAT | Atualizar com relação inativa | PROFISSIONAL | PUT /api/Pacientes/{id} | Update | 403 Forbidden |
| CT-REL-ATIV-200 | Ativar relação | ADMIN | POST /api/Relacoes/ativar | ids do par | 200 OK |
| CT-PUT-PAC-200-ATV | Atualizar após reativar | PROFISSIONAL | PUT /api/Pacientes/{id} | Update | 200 OK |

## Fluxo sugerido (execução guiada)
1) Login ADMIN e guardar token.
2) Registrar PACIENTE (CPF válido) e PROFISSIONAL (CPF válido).
3) Como ADMIN, obter IDs via GET /api/Pacientes e /api/Profissionais (filtrar por EmailUsuario).
4) Como PROFISSIONAL, tentar PUT no paciente recém-criado (403 esperado).
5) Criar consulta (ADMIN, PROF ou PAC) entre o par para ativar relação.
6) Repetir PUT como PROFISSIONAL (200 esperado).
7) Inativar relação (ADMIN) e verificar PUT volta a 403.
8) Reativar relação (ADMIN) e verificar PUT volta a 200.

## Exemplos de Payloads
- Registro PACIENTE:
{
	"email": "paciente+<uniq>@sghss.local",
	"senha": "Senha@123",
	"perfil": "PACIENTE",
	"pacienteData": {
		"nomeCompleto": "Paciente Teste",
		"dataNascimento": "1990-05-10",
		"cpf": "<CPF_VALIDO>",
		"telefone": "11999999999",
		"endereco": "Rua X, 123",
		"historicoClinico": "—",
		"rg": "RG123<uniq>",
		"sexo": "M",
		"convenio": "Particular",
		"emailPaciente": "paciente+<uniq>@mail.com"
	}
}

- Registro PROFISSIONAL:
{
	"email": "prof+<uniq>@sghss.local",
	"senha": "Senha@123",
	"perfil": "PROFISSIONAL",
	"profissionalData": {
		"nomeCompleto": "Dr. Teste",
		"cpf": "<CPF_VALIDO>",
		"rg": "PRG<uniq>",
		"crmOuConselho": "CRM<uniq>",
		"especialidade": "Clínico Geral",
		"telefone": "11988888888",
		"emailProfissional": "prof+<uniq>@mail.com",
		"disponibilidadeAgenda": "Seg-Sex 9-17h"
	}
}

- Criar Consulta (ADMIN):
{
	"idPaciente": <idPaciente>,
	"idProfissional": <idProfissional>,
	"dataHoraConsulta": "2025-11-20T14:00:00Z",
	"tipoConsulta": "Presencial",
	"observacoes": "Consulta de rotina"
}

- Ativar/Inativar Relação (ADMIN):
{
	"idProfissional": <idProfissional>,
	"idPaciente": <idPaciente>
}

## Evidências (prints)
- Login (ADMIN) – 200
- GET /api/Pacientes sem token – 401
- GET /api/Pacientes com token ADMIN – 200
- PUT /api/Pacientes/{id} com PROFISSIONAL sem relação – 403
- POST /api/Consultas – 201 (mostra pares)
- PUT /api/Pacientes/{id} com PROFISSIONAL com relação – 200
- PUT /api/Relacoes/inativar – 200
- PUT /api/Pacientes/{id} com relação inativa – 403
- POST /api/Relacoes/ativar – 200
- PUT /api/Pacientes/{id} após ativar – 200

Dica: use a coleção Insomnia (full) já inclusa em assistiveFiles; preencha os bodies e tokens conforme acima e capture os screenshots.
