# Gestão de Usuários

Aplicação web ASP.NET Core MVC para cadastro, consulta, edição e exclusão de usuários. O projeto utiliza Entity Framework Core com SQL Server e jQuery AJAX para executar as operações sem recarregar a página inteira.

## Requisitos

É necessário ter instalado o .NET SDK 8.0 ou superior e uma instância do SQL Server local, em contêiner ou em servidor acessível pela aplicação. O projeto referencia os pacotes `Microsoft.EntityFrameworkCore.SqlServer` e `Microsoft.EntityFrameworkCore.Tools`, ambos na linha 8.0.

## Banco de dados

Execute `Database/01-criar-tabela-usuario.sql` no SQL Server Management Studio, Azure Data Studio ou sqlcmd. O script cria o banco `GestaoUsuarios` caso ele não exista e cria a tabela `dbo.Usuario` com as colunas `ID`, `Nome`, `ValorHora`, `DataCadastro` e `Ativo`.

A string de conexão padrão está em `appsettings.json`. Ajuste o servidor, a autenticação e demais parâmetros conforme o ambiente local. O arquivo `appsettings.Production.json` não foi alterado.

## Execução local

No terminal, entre na pasta do projeto e execute:

```bash
dotnet restore
dotnet build
dotnet run
```

Em seguida, acesse a URL exibida no terminal, normalmente `http://localhost:5000` ou a porta configurada pelo perfil de execução.

## Funcionalidades

A tela principal consulta a lista de usuários com AJAX, apresenta nome, valor por hora, data de cadastro e status, e permite editar ou excluir cada registro. O botão “Novo usuário” abre o formulário de cadastro em modal. Salvar, editar e excluir retornam JSON padronizado com `sucesso`, `mensagem`, `dados` e, quando aplicável, `erros`.

A validação no navegador é feita em `wwwroot/js/usuarios.js`. A validação no servidor usa Data Annotations e a função auxiliar `ValidarRegrasDeNegocio` da entidade `Usuario`. As operações de persistência estão concentradas no `ApplicationDbContext` e no controller da funcionalidade.

## Estrutura principal

| Caminho | Responsabilidade |
|---|---|
| `Models/Usuarios/Usuario.cs` | Entidade, DTO de entrada e regras de validação |
| `Data/ApplicationDbContext.cs` | Mapeamento EF Core da tabela `Usuario` |
| `Controllers/Usuarios/UsuariosController.cs` | Endpoints MVC e JSON do CRUD |
| `Views/Usuarios/Index.cshtml` | Tela de consulta e formulário modal |
| `wwwroot/js/usuarios.js` | AJAX, validação cliente e atualização da tabela |
| `Database/01-criar-tabela-usuario.sql` | Criação do banco e da tabela |

## Endpoints AJAX

| Método | Rota | Finalidade |
|---|---|---|
| `GET` | `/Usuarios/Listar` | Lista todos os usuários |
| `GET` | `/Usuarios/Obter?id={id}` | Consulta um usuário |
| `POST` | `/Usuarios/Salvar` | Insere ou atualiza um usuário |
| `DELETE` | `/Usuarios/Excluir?id={id}` | Exclui um usuário |

As requisições de alteração exigem o token antiforgery enviado pelo formulário Razor e pelo cabeçalho `RequestVerificationToken`.
