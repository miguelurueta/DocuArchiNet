# 03 — Migración de consumidores y retiro legacy

## Rol esperado

Arquitecto senior de modernización incremental en ASP.NET Web Forms, responsable de trazabilidad de referencias y compatibilidad.

## Objetivo

Migrar los consumidores restantes del listado de autorizaciones y retirar únicamente la ruta legacy comprobada como no utilizada.

## Contexto obligatorio

- Requiere 02 aprobado sobre la implementación final.
- Leer `00-contexto-obligatorio.md`, evidencia de 01/02 e inventario actualizado.
- Revisar quirúrgicamente referencias de `Class_autoriza_tarea_worklfow`, `Panel_lista_autorizacion`, `ModalPopupExtender_edition_lista_autorizacion`, `data_grid_listado_solicitudes`, `Hidden_selec_list`, botones de descarga y handlers relacionados.

## Requisitos positivos

- Adaptar `WebFormConsultaTareasWorkflow` y cualquier consumidor real al servicio/repo seguro o a un adaptador compatible, según su experiencia aprobada.
- Ejecutar regresión antes de retirar código compartido.
- Retirar del Centro de Trabajo el postback, controles ocultos, UpdatePanel, GridView, modal y JavaScript de descarga solo cuando la ruta moderna sea única.
- Eliminar clase o métodos legacy únicamente cuando una búsqueda completa y compilación confirmen cero consumidores activos.
- Mantener fuera del retiro cualquier capacidad de creación, aprobación, anulación u otro historial no incluido.
- Documentar cada elemento conservado, migrado o eliminado y su razón.

## Restricciones críticas

- No convertir la ausencia de referencias textuales en prueba única; considerar markup dinámico, diseñador, eventos, reflexión y consumidores externos documentados.
- No borrar código compartido para forzar la compilación ni alterar comportamiento no aprobado del consumidor secundario.
- No mantener dos triggers alcanzables para el mismo listado en el Centro de Trabajo.

## Criterios de aceptación

- Todos los consumidores inventariados funcionan sobre reglas seguras o quedan explícitamente fuera con dependencia justificada.
- En el Centro de Trabajo existe un solo recorrido alcanzable.
- Los controles y handlers retirados tienen cero referencias activas y el proyecto compila.
- El rollback revierte artefactos de despliegue; no requiere reactivar inseguramente una ruta oculta.

## Pruebas obligatorias

Ejecutar pruebas focales y compilación para ambos consumidores, descargas, vacío, error, permisos y no mutación. Integrar la E2E de regresión aplicable conforme a `bloque-e2e-integrado.md`, incluida la ruta moderna oficial y el consumidor secundario cuando sea accesible mediante el arnés aprobado. Registrar evidencia saneada o bloqueo; no simular consumidores.

## Contratos que deben quedar documentados

Documentar por consumidor el trigger/evento, request y response, DTOs, códigos funcionales, capacidades de descarga, callbacks de éxito/error y transporte del archivo. Comparar el contrato anterior con el moderno y justificar cada adaptador temporal conservado.

## Secuencia de migración

1. Inventariar y clasificar cada referencia como consumidor, compatibilidad o código muerto.
2. Migrar y probar primero el consumidor secundario sin retirar el adaptador compartido.
3. Confirmar el recorrido moderno del Centro de Trabajo y la regresión de ambos consumidores.
4. Retirar controles, handlers y scripts legacy del Centro de Trabajo.
5. Eliminar métodos o clases compartidos solo después de demostrar referencias activas en cero y compilar.
6. Repetir pruebas focales y E2E sobre el código final retirado.

Ejecutar pruebas unitarias/focales VB.NET y CJS, además de MSBuild, con comandos y resultados. Código, E2E, validación autorizada y evidencia saneada forman una única unidad del cambio; no crear una entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas, tareas y datos expresamente autorizados. Usar secretos efímeros, verificaciones solo `SELECT` y evidencia saneada; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión.

La cobertura E2E incluye autorización/control de acceso, lectura sin mutación, ruta moderna única, descarga, consumidor secundario y regresión. Escrituras autorizadas y concurrencia mutante no aplican. Respetar gates y seguridad sin habilitarlos arbitrariamente; registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

La E2E es parte integral del mismo cambio funcional y de su criterio de cierre. Antes de autenticar se requieren ambiente, cuentas y datos o tareas descartables expresamente autorizados. No cerrar sin validación autorizada; respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente.

## Documentación técnica

Actualizar inventario final, diagramas, contratos, matriz de trazabilidad, archivos retirados, pruebas, riesgos y rollback en la documentación existente; si falta una ruta, registrar su ausencia.

## Entregable final

Entregar diff quirúrgico, referencias cero demostrables, pruebas focales, MSBuild, E2E autorizada o bloqueo, documentación y relevo a 04.
