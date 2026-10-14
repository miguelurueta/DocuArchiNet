## Why

REGRESION-TRANSVERSAL. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-105.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt 07 — Regresión transversal, validación remota y despliegue
> ## Contexto de ejecución OPSXJ
> Este prompt corresponde a la Jira independiente que cierra la secuencia. Ejecuta `opsxj:new` al iniciar para crear su OpenSpec autónomo. Inspecciona implementación, pruebas y limitaciones reales de los prompts 02–06; no declares completa una corrección ausente ni reutilices sus cambios OpenSpec.
> ## Rol esperado
> Actúa como arquitecto de calidad y rendimiento full stack con experiencia en WebForms, Playwright, redes y despliegues reversibles.
> ## Fuentes obligatorias
> - Exploración y todas las evidencias `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/` existentes.
> - Los tres constructores del visor enriquecido:
>   - `Docuarchi/ClassVisualisaDocumento.vb`;
>   - `workflow/ClassWorflowVisor.vb`;
>   - `Gestion/ClassGaProducionDocumental.vb`.
> - Consumidores de `Visualiza_documento_workflow_visor(...)` en Workflow y Gestión de Correspondencia.
> - Consumidores simples de `Handler_image_wf.ashx` en Workflow, Gestión, Docuarchi, versiones y migración.
> - `tools/e2e` y el runbook vigente.
> ## Objetivo
> Demostrar que las optimizaciones funcionan de forma transversal, cuantificar el comportamiento remoto y entregar activación/rollback sin afectar módulos que comparten visor, handler o procesamiento PDF.
> ## Matriz obligatoria
> Como mínimo cubre:
> | Consumidor | Visualización | Scroll/zoom | Capacidad opcional | Mutación |
> |---|---|---|---|---|
> | Workflow principal | PDF enriquecido | Sí | grafo/firma/impresión según permisos | grafo descartable |
> | Gestión de Correspondencia | función central | Sí | según sesión | no obligatoria |
> | Producción Documental | URL directa enriquecida | Sí | permisos propios | no obligatoria |
> | Visor de aprobación/externo autenticado | `ClassWorflowVisor` | Sí | según configuración | no obligatoria |
> | Versiones/migración/descarga | handler simple | No aplica | descarga/preview | ninguna |
> Identifica explícitamente consumidores pendientes; no declares cobertura total si no pudieron ejecutarse.
> ## Implementación obligatoria
> 1. Consolidar perfiles E2E no sensibles y reutilizar autenticación/selección existentes.
> 2. Ejecutar línea base y resultado final con el mismo PDF local para comparar bytes, primera página, `domInteractive`, tareas largas, scroll y zoom.
> 3. Ejecutar contra servidor remoto autorizado o emulación de red documentada al menos en perfiles equivalentes a 10, 20, 50 y 100 Mbps, con latencia declarada.
> 4. Verificar que rangos reduzcan bytes necesarios para primera página y que recargas/caché respeten autorización.
> 5. Ejecutar la matriz funcional por módulo y los recorridos simples del handler.
> 6. Ejecutar guardado exactamente una vez sobre PDF descartable, verificando integridad, auditoría permitida y ausencia de doble solicitud.
> 7. Definir presupuestos finales basados en evidencia, separando carga fría/caliente y local/remota.
> 8. Preparar despliegue gradual, observación y rollback de archivos/configuración sin crear un gate legacy por usuario o grupo.
> ## Restricciones críticas
> - No realizar cargas, grafos o cambios reales sin autorizaciones vigentes y datos descartables.
> - Consultas de control, si se requieren, serán exclusivamente `SELECT` con cuenta de solo lectura.
> - No guardar documentos, capturas, rutas, cookies, credenciales o payloads en artefactos.
> - No ocultar módulos no probados bajo una afirmación de no regresión completa.
> - No activar producción desde este prompt salvo autorización explícita adicional.
> ## Criterios de aceptación
> - Primera página remota ya no depende de transferir el PDF completo cuando el servidor admite rangos.
> - Scroll y zoom mejoran materialmente frente a 3.548 ms y 435 ms bloqueantes de la línea base.
> - No hay errores JavaScript no clasificados, solicitudes duplicadas ni temporales compartidos.
> - Todos los consumidores ejecutados conservan comportamiento y permisos.
> - Existe rollback probado y documentación de pendientes.
> ## Documentación técnica
> Entrega en `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`:
> - matriz de consumidores;
> - resultados antes/después;
> - perfiles de red;
> - inventario de pruebas y comandos;
> - errores y pendientes;
> - plan de despliegue y rollback;
> - metadata OPSXJ y trazabilidad D/RQ/tareas.
> ## Entregable final
> Entrega evidencia reproducible, documentación y estado real de cobertura. No archives el OpenSpec si existen regresiones, si el escenario remoto no fue ejecutado y se afirma que pasó, o si el rollback no puede restaurar el comportamiento previo.
> ## Correcciones opsxj:prompt-review
> Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> ## Contexto obligatorio
> Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.
> ## Pruebas obligatorias
> Ejecutar pruebas focales existentes, MSBuild según las capas modificadas y la matriz E2E con Playwright; registrar comandos y resultados.
> ## Documentacion tecnica
> Actualizar el paquete documental canonico del ticket.
> ## Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> Agregar regla para [EXISTING_DOC_UPDATE_REQUIRED]: Ubicar documentacion existente, actualizarla o documentar la ausencia/ruta requerida.
> Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.
> Exigir MSBuild de los proyectos afectados, validaciones Node focales y comandos E2E reproducibles; registrar el resultado.
> Exigir las pruebas focales que ya use cada capa; no introducir Vitest ni Testing Library solo para cerrar la tarea.
> Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.
> Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.
> Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.
> Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.
> Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.
> Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: REGRESION, TRASVERSAL

## Capabilities

### New Capabilities
- `regresion-transversal`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

