# Prompts Jira/OpenSpec: rendimiento y estabilidad del visor PDF con grafos

Fuente arquitectónica común: `../Exploracion/01-diagnostico-rendimiento-visor-pdf-grafos.md`.

La exploración ya cumple la función de diagnóstico y no se convierte en un prompt `00`. Cada prompt se monta como una tarea Jira independiente y crea o continúa su propio OpenSpec Spec-Driven dentro del contexto OPSXJ recibido.

| Orden | Prompt | Resultado implementable | Verificación incluida |
|---:|---|---|---|
| 01 | `01-observabilidad-y-error-javascript.md` | Error JavaScript clasificado/corregido y línea base reproducible. | Contratos del runner y E2E de lectura autorizada. |
| 02 | `02-planificador-render-y-viewport.md` | Cola secuencial, deduplicación, cancelación y scroll/zoom no bloqueantes. | Pruebas deterministas y comparación con línea base. |
| 03 | `03-arranque-y-dependencias-diferidas.md` | Dependencias únicas, locales cuando corresponda y carga diferida de edición. | Matriz de capacidades y regresión visual/funcional. |
| 04 | `04-streaming-http-range-y-cache.md` | Handler con rangos/streaming y PDF.js consumiendo URL sin materialización completa. | Contratos HTTP y E2E local/remota de primera página. |
| 05 | `05-transaccion-frontend-guardado-grafo.md` | Guardado esperado, single-flight, errores saneados y UI consistente. | Dobles sin escritura y E2E mutadora descartable autorizada. |
| 06 | `06-procesamiento-servidor-grafo.md` | Temporales aislados y procesamiento iText/transparencia medido y seguro. | Caracterización documental, concurrencia y rollback. |
| 07 | `07-regresion-transversal-y-validacion-remota.md` | Matriz final de módulos, presupuestos de rendimiento y despliegue gradual. | E2E por consumidor, prueba remota y rollback verificable. |

Reglas comunes:

- Ejecutar en orden. Cada tarea inspecciona el código vigente y los artefactos reales de tareas anteriores; no asume que una propuesta previa fue implementada.
- No ejecutar `opsxj:new` dentro del prompt. El ticket OPSXJ ya debe existir y cada Jira mantiene un único OpenSpec autónomo con `proposal.md`, `design.md`, delta spec y `tasks.md` atómicas.
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

La secuencia separa deliberadamente lectura y mutación. Los prompts 01–04 no deben guardar grafos ni modificar PDFs; los prompts 05–06 trabajan únicamente con documentos descartables y controles antes/después.
