# Validación

Entorno: Windows, Unity 6000.3.16f1, exportación StandaloneWindows64 de desarrollo.

## Resultado observado

Validación realizada el **29 de octubre de 2026**: compilación de Windows exitosa, comprobación de escenas y audio exitosa, y **22 de 22 comprobaciones de ejecución aprobadas**, incluidas las cuatro opciones del selector de niveles. Las capturas del repositorio proceden del ejecutable y se revisaron visualmente.

## Comprobaciones de editor

`CampaignValidation.BuildWindows` comprueba que estén habilitadas las siete escenas, que cada una tenga cámara, que no haya componentes con scripts faltantes y que cada nivel tenga un jugador y cámara de seguimiento. También comprueba los clips de música y SFX y genera el ejecutable. El resultado esperado es `CAMPAIGN_VALIDATION_OK`.

Ejemplo en PowerShell, ajustando la ruta del editor:

```powershell
& 'E:\Unity xd\Editor\Unity.exe' -batchmode -nographics -quit -projectPath (Get-Location).Path -executeMethod CampaignValidation.BuildWindows -logFile unity-build.log
```

Para reconstruir las escenas explícitamente se puede usar `-executeMethod CampaignValidation.BuildAll`. Esto sobrescribe las escenas generadas.

## Comprobaciones de ejecución

El ejecutable de desarrollo incluye un comprobador optativo. La partida normal no lo activa.

```powershell
& '.\Builds\Windows\Mario.exe' -campaignSmokeTest -screen-width 1280 -screen-height 720 -screen-fullscreen 0 -logFile unity-smoke.log
```

Este modo recorre los siguientes casos y genera ocho imágenes desde la cámara real del juego:

1. Partida nueva con tres vidas y puntaje cero.
2. Pausa del reloj.
3. Cien monedas: una vida extra y 20000 puntos.
4. Moneda siguiente: contador y puntaje correctos.
5. Conservación de la sesión al cambiar de nivel.
6. Salida física del subterráneo hacia el acuático.
7. Natación habilitada en el acuático.
8. Salida física del acuático hacia el castillo.
9. Activación del hacha, derrumbe del puente y victoria.
10. Crecimiento con powerup.
11. Reducción e inmunidad temporal al recibir daño.
12. Muerte repetida: solo se consume una vida.
13. Game Over al llegar a cero vidas.
14. Reinicio completo de contadores al reintentar.

Además se comprueban las cuatro opciones del selector y que cada botón abra el nivel correcto con una sesión nueva: **22 comprobaciones en total**.

El informe local se escribe en `TestResults/campaign-smoke.txt` y el proceso devuelve 0 cuando todos los casos pasan. El archivo de log contiene el resultado de cada caso. `Builds`, `TestResults` y los logs locales no se suben a Git.

## Alcance

La prueba coloca al jugador cerca de las salidas para comprobar colisiones y transiciones; no sustituye recorrer todos los obstáculos con teclado. Antes de presentar, conviene jugar la campaña completa, comprobar la dificultad de los saltos y escuchar los sonidos con el volumen del equipo de exposición.

Los mapas se reconstruyeron por celdas a partir de las referencias indicadas en el README. Se adaptaron las físicas, enemigos, ascensores y conexiones de escenas; no se afirma equivalencia exacta con el juego de NES.
