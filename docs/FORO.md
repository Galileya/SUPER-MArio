# Borrador de publicación individual

Antes de publicar, añade tu nombre y ajusta especialmente la opinión personal para que represente tu experiencia. Adjunta la imagen como archivo en el foro: un enlace a este documento no sustituye la captura que pide la actividad.

## Características agregadas

El fork incorpora un menú inicial que se puede usar con mouse o teclado, música para cada ambiente y efectos de salto, pisotón, powerup, daño y muerte. También muestra tiempo restante, puntaje, monedas y vidas. El puntaje aumenta al recoger monedas y eliminar enemigos, y al reunir 100 monedas se obtiene una vida adicional.

La campaña pasa del nivel de superficie a un escenario subterráneo basado en el mapa original 1-2, después a un nivel acuático basado en 2-2 y finalmente a un castillo de diseño propio. El nivel acuático permite nadar con impulsos y contiene enemigos animados. En el castillo hay lava, plataformas móviles y un encuentro con Bowser: al alcanzar el hacha, el puente se derrumba. Se agregó una escena de Game Over cuando se terminan las vidas y una pantalla de victoria al completar la campaña.

## Dificultades identificadas

Una dificultad técnica fue mantener vidas, monedas y puntaje al cambiar de escena, pero reiniciarlos al comenzar una nueva partida. También fue necesario evitar que varios eventos de muerte descontaran más de una vida. La solución utiliza una sesión persistente y un control que impide repetir la transición.

Otra dificultad fue adaptar el movimiento al agua: reutilizar la gravedad y el salto de tierra hacía que Mario descendiera demasiado rápido. Se implementaron una gravedad reducida, una velocidad de caída limitada y nuevos impulsos al presionar Espacio. Además, se ajustaron los límites de cámara y las posiciones de salida para que el recorrido continuara correctamente.

## Característica preferida — propuesta para personalizar

La característica que más me gustó fue el castillo de Bowser, porque permitió proponer un escenario propio. El nivel no utiliza el prefab de castillo: su fortaleza tiene dos torres y un portón construidos bloque por bloque. Las plataformas sobre la lava y el puente final aportan una dificultad distinta a los otros niveles. El derrumbe del puente, la caída de Bowser y el sonido de final ayudan a comunicar que se completó el objetivo.

## Captura y repositorio

La siguiente captura muestra el diseño propio del castillo, el puente, Bowser y los contadores mencionados.

![Castillo original con Bowser y HUD](capturas/08-fortaleza-original.png)

**Adjunto recomendado:** `docs/capturas/08-fortaleza-original.png`.

**Repositorio público (fork):** https://github.com/Galileya/SUPER-MArio

El comentario opcional a otro compañero debe redactarse después de leer su publicación para señalar un aspecto positivo y una sugerencia concretos.
