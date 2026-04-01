IF DB_ID('DesafioPlenoDb') IS NULL
BEGIN
    CREATE DATABASE DesafioPlenoDb;
END;
GO

USE DesafioPlenoDb;
GO

IF OBJECT_ID('dbo.LancamentoFinanceiro', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LancamentoFinanceiro;
END;
GO

CREATE TABLE dbo.LancamentoFinanceiro
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Descricao NVARCHAR(200) NOT NULL,
    Tipo INT NOT NULL,
    ValorOriginal DECIMAL(18,2) NOT NULL,
    PercentualTaxa DECIMAL(5,2) NULL,
    PercentualDesconto DECIMAL(5,2) NULL,
    ValorCalculado DECIMAL(18,2) NOT NULL,
    DataLancamento DATETIME2 NOT NULL,
    DataCriacao DATETIME2 NOT NULL CONSTRAINT DF_LancamentoFinanceiro_DataCriacao DEFAULT SYSUTCDATETIME(),
    DataPagamento DATETIME2 NULL,
    DataCancelamento DATETIME2 NULL,
    Competencia CHAR(7) NOT NULL,
    Status INT NOT NULL
);
GO

CREATE UNIQUE INDEX UX_LancamentoFinanceiro_Competencia_Descricao_Tipo
    ON dbo.LancamentoFinanceiro (Competencia, Descricao, Tipo);
GO
