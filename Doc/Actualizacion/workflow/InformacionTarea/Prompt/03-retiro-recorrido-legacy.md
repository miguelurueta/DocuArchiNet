# 03 — Retiro del recorrido legacy de Información de la tarea

## Rol esperado

Arquitecto senior de modernización incremental de ASP.NET Web Forms, responsable de inventario, compatibilidad y retiro reversible.

## Objetivo

Retirar exclusivamente la ruta legacy `S-DTS` cuando la experiencia moderna esté aprobada y no existan consumidores activos de sus componentes exclusivos.

## Contexto obligatorio

- Requiere 02 aprobado sobre la versión final.
- Leer `00-contexto-obligatorio.md`, evidencia de 01/02 e inventario actualizado.
- Buscar referencias estáticas y dinámicas de `S-DTS`, `Panel_detalle_flujo`, `ModalPopupExtender_edition_detalle_flujo`, `Table_detalle_flujo`, `UpdatePanel_detalle_flujo`, `Button_detalle_flujo`, `Listar_datos_tarea_workflow`, `Genera_interface_detalle_tarea_workflow` y `auto_zise_popup_detalle_tarea_workflow`.
- Limitar cambios a componentes demostrados como exclusivos del recorrido.

## Secuencia de retiro

1. Clasificar referencias como consumidor, compatibilidad o código muerto.
2. Ejecutar regresión moderna antes de retirar.
3. Desconectar `S-DTS` del despachador genérico sin afectar otros comandos.
4. Retirar markup, code-behind y JavaScript exclusivos.
5. Eliminar funciones de `Class_DAT_ADIC_TAR` solo si sus consumidores activos son cero.
6. Compilar, repetir pruebas focales y ejecutar la E2E final autorizada sobre el código retirado.

## Contratos y eventos

Documentar trigger, listener, request/response, DTOs, estados y eventos conservados. Comparar el contrato moderno con `S-DTS` y demostrar que ningún callback, postback o botón oculto legacy permanece alcanzable.

## Restricciones críticas

- No eliminar `Button_tool_menucab_Click` completo ni otros casos de su despachador.
- No retirar métodos compartidos basándose únicamente en una búsqueda textual.
- No reactivar la ruta legacy como fallback, gate o workaround.
- No cambiar contratos de otras opciones del menú Detalle.

## Reglas de antirregresión

- Preservar Detalle del trámite, Transacciones, Trazabilidad, tabla de tareas, colores, iconos, índice, visor y scroll.
- Mantener autorización y catálogo del backend moderno sin duplicarlos en un adaptador legacy.
- El rollback revierte artefactos; no restaura datos porque el recorrido no muta.

## Criterios de aceptación

- Existe un solo recorrido alcanzable para Información de la tarea.
- Componentes retirados tienen referencias activas en cero o una dependencia justificada impide su eliminación.
- Proyecto, pruebas y UI vecina continúan funcionando.
- No quedan `SELECT *`, postback o controles ocultos alcanzables desde esta opción.

## Pruebas obligatorias

Ejecutar pruebas unitarias/focales VB.NET y CJS, búsquedas de referencias y MSBuild; registrar comandos, resultados y evidencia. Cubrir trigger moderno, ausencia de postback, despachador vecino, error, vacío y no mutación.

La E2E es parte integral del mismo cambio funcional y de su cierre. Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración o `.env` paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usar secretos efímeros; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión; controles solo `SELECT` y evidencia saneada.

Cubrir autorización/control de acceso, lectura sin mutación, ruta moderna única y regresión de Detalle. Escrituras autorizadas y concurrencia mutante no aplican. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente; no cerrar sin validación autorizada. Registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

## Documentación técnica

Actualizar inventario, diagramas, trazabilidad, archivos retirados, pruebas, riesgos y rollback en la documentación existente; registrar cualquier ruta faltante.

## Entregable final

Entregar diff quirúrgico, referencias cero, pruebas focales, MSBuild, E2E autorizada o bloqueo, documentación y relevo a 04.
