IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Vehicles')
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

SET IDENTITY_INSERT [dbo].[Vehicles] ON;

INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, 'B4940139-788A-46A3-A3F3-2E0B8F8D0BDF', '0095354652540565', 'CJL2066', 'Santana Evidence 2.0 5 portas', 'Volkswagen', 1997, 1, '2025-10-05 09:40:54.7569771', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, '450803DD-940E-408A-AD73-B5E939FE60D6', '0006656546465484', 'CCL5499', 'Actros', 'Mercedes-Benz ', 2016, 1, '2025-10-05 09:59:55.4763680', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, '8D545681-CDDA-4D3D-8AEB-49E53D8B30FF', '00565646548465465', 'DHC7G05', 'Volvo FH', 'Volvo', 2020, 1, '2025-10-05 10:00:58.5522679', '2025-10-05 10:01:21.4233844');
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, '2A9DD0BA-70E3-4BE5-9DD9-BE10BCCD0997', '9956146354654324', 'HGG9C50', 'Actros', 'Mercedes-Benz', 2025, 1, '2025-10-05 10:02:34.3369217', '2025-10-05 10:02:43.3085738');
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (5, '4A84CCE8-6A22-486F-9B28-996F091E4F22', '19056546845498', 'ISX0J70', 'Vertis 90v18 Baú', 'Iveco', 2013, 1, '2025-10-05 10:16:24.3314172', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (6, '565EF3E6-CD17-466B-A50C-630095CAEEAF', '00956468458', 'JBD2F63', 'Daily 35s14 2012 Baú', 'Iveco', 2012, 1, '2025-10-05 10:17:25.8262749', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (7, '2F80AFBC-BE42-418A-9D99-2ACB9665AFB5', '9565468654685', 'IZT1A90', 'Daily 35s14 Ducato Bau', 'Iveco', 2012, 1, '2025-10-05 10:18:20.2047940', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (8, '50B1AF4A-B6D2-499B-BD98-58DD2A7F15BC', '007599635005468', 'IZT1C95', 'Volkswagen 8.150e Delivery Baú', 'Volkswagen', 2010, 1, '2025-10-05 10:19:42.1221560', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (9, '5079CA13-2D43-41C9-9027-65C05C8C0057', '005569000984045', 'IFT1F82', 'Hyundai HR', 'Hyundai', 2018, 1, '2025-10-05 10:21:09.4210617', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (10, '726FD1D1-FCF0-406E-BF44-2130D5453ECC', '0985056099890446', 'ACT1F55', 'Volkswagen Delivery Express', 'Volkswagen ', 2022, 1, '2025-10-05 10:22:38.7153755', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (11, '96C7C802-23F0-40DF-A3D5-54573BA2101C', '0985056098005446', 'ACC2H86', 'Volkswagen Delivery Express', 'Volkswagen ', 2022, 1, '2025-10-05 10:24:07.7907560', NULL);
INSERT INTO [dbo].[Vehicles] ([Id], [UniqueId], [Renavam], [Plate], [Model], [Brand], [Year], [Status], [CreatedAt], [UpdatedAt]) VALUES (12, '0EB1C3C4-2749-4A15-B864-64806085CC6E', '7802099950415', 'IPO0E20', 'Hyundai HR', 'Hyundai', 2025, 1, '2025-10-05 21:42:18.4465630', NULL);

SET IDENTITY_INSERT [dbo].[Vehicles] OFF;


SET IDENTITY_INSERT [dbo].[VehicleLocations] ON;

INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (1, 253604182, 6, -29.943495, -51.204738, '2025-09-26 12:58:22.0000000', '2025-09-26 15:58:22.0000000', '2025-10-05 22:25:27.7696281', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (2, 253604607, 6, -29.931302, -51.212840, '2025-09-26 12:59:22.0000000', '2025-09-26 15:59:22.0000000', '2025-10-05 22:25:27.7803626', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (3, 253604633, 6, -29.924318, -51.226985, '2025-09-26 13:00:22.0000000', '2025-09-26 16:00:22.0000000', '2025-10-05 22:25:27.7803750', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (4, 253604887, 6, -29.913233, -51.236955, '2025-09-26 13:01:22.0000000', '2025-09-26 16:01:22.0000000', '2025-10-05 22:25:27.7803755', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (5, 253605168, 6, -29.901885, -51.244727, '2025-09-26 13:02:22.0000000', '2025-09-26 16:02:22.0000000', '2025-10-05 22:25:27.7803758', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (6, 253605446, 6, -29.889005, -51.240348, '2025-09-26 13:03:22.0000000', '2025-09-26 16:03:22.0000000', '2025-10-05 22:25:27.7803760', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (7, 253605804, 6, -29.883772, -51.229518, '2025-09-26 13:04:22.0000000', '2025-09-26 16:04:22.0000000', '2025-10-05 22:25:27.7803763', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (8, 253606037, 6, -29.888553, -51.216300, '2025-09-26 13:05:22.0000000', '2025-09-26 16:05:22.0000000', '2025-10-05 22:25:27.7803766', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (9, 253606375, 6, -29.891317, -51.202380, '2025-09-26 13:06:22.0000000', '2025-09-26 16:06:22.0000000', '2025-10-05 22:25:27.7803768', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (10, 253604256, 12, -30.097372, -51.342875, '2025-09-26 12:58:44.0000000', '2025-09-26 15:58:44.0000000', '2025-10-05 22:25:28.0898692', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (11, 253604464, 12, -30.084052, -51.341743, '2025-09-26 12:59:44.0000000', '2025-09-26 15:59:44.0000000', '2025-10-05 22:25:28.0898689', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (12, 253604703, 12, -30.071545, -51.340680, '2025-09-26 13:00:44.0000000', '2025-09-26 16:00:44.0000000', '2025-10-05 22:25:28.0898686', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (13, 253605004, 12, -30.058378, -51.337827, '2025-09-26 13:01:44.0000000', '2025-09-26 16:01:44.0000000', '2025-10-05 22:25:28.0898683', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (14, 253605278, 12, -30.045988, -51.331847, '2025-09-26 13:02:44.0000000', '2025-09-26 16:02:44.0000000', '2025-10-05 22:25:28.0898680', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (15, 253605538, 12, -30.034077, -51.326108, '2025-09-26 13:03:44.0000000', '2025-09-26 16:03:44.0000000', '2025-10-05 22:25:28.0898678', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (16, 253605848, 12, -30.022330, -51.320503, '2025-09-26 13:04:44.0000000', '2025-09-26 16:04:44.0000000', '2025-10-05 22:25:28.0898675', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (17, 253606255, 12, -30.009652, -51.314453, '2025-09-26 13:05:44.0000000', '2025-09-26 16:05:44.0000000', '2025-10-05 22:25:28.0898672', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (18, 253606342, 12, -30.004178, -51.310730, '2025-09-26 13:06:18.0000000', '2025-09-26 16:06:18.0000000', '2025-10-05 22:25:28.0898659', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (19, 253606387, 12, -30.003857, -51.309730, '2025-09-26 13:06:26.0000000', '2025-09-26 16:06:26.0000000', '2025-10-05 22:25:28.0898695', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (20, 253606419, 12, -30.003768, -51.308978, '2025-09-26 13:06:33.0000000', '2025-09-26 16:06:33.0000000', '2025-10-05 22:25:28.0898698', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (21, 253606548, 12, -30.003023, -51.306530, '2025-09-26 13:06:58.0000000', '2025-09-26 16:06:58.0000000', '2025-10-05 22:25:28.0898701', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (22, 253604305, 5, -30.074187, -51.015830, '2025-09-26 12:58:35.0000000', '2025-09-26 15:58:35.0000000', '2025-10-05 22:25:28.2186755', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (23, 253604428, 5, -30.065182, -51.016413, '2025-09-26 12:59:35.0000000', '2025-09-26 15:59:35.0000000', '2025-10-05 22:25:28.2186752', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (24, 253604779, 5, -30.056612, -51.011218, '2025-09-26 13:00:35.0000000', '2025-09-26 16:00:35.0000000', '2025-10-05 22:25:28.2186750', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (25, 253604952, 5, -30.047532, -51.002817, '2025-09-26 13:01:35.0000000', '2025-09-26 16:01:35.0000000', '2025-10-05 22:25:28.2186746', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (26, 253605230, 5, -30.039725, -50.995747, '2025-09-26 13:02:35.0000000', '2025-09-26 16:02:35.0000000', '2025-10-05 22:25:28.2186743', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (27, 253605495, 5, -30.027977, -50.993557, '2025-09-26 13:03:35.0000000', '2025-09-26 16:03:35.0000000', '2025-10-05 22:25:28.2186740', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (28, 253606481, 5, -29.992052, -50.992883, '2025-09-26 13:06:35.0000000', '2025-09-26 16:06:35.0000000', '2025-10-05 22:25:28.2186737', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (29, 253606482, 5, -30.003792, -50.993127, '2025-09-26 13:05:35.0000000', '2025-09-26 16:05:35.0000000', '2025-10-05 22:25:28.2186734', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (30, 253606483, 5, -30.015005, -50.993328, '2025-09-26 13:04:35.0000000', '2025-09-26 16:04:35.0000000', '2025-10-05 22:25:28.2186713', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (31, 253605058, 7, -24.177133, -50.964462, '2025-09-26 12:52:01.0000000', '2025-09-26 15:52:01.0000000', '2025-10-05 22:25:28.3712660', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (32, 253605064, 7, -24.132273, -51.010120, '2025-09-26 13:02:01.0000000', '2025-09-26 16:02:01.0000000', '2025-10-05 22:25:28.3712657', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (33, 253605065, 7, -24.136825, -51.003160, '2025-09-26 13:01:01.0000000', '2025-09-26 16:01:01.0000000', '2025-10-05 22:25:28.3712654', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (34, 253605066, 7, -24.138725, -51.005832, '2025-09-26 13:00:25.0000000', '2025-09-26 16:00:25.0000000', '2025-10-05 22:25:28.3712651', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (35, 253605067, 7, -24.140517, -51.005117, '2025-09-26 13:00:01.0000000', '2025-09-26 16:00:01.0000000', '2025-10-05 22:25:28.3712648', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (36, 253605075, 7, -24.143583, -51.001450, '2025-09-26 12:59:01.0000000', '2025-09-26 15:59:01.0000000', '2025-10-05 22:25:28.3712645', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (37, 253605076, 7, -24.143393, -50.992383, '2025-09-26 12:58:01.0000000', '2025-09-26 15:58:01.0000000', '2025-10-05 22:25:28.3712642', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (38, 253605077, 7, -24.145258, -50.983655, '2025-09-26 12:57:01.0000000', '2025-09-26 15:57:01.0000000', '2025-10-05 22:25:28.3712638', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (39, 253605078, 7, -24.151572, -50.976000, '2025-09-26 12:56:01.0000000', '2025-09-26 15:56:01.0000000', '2025-10-05 22:25:28.3712635', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (40, 253605079, 7, -24.156585, -50.974402, '2025-09-26 12:55:01.0000000', '2025-09-26 15:55:01.0000000', '2025-10-05 22:25:28.3712629', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (41, 253605083, 7, -24.159180, -50.972625, '2025-09-26 12:54:24.0000000', '2025-09-26 15:54:24.0000000', '2025-10-05 22:25:28.3712609', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (42, 253605084, 7, -24.161532, -50.970668, '2025-09-26 12:54:01.0000000', '2025-09-26 15:54:01.0000000', '2025-10-05 22:25:28.3712421', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (43, 253605085, 7, -24.169327, -50.967192, '2025-09-26 12:53:01.0000000', '2025-09-26 15:53:01.0000000', '2025-10-05 22:25:28.3712663', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (44, 253605353, 7, -24.125443, -51.018502, '2025-09-26 13:03:01.0000000', '2025-09-26 16:03:01.0000000', '2025-10-05 22:25:28.3712666', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (45, 253605376, 7, -24.124292, -51.018605, '2025-09-26 13:03:07.0000000', '2025-09-26 16:03:07.0000000', '2025-10-05 22:25:28.3712669', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (46, 253605624, 7, -24.118905, -51.019278, '2025-09-26 13:04:01.0000000', '2025-09-26 16:04:01.0000000', '2025-10-05 22:25:28.3712672', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (47, 253605916, 7, -24.116088, -51.026182, '2025-09-26 13:05:01.0000000', '2025-09-26 16:05:01.0000000', '2025-10-05 22:25:28.3712675', NULL);
INSERT INTO [dbo].[VehicleLocations] ([Id], [PositionId], [VehicleId], [Latitude], [Longitude], [Date], [DateUTC], [CreatedAt], [UpdatedAt]) VALUES (48, 253606239, 7, -24.113562, -51.035357, '2025-09-26 13:06:01.0000000', '2025-09-26 16:06:01.0000000', '2025-10-05 22:25:28.3712680', NULL);


SET IDENTITY_INSERT [dbo].[VehicleLocations] OFF;
