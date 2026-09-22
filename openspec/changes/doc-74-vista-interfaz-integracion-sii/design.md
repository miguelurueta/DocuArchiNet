## Context

DOC-74: VISTA-INTERFAZ-INTEGRACION-SII

## Jira Details

> # Prompt 03 — Vista segura de recursos externos
> 
> Extiende el núcleo y el adaptador SII sin reutilizar como autoridad las URL recibidas en las filas.
> 
> Depende de `GetPreview` y del handler de streaming publicados por backend 01, 02, 06 y 10. La integración productiva queda bloqueada hasta completar B10.
> 
> ## Objetivo
> 
> Mostrar un recurso externo todavía no importado mediante una vista mediada y mantener separado el documento ya almacenado.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> js/workflow/importar-servicio-web/
> ├── importar-servicio-web-preview.js
> └── importar-servicio-web-preview-state.js
> 
> Tests/
> ├── importar-servicio-web-preview.test.cjs
> ├── importar-servicio-web-preview-security.test.cjs
> └── importar-servicio-web-preview-accessibility.test.cjs
> ```
> 
> - El panel se agrega de forma aditiva en `workflow/Webworkflow.aspx`; sus estilos van únicamente en `Styles/importar-servicio-web-modern.css`.
> - Preview usa `importar-servicio-web-api.js`; no crea un segundo cliente HTTP ni inserta URL externa como autoridad.
> - Registrar scripts en el `.vbproj`; no usar JavaScript inline, `window.open` como flujo principal ni modificar visores documentales existentes.
> 
> ## Implementa
> 
> - Panel lateral dentro del modal; en pantallas pequeñas, subvista completa con **Volver a la lista**.
> - Solicitud por proveedor e identidad externa para obtener `DescriptorId`, seguida del handler seguro; nunca por URL SII enviada como autoridad.
> - Estados preparando, disponible, formato no visualizable, recurso vencido, proveedor indisponible y acceso no autorizado.
> - Descarga temporal controlada mediante el mismo handler cuando el formato no sea visualizable.
> - Acción **Ver documento importado** mediante el visor documental existente solo después de reconciliar un identificador interno autorizado.
> - Conservación de selección, filtros, scroll y foco al abrir y cerrar la vista.
> 
> ## Restricciones
> 
> - No insertes directamente una URL externa de la fila en `iframe` ni uses `window.open` como recorrido principal.
> - No transportes bytes como JSON/base64, no construyas URLs técnicas y no reutilices descriptores vencidos.
> - No registres ni muestres tokens, rutas físicas o respuestas externas completas.
> - Si el backend mediador aún no existe, deja el estado bloqueado y documenta el contrato requerido; no lo simules en producción.
> - No modifiques almacenamiento, `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni rutas legacy.
> 
> ## Aceptación
> 
> - La vista externa se distingue visual y semánticamente del documento importado.
> - La expiración permite solicitar un recurso nuevo sin mutación.
> - Cambios de foco, tamaño o layout no repiten la descarga del contenido.
> - Las pruebas cubren foco, cierre y fallback de formato.
> 
> ## Correcciones opsxj:prompt-review
> 
> Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> 
> ## Rol esperado
> Definir el rol tecnico esperado para ejecutar el ticket.
> 
> ## Objetivo
> Describir el objetivo funcional y tecnico verificable.
> 
> ## Restricciones criticas
> - No introducir cambios fuera del alcance declarado.
> - No romper comportamiento existente ni contratos publicos.
> 
> ## Criterios de aceptacion
> - El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.
> 
> ## Contexto obligatorio
> Leer F01–F02, contrato `GetPreview`, fixtures B01/B06, `workflow/Webworkflow.aspx` y el visor documental vigente. El visor existente es referencia/reutilización y no se modifica.
> 
> ## Pruebas obligatorias
> Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> 
> ## Documentacion tecnica
> Actualizar exclusivamente el paquete definido en **Ruta documental obligatoria**, incluida seguridad del preview, expiración, fallbacks, accesibilidad, pruebas y diagramas.
> 
> ## Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> 
> ## Reglas de ubicacion de codigo
> - Usar exclusivamente las rutas declaradas en **Rutas canónicas de implementación**.
> - No crear `src/app`, `src/modules`, otra raíz frontend ni una segunda implementación del visor.
> 
> Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.
> 
> Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.
> 
> Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
> 
> Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.
> 
> Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
> 
> ## Correcciones opsxj:prompt-review
> 
> Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-vista-segura-recursos-externos/
> ```
> 
> Sustituir `SCRUMCORE-000` por el ticket real. No crear documentación en otra ruta ni duplicarla bajo `Doc/Actualizacion`.
> 
> ## Paquete documental minimo
> Generar como minimo:
> 
> ```txt
> 00-Indice.md
> 01-Arquitectura.md
> 02-FlujoIntegracion.md
> 03-ContratoUploadYMapping.md
> 04-EstadosErroresYAntiregresion.md
> 05-PruebasEvidencia.md
> 06-Diagramas.md
> 07-Metadata.md
> ```
> 
> 00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.
> 
> 01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.
> 
> 02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.
> 
> 03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.
> 
> 04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.
> 
> 05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.
> 
> 06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.
> 
> 07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.
> 
> Crear carpeta `Diagramas/` dentro del paquete documental para diagramas individuales.
> 
> ## Tabla de funciones creadas o modificadas
> | Funcion | Ruta | Ubicacion | Parametros | Responsabilidad |
> | --- | --- | --- | --- | --- |
> | `<nombre>` | `<path>` | `<componente/hook/service/adapter>` | `<params>` | `<responsabilidad>` |

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
