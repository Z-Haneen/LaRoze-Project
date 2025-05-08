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

-- Check if the migration already exists
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250508145511_UpdateUserAddressModel')
BEGIN
    -- Add the migration to the history table
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250508145511_UpdateUserAddressModel', N'8.0.12');
    
    PRINT 'Migration 20250508145511_UpdateUserAddressModel added to history table.';
END
ELSE
BEGIN
    PRINT 'Migration 20250508145511_UpdateUserAddressModel already exists in history table.';
END
GO
