# VaccinationCard.Application

Orquestra as regras de negócio aplicando o padrão **CQRS** com **MediatR**: cada operação é um `Command` (escrita) ou `Query` (leitura) com seu respectivo `Handler`.
Depende apenas do `Domain` e define as **interfaces** (`IPersonRepository`, `IVaccineRepository`, `IUnitOfWork`, `IJwtTokenGenerator`) que a Infrastructure implementa.

## Responsabilidades

- **Commands/Queries + Handlers:** criar/remover pessoa, criar vacina, registrar/excluir vacinação, consultar cartão, listar pessoas e vacinas.
- **DTOs:** modelos de saída (ex.: `VaccinationCardDto`, `PersonSummaryDto`).
- **Validação fail-fast:** validadores do FluentValidation rodam num `ValidationBehavior` antes de cada Handler, para que a entrada inválida nunca chegue à lógica de negócio.
- **Regras de negócio:** ex.: ordem e unicidade das doses no `CreateVaccinationRecordCommandHandler`.
- **Exceções de domínio:** `NotFoundException`, `ConflictException`.

## Decisões

- **Interfaces aqui, implementação na Infrastructure** (Dependency Inversion): o caso de uso não conhece EF Core nem JWT.

## Principais bibliotecas

MediatR, FluentValidation.
