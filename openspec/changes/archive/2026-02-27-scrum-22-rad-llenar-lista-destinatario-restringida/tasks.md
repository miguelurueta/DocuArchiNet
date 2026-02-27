## 1. Contrato de autocompletado restringido

- [x] 1.1 Ajustar el payload de `Destinatario_Cor` para enviar `ValueAuto` y `CDeRelacionEstadoRetriccionDto` al endpoint restringido.
- [x] 1.2 Normalizar valores de `CDeRelacionEstadoRetriccionDto` (`null` a `0`/`""`) antes de construir la solicitud.
- [x] 1.3 Verificar que no se use el formato legado (`TextoBuscado`, `idScript:0`) en la llamada restringida.

## 2. Sincronización por cambio de trámite

- [x] 2.1 Confirmar que el cambio en `pl-radicacion-spe-Descripcion_Documento` actualiza `selectedTramiteId`.
- [x] 2.2 Asegurar que la consulta de estructura de restricción use el `selectedTramiteId` actualizado.
- [x] 2.3 Propagar la última `CDeRelacionEstadoRetriccionDto` al control de `Destinatario_Cor` antes de consultar autocomplete.

## 3. Control de disparo de consultas

- [x] 3.1 Mantener deshabilitada la consulta restringida en primer render sin interacción del usuario.
- [x] 3.2 Habilitar consulta solo cuando el usuario digite en `Destinatario_Cor` con texto válido.
- [x] 3.3 Mantener comportamiento estándar del control (sin auto-selección implícita del primer item).

## 4. Pruebas y evidencia

- [x] 4.1 Actualizar/agregar pruebas en `RadicacionForm.spec.test.tsx` para escenario de no-disparo en primer render (`[SPEC:...]`).
- [x] 4.2 Actualizar/agregar pruebas de payload restringido para validar envío de `ValueAuto` + `CDeRelacionEstadoRetriccionDto`.
- [x] 4.3 Ejecutar pruebas del módulo Radicación y registrar evidencia de ejecución para el cambio.
