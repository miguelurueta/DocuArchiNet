# DOC-90 — Corrección del adjunto en Radicación Entrante

- Ticket: DOC-90
- Cambio OpenSpec: doc-90-actualizar-adjuntar-documento-radicacion
- Clasificacion: cross_cutting
- Alcance funcional: Radicación Entrante clásica
- Frontera protegida: Radicación Simplificada / DOC-85 y demás consumidores del cargador

## Causa raíz

Radicación Entrante y Radicación Simplificada usaban `evento_adjunta = "ADJUNTARADICACION"`, aunque sus contratos de identidad eran distintos. El cambio `57d43b4f` agregó al transporte el ID de estado y el radicado informativo. Luego `f7171925` reservó la sobrecarga de doce argumentos y `ServicioAdjuntoRadicacion` para Simplificada. El cliente clásico no envía esos campos, por lo que llegaba con ID cero y era rechazado antes del almacenamiento.

La revisión de `57d43b4f^`, `57d43b4f`, `f7171925^` y `f7171925` confirmó además que el recorrido previo resolvía `RA_ID_REGISTRO_RADICADO` en servidor y pasaba el consecutivo resuelto al prealmacenamiento.

Durante la validación funcional se identificó un segundo defecto en la misma pantalla: `inicio_tab_radicador`, `nuevo_radicado_tab` y `terminar_radicado_tab` interpretaban `Hidden_numero_rad_pend > 0` como si existiera una tarea asignada. Esto activaba “Envío y soporte documental” y exponía sus acciones aunque `Hidden_radicado_seleccion` estuviera vacío y `HiddenIdFlujo` no identificara una tarea vigente.

## Solución

- Radicación Entrante conserva `NameLoadProceso = "ADJUNTARADICACION"`, pero emite `ADJUNTARADICACION_CLASICA`.
- El handler reconoce ese evento en una rama propia y llama la sobrecarga legacy de diez argumentos.
- La sobrecarga legacy valida que el módulo sea Radicación Entrante, que la plantilla activa coincida con el registro y que `RA_ID_REGISTRO_RADICADO` sea válido; después resuelve `stru_registro_estado` y rechaza radicado, tarea o trámite inválidos antes de prealmacenar.
- El radicado autoritativo de la estructura prevalece en la nueva ruta clásica; los demás llamadores no suministran ese parámetro opcional.
- `ADJUNTARADICACION` continúa reservado a Simplificada y a la sobrecarga de doce argumentos de DOC-85.
- El contador de pendientes deja de controlar la pestaña documental. Sin radicado restaurado, la pantalla mantiene activa “Recepción y radicación”; cuando el servidor confirma una asignación con `YES`, el callback habilita Soporte sin revalidar controles que pudieran estar fuera de la respuesta parcial.
- `Hidden_radicado_seleccion` viaja en `UpdatePanel_boton_tool`, mientras que campana, total y `Hidden_numero_rad_pend` viajan en un panel visible independiente, evitando estado obsoleto después de asignar.

## Archivos funcionales

- `js/radicacion/WebFormRadicacionEntrante.js`
- `radicador/WebFormRadicacionEntrante.aspx`
- `generic_control/FileUploadHandler_.ashx.vb`
- `workflow/ClassAlmacenamiento.vb`

No se modificaron el cliente ni el ASPX de Radicación Simplificada, `generic_control/FileUploadHandler.js`, `ServicioAdjuntoRadicacion`, su modelo o sus repositorios.

La infraestructura de validación autenticada se agregó bajo `tools/e2e`: escenario Playwright DOC-90, runner interactivo, validador, perfil de ejemplo y prueba local de política. Esta infraestructura no se ejecuta contra un ambiente hasta recibir autorización explícita.

## Reversa

La reversa debe ser conjunta: retirar el evento del cliente clásico y su versión de recurso, retirar la rama del handler y retirar la rama `ADJUNTARADICACION_CLASICA` junto con el parámetro opcional del prealmacenamiento legacy. No requiere migración de datos ni esquema. Una reversa parcial volvería a crear una identidad sin consumidor compatible.
