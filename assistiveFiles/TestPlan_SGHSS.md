# Plano de Testes — SGHSS

Data: 21/10/2025
Escopo: API Back-end (.NET 8)
Ferramenta primária: Insomnia (coleção incluída). Ferramentas opcionais: JMeter/Locust (carga), OWASP ZAP (segurança).

## 1. Estratégia
- Funcionais: validar RFs via coleção Insomnia, cobrindo fluxos felizes e erros comuns.
- Segurança: verificar autenticação JWT, autorização por perfis, e entradas inválidas (sem expor detalhes sensíveis).
- Desempenho (sugestão): cenários de listagem/consulta e criação de consultas sob carga moderada.

## 2. Cenários Funcionais (exemplos)

| Caso | Descrição | Passos | Resultado Esperado |
|------|-----------|--------|--------------------|
| CT001 | Login ADMIN | POST /api/Auth/login (admin@sghss.local/Admin@123) | 200 OK + token JWT válido |
| CT002 | Registrar Paciente válido | POST /api/Auth/register (perfil PACIENTE) | 201/200 OK + usuário/paciente criado |
| CT003 | Registrar Profissional válido | POST /api/Auth/register (perfil PROFISSIONAL) | 201/200 OK + usuário/profissional criado |
| CT004 | Listar Pacientes (ADMIN) | GET /api/Pacientes com Bearer | 200 OK + lista |
| CT005 | Obter Paciente por id (self/ADMIN/PROFISSIONAL autorizado) | GET /api/Pacientes/{id} | 200 OK (ou 403/404 conforme regra) |
| CT006 | Atualizar Paciente (self/ADMIN) | PUT /api/Pacientes/{id} | 200 OK + dados atualizados |
| CT007 | Deletar Paciente (ADMIN) | DELETE /api/Pacientes/{id} | 204 No Content |
| CT008 | Listar Profissionais (ADMIN/PROFISSIONAL) | GET /api/Profissionais | 200 OK |
| CT009 | Atualizar Profissional (self/ADMIN) | PUT /api/Profissionais/{id} | 200 OK |
| CT010 | Deletar Profissional com consultas | DELETE /api/Profissionais/{id} | 409 Conflict |
| CT011 | Criar Consulta sem conflito | POST /api/Consultas | 201 Created |
| CT012 | Criar Consulta em horário conflitante | POST /api/Consultas | 409 Conflict |
| CT013 | Listar Consultas com escopo por perfil | GET /api/Consultas | 200 OK (somente do usuário ou todas se ADMIN) |
| CT014 | Atualizar Consulta (status/regra) | PUT /api/Consultas/{id} | 200 OK (ou 400/409 se regra violada) |
| CT015 | Remover Consulta (ADMIN) | DELETE /api/Consultas/{id} | 204 No Content |

## 3. Casos de Erro/Segurança
- Sem token: endpoints protegidos devem retornar 401.
- Token de perfil incorreto: 403 quando tentar acessar recurso não permitido.
- Payload inválido: 400 Bad Request com mensagem amigável.

## 4. Não Funcionais (sugestões)
- Carga moderada: 10-50 req/s em GET /api/Consultas e POST /api/Consultas por 1-3 minutos (JMeter/Locust).
- Segurança: passagem com OWASP ZAP para vetores comuns (XSS, SQLi). Observação: API usa EF Core com parâmetros, risco de SQLi reduzido; ainda assim testar.

## 5. Aceite
- Todos os casos funcionais prioritários (RF001-RF005) passando.
- Respostas de erro padronizadas e com status HTTP corretos.

## 6. Evidências
- Prints do Insomnia (login, criação, conflitos, autorizações).
- Logs da aplicação (se necessário, anexar trechos relevantes).
