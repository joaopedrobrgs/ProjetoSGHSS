# SGHSS — Guia de Prints (Insomnia)

Use a coleção em `assistiveFiles/Insomnia_SGHSS.json`.
1) Login ADMIN (POST /api/auth/login) — salve print do 200 com token.
2) GET /api/Pacientes sem Bearer — salve 401.
3) GET /api/Pacientes com Bearer ADMIN — salve 200 (lista).
4) Registrar PACIENTE e PROFISSIONAL — salve 201 e corpo.
5) PUT /api/Pacientes/{id} com token PROFISSIONAL (sem relação) — salve 403.
6) POST /api/Consultas (criar relação) — salve 201.
7) PUT /api/Pacientes/{id} com token PROFISSIONAL (com relação) — salve 200.
8) PUT /api/Relacoes/inativar — salve 200.
9) PUT /api/Pacientes/{id} com relação inativa — salve 403.
10) POST /api/Relacoes/ativar — salve 200.
11) PUT /api/Pacientes/{id} após reativar — salve 200.

Dica: usa variáveis de ambiente para tokens (adminToken, profToken, pacToken) e ids (idPaciente, idProfissional) para agilizar os testes.