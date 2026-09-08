IF DB_ID(N'GestaoUsuarios') IS NULL
    CREATE DATABASE GestaoUsuarios;
GO

USE GestaoUsuarios;
GO

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TbUsuario
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
        Nome NVARCHAR(150) NOT NULL,
        ValorHora DECIMAL(18,2) NOT NULL,
        DataCadastro DATETIME NOT NULL CONSTRAINT DF_Usuario_DataCadastro DEFAULT GETDATE(),
        Ativo BIT NOT NULL CONSTRAINT DF_Usuario_Ativo DEFAULT 1,
        CONSTRAINT CK_Usuario_ValorHora CHECK (ValorHora > 0)
    );
END;
GO

SELECT ID, Nome, ValorHora, DataCadastro, Ativo FROM dbo.Usuario ORDER BY Nome;
GO
