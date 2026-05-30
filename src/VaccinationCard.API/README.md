# VaccinationCard.API

Expõe os endpoints REST, configura autenticação, documentação e o tratamento global de erros. 
Os Controllers apenas traduzem requisições HTTP em Commands/Queries e os enviam via MediatR.

## Responsabilidades

- **Controllers:** `Auth`, `People`, `Vaccines`, `VaccinationRecords` — recebem os `Request`, despacham para a Application e retornam o status HTTP adequado.
- **Composição (`Program.cs`):** registra Application + Infrastructure, autenticação JWT, CORS (para o frontend) e o pipeline da requisição.
- **Swagger/OpenAPI:** documentação interativa em `/swagger`, com suporte a Bearer token.
- **Tratamento de erros:** o `GlobalExceptionHandler` intercepta exceções e as traduz para **ProblemDetails** (RFC 7807) com o status correto: `NotFoundException` → 404, `ConflictException` → 409, `ValidationException` → 400.

## Decisões

- **Contratos (`Request`) separados dos Commands** do MediatR, isolando o contrato HTTP da lógica interna.
- **Erros centralizados no handler global:** os Controllers não precisam de try/catch; exceções de domínio viram respostas HTTP padronizadas.

## Principais bibliotecas

ASP.NET Core, MediatR, Swashbuckle (Swagger), Microsoft.AspNetCore.Authentication.JwtBearer.
