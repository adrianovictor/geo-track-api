IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Vehicle')
BEGIN
    CREATE TABLE [dbo].[Vehicles] (
        [Id]        INT            IDENTITY (1, 1) NOT NULL,
        [UniqueId]  UNIQUEIDENTIFIER CONSTRAINT [DF_UNIQUE_ID_USER_ROLES] DEFAULT (newid()) NOT NULL,
        [Renavam]   VARCHAR (20)   NOT NULL,
        [Plate]     VARCHAR (10)   NOT NULL,
        [Model]     VARCHAR (50)   NOT NULL,
        [Brand]     VARCHAR (50)   NOT NULL,
        [Year]      INT            NOT NULL,
        [Status]    INT            NOT NULL,
        [CreatedAt] DATETIME2(7)   NOT NULL,
        [UpdatedAt] DATETIME2(7)   NULL,
        CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE INDEX [IX_Vehicle_Renavam] ON [dbo].[Vehicles]([Renavam] ASC);
    CREATE UNIQUE INDEX [IX_Vehicle_Plate] ON [dbo].[Vehicles]([Plate] ASC);
END;