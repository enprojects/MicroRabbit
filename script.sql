﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Accounts] (
    [Id] int NOT NULL IDENTITY,
    [AccountType] nvarchar(max) NULL,
    [AccountBalance] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190725083924_InitMigration', N'2.2.6-servicing-10079');

GO

