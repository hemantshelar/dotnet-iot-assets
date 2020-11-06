﻿using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;

I2cConnectionSettings i2cSettings = new I2cConnectionSettings(1, Bme280.DefaultI2cAddress);
using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
using Bme280 bme280 = new Bme280(i2cDevice);

int measurementTime = bme280.GetMeasurementDuration();

while (true)
{
    Console.Clear();

    bme280.SetPowerMode(Bmx280PowerMode.Forced);
    Thread.Sleep(measurementTime);

    bme280.TryReadTemperature(out var tempValue);
    bme280.TryReadPressure(out var preValue);
    bme280.TryReadHumidity(out var humValue);
    bme280.TryReadAltitude(out var altValue);

    Console.WriteLine($"Temperature: {tempValue.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Pressure: {preValue.Hectopascals:#.##} hPa");
    Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");
    Console.WriteLine($"Estimated altitude: {altValue.Meters:#} m");

    Thread.Sleep(1000);
}