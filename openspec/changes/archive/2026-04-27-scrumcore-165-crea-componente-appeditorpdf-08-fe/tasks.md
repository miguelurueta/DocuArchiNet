## 1. Contrato FE-08 y modelo de contexto de pagina

- [x] 1.1 Definir en `AppEditorPdf` el contrato tipado para estado de contexto de pagina requerido por el contador `Pagina X de Y`.
- [x] 1.2 Implementar la estrategia deterministica de pagina activa con prioridad cursor y fallback por scroll.
- [x] 1.3 Incorporar guards de igualdad para evitar actualizaciones de estado cuando el contexto de pagina no cambia.

## 2. Integracion visual del contador en AppEditorPdf

- [x] 2.1 Renderizar overlay de contador en modo paginacion visual usando el formato `Pagina X de Y`.
- [x] 2.2 Ajustar estilos del contador para no interferir con la interaccion del editor (overlay discreto y no bloqueante).
- [x] 2.3 Validar comportamiento del contador ante cambios de zoom, resize y cambio de documento fuente.

## 3. Publicacion de contexto y compatibilidad reusable

- [x] 3.1 Exponer/ajustar callback opcional de contexto de pagina para consumidores avanzados sin volverlo obligatorio.
- [x] 3.2 Preservar compatibilidad con comportamiento FE-07 (guias visuales y metricas existentes).
- [x] 3.3 Integrar el cambio en un consumidor piloto para verificar uso real sin romper rutas ni layout.

## 4. Pruebas automatizadas y validacion

- [x] 4.1 Crear/actualizar pruebas con Vitest + Testing Library para contador visible, formato y resolucion por prioridad cursor.
- [x] 4.2 Agregar pruebas de fallback por scroll y estabilidad ante eventos rapidos (sin llamadas de red).
- [x] 4.3 Ejecutar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`, registrando evidencia en artefactos OpenSpec del cambio.
