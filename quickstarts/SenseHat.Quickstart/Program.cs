using System;
using System.Drawing;
using System.Threading;
using Iot.Device.Common;
using Iot.Device.SenseHat;

// set this to the current sea level pressure in the area for correct altitude readings
var defaultSeaLevelPressure = WeatherHelper.MeanSeaLevel;

using SenseHat sh = new SenseHat();
int n = 0;
int x = 3, y = 3;


UnitsNet.Temperature tempValue = default;
UnitsNet.RelativeHumidity humValue = default;

var msg = "Hello Asha from Aaroh!";
Sense.Led.LedMatrix.ShowMessage(msg);

while (true)
{
    Console.Clear();
    var th = new Thread(ShowInfo);

    th.Start();


    (int dx, int dy, bool holding) = JoystickState(sh);

    if (holding)
    {
        n++;
    }

    x = (x + 8 + dx) % 8;
    y = (y + 8 + dy) % 8;

    sh.Fill(n % 2 == 0 ? Color.DarkBlue : Color.DarkRed);
    sh.SetPixel(x, y, Color.Yellow);

    tempValue = sh.Temperature;
    var temp2Value = sh.Temperature2;
    var preValue = sh.Pressure;
    humValue = sh.Humidity;
    var accValue = sh.Acceleration;
    var angValue = sh.AngularRate;
    var magValue = sh.MagneticInduction;
    var altValue = WeatherHelper.CalculateAltitude(preValue, defaultSeaLevelPressure, tempValue);

    Console.WriteLine($"Temperature Sensor 1: {tempValue.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Temperature Sensor 2: {temp2Value.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Pressure: {preValue.Hectopascals:0.##} hPa");
    Console.WriteLine($"Altitude: {altValue.Meters:0.##} m");
    Console.WriteLine($"Acceleration: {sh.Acceleration} g");
    Console.WriteLine($"Angular rate: {sh.AngularRate} DPS");
    Console.WriteLine($"Magnetic induction: {sh.MagneticInduction} gauss");
    Console.WriteLine($"Relative humidity: {humValue.Percent:0.#}%");
    Console.WriteLine($"Heat index: {WeatherHelper.CalculateHeatIndex(tempValue, humValue).DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Dew point: {WeatherHelper.CalculateDewPoint(tempValue, humValue).DegreesCelsius:0.#}\u00B0C");

    Thread.Sleep(1000);
}

(int, int, bool) JoystickState(SenseHat sh)
{
    sh.ReadJoystickState();

    int dx = 0;
    int dy = 0;

    if (sh.HoldingUp)
    {
        dy--; // y goes down
    }

    if (sh.HoldingDown)
    {
        dy++;
    }

    if (sh.HoldingLeft)
    {
        dx--;
    }

    if (sh.HoldingRight)
    {
        dx++;
    }

    return (dx, dy, sh.HoldingButton);
}

void ShowInfo()
{
    int option = 0;
    var slepDuration = 3000;
    string msgToDisplay = string.Empty;
    while (true)
    {
        switch (option)
        {
            case 0:
                Console.WriteLine("Show Temperature Sensor 1");
                msgToDisplay = $"Temperature Sensor 1: {tempValue.DegreesCelsius:0.#}\u00B0C";
                break;
            case 1:
                Console.WriteLine("Show Time");
                msgToDisplay = $"Time: {DateTime.Now:HH:mm:ss}";
                break;
            case 2:
                Console.WriteLine("Show Humidity");
                msgToDisplay = $"Relative humidity: {humValue.Percent:0.#}%";
                break;
        }
        Sense.Led.LedMatrix.ShowMessage(msgToDisplay);

        Thread.Sleep(slepDuration);
        option = (option + 1) % 3;
    }
}
