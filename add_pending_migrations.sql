-- Check if the __EFMigrationsHistory table exists
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

-- Add pending migrations to the history table
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250426183347_FixCascadePaths')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250426183347_FixCascadePaths', N'8.0.12');
    
    PRINT 'Migration 20250426183347_FixCascadePaths added to history table.';
END
ELSE
BEGIN
    PRINT 'Migration 20250426183347_FixCascadePaths already exists in history table.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250501163321_InitialCreateLogin')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250501163321_InitialCreateLogin', N'8.0.12');
    
    PRINT 'Migration 20250501163321_InitialCreateLogin added to history table.';
END
ELSE
BEGIN
    PRINT 'Migration 20250501163321_InitialCreateLogin already exists in history table.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250501214123_UpdateDbContext')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250501214123_UpdateDbContext', N'8.0.12');
    
    PRINT 'Migration 20250501214123_UpdateDbContext added to history table.';
END
ELSE
BEGIN
    PRINT 'Migration 20250501214123_UpdateDbContext already exists in history table.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250502071723_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250502071723_Initial', N'8.0.12');
    
    PRINT 'Migration 20250502071723_Initial added to history table.';
END
ELSE
BEGIN
    PRINT 'Migration 20250502071723_Initial already exists in history table.';
END
GO
