# SCRUMCORE-275C - Loader Contasoft para digitalizacion

## Motivacion

El overlay corporativo de digitalizacion ya centraliza los estados de escaneo, procesamiento y generacion PDF. Para cerrar la duplicidad visual y evitar loaders genericos, el indicador del overlay se reemplaza por una animacion basada en el isotipo azul de Contasoft.

El objetivo visual es reforzar identidad de marca sin interferir con `AcquireImage()`, PaperStream IP, Dynamsoft Web TWAIN ni el contrato de progreso `ScanProgressSnapshot`.

## Arquitectura

Se crea `AppContasoftLoader` como componente UI reutilizable en:

- `src/app/Components/UI/AppContasoftLoader/AppContasoftLoader.tsx`
- `src/app/Components/UI/AppContasoftLoader/AppContasoftLoader.module.css`
- `src/app/Components/UI/AppContasoftLoader/AppContasoftLoader.spec.tsx`

El componente no mantiene estado React, no usa timers, no usa canvas y no depende de assets externos en runtime. La animacion vive completamente en CSS sobre un SVG inline.

## SVG Utilizado

El SVG representa unicamente el isotipo azul izquierdo del logo Contasoft como una `C` corporativa vectorial. No incluye texto `Contasoft`, `Company SAS` ni elementos tipograficos.

La marca usa tres trazos:

- `track`: recorrido base tenue de la `C`.
- `fill`: trazo azul principal que se dibuja en sentido horario.
- `innerCut`: trazo interno sutil que sugiere flujo continuo de escaneo/procesamiento.

## Estrategia De Animacion

La animacion usa `stroke-dasharray` y `stroke-dashoffset` con `pathLength="100"`:

1. El trazo inicia vacio.
2. La `C` se llena progresivamente en sentido horario.
3. El recorrido queda completo por un intervalo breve.
4. El trazo reinicia para mantener loop continuo.

La animacion se ejecuta en CSS, evita re-renderes React y mantiene bajo costo durante `AcquireImage()`.

## Integracion Con AppDigitalizador

`DigitalizacionDocumentalWorkspace` integra `AppContasoftLoader` exclusivamente dentro del overlay corporativo de progreso.

Se conservan sin cambios:

- Boton `Cancelar operacion`
- PaperStream IP
- Dynamsoft Web TWAIN
- `AcquireImage()`

La UI visible del overlay se simplifica a:

- `Escaneando documentos`
- `Procesando documentos`
- `Generando PDF`

No se muestran barra de progreso, porcentajes, pagina actual, mensajes tecnicos, SDK, runtime ni driver dentro del overlay.

La integracion aplica para los estados visuales derivados de `scanner.progress` o `scanner.loading`, incluyendo `initializing`, `scanning`, procesamiento posterior y `generatingPdf`.

## Validacion Visual

La validacion esperada en hardware real es:

- No aparece spinner circular generico en el overlay corporativo.
- Se visualiza la `C` azul animada.
- PaperStream IP sigue funcionando cuando el modo driver lo requiere.
- El overlay corporativo mantiene solo el texto corporativo del estado actual.
- No hay degradacion perceptible durante escaneo simple, ADF, duplex, Deskew, Auto Crop, Auto Rotate y generacion PDF.
