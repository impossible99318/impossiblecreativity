// Initialize variables
let map, directionsService, directionsRenderer, autocompleteStart, autocompleteEnd, autocompleteWaypoint;

// Initialize the map
function initMap() {
    // Initialize the map
    map = new google.maps.Map(document.getElementById('map'), {
        center: {lat: 37.7749, lng: -122.4194}, // Default to San Francisco
        zoom: 12
    });
    
    // Set up directions service and renderer
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer();
    directionsRenderer.setMap(map);
    
    // Autocomplete for the search bars
    autocompleteStart = new google.maps.places.Autocomplete(document.getElementById('start'));
    autocompleteEnd = new google.maps.places.Autocomplete(document.getElementById('end'));
    autocompleteWaypoint = new google.maps.places.Autocomplete(document.getElementById('waypoint'));
    
    // Event listener for calculating route
    document.getElementById('calculate-route').addEventListener('click', calculateRoute);
}

// Calculate route
function calculateRoute() {
    const start = document.getElementById('start').value;
    const waypoint = document.getElementById('waypoint').value;
    const end = document.getElementById('end').value;
    const transportMode = document.getElementById('transport-mode').value;
    const routePreference = document.getElementById('route-preference').value;
    
    // Create a waypoint if the user added one
    let waypoints = [];
    if (waypoint) {
        waypoints.push({
            location: waypoint,
            stopover: true
        });
    }
    
    // Set route preferences (fastest, eco-friendly, shortest)
    let drivingOptions = {};
    if (routePreference === 'eco') {
        drivingOptions = { avoidTolls: true };
    } else if (routePreference === 'shortest') {
        drivingOptions = { avoidFerries: true };
    }
    
    // Request route
    const request = {
        origin: start,
        destination: end,
        waypoints: waypoints,
        travelMode: transportMode,
        drivingOptions: drivingOptions,
        unitSystem: google.maps.UnitSystem.METRIC
    };
    
    directionsService.route(request, function(result, status) {
        if (status === 'OK') {
            directionsRenderer.setDirections(result);
            
            // Calculate carbon emissions
            const distance = result.routes[0].legs.reduce((sum, leg) => sum + leg.distance.value, 0) / 1000; // in km
            calculateCarbonEmission(distance);
        } else {
            alert('Could not calculate route. Please try again.');
        }
    });
}

// Carbon emission calculation
function calculateCarbonEmission(distance) {
    const transportMode = document.getElementById('transport-mode').value;
    
    let emissionFactor; // kg CO2 per km
    
    if (transportMode === 'DRIVING') {
        emissionFactor = 0.12; // Average car emission
    } else if (transportMode === 'BICYCLING') {
        emissionFactor = 0; // Bikes emit no CO2
    } else if (transportMode === 'TRANSIT') {
        emissionFactor = 0.06; // Public transport emission
    }
    
    const carbonEmission = (distance * emissionFactor).toFixed(2); // in kg
    
    document.getElementById('emission-result').innerText = `Estimated Carbon Emission: ${carbonEmission} kg`;
}

// Load the map when the window loads
window.onload = initMap;
