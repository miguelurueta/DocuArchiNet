## Context

DOC-73: ADAPTADOR-INTERFAZ-INTEGRACION-SII

## Jira Details

> # Prompt 02 — Adaptador INTEGRACIONSII: consulta y listado
> 
> Implementa el primer adaptador del núcleo creado en el Prompt 01. Lee la exploración completa y conserva los controles de permiso, tarea, ruta, trámite y servicio configurado.
> 
> Depende de los prompts backend 01, 02, 06 y 09. La integración productiva queda bloqueada hasta publicar B09; consume exclusivamente `ResolveCapabilities` y `QueryItems`.
> 
> ## Objetivo
> 
> Encapsular la consulta SII existente y traducirla al contrato común sin mutación.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> js/workflow/importar-servicio-web/sii/
> ├── importar-servicio-web-sii-adapter.js
> ├── importar-servicio-web-sii-contract-mapper.js
> └── importar-servicio-web-sii-list.js
> 
> Tests/
> ├── importar-servicio-web-sii-adapter.test.cjs
> ├── importar-servicio-web-sii-list.test.cjs
> └── importar-servicio-web-sii-query-contract.test.cjs
> ```
> 
> - Reutilizar fixtures de `Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/` y `sii-v1/`; no copiarlos.
> - Registrar módulos nuevos en el `.vbproj` y consumir `importar-servicio-web-api.js`; no hacer AJAX desde lista o mapper.
> - No agregar reglas SII al núcleo, `Webworkflow.aspx`, `Webworkflow.aspx.vb` o scripts globales.
> - No modificar `Webworkflow.js`, ASMX legacy, `Class_consultarInformacionSello` ni almacenamiento.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-adaptador-sii-consulta-listado/
> ```
> 
> Sustituir `SCRUMCORE-000` por el ticket real; crear el paquete canónico y `Diagramas/` sin duplicarlo bajo `Doc/Actualizacion`.
> 
> ## Implementa
> 
> - Adaptador registrado únicamente para `INTEGRACIONSII`.
> - Lectura de cada item ya normalizado por backend: identidad, título, fecha, descripción, estado, metadatos y acciones; no analizar `ExternalKey` para reconstruir campos.
> - Tabla SII con libro, inscripción, fecha, naturaleza/acto, noticia y referencia, sin convertir esas columnas en parte del núcleo.
> - Estados de consulta: preparando, disponible, vacío, indisponible, respuesta inválida y no autorizado.
> - Filtros Todos, Disponibles, Importados y Con novedad; selección solo para elementos importables.
> - Catálogo de tipologías recibido con `ResolveCapabilities`; no consultar tablas ni endpoints legacy desde el frontend.
> - Saneamiento de textos y retiro de registros sensibles en consola.
> 
> ## Restricciones
> 
> - La consulta no cambia documentos, expedientes, índices, caché ni auditoría funcional.
> - Filtrar, seleccionar tipología o paginar localmente no vuelve a llamar SII; una nueva consulta ocurre solo por actualización explícita.
> - No llames directamente el transporte SII ni interpretes respuestas ASMX o códigos legacy desde el frontend moderno.
> - No modifiques `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni el recorrido vigente.
> - No expongas token, credenciales, URL técnica permanente, ruta física ni respuesta externa cruda.
> - No ejecutes consultas reales contra SII durante pruebas automatizadas.
> 
> ## Aceptación
> 
> - Cero, uno y múltiples elementos se representan correctamente.
> - Una consulta produce una sola invocación backend y los filtros operan sobre la respuesta recibida.
> - Elementos importados quedan fuera de la selección masiva.
> - Errores seguros no pierden el contexto de tarea.
> - Las pruebas usan fixtures deterministas y sin red.
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
> Leer F01, contratos/fixtures B01, endpoints publicados por B06 y el comportamiento de consulta en `webservice/WebService_integracion_sii.asmx.vb` y `Integracionccv/Class_consultarInformacionSello.vb` solo como referencia; no modificarlos.
> 
> ## Pruebas obligatorias
> Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> 
> ## Documentacion tecnica
> Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, incluida la frontera núcleo/SII, mapping contractual, estados de consulta, pruebas y diagramas.
> 
> ## Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> 
> Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.
> 
> Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.
> 
> Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
> 
> Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

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
