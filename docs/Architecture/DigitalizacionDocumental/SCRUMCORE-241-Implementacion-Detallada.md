# SCRUMCORE-241 Implementacion Detallada

## Layout

`DigitalizacionDocumentalModal` conserva el contrato publico existente y ahora renderiza un workbench operativo:

- Header contextual.
- Toolbar de scanner.
- Tres paneles: miniaturas, preview y metadata.
- Footer operacional dentro del cuerpo del modal.
- Acciones primarias/secundarias del `AppModal`.

## Scanner

La UI usa `useDigitalizacionScanner({ client: scannerClient })`. El cliente se inyecta por prop para soportar Dynamsoft real, fakes de prueba o fallback controlado sin scanner.

Acciones implementadas:

- Inicializar/reintentar.
- Seleccionar dispositivo.
- Escanear.
- Rotar pagina seleccionada.
- Eliminar pagina seleccionada.
- Limpiar.
- Generar PDF.

## Miniaturas Y Preview

Las miniaturas se leen desde `scanner.pages`. La seleccion visual se guarda en `selectedPageId`; si no hay seleccion se usa la primera pagina disponible. El preview muestra pagina seleccionada y nombre de PDF generado cuando existe.

## Metadata

El panel muestra el estado actual de `useDigitalizacionDocumentalState`: metadata requerida/opcional, TRD resuelto o pendiente y documento destino en modo `adjuntar`.

## Validaciones Runtime

Las acciones se bloquean cuando hay:

- Contexto invalido.
- Scanner no seleccionado.
- Operacion en progreso.
- Sin paginas para PDF.
- Metadata/documento destino requerido pendiente.

Los errores funcionales se muestran con `role="alert"`.

## Anti-Stale

Al cancelar o cerrar se limpia estado documental, se libera el scanner con `dispose()` y se borra la seleccion de pagina. El hook de scanner ya invalida generaciones antiguas para ignorar respuestas stale.
