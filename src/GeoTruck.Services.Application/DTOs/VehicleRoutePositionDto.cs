using System;

namespace GeoTruck.Services.Application.DTOs;

public record VehicleRoutePositionDto(
    string Plate,
    string DeviceModel,
    long DeviceId,
    long PositionId,
    DateTime Date,
    DateTime DateUTC,
    string Realtime,
    string Ignition,
    double Odometer,
    string Horimeter,
    string Address,
    double Direction,
    string Header,
    string GpsFix,
    int Speed,
    string MainBattery,
    string BackupBattery,
    decimal Latitude,
    decimal Longitude,
    string DriverName,
    string DriverId,
    string Input1,
    string Input2,
    string Output1,
    string Output2,
    string Rs232,
    int IsLbs,
    string Rpm,
    string OriginPosition,
    string BatteryPercentual
);
