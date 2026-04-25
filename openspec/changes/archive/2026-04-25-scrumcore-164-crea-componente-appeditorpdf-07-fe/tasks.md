## 1. Contrato reusable de AppEditorPdf

- [x] 1.1 Definir y tipar las props minimas de `AppEditorPdf` (fuente del documento, pagina activa y configuracion de guias) en `src/app/Components/UI/AppEditorPdf/`.
- [x] 1.2 Ajustar la exportacion publica del componente para mantener un API reusable estable desde `index.ts`.
- [x] 1.3 Verificar que el render con contrato minimo no genera errores de props en tiempo de ejecucion.

## 2. Guias visuales y metricas de pagina

- [x] 2.1 Implementar la capa de guias visuales para limites de pagina/frame de lectura en el viewport activo.
- [x] 2.2 Implementar el calculo de metricas visuales de pagina (incluyendo cambios por pagina y zoom).
- [x] 2.3 Exponer callback opcional `onMetricsChange` y emitir metricas solo ante cambios relevantes.

## 3. Navegacion e integracion consumidora

- [x] 3.1 Asegurar que la navegacion de pagina actualiza guias y recalcula metricas de forma consistente.
- [x] 3.2 Integrar el componente en un consumidor piloto del modulo de gestion sin romper rutas ni layout existentes.
- [x] 3.3 Validar que la integracion no introduce wrappers duplicados ni dependencias nuevas no justificadas.

## 4. Pruebas y evidencia

- [x] 4.1 Crear/ajustar pruebas con Vitest + Testing Library para contrato base, visibilidad de guias y emision de metricas.
- [x] 4.2 Agregar escenarios de prueba para cambios de pagina y zoom sin llamadas de red reales (mocks locales).
- [x] 4.3 Ejecutar la suite focalizada y registrar evidencia de resultados en verde para FE-07.
