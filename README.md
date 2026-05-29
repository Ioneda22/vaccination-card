# Cartão de Vacinação — API REST

API para gerenciar cartões de vacinação: cadastro de vacinas e pessoas, registro de vacinações (com validação de dose), consulta do cartão e exclusão de registros/pessoas.

## Tecnologias

- **.NET 9** / ASP.NET Core Web API
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS** com **MediatR**
- **FluentValidation** (validação via pipeline behavior)
- **Entity Framework Core** (banco **InMemory** — não exige instalação de SGBD)
- **Autenticação JWT** (Bearer)
- **Swagger / OpenAPI**

## Estrutura do projeto

```
src/
├── VaccinationCard.Domain          # Entidades e constantes (regras de domínio)
├── VaccinationCard.Application      # Commands/Queries, Handlers, Validadores, Interfaces
├── VaccinationCard.Infrastructure  # EF Core, Repositórios, JWT, DI da infraestrutura
└── VaccinationCard.API             # Controllers, Program.cs, middleware de exceções
```

---

## 1. Setup e execução

Pré-requisito: **.NET SDK 9**.

```bash
# Na raiz do projeto
dotnet restore
dotnet build

# Rodar a API (perfil HTTP)
dotnet run --project src/VaccinationCard.API --launch-profile http
```

A API sobe em **http://localhost:5087** (e https://localhost:7061 no perfil `https`).

> O banco é **InMemory**: os dados são reiniciados a cada execução. Ideal para testes.

### Swagger

Com a aplicação rodando, abra:

```
http://localhost:5087/swagger
```

No Swagger há o botão **Authorize** — cole o token JWT (sem a palavra `Bearer`) para autorizar as chamadas pela UI.

---

## 2. Autenticação

Todos os endpoints (exceto o login) exigem um token **JWT Bearer**.

Credenciais de demonstração: **`admin` / `admin`**.

### `POST /api/auth/login`

Request:
```json
{ "userName": "admin", "password": "admin" }
```

Response `200 OK`:
```json
{ "accessToken": "eyJhbGciOiJIUzI1NiI...", "tokenType": "Bearer" }
```

Use o token nas demais chamadas no header:
```
Authorization: Bearer <accessToken>
```

---

## 3. Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/auth/login` | Autentica e retorna o token JWT |
| `POST` | `/api/vaccines` | Cadastra uma vacina |
| `POST` | `/api/people` | Cadastra uma pessoa |
| `DELETE` | `/api/people/{personId}` | Remove a pessoa e todo o seu cartão (cascata) |
| `GET` | `/api/people/{personId}/vaccination-card` | Consulta o cartão de vacinação |
| `POST` | `/api/people/{personId}/vaccination-records` | Registra uma vacinação |
| `DELETE` | `/api/people/{personId}/vaccination-records/{recordId}` | Exclui um registro de vacinação |

### Doses aceitas (validadas pelo sistema)

`Single`, `First`, `Second`, `Third`, `Booster` (case-insensitive).

---

## 4. Tutorial — fluxo completo (passo a passo)

Os exemplos usam `curl`. Substitua os IDs pelos retornados em cada passo.

### Passo 1 — Login
```bash
curl http://localhost:5087/api/auth/login \
  -X POST -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"admin"}'
```
Guarde o `accessToken`. Nos próximos passos usaremos `-H "Authorization: Bearer <TOKEN>"`.

### Passo 2 — Cadastrar uma vacina
```bash
curl http://localhost:5087/api/vaccines \
  -X POST -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" \
  -d '{"name":"COVID-19 (Pfizer)"}'
```
Resposta `201 Created`: `{ "id": "<vaccineId>" }`

### Passo 3 — Cadastrar uma pessoa
```bash
curl http://localhost:5087/api/people \
  -X POST -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" \
  -d '{"name":"Maria Silva","identificationNumber":"12345678900"}'
```
Resposta `201 Created`: `{ "id": "<personId>" }`

### Passo 4 — Registrar uma vacinação
```bash
curl http://localhost:5087/api/people/<personId>/vaccination-records \
  -X POST -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" \
  -d '{"vaccineId":"<vaccineId>","dose":"First","applicationDate":"2024-03-10T00:00:00Z"}'
```
Resposta `201 Created`: `{ "id": "<recordId>" }`

### Passo 5 — Consultar o cartão de vacinação
```bash
curl http://localhost:5087/api/people/<personId>/vaccination-card \
  -H "Authorization: Bearer <TOKEN>"
```
Resposta `200 OK`:
```json
{
  "personId": "...",
  "personName": "Maria Silva",
  "identificationNumber": "12345678900",
  "vaccinations": [
    {
      "recordId": "...",
      "vaccineId": "...",
      "vaccineName": "COVID-19 (Pfizer)",
      "dose": "First",
      "applicationDate": "2024-03-10T00:00:00Z"
    }
  ]
}
```

