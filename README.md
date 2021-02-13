# Procedural-Exploration
### Delving into procedural generation for 3d games in Unity

# Section 1: Noise

For this example we will be using Perlin noise - Unity has built in implementation for this, making it the most logical choice.

Amplitude will control our Y axis, and frequency will control our X axis for noise. In order to smooth transitions between terrain we will
use multiple noise maps referred to as octaves. The frequency of these octaves will be controlled by what we will call Lacunarity. 

- Octave 1: Main outline
    - frequency = lacunarity^0
- Octave 2: Boulders
    - frequency = lacunarity^1
- Octave 3: Small rocks
    - frequency = lacunarity^2

Using a Lacunarity of 2, this gives us a frequency of 1,2, and 4 respectively. As each octave increases in detail(frequency), it's overall 
influence on terrain should decrease, from the example above, the smaller the rock the less effect it should have on the main outline.

We can control this with Persistence, which controls the change in amplitude of our octaves.

- Octave 1: Main outline
    - frequency = lacunarity^0
    - amplitude = persistance^0
- Octave 2: Boulders
    - frequency = lacunarity^1
    - amplitude = persistance^1
- Octave 3: Small rocks
    - frequency = lacunarity^2
    - amplitude = persistance^2

With a Persistance value of 0.5, our amplitude will be 1, 0.5, and 0.25 respectively.

Applied to a topographical 2d map, increasing our lacunarity will increase the number of small features, and persisitence will increase the
overall effect of small features on the map.

