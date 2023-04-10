import Chart from 'chart.js/auto'

(async function () {

    var response = await fetch("https://localhost:7054/api/cityweather/minTempByMinute");
    var data = await response.json();

    data.forEach((d) => {
        d.timestamp = new Date(d.time);
    });

    // minimum temperature
    var minTempCanvas = document.getElementById('minTemp');
    new Chart(
        minTempCanvas,
        {
            type: "line",
            data: {
                labels: data.map(x => x.time + '\r\n' + x.cityName),
                datasets: [{
                    label: 'Min temp over time',
                    data: data
                }]
            },
            options: {
                parsing: {
                    xAxisKey: 'timestamp',
                    yAxisKey: 'temperature'
                }
            }
        }
    );

    var response2 = await fetch("https://localhost:7054/api/cityweather/maxWindSpeedByMinute");
    var data2 = await response2.json();

    data2.forEach((d) => {
        d.timestamp = new Date(d.time);
    });

    // higest windspeed
    var windSpeedCanvas = document.getElementById('windSpeed');
    new Chart(
        windSpeedCanvas,
        {
            type: "line",
            data: {
                labels: data2.map(x => x.time + '\r\n' + x.cityName),
                datasets: [{
                    label: 'Max wind speed over time',
                    data: data2
                }]
            },
            options: {
                parsing: {
                    xAxisKey: 'timestamp',
                    yAxisKey: 'windSpeed'
                }
            }
        }
    );

    windSpeedCanvas.onclick = trend;
    minTempCanvas.onclick = trend;
})();

async function trend (e) {
    var trendResponse = await fetch("https://localhost:7054/api/cityweather/weatherTrendLastTwoHours");
    var status = trendResponse.status;
    console.log(status);
    if (status == 400) {
        alert("Error analyzing trends, probably not enough data!");
        return;
    }
    var data = await trendResponse.json();    
    alert("Average weather from all cities last two hours tells us that: \n Temperature is " + data.temperatureTrend + '\n' + " and windspeeed is " + data.windTrend );
}

