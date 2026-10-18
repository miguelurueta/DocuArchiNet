<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-105-regresion-transversal

## Fuente y alcance

- Ticket: `DOC-105` — REGRESION-TRANSVERSAL.
- Cambio: `doc-105-regresion-transversal`.
- Perfil: WebForms/VB.NET + handler HTTP + PDF.js + Playwright/Node.
- Alcance: arnés transversal, medición reproducible, matriz real, documentación, despliegue y rollback. Sin cambio inicial de almacenamiento ni activación productiva.

## Contexto inspeccionado

- Constructores: `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb`, `Gestion/ClassGaProducionDocumental.vb`.
- Política/entrega: `Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb`, `workflow/Handler_image_wf.ashx.vb`, `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`.
- Consumidores centrales: `workflow/Webworkflow.aspx.vb`, `Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx.vb`.
- Consumidores simples: visores/descargas de Docuarchi, Gestión, versiones, migración y otros formatos enlazados directamente al handler.
- Automatización: `tools/e2e`, `tools/e2e/AGENT-RUNBOOK.md` y evidencia saneada DOC-100 a DOC-104.

## Decisiones propuestas para aprobación

| ID | Decisión verificable | Evidencia de código | Requirement |
| --- | --- | --- | --- |
| D-01 | Inventariar llamadas reales y estado individual por consumidor. | Tres constructores y consumidores del handler. | RQ-01 |
| D-02 | Usar un orquestador DOC-105 dentro de `tools/e2e`, sin autenticación paralela. | Helpers y runners existentes en `tools/e2e`. | RQ-02 |
| D-03 | Separar adaptadores de navegación del medidor común. | Rutas WebForms heterogéneas y visor/handler compartidos. | RQ-03 |
| D-04 | Medir 10/20/50/100 Mbps con latencia declarada y ciclos frío/caliente. | Runner Playwright de rendimiento existente. | RQ-04 |
| D-05 | Separar unión de rangos (bytes únicos) de transferencia física. | `Content-Range`, `Content-Length` y Resource Timing. | RQ-05 |
| D-06 | Saneamiento estricto y estados PASSED/FAILED/BLOCKED/NOT_APPLICABLE. | Política de evidencia y runbook vigentes. | RQ-06 |
| D-07 | Permitir exactamente un guardado de grafo y solo SELECT de control. | Runner DOC-104 y `workflowdocument`. | RQ-07 |
| D-08 | Derivar presupuestos con el mismo PDF y condiciones comparables. | Baselines DOC-100 y resultados DOC-101/102. | RQ-08 |
| D-09 | Preparar despliegue/rollback sin gate por usuario o grupo. | Restricción de AGENTS.md y artefactos DOC-100–104. | RQ-09 |
| D-10 | Corregir código productivo solo ante regresión reproducible y localizada. | Proyectos WebForms afectados y pruebas focales existentes. | RQ-10 |
| D-11 | Preparar en segundo plano un derivado temporal linealizado, conservando el original para negocio y fallback. | Estrategia, emisores enriquecidos y evidencia remota STREAM/RANGE. | RQ-11 |

## Requisitos verificables

| ID | Resultado observable | Criterio principal | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Matriz enumera todo consumidor conocido. | Ninguna fila omitida o declarada aprobada sin ejecución. | Cobertura falsa. |
| RQ-02 | Un arnés reutiliza autenticación y políticas existentes. | No aparece login, `.env` o Playwright paralelo. | Fuga de secretos. |
| RQ-03 | Todos los módulos producen el mismo esquema de medición. | Adaptador navega; medidor observa. | Selectores frágiles. |
| RQ-04 | Hay muestras frías/calientes para cuatro redes. | Perfil y latencia quedan declarados. | Caché sesgada. |
| RQ-05 | Evidencia distingue bytes únicos/físicos. | Parcial solo con 206 y unión menor al total. | Falso ahorro. |
| RQ-06 | Evidencia es saneada y cobertura honesta. | Bloqueos no se convierten en éxitos. | Datos sensibles. |
| RQ-07 | Existe un solo POST de grafo autorizado. | Conteo POST=1 y consultas solo SELECT. | Doble mutación. |
| RQ-08 | Presupuestos provienen de comparaciones equivalentes. | Reporte separa local/remoto y frío/caliente. | Meta arbitraria. |
| RQ-09 | Rollback es reproducible y no usa gate legacy. | Smoke posterior restaura visor anterior. | Despliegue irreversible. |
| RQ-10 | Toda corrección queda localizada y revalidada. | Prueba focal + MSBuild + celda repetida. | Cambio transversal accidental. |
| RQ-11 | Un PDF grande no linealizado puede lograr descarga parcial sin cambiar almacenamiento. | Primera apertura no bloquea; una posterior usa 206 y menos bytes antes del canvas. | Costo CPU, licencia del linealizador y caché temporal. |

## Resultado del refinamiento

- Estado: aprobado por el usuario para implementar D-01 a D-10.
- La aprobación no autoriza E2E autenticada, mutaciones ni despliegue productivo; esas acciones conservan sus compuertas independientes.
