# VaccinationCard.Domain

Contém as **entidades** e as **regras de domínio**, sem nenhuma dependência externa (sem EF Core, sem ASP.NET, sem MediatR). 
Todas as outras camadas dependem desta e ela de ninguém.

## Responsabilidades

- **Entidades:** `Person`, `Vaccine`, `VaccinationRecord` — os modelos do domínio e suas propriedades de navegação.
- **Constantes:** `VaccineDoses` (`Single`, `First`, `Second`, `Third`, `Booster`) — o conjunto de doses válidas.

## Decisões

- **`Id` (GUID) separado do número de identificação da pessoa.** O `Id` é a chave técnica e não deve carregar significado de negócio; o `IdentificationNumber` é um dado de domínio (único, com significado real). Por isso são campos distintos.
- **Doses como constantes `string` em vez de `enum`,** para simplificar a validação (FluentValidation) e a persistência sem conversões.

## Dependências

Nenhuma biblioteca externa.
