# Prompts Jira/OpenSpec: rendimiento y estabilidad del visor PDF con grafos

Fuente arquitectónica común: `../Exploracion/01-diagnostico-rendimiento-visor-pdf-grafos.md`.

La exploración ya cumple la función de diagnóstico, observabilidad inicial y línea base; no se convierte en un prompt de implementación. Se conserva la numeración original porque los archivos ya fueron adjuntados en Jira. El Prompt 01 quedó retirado y su única deuda pendiente fue absorbida por el Prompt 02. Cada prompt activo comienza con `opsxj:new` para crear su propio OpenSpec Spec-Driven.

| Orden | Prompt | Resultado implementable | Verificación incluida |
|---:|---|---|---|
| 01 | [01-observabilidad-y-error-javascript.md](01-observabilidad-y-error-javascript.md) | **Retirado.** Se conserva para trazabilidad con Jira; no se ejecuta. | Alcance absorbido por el Prompt 02. |
| 02 | [02-planificador-render-y-viewport.md](02-planificador-render-y-viewport.md) | Error JavaScript clasificado, resultado E2E honesto, cola secuencial, deduplicación, cancelación y scroll/zoom no bloqueantes. | Contratos del runner, pruebas deterministas y comparación con línea base. |
| 03 | [03-arranque-y-dependencias-diferidas.md](03-arranque-y-dependencias-diferidas.md) | Dependencias únicas, locales cuando corresponda y carga diferida de edición. | Matriz de capacidades y regresión visual/funcional. |
| 04 | [04-streaming-http-range-y-cache.md](04-streaming-http-range-y-cache.md) | Handler con política adaptativa `RANGE`/`STREAM` y PDF.js consumiendo URL sin materialización completa. | Contratos HTTP, métricas de bytes únicos/físicos y POC efímera de Fast Web View. |
| 05 | [05-transaccion-frontend-guardado-grafo.md](05-transaccion-frontend-guardado-grafo.md) | Guardado esperado, single-flight, errores saneados y UI consistente. | Dobles sin escritura y E2E mutadora descartable autorizada. |
| 06 | [06-procesamiento-servidor-grafo.md](06-procesamiento-servidor-grafo.md) | Temporales aislados, procesamiento iText seguro y clasificación del PDF resultante para `RANGE`/`STREAM`. | Caracterización documental incluida `/Linearized` antes/después, concurrencia y rollback. |
| 07 | [07-regresion-transversal-y-validacion-remota.md](07-regresion-transversal-y-validacion-remota.md) | Matriz final de módulos, presupuestos de rendimiento y despliegue gradual. | E2E por consumidor, prueba remota y rollback verificable. |

Reglas comunes:

- Ejecutar los prompts activos 02–07 en orden. Cada tarea inspecciona el código vigente y los artefactos reales de tareas anteriores; no asume que una propuesta previa fue implementada.
- Ejecutar `opsxj:new` al iniciar cada prompt activo. Cada Jira mantiene un único OpenSpec autónomo con `proposal.md`, `design.md`, delta spec y `tasks.md` atómicas; no reutilizar el cambio de otra Jira ni abrir un segundo cambio para la misma.
- Antes de editar, registrar en `design.md` rutas, símbolos y firmas exactas que se modificarán. No crear capas, controladores, servicios o DTOs innecesarios para una corrección localizada de JavaScript/WebForms.
- No agregar tablas ni columnas. No cambiar contratos documentales, permisos, selección de tarea, firmas digitales, auditoría o reglas de negocio salvo que el prompt lo indique expresamente.
- Preservar los consumidores reales: Workflow, Gestión de Correspondencia, Producción Documental, visores de aprobación/externos y los recorridos que continúan usando `Handler_image_wf.ashx` sin visor enriquecido.
- Conservar compatibilidad de `ClassVisualisaDocumento.Visualiza_documento_workflow_visor(...)`, `ClassWorflowVisor`, `ClassGaProducionDocumental`, `WebService_itext.service_anotate_pdf_image(...)` y `Class_ItexShare.add_anotate_image_pdf(...)`, salvo una evolución explícita y probada del contrato.
- No exponer en URL, evidencia, logs o errores nuevas rutas físicas, cookies, cadenas de conexión, contenido PDF, firmas, datos de usuario o payloads base64. Toda evidencia E2E debe permanecer saneada.
- Reutilizar `tools/e2e`, Playwright, `authenticated-workflow-session.cjs` y el runner de rendimiento existente. No crear otro arnés de navegador.
- Cada prompt incorpora pruebas desde su propia entrega. Si introduce una regresión, debe corregirla y repetir la matriz afectada dentro de la misma Jira; no diferir toda la validación al Prompt 07.
- Las E2E reales requieren autorización vigente de ambiente y cuenta. El guardado de grafos exige además un PDF descartable y autorización explícita de modificación.
- Todo archivo nuevo que forme parte del proyecto WebForms debe registrarse en `GestionDocumental-Docuarchi.net.vbproj` reproduciendo el tipo de elemento de un equivalente existente.
- Registrar comandos y códigos de salida reales. Un bloqueo se informa sin afirmar que la prueba pasó.
- No equiparar soporte HTTP Range con descarga parcial garantizada. La evidencia debe separar cobertura única del PDF y bytes físicos de red; documentos no linealizados pueden usar `STREAM` sin modificar el almacenamiento.

La secuencia separa deliberadamente lectura y mutación. Los prompts activos 02–04 no deben guardar grafos ni modificar PDFs; los prompts 05–06 trabajan únicamente con documentos descartables y controles antes/después. El Prompt 07 solo ejecuta mutaciones cuando exista autorización explícita y un documento descartable.
