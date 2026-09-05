# Prompt 02 — Adaptador INTEGRACIONSII: consulta y listado

Implementa el primer adaptador del núcleo creado en el Prompt 01. Lee la exploración completa y conserva los controles de permiso, tarea, ruta, trámite y servicio configurado.

## Objetivo

Encapsular la consulta SII existente y traducirla al contrato común sin mutación.

## Implementa

- Adaptador registrado únicamente para `INTEGRACIONSII`.
- Normalización de cada inscripción a identidad externa, título, fecha, descripción, estado, metadatos presentables y acciones.
- Tabla SII con libro, inscripción, fecha, naturaleza/acto, noticia y referencia, sin convertir esas columnas en parte del núcleo.
- Estados de consulta: preparando, disponible, vacío, indisponible, respuesta inválida y no autorizado.
- Filtros Todos, Disponibles, Importados y Con novedad; selección solo para elementos importables.
- Saneamiento de textos y retiro de registros sensibles en consola.

## Restricciones

- La consulta no cambia documentos, expedientes, índices, caché ni auditoría funcional.
- No expongas token, credenciales, URL técnica permanente, ruta física ni respuesta externa cruda.
- No ejecutes consultas reales contra SII durante pruebas automatizadas.

## Aceptación

- Cero, uno y múltiples elementos se representan correctamente.
- Elementos importados quedan fuera de la selección masiva.
- Errores seguros no pierden el contexto de tarea.
- Las pruebas usan fixtures deterministas y sin red.

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

Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.
