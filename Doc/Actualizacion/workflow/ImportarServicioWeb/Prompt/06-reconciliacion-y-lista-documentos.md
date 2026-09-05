# Prompt 06 — Reconciliación y lista de documentos

Implementa el cierre funcional de una importación utilizando el backend como fuente de verdad.

## Objetivo

Actualizar cada elemento externo y hacer visibles los documentos confirmados en la lista principal de documentos de la tarea original.

## Implementa

- Estado por elemento: Disponible, Procesando, Verificando, Importada, Omitida, Fallida o No procesada.
- Consulta de reconciliación por intención, tarea, proveedor e identidad externa.
- Relación entre elemento externo e identificador interno del documento.
- Actualización o refresco de la lista de documentos sin duplicados.
- Puente temporal con el resultado que alimenta `insert_row_documento_relacionado(...)`, encapsulado en el adaptador.
- Acción **Ver documento importado** cuando exista identificador autorizado.
- Conservación de filtros y scroll; limpieza de selección y foco predecible al volver.

## Restricciones

- No elimines una fila externa ni la marques importada por una actualización optimista.
- Timeout o ausencia de respuesta conduce a Verificando, no a Disponible ni Importada.
- Si la vista actual corresponde a otra tarea, no insertes allí los documentos de la tarea original.

## Aceptación

- Cada documento confirmado aparece una sola vez en la lista de la tarea correcta.
- Un resultado incierto se resuelve mediante reconciliación.
- Cerrar y volver a abrir conserva el estado persistido.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
