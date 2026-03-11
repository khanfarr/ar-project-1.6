# AR Knick-Knack: NYC x Toronto

Developer: Fareena

## Project Summary
This project is a Unity + Vuforia augmented reality experience built around two physical tracked cubes: one for New York City and one for Toronto. When each cube is detected by the webcam, a miniature city scene appears on top of it with live city information (time, weather, and flights in the sky). When both cubes are visible at once, a plane animates between the two cities.

## Motivation and Chosen Locations
I chose New York City and Toronto because both cities are personally meaningful to me and have strong visual identities that translate well into miniature AR scenes. I also intially thought they were popular enough for me to easily find assets.

These two cities also work well together conceptually because travel between them is common. That relationship shaped the core interaction in the project: the animated plane that appears only when both city cubes are tracked at the same time.

## Design
### Knick-knack models
Each cube contains a custom miniature scene made from a mix of generated and hand-made 3D assets.

New York models:
- Generated with Meshy AI: Empire State Building, Statue of Liberty, Brooklyn Bridge
- Modeled myself in Blender: taxi, bagel

Toronto models:
- Generated with Meshy AI: CN Tower, Time Hortons logo as a maple-leaf, beaver (national animal), hockey-themed objects.

I used this hybrid approach because it let me keep visual consistency while still building specific location-based props that were hard to find in asset stores.

### Visual elements in the experience
- Live data text on cube faces: city name, current temperature, local time, and nearby flights in the sky
- Plane animation between cities: enabled only when both Vuforia targets are tracked
- Time-of-day lighting: directional lighting shifts between day, sunset, and night
- Weather-based cube shell color: clear/rain/snow states mapped to different colors
- Ambient city audio: adds atmosphere around both knick-knacks

### Screenshots (link these to the text above)
Add your screenshots to `Assets/Media/screenshots/` and keep these filenames so the README renders automatically.

![Figure 1 - Both tracked cubes visible with NYC and Toronto scenes](Assets/Media/screenshots/01-both-cubes-overview.png)
Figure 1 relates to: overall two-cube design and city-specific knick-knack scenes.

![Figure 2 - NYC cube close-up showing landmarks and info text](Assets/Media/screenshots/02-nyc-closeup.png)
Figure 2 relates to: NYC model choices (Empire State, Statue of Liberty, bridge, taxi, bagel) and face text.

![Figure 3 - Toronto cube close-up showing Canadian-themed models](Assets/Media/screenshots/03-toronto-closeup.png)
Figure 3 relates to: Toronto model choices (CN Tower, beaver, maple/hockey props) and face text.

![Figure 4 - Plane animation active when both cubes are tracked](Assets/Media/screenshots/04-plane-between-cities.png)
Figure 4 relates to: inter-cube interaction and travel metaphor.

![Figure 5 - Day/night lighting + weather color variation](Assets/Media/screenshots/05-lighting-weather-variation.png)
Figure 5 relates to: visual systems (time-based lighting and weather-based color changes).

## Process
### How the application is structured
Core scripts are under `Assets/Scripts/`:
- `NYCWeatherAPI.cs`, `TORWeatherAPI.cs`: fetch temperature from Open-Meteo
- `NYCTimeAPI.cs`, `TORTimeAPI.cs`: fetch and format local city time from Open-Meteo
- `NYCNumFlightsAPI.cs`, `TORNumFlight.cs`: fetch flight state data from OpenSky Network and count nearby flights
- `PlanePresenceManager.cs`: checks Vuforia target visibility and controls plane orbit behavior
- `TaxiMove.cs`: animates a taxi model around a local perimeter path
- `CityTimeController.cs`: applies day/sunset/night lighting profile
- `CityWeatherController.cs`: applies weather-based cube shell colors

### Tools, libraries, and APIs
- Unity (project created with `6000.3.6f1`)
- Vuforia Engine (image/multi-target tracking)
- TextMeshPro (in-scene text labels)
- Blender (custom model creation)
- Meshy AI (generated city props)
- Open-Meteo API (`api.open-meteo.com`) for weather and local time
- OpenSky Network API (`opensky-network.org`) for flights-in-sky counts

### How to run the project
1. Install Unity Hub and Unity Editor `6000.3.6f1`.
2. Clone the repository:
   - `git clone https://github.com/khanfarr/ar-project-1.6.git`
3. Open the project folder in Unity Hub.
4. Open scene: `Assets/Scenes/SampleScene.unity`.
5. Add target database in [vuforia engine](https://developer.vuforia.com/develop/)
5. Confirm Vuforia is enabled and your webcam is selected.
6. Enter Play mode and present the tracked cube to the camera.

### Code and live links
- Source code: https://github.com/khanfarr/ar-project-1.6
- Live build: Not deployed yet

## Challenges and Future Work
### Challenges
- Asset sourcing: it was difficult to find Toronto-specific props with a consistent visual style, so I generated several models and then iterated on placement/scaling in Unity.
- Tracking stability: showing both cubes at once while keeping scene alignment stable required hierarchy and transform tuning.
- Real-time data robustness: API requests can fail or return incomplete data, so scripts use fallback displays (`N/A` or last successful value) to keep the UI readable.

### Future work
- Add direct interactivity (click/tap events and city-specific mini interactions)
- Replace basic weather-color mapping with full weather VFX (rain/snow particle systems)
- Improve model polish and scene composition for stronger visual storytelling
- Add more city cubes and a route-selection interaction between locations
- Publish a standalone build and add analytics/performance profiling

## Use of AI and Collaboration

- Meshy AI for generating hard-to-find location-specific 3D models
- Generative AI assistants for a lot of debugging support, initial ideation, and as a script guide.
- AI outputs were always reviewed and adjusted manually in 
I also discussed debugging issues and AR setup decisions with classmates during development.


## Demo Video (2-3 minutes)
Local demo file in this repository:
- `Assets/Media/ar-project-1-demo.mp4`
