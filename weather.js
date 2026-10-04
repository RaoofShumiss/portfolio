async function getUserLocation() {
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
    return { latitude, longitude };
}

async function fetchTemperature(latitude, longitude) {
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
    return data.current?.temperature_2m;
}

function formatTemperature(temperature) {
    return new Intl.NumberFormat('nl-NL', {
        maximumFractionDigits: 1
    }).format(temperature) + ' °C';
}

function renderWeather(weatherStatus, state, message) {
    weatherStatus.dataset.state = state;
    weatherStatus.textContent = message;
}

function initWeather() {
    const weatherStatus = document.querySelector('#weather-status');
    const weatherRetry = document.querySelector('#weather-retry');
    if (!weatherStatus || !weatherRetry) return;
    let loading = false;

    async function loadTemperature() {
        if (loading) return;
        loading = true;
        weatherRetry.disabled = true;
        renderWeather(weatherStatus, 'loading', 'Je locatie wordt bepaald. Geef toestemming in je browser.');

        try {
            const { latitude, longitude } = await getUserLocation();
            renderWeather(weatherStatus, 'loading', 'De temperatuur op jouw locatie wordt geladen…');
            const temperature = await fetchTemperature(latitude, longitude);
            renderWeather(weatherStatus, 'success', formatTemperature(temperature));
            weatherRetry.hidden = true;
        } catch (error) {
            renderWeather(weatherStatus, 'error', 'De temperatuur kon niet worden geladen. Probeer het opnieuw.');
            weatherRetry.hidden = false;

        } finally {
            loading = false;
            weatherRetry.disabled = false;
        }
    }

    weatherRetry.addEventListener('click', loadTemperature);
    loadTemperature();
}

initWeather();
