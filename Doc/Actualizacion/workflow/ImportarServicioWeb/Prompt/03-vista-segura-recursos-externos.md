# Prompt 03 — Vista segura de recursos externos

Extiende el núcleo y el adaptador SII sin reutilizar como autoridad las URL recibidas en las filas.

## Objetivo

Mostrar un recurso externo todavía no importado mediante una vista mediada y mantener separado el documento ya almacenado.

## Implementa

- Panel lateral dentro del modal; en pantallas pequeñas, subvista completa con **Volver a la lista**.
- Solicitud por proveedor e identidad externa estable, no por URL manipulable enviada como autoridad.
- Estados preparando, disponible, formato no visualizable, recurso vencido, proveedor indisponible y acceso no autorizado.
- Descarga temporal controlada cuando el formato no sea visualizable.
- Acción **Ver documento importado** mediante el visor documental existente solo después de reconciliar un identificador interno autorizado.
- Conservación de selección, filtros, scroll y foco al abrir y cerrar la vista.

## Restricciones

- No insertes directamente una URL externa de la fila en `iframe` ni uses `window.open` como recorrido principal.
- No registres ni muestres tokens, rutas físicas o respuestas externas completas.
- Si el backend mediador aún no existe, deja el estado bloqueado y documenta el contrato requerido; no lo simules en producción.

## Aceptación

- La vista externa se distingue visual y semánticamente del documento importado.
- La expiración permite solicitar un recurso nuevo sin mutación.
- Las pruebas cubren foco, cierre y fallback de formato.

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

## Reglas de ubicacion de codigo
- Si se construye una app reusable o componente compartido, ubicarlo bajo `src/app/Components/<NombreComponente>/` o la ruta compartida equivalente existente.
- Si se implementa comportamiento de modulo funcional, ubicarlo bajo `src/modules/<modulo>/components/`, `hooks/`, `services/`, `adapters/` o `types/` segun responsabilidad.
- Adaptarse a la estructura existente del repo antes de crear carpetas nuevas.

Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Ruta documental obligatoria
La documentacion debe quedar en una ruta canonica segun el contexto:

```txt
Modulo funcional:
docs/modulos/<modulo>/<feature>/SCRUMCORE-000-resumen-del-asunto/

App reusable / nucleo compartido:
docs/Architecture/<area>/<feature>/SCRUMCORE-000-resumen-del-asunto/

Componente compartido documentado historicamente:
docs/Components/<componente>/SCRUMCORE-000-resumen-del-asunto/
```

Usar siempre identificador SCRUMCORE para el paquete documental del frontend.

## Paquete documental minimo
Generar como minimo:

```txt
00-Indice.md
01-Arquitectura.md
02-FlujoIntegracion.md
03-ContratoUploadYMapping.md
04-EstadosErroresYAntiregresion.md
05-PruebasEvidencia.md
06-Diagramas.md
07-Metadata.md
```

00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.

01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.

02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.

03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.

04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.

05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.

06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.

07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.

Crear carpeta `Diagramas/` dentro del paquete documental para diagramas individuales.

## Tabla de funciones creadas o modificadas
| Funcion | Ruta | Ubicacion | Parametros | Responsabilidad |
| --- | --- | --- | --- | --- |
| `<nombre>` | `<path>` | `<componente/hook/service/adapter>` | `<params>` | `<responsabilidad>` |
