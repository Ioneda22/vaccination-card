# VaccinationCard.Tests

Testes unitários com **xUnit**, focados nos **Handlers** e nas **regras de negócio** da camada Application.

## Abordagem

Cada teste executa o Handler com os repositórios e o `UnitOfWork` sobre um instância de banco 'In memory'.

## Testes feitos

- Criar vacina e criar pessoa (persistência).
- Conflito ao cadastrar pessoa com número de identificação duplicado.
- Registrar vacinação; erro ao usar vacina inexistente.
- Regras de dose: bloquear dose duplicada e bloquear 2ª dose sem a 1ª.
- Remover pessoa e verificar a **exclusão em cascata** dos registros de vacinação.

## Como rodar

```bash
dotnet test
```

## Principais bibliotecas

xUnit, Microsoft.EntityFrameworkCore.InMemory.
