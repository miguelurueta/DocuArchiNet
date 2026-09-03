# Contexto obligatorio — Modernización del listado de autorizaciones

## Uso

Este archivo establece contexto y controles comunes. Por sí solo no autoriza implementar, ejecutar E2E, cambiar gates, descargar información real ni avanzar entre etapas.

- Leer este archivo junto con el prompt numerado enlazado desde Jira.
- Jira controla orden, aprobaciones, bloqueos y cierre.
- No recorrer automáticamente todas las etapas.
- Si falta el DOC, la etapa, su predecesor o una decisión funcional obligatoria, registrar el bloqueo y detener el trabajo afectado.

## Objetivo

Modernizar exclusivamente la consulta y descarga del listado de autorizaciones accesible desde `Historial` en `workflow/Webworkflow.aspx`, conservando el contexto de la tarea y eliminando la navegación a XML crudo.

## Fuentes obligatorias

- `../Exploracion/01-exploracion-modernizacion-listado-autorizaciones.md`.
- `../Exploracion/02-modelo-ui-listado-autorizaciones-moderno.html`.
- `AGENTS.md`.
- `tools/e2e/AGENT-RUNBOOK.md`, antes de diseñar o ejecutar una E2E real.
- Implementación existente en `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/Class_autoriza_tarea_worklfow.vb`, `js/workflow/Webworkflow.js`, `js/workflow/Webworkflow_2.js` y consumidores encontrados por búsqueda completa.

## Alcance común

- Identidad de tarea explícita y revalidada contra el contexto autenticado.
- Consulta paginada y ordenada mediante SQL parametrizado.
- Autorización y pertenencia reconstruidas en servidor.
- Descarga individual y consolidada como adjunto controlado, sin revelar rutas ni mostrar XML crudo.
- Modal accesible, estable, con scroll interno, filtros, detalle, estados de carga/vacío/error y conservación de contexto.
- Migración segura de consumidores y retiro legacy únicamente cuando las referencias activas sean cero.

## Restricciones críticas

- No confiar en tarea, autorización, usuario, estado, columna de orden, ruta o nombre de archivo enviados por el navegador.
- No usar `Session("ID_TAREA_SELECCIONDA")`, campos ocultos o ViewState como autorización.
- No concatenar valores cliente en SQL. Campos y direcciones de orden se resuelven con lista blanca interna.
- No exponer `ex.Message`, SQL, rutas físicas, sesión, secretos ni cuerpos sin sanear.
- No crear login, arnés Playwright, configuración o `.env` paralelo.
- No habilitar gates, usuarios o grupos arbitrariamente para hacer pasar pruebas.
- No retirar `Class_autoriza_tarea_worklfow` ni controles compartidos mientras exista un consumidor no migrado.
- No ampliar el alcance a creación, aprobación, anulación o modificación de autorizaciones.

## Seguridad y no mutación

El listado, detalle y descarga son recorridos de lectura. Sus verificaciones de base de datos son exclusivamente `SELECT`; no cambian tarea, autorización, estado, auditoría ni datos de negocio. Una descarga individual debe revalidar acceso a la tarea y pertenencia de la autorización en el momento de generar el archivo.

## E2E integrada

Toda etapa que cree o modifique un recorrido ejecutable incorpora `bloque-e2e-integrado.md` en el mismo cambio. Código, pruebas focales, E2E autorizada, compilación y evidencia saneada son una sola unidad. Si faltan autorización, ambiente, cuenta o datos adecuados, la etapa queda bloqueada explícitamente; no se sustituye por mocks o evidencia inventada.

## Documentación técnica

Una vez asignado el DOC, crear o actualizar en su carpeta propia:

- `00-indice.md`;
- `01-arquitectura.md`;
- `02-contrato-y-datos.md`;
- `03-flujo-seguridad-y-descargas.md`;
- `04-pruebas-y-evidencia.md`;
- `05-inventario-legacy-y-rollback.md`;
- diagramas profesionales de arquitectura, caso de uso, clases, secuencia y estados cuando aporten trazabilidad.

## Entregable común

Reportar ticket, alcance, archivos modificados, decisiones aplicadas, comandos y resultados, evidencia saneada, riesgos, deuda, rollback y siguiente etapa desbloqueada. No declarar cierre si la implementación final no fue la versión validada.

