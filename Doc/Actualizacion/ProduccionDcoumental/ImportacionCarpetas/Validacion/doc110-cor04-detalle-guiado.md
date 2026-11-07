# COR-04 — Detalle guiado por interacción

Implementado por autorización expresa del usuario en DOC-110. Tareas 12.1–12.6 y desglose individual 12.7–12.17. Sin tablas nuevas, cambios SQL, cambios SII ni publicación al servidor principal.

## Resultado por hallazgo

| Tarea | Resultado implementado | Verificación |
|---|---|---|
| 12.7 | Carga automática de cinco catálogos independientes al abrir; ocho selectores sin Cargar/Recargar opciones. | Prueba editor y recorrido autenticado. |
| 12.8 | Área/serie/subserie esperan contexto; estados esperando, cargando, disponible, vacío y error. | Prueba de dependencias y UI real. |
| 12.9 | Cada cambio de padre limpia descendientes y reinicia página 1; Más resultados solo cuando hay otra página potencial. | Prueba con 50 opciones y cambio de contexto. |
| 12.10 | Época por campo/vista descarta respuestas anteriores. Consultas de catálogo independientes del bloqueo de mutaciones. | Pruebas de respuestas desordenadas y reapertura. |
| 12.11 | Consulta puntual autorizada de referencia guardada, aun fuera de primera página; etiquetas también en publicado. | Prueba de publicado, validación del adaptador y 24 referencias reales RA/DA. |
| 12.12 | Índices cargados al abrir gabinete guardado o seleccionarlo; páginas de 50 hasta completar el límite servidor de 128. | Prueba paginada y recorrido autenticado. |
| 12.13 | Cambio explícito de gabinete limpia destinos anteriores; no elige el único índice automáticamente. Referencia ausente al abrir se conserva con aviso. | Prueba de cambio de gabinete. |
| 12.14 | Comparación contra edición inicial detecta cambios; navegación/cierre/acciones piden decisión antes de descartarlos. Recuperación de estado aparece ante conflicto/resultado incierto. | Prueba cancelación/recuperación y navegador real. |
| 12.15 | Publicar deshabilitado con cambios pendientes; aviso de guardar primero. No guarda automáticamente. | Prueba editor y publicación autenticada posterior al guardado UI. |
| 12.16 | Constante/herencia/extracción/separador/posición/extensión/formato se activan según fuente y tipo; cambio a expediente invalida fuente archivo/herencia. Índices de tipo no soportado no son seleccionables. | Pruebas de controles y payload/validador existentes. |
| 12.17 | Parcial muestra NO_APLICA y deshabilita ámbito; completo vuelve a exigir selección. | Prueba editor y navegador real. |

## Contrato y límites

ConsultarCatalogo incorpora referencia opcional, usando el filtro puntual ya existente en el adaptador. Conserva sesión, CSRF, permisos y separación RA/DA. El editor solicita página 1/tamaño 1 para etiquetas faltantes; no recorre todo el catálogo para encontrar un ID. Un valor ausente no se sustituye por otro. No se inventa dependencia Fondo → Instrumento.

Las consultas no modifican la configuración guardada. En una publicación los campos siguen de solo lectura; los reintentos de lectura fallida quedan fuera del fieldset deshabilitado. Cambiar un padre o gabinete en un borrador sí invalida explícitamente sus valores dependientes, y deja el formulario pendiente de guardar.

Las mutaciones bloquean temporalmente los fieldsets para no perder ediciones realizadas durante una respuesta. Un resultado incierto bloquea nuevas escrituras hasta consultar el estado. La confirmación de descarte es del formulario, no un permiso de despliegue. No se cambian los controles de autorización servidor.

## Evidencia ejecutada

- MSBuild Debug del proyecto WebForms real: PASS; salida .tmp/doc110-cor04-build. DLL anterior respaldado en .tmp/doc110-before-cor04.dll y DLL compatible copiado al bin local para revisión.
- Suite Node: 33/33 PASS. Incluye código VB/C# real del perfil, cliente/editor, soporte E2E y 14 contratos estáticos DOC-88. Los contratos estáticos no acreditan carga documental real.
- Playwright de interfaz con transporte simulado: búsqueda COR-03 y detalle COR-04 PASS. Prueba selección de dependencias, error/reintento, vacío, guardado, protección de navegación e integridad parcial; no sustituye backend real.
- Adaptador RA/DA real mediante SELECT: 24 referencias verificadas, cero referencias no disponibles. Evidencia doc110-cor04-catalogos.json; sin login HTTP ni escrituras de control.
- E2E autenticada local con infraestructura existente y consola visible: PASS. Evidencia saneada doc110-cor04-publicacion-ui.json, finalizada 2026-10-21T00:58:31.979Z (2026-10-20 en Bogotá). Incluye GUIDED_CONFIGURATION_AND_SAVE_UI, publicación EXPEDIENTE, segunda versión LOTE, inmutabilidad, historial y desactivación del perfil descartable.

El recorrido autenticado ahora usa selecciones y campos de reglas de la UI para la primera configuración y su guardado; se retiró su dependencia de recargar tras un guardado por API. El descubrimiento de referencias autorizadas y algunas verificaciones posteriores siguen usando el servicio real, explícitamente.

La corrida autenticada acredita el flujo nominal con una cuenta; no acredita carreras entre dos sesiones ni revocación externa, tareas independientes todavía abiertas en DOC-110. La última corrección de fuente incompatible al cambiar destino se verifica por prueba local específica; no se atribuye ese caso a la corrida autenticada.

Comprobación final: 15/15 Playwright PASS en 11.6 s (dos pruebas UI con transporte simulado y trece de login/rechazo anónimo real); separada de la corrida autenticada preservada en JSON. OpenSpec estricto PASS. Bin local y compilación aislada tienen SHA-256 coincidente. DOC-110 conserva 14 tareas independientes pendientes; COR-04 no representa cierre total del ticket.
