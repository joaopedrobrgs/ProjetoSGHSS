# Postman Runner – SGHSS

Este pacote permite executar todo o fluxo de teste em 1 clique usando o Postman Runner.

Conteúdo:
- SGHSS.postman_collection.json – coleção com os requests encadeados
- SGHSS.postman_environment.json – ambiente com variáveis (baseUrl, credenciais ADMIN, etc.)

## Como usar

1) Importar no Postman
- Abra o Postman > Import.
- Importe os dois arquivos deste diretório.

2) Verificar a API em execução
- Rode a API localmente (em um terminal separado):
  - HTTPS (Swagger em https://localhost:7098):
    - dotnet run --project backend/SGHSS-Backend/SGHSS-Backend.csproj --launch-profile https
  - ou HTTP (http://localhost:5054):
    - dotnet run --project backend/SGHSS-Backend/SGHSS-Backend.csproj --launch-profile http
- No Postman, abra o ambiente “SGHSS Local” e ajuste `baseUrl` conforme o perfil escolhido (http ou https).

3) Executar no Runner (um clique)
- Selecione a coleção “SGHSS Runner”.
- Clique em “Run collection”.
- Escolha o ambiente “SGHSS Local”.
- Deixe a ordem padrão e execute.

O que acontece:
- Login como ADMIN (captura token_admin)
- Registro de um PACIENTE (gera CPF/RG/e-mail válidos automaticamente)
- Registro de um PROFISSIONAL (gera CPF/RG/CRM e e-mail válidos automaticamente)
- ADMIN lista Pacientes e Profissionais e captura `idPaciente` e `idProfissional`
- Login do PROFISSIONAL (captura token_prof)
- ADMIN ativa a relação PROFISSIONAL↔PACIENTE
- PROFISSIONAL atualiza o Paciente (PUT)
- PROFISSIONAL cria uma Consulta para esse Paciente (POST)

4) Evidências (prints)
- Faça prints dos resultados no Runner (cada request mostra o status e resposta), ou abra os requests e use o botão “Send” para ver o body completo.

## Relatório HTML com Newman (opcional, 1 comando)

Gere um relatório bonitão em HTML com o htmlextra.

1) Instalar dependências (primeira vez)

```bash
cd "assistiveFiles/postman"
npm install
```

2) Rodar a coleção e gerar relatório

```bash
# Certifique-se de que a API está rodando e o baseUrl confere (http/https)
cd "assistiveFiles/postman"
npm run test:report
```

Saída:
- O relatório será salvo em `assistiveFiles/postman/reports/SGHSS-Runner-Report.html`.
- Abra o HTML no navegador para anexar no PDF final conforme as diretrizes.

## Notas importantes
- O gerador de CPF nos requests de registro cria um CPF válido a cada execução, evitando conflitos.
- Se quiser repetir o fluxo, basta rodar o Runner novamente (novos e-mails/CPFs serão criados automaticamente).
- Caso use HTTPS, confie no certificado de desenvolvimento:
  - dotnet dev-certs https --trust
- Credenciais seed (ADMIN): admin@sghss.local / Admin@123
