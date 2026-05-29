# Cartão de Vacinação

Sistema completo (API + frontend) para gerenciar cartões de vacinação: cadastro de vacinas e pessoas, registro de vacinações (com validação de dose), consulta do cartão e exclusão de registros/pessoas.

- **Backend:** ASP.NET Core 9 (Clean Architecture, CQRS, JWT)
- **Frontend:** React 18 + TypeScript + Vite + Tailwind CSS

---

## Sumário

1. [Visão geral / arquitetura](#1-visão-geral--arquitetura)
2. [Pré-requisitos](#2-pré-requisitos)
3. [Como rodar (passo a passo)](#3-como-rodar-passo-a-passo)
4. [Tutorial de uso da aplicação](#4-tutorial-de-uso-da-aplicação)
5. [Referência da API](#5-referência-da-api)
6. [Decisões arquiteturais](#6-decisões-arquiteturais)
7. [Próximos passos](#7-próximos-passos)

---

## 1. Visão geral / arquitetura

```
VaccineCard/
├── src/                              # Backend (.NET 9)
│   ├── VaccinationCard.Domain          # Entidades e constantes (regras de domínio)
│   ├── VaccinationCard.Application      # Commands/Queries, Handlers, Validadores, Interfaces
│   ├── VaccinationCard.Infrastructure  # EF Core, Repositórios, JWT, DI da infraestrutura
│   └── VaccinationCard.API             # Controllers, Program.cs, middleware de exceções
├── frontend/                         # Frontend (React + Vite + TS)
│   └── src/
│       ├── api/                        # Axios (instância + interceptors) e serviços tipados
│       ├── components/                 # Layout, rota protegida, modal, spinner
│       ├── context/                    # AuthContext (token/usuário no localStorage)
│       ├── pages/                      # Login, Pessoas, Vacinas, Cartão de vacinação
│       └── types.ts                    # Tipagens dos contratos da API
└── README.md
```

**Stack backend:** .NET 9, Clean Architecture, CQRS com MediatR, FluentValidation, EF Core (InMemory), JWT Bearer, Swagger.
**Stack frontend:** React 18, TypeScript, Vite, Tailwind CSS, React Router v6, React Hook Form, Axios.

---

## 2. Pré-requisitos

- **.NET SDK 9** (backend)
- **Node.js 18+** e **npm** (frontend)

---

## 3. Como rodar (passo a passo)

A aplicação tem duas partes. Abra **dois terminais** (um para o backend, outro para o frontend).

### 3.1. Backend (API)

```bash
# Na raiz do projeto (VaccineCard/)
dotnet restore
dotnet run --project src/VaccinationCard.API --launch-profile http
```

- API disponível em **http://localhost:5087**
- Swagger (documentação interativa): **http://localhost:5087/swagger**

> O banco é **InMemory**: os dados são reiniciados a cada execução do backend.

### 3.2. Frontend (React)

```bash
# Em outro terminal, a partir da raiz
cd frontend
npm install          # apenas na primeira vez
npm run dev
```

- App disponível em **http://localhost:5173**
- A URL da API é configurável em `frontend/.env` (`VITE_API_BASE_URL`). O padrão já aponta para `http://localhost:5087/api`.

### 3.3. Acessar

Abra **http://localhost:5173** e faça login com as credenciais de demonstração:

| Utilizador | Senha |
|------------|-------|
| `admin`    | `admin` |

> **CORS:** o backend já libera as origens do Vite (`http://localhost:5173` e `http://localhost:4173`).

---

## 4. Tutorial de uso da aplicação

Depois de logar, você cai no **Dashboard (Pessoas)**. O fluxo recomendado:

1. **Cadastre uma vacina** — vá em **Vacinas → "+ Nova vacina"** (ex.: `COVID-19 (Pfizer)`). É necessário ter ao menos uma vacina para registrar vacinações.
2. **Cadastre uma pessoa** — no **Dashboard → "+ Nova pessoa"**, informe nome e número de identificação (único).
3. **Abra o cartão** — clique em **"Ver cartão"** na linha da pessoa.
4. **Registre uma vacinação** — no formulário lateral, selecione a vacina, a dose e a data de aplicação, e clique em **Registrar**. A tabela atualiza na hora.
5. **Exclua** — registros de vacinação têm botão **Excluir**; pessoas têm botão **Remover** (apaga também o cartão inteiro, em cascata).

Doses disponíveis: **Dose única, 1ª dose, 2ª dose, 3ª dose, Reforço** (validadas pelo backend).

---

## 5. Referência da API

URL base: `http://localhost:5087/api`. Todos os endpoints (exceto o login) exigem **JWT Bearer**.

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/auth/login` | Autentica e retorna o token JWT |
| `GET` | `/vaccines` | Lista as vacinas |
| `POST` | `/vaccines` | Cadastra uma vacina |
| `GET` | `/people` | Lista as pessoas |
| `POST` | `/people` | Cadastra uma pessoa |
| `DELETE` | `/people/{personId}` | Remove a pessoa e todo o seu cartão (cascata) |
| `GET` | `/people/{personId}/vaccination-card` | Consulta o cartão de vacinação |
| `POST` | `/people/{personId}/vaccination-records` | Registra uma vacinação |
| `DELETE` | `/people/{personId}/vaccination-records/{recordId}` | Exclui um registro de vacinação |

### Autenticação

`POST /api/auth/login` — body `{ "userName": "admin", "password": "admin" }` → `{ "accessToken": "...", "tokenType": "Bearer" }`.
Use o token nas demais chamadas: header `Authorization: Bearer <accessToken>`.

### Exemplos de body

**Criar vacina** — `POST /api/vaccines`
```json
{ "name": "COVID-19 (Pfizer)" }
```

**Criar pessoa** — `POST /api/people`
```json
{ "name": "Maria Silva", "identificationNumber": "12345678900" }
```

**Registrar vacinação** — `POST /api/people/{personId}/vaccination-records`
```json
{
  "vaccineId": "d3f1f7a0-0000-0000-0000-000000000000",
  "dose": "First",
  "applicationDate": "2024-03-10T00:00:00Z"
}
```
Doses aceitas (case-insensitive): `Single`, `First`, `Second`, `Third`, `Booster`.

**Resposta do cartão** — `GET /api/people/{personId}/vaccination-card`
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

### Tratamento de erros

A API responde no formato **ProblemDetails** (RFC 7807) com o status apropriado: `400` (validação), `401` (sem token / credenciais inválidas), `404` (recurso inexistente), `409` (identificação duplicada). O frontend exibe essas mensagens diretamente na interface.

---

## 6. Decisões arquiteturais

**Backend**
- **Clean Architecture + CQRS:** separa regras de negócio (Application/Domain) da infraestrutura e da API. Cada caso de uso é um Command/Query com seu Handler.
- **Contratos da API separados dos Commands:** os `Request` records em `API/Contracts` isolam a interface HTTP da lógica interna.
- **Validação via pipeline (`ValidationBehavior`):** todo Command passa pelos validadores do FluentValidation antes do Handler.
- **Tratamento global de exceções:** `GlobalExceptionHandler` traduz exceções de domínio/validação para respostas HTTP padronizadas (ProblemDetails).
- **`Id` (GUID) separado do número de identificação:** a chave primária é um GUID técnico, sem acoplar a PK a um dado de negócio.
- **Cascata vs. restrição:** excluir uma pessoa apaga seus registros (cascade); uma vacina não pode ser excluída se houver registros (restrict).
- **Banco InMemory:** roda sem dependências externas. A camada de repositórios/`IUnitOfWork` permite trocar por um provedor real alterando apenas a Infrastructure.

**Frontend**
- **Camada de serviços tipada (`api/services.ts`):** centraliza as chamadas e usa as interfaces de `types.ts`, derivadas dos contratos da API.
- **Axios com interceptors:** injeta o token automaticamente e, em `401`, limpa a sessão e redireciona para o login.
- **AuthContext + rota protegida:** estado de autenticação simples (token/usuário no `localStorage`), com `ProtectedRoute` guardando as rotas privadas.
- **React Hook Form:** formulários de login, pessoa, vacina e registro de vacinação com validação no cliente; erros do servidor exibidos via parser de ProblemDetails.
- **Tailwind CSS:** UI responsiva no estilo dashboard administrativo.

---

## 7. Próximos passos

- Projeto de **testes unitários** (xUnit) para Handlers e validadores; testes de componente no frontend.
- Persistência real (SQL Server/PostgreSQL) via EF Core + migrations.
- Gestão de usuários real para a autenticação (hoje é um login fixo de demonstração).
- Paginação/busca nas listagens de pessoas e vacinas.