### Passo 6 — Excluir um registro de vacinação
```bash
curl http://localhost:5087/api/people/<personId>/vaccination-records/<recordId> \
  -X DELETE -H "Authorization: Bearer <TOKEN>"
```
Resposta `204 No Content`.

### Passo 7 — Remover a pessoa (apaga o cartão em cascata)
```bash
curl http://localhost:5087/api/people/<personId> \
  -X DELETE -H "Authorization: Bearer <TOKEN>"
```
Resposta `204 No Content`.

---

## 5. Exemplos de body das requisições

**Criar vacina** — `POST /api/vaccines`
```json
{ "name": "Tétano" }
```

**Criar pessoa** — `POST /api/people`
```json
{ "name": "João Souza", "identificationNumber": "98765432100" }
```

**Registrar vacinação** — `POST /api/people/{personId}/vaccination-records`
```json
{
  "vaccineId": "d3f1f7a0-0000-0000-0000-000000000000",
  "dose": "First",
  "applicationDate": "2024-03-10T00:00:00Z"
}
```

---

## 6. Testes de validação (casos de erro)

A API responde com **ProblemDetails** (RFC 7807) e o status HTTP apropriado.

| Cenário | Status esperado |
|---------|-----------------|
| Chamar qualquer endpoint sem token | `401 Unauthorized` |
| Login com credenciais erradas | `401 Unauthorized` |
| Dose inválida (ex.: `"Quinta"`) | `400 Bad Request` |
| Data de aplicação no futuro | `400 Bad Request` |
| Nome obrigatório vazio | `400 Bad Request` |
| Número de identificação duplicado | `409 Conflict` |
| Vacina inexistente no registro | `404 Not Found` |
| Cartão de pessoa inexistente | `404 Not Found` |
| Excluir registro inexistente | `404 Not Found` |

### Exemplos

**Dose inválida** → `400`
```bash
curl http://localhost:5087/api/people/<personId>/vaccination-records \
  -X POST -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" \
  -d '{"vaccineId":"<vaccineId>","dose":"Quinta","applicationDate":"2024-01-01T00:00:00Z"}'
```
```json
{
  "title": "Um ou mais erros de validação ocorreram.",
  "status": 400,
  "errors": {
    "Dose": ["Dose inválida. Os valores aceitos são: Single, First, Second, Third, Booster."]
  }
}
```

**Data futura** → `400`
```json
{ "status": 400, "errors": { "ApplicationDate": ["A data de aplicação não pode ser uma data futura."] } }
```

**Identificação duplicada** → `409`
```json
{ "status": 409, "title": "Conflito.", "detail": "Já existe uma pessoa com o número de identificação '12345678900'." }
```

**Vacina inexistente** → `404`
```json
{ "status": 404, "title": "Recurso não encontrado.", "detail": "\"Vaccine\" com identificador '...' não foi encontrado(a)." }
```

---

## 7. Decisões arquiteturais

- **Clean Architecture + CQRS:** separa regras de negócio (Application/Domain) da infraestrutura e da API, facilitando testes e manutenção. Cada caso de uso é um Command/Query com seu Handler.
- **Contratos da API separados dos Commands:** os `Request` records em `API/Contracts` isolam a interface HTTP da lógica interna, permitindo evoluí-las de forma independente.
- **Validação via pipeline (ValidationBehavior):** todo Command passa pelos validadores do FluentValidation antes do Handler — centraliza a validação e mantém os handlers limpos.
- **Tratamento global de exceções:** `GlobalExceptionHandler` traduz exceções de domínio (`NotFoundException`, `ConflictException`) e de validação para respostas HTTP padronizadas (ProblemDetails).
- **`Id` (GUID) separado do número de identificação:** o número de identificação tem significado de negócio e é único, mas a chave primária é um GUID técnico — boa prática para não acoplar a PK a um dado de domínio.
- **Cascata vs. restrição:** excluir uma pessoa apaga seus registros de vacinação (cascade); uma vacina não pode ser excluída se houver registros associados (restrict).
- **Banco InMemory:** escolhido para o desafio rodar sem dependências externas. A camada de repositórios/`IUnitOfWork` permite trocar por um provedor real (SQL Server, PostgreSQL) alterando apenas a Infrastructure.

---

## 8. Próximos passos sugeridos

- Projeto de **testes unitários** (xUnit) para os Handlers e validadores.
- Endpoints de listagem de vacinas e pessoas (`GET`).
- Persistência real (SQL Server/PostgreSQL) via EF Core + migrations.
- Gestão de usuários real para a autenticação (hoje é um login fixo de demonstração).
