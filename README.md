# Cartão de Vacinação

Segue a descrição do projeto [aqui](https://gist.github.com/DouglasLutz/aa25728e3a438dc966490870f03cc770/).

---

## Tecnologias

**Backend**

- .NET 9 / ASP.NET Core Web API
- Clean Architecture + CQRS (MediatR)
- FluentValidation
- Entity Framework Core (provedor InMemory)
- Autenticação JWT (Bearer)
- Swagger / OpenAPI
- xUnit (testes)

**Frontend**

- React 18 + TypeScript
- Vite
- Tailwind CSS
- React Router, React Hook Form, Axios

---

## Pré-requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) e npm

---

## Como rodar

Há duas formas: com **Docker** (um único comando) ou **manualmente** (dois terminais).

### Opção A — Docker (recomendado para avaliar rápido)

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/) (no Windows, com o backend WSL2).

```bash
# Na raiz do projeto
docker compose up --build
```

- App: **http://localhost:5173** (login `admin` / `admin`)
- Swagger: **http://localhost:5087/swagger**

O frontend (nginx) faz proxy de `/api` para o container da API, então tudo
trafega pela mesma origem — não é preciso configurar `.env` nem CORS. Para
parar: `Ctrl+C` e, opcionalmente, `docker compose down`.

> Não é preciso ter .NET ou Node instalados nesta opção — apenas o Docker.

### Opção B — Manual

A aplicação tem duas partes. Use **dois terminais** (um para a API, outro para o frontend).

#### 1. Backend (API)

```bash
# Na raiz do projeto
dotnet run --project src/VaccinationCard.API --launch-profile http
```

- API: **http://localhost:5087**
- Swagger: **http://localhost:5087/swagger**

> O banco é **InMemory**: os dados são reiniciados a cada execução. No startup,
> ele é populado automaticamente com dados de demonstração (ver
> [Dados de demonstração](#dados-de-demonstração)).

#### 2. Frontend

```bash
cd frontend
npm install        # apenas na primeira vez
npm run dev
```

- App: **http://localhost:5173**
- A URL da API é configurável em `frontend/.env` (`VITE_API_BASE_URL`).

#### 3. Acessar

Abra **http://localhost:5173** e faça login com as credenciais de demonstração:

| Utilizador | Senha   |
| ---------- | ------- |
| `admin`    | `admin` |

---

## Como rodar os testes

```bash
dotnet test
```

---

## Endpoints da API

URL base: `http://localhost:5087/api`. Todos exigem JWT Bearer (exceto login):

| Método   | Rota                                                | Descrição                              |
| -------- | --------------------------------------------------- | -------------------------------------- |
| `POST`   | `/auth/login`                                       | Autentica e retorna o token JWT        |
| `GET`    | `/vaccines`                                         | Lista as vacinas                       |
| `POST`   | `/vaccines`                                         | Cadastra uma vacina                    |
| `GET`    | `/people`                                           | Lista as pessoas                       |
| `POST`   | `/people`                                           | Cadastra uma pessoa                    |
| `DELETE` | `/people/{personId}`                                | Remove a pessoa e seu cartão (cascata) |
| `GET`    | `/people/{personId}/vaccination-card`               | Consulta o cartão de vacinação         |
| `POST`   | `/people/{personId}/vaccination-records`            | Registra uma vacinação                 |
| `DELETE` | `/people/{personId}/vaccination-records/{recordId}` | Exclui um registro                     |

**Doses aceitas:** `Single`, `First`, `Second`, `Third`, `Booster`.

---

## Arquitetura

O backend segue **Clean Architecture** (dependências sempre apontando para o domínio) com **CQRS** via MediatR. Cada camada tem um README com suas responsabilidades:

- [`src/VaccinationCard.Domain`](src/VaccinationCard.Domain/README.md) — entidades e regras de domínio
- [`src/VaccinationCard.Application`](src/VaccinationCard.Application/README.md) — casos de uso (Commands/Queries), validação
- [`src/VaccinationCard.Infrastructure`](src/VaccinationCard.Infrastructure/README.md) — acesso a dados e serviços externos
- [`src/VaccinationCard.API`](src/VaccinationCard.API/README.md) — Controllers, Swagger, tratamento de erros
- [`src/VaccinationCard.Tests`](src/VaccinationCard.Tests/README.md) — testes unitários

```
VaccineCard/
├── src/
│   ├── VaccinationCard.Domain          # núcleo (sem dependências externas)
│   ├── VaccinationCard.Application     # depende de Domain
│   ├── VaccinationCard.Infrastructure  # depende de Application
│   ├── VaccinationCard.API             # depende de Application + Infrastructure
│   └── VaccinationCard.Tests           # testes dos Handlers
└── frontend/                           # SPA React + Vite
```

---

## Regras de negócio (registro de vacinação)

Validadas no `CreateVaccinationRecordCommandHandler`, por vacina:

- Não é possível registrar a **mesma dose** da mesma vacina duas vezes.
- **Ordem lógica:** `Second` exige `First`; `Third` exige `Second`; `Booster` exige `Single` ou `Second`.
- A data de aplicação não pode ser futura (validada via FluentValidation).

Violações retornam **409 Conflict**; dados inválidos retornam **400**; recursos inexistentes, **404** — sempre no formato ProblemDetails.

---

## Decisões de projeto

- **GUID como chave primária, separado do número de identificação.** A chave (`Id`) é técnica e sem significado de negócio; o número de identificação da pessoa é um dado de domínio (único, com significado no mundo real). Acoplar a PK a um dado de negócio é má prática — por isso são campos distintos.
- **Doses como constantes `string`, não `enum`.** Facilita a validação com FluentValidation (que compara contra um conjunto de strings) e o mapeamento direto no JSON/banco, sem conversões.
- **Cascata vs. restrição na exclusão.** Excluir uma pessoa apaga seus registros de vacinação (cascade); uma vacina do catálogo **não** pode ser excluída se houver registros associados (restrict).
- **Contratos da API separados dos Commands.** Os `Request` da camada API isolam o contrato HTTP da lógica interna, permitindo evoluí-los de forma independente.
- **Banco InMemory.** Mantém o desafio executável sem dependências externas. A camada de repositórios/`IUnitOfWork` permite trocar por um provedor real (SQL Server/PostgreSQL) alterando apenas a Infrastructure.
- **Autenticação de demonstração.** O login (`admin`/`admin`) gera um JWT válido para exercitar a proteção das rotas; não há gestão de usuários real.
