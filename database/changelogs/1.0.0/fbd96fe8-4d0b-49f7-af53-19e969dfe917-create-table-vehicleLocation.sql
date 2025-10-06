IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'VehicleLocations')
BEGIN
    CREATE TABLE [dbo].[VehicleLocations] (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [PositionId] BIGINT         NULL,
        [VehicleId]  INT            NOT NULL,
        [Latitude]   DECIMAL(9,6)   NOT NULL,
        [Longitude]  DECIMAL(9,6)   NOT NULL,
        [Date]       DATETIME2      NOT NULL,
        [DateUTC]    DATETIME2      NOT NULL,
        [CreatedAt]  DATETIME2(7)   NOT NULL,
        [UpdatedAt]  DATETIME2(7)   NULL,
        FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id)
    );
END
