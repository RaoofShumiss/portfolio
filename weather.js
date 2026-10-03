// Haal de actuele temperatuur op voor de locatie die de gebruiker toestaat.
const weatherStatus = document.querySelector('#weather-status');
const weatherRetry = document.querySelector('#weather-retry');

if (weatherStatus && weatherRetry) {
    let loading = false;

    async function loadTemperature() {
        if (loading) return;
        loading = true;
        weatherRetry.disabled = true;
        weatherStatus.setAttribute('data-state', 'loading');
        weatherStatus.textContent = 'Je locatie wordt bepaald. Geef toestemming in je browser.';
        let timeout;
        let locating = true;

        try {
            const position = await new Promise((resolve, reject) => {
                navigator.geolocation.getCurrentPosition(resolve, reject, {
                    enableHighAccuracy: false,
                    timeout: 10000,
                });
            });

            const { latitude, longitude } = position.coords;
            if (!Number.isFinite(latitude) || !Number.isFinite(longitude)
                || Math.abs(latitude) > 90 || Math.abs(longitude) > 180) {
                throw new Error('Je locatie kon niet worden bepaald. Probeer het opnieuw.');
            }

            locating = false;
            weatherStatus.textContent = 'De temperatuur op jouw locatie wordt geladen…';
            const parameters = new URLSearchParams({
                latitude: latitude.toFixed(2),
                longitude: longitude.toFixed(2),
                current: 'temperature_2m',
                temperature_unit: 'celsius',
                timezone: 'auto'
            });
            const weatherUrl = `https://api.open-meteo.com/v1/forecast?${parameters}`;
            const response = await fetch(weatherUrl);

            if (!response.ok) throw new Error('Weerbericht niet beschikbaar.');

            const data = await response.json();
            const temperature = data.current?.temperature_2m;

            const formattedTemperature = new Intl.NumberFormat('nl-NL', {
                maximumFractionDigits: 1
            }).format(temperature);

            weatherStatus.dataset.state = 'success';
            weatherStatus.textContent = `${formattedTemperature} °C`;
            weatherRetry.hidden = true;
        } catch (error) {
            weatherStatus.dataset.state = 'error';
            weatherStatus.textContent =
                'De temperatuur kon niet worden geladen. Probeer het opnieuw.';
            weatherRetry.hidden = false;

        } finally {
            loading = false;
            weatherRetry.disabled = false;
        }
    }

    weatherRetry.addEventListener('click', loadTemperature);
    loadTemperature();
}