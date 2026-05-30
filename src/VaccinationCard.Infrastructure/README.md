# VaccinationCard.Infrastructure

Implementa os detalhes técnicos abstraídos pela `Application`: **acesso a dados** e **serviços externos**.

## Responsabilidades

- **Persistência:** `ApplicationDbContext` (EF Core com provedor **InMemory**) e as configurações de mapeamento (`IEntityTypeConfiguration`), incluindo as regras de cascata/restrição na exclusão.
- **Repositories:** implementações de `IPersonRepository`, `IVaccineRepository`, `IVaccinationRecordRepository`.
- **Unit of Work:** `UnitOfWork` centraliza o `SaveChanges`, garantindo que cada caso de uso persista numa única transação lógica.
- **Identidade:** `JwtTokenGenerator` (implementa `IJwtTokenGenerator`) e `JwtSettings`.
- **Injeção de dependência:** `AddInfrastructure(...)` registra DbContext, repositórios e serviços no container.

## Decisões

- **Banco InMemory** para rodar sem dependências externas. Como o acesso é feito por interfaces, trocar para SQL Server/PostgreSQL exige mudanças **apenas nesta camada**.
- **Cascata vs. restrição:** excluir pessoa apaga seus registros (cascade); vacina com registros não pode ser excluída (restrict) -> definido nas configurations.

## Principais bibliotecas

Microsoft.EntityFrameworkCore (InMemory), Microsoft.IdentityModel / System.IdentityModel.Tokens.Jwt.
