# 02 — Interfaz moderna del listado de autorizaciones

## Rol esperado

Desarrollador senior de ASP.NET Web Forms y JavaScript accesible, con experiencia en modales, descargas y regresión visual del Centro de Trabajo.

## Objetivo

Integrar en `Historial` una experiencia moderna oficial para consultar, detallar y descargar autorizaciones sin abandonar el Centro de Trabajo.

## Contexto obligatorio

- Requiere 01 aprobado y contratos disponibles.
- Leer `00-contexto-obligatorio.md`, la evidencia de 01 y el prototipo HTML de Exploración.
- Inspeccionar componentes, estilos y patrones modernos ya existentes; el prototipo guía la interacción, no se copia como arnés paralelo.

## Requisitos positivos

- Conservar el acceso `Historial → Lista de autorizaciones` con un trigger moderno inequívoco y accesible.
- Abrir un modal superpuesto de tamaño estable, con scroll interno, encabezado, contexto único de tarea, contador semántico y cierre seguro.
- Consumir únicamente los contratos de 01 con `IdTarea` explícito; descartar respuestas obsoletas al cambiar tarea, filtro, orden o página.
- Implementar carga, vacío, error/reintento, lista, detalle de solo lectura, filtros aprobados, orden y paginación limitada.
- Descargar individual y consolidado sin abandonar el modal; conservar filtros, página, foco y contexto después de la descarga.
- Proveer nombres accesibles para acciones, navegación por teclado, trampa/restauración de foco, Escape y comportamiento responsive.
- Deshabilitar acciones durante solicitudes incompatibles y evitar duplicados por doble clic.
- Mantener colores, iconos, tabla de tareas, índice, scroll y operaciones vecinas del Centro de Trabajo sin regresión.

## Restricciones críticas

- No autorizar en JavaScript ni inferir permisos por visibilidad.
- No usar `GridView`, `UpdatePanel`, `ModalPopupExtender`, postback, botón oculto, `Hidden_selec_list` o sesión como contrato moderno.
- No crear un segundo framework, proyecto, login, modal genérico incompatible ni feature gate arbitrario.
- No retirar todavía el consumidor compartido ni sus dependencias.

## Criterios de aceptación

- El modal conserva contexto y tamaño con cero, uno y muchos registros.
- El detalle es legible sin exponer campos no autorizados.
- La descarga no reemplaza la página con XML y el usuario puede continuar trabajando.
- Cambiar de tarea no mezcla resultados y el foco regresa al trigger correcto.
- La superficie funciona por teclado y en resoluciones soportadas.

## Pruebas obligatorias

Agregar pruebas CJS/focales de trigger, apertura/cierre, foco, Escape, filtros, orden, paginación, respuesta obsoleta, carga, vacío, error/reintento, detalle, capacidades, descarga sin navegación, consolidado, doble clic, cambio de tarea, responsive y ausencia de postback legacy desde el nuevo trigger.

Integrar en este mismo cambio la E2E real del recorrido conforme a `bloque-e2e-integrado.md`: acceso autorizado, lectura sin mutación, caso vacío, múltiples registros, descarga individual/consolidada cuando los datos y permisos estén autorizados, tarea cruzada y regresión visual relacionada. Ejecutar compilación y registrar evidencia saneada o bloqueo explícito.

## Secuencia funcional esperada

1. El usuario selecciona una tarea y abre `Historial → Lista de autorizaciones`.
2. La UI abre el modal, conserva el foco de origen y consulta con `IdTarea` explícito.
3. El servidor responde con datos y capacidades autorizadas; la UI representa carga, vacío, error o lista.
4. Filtros, orden y página invalidan respuestas anteriores sin mezclar tareas.
5. Ver detalle abre una lectura contextual; descargar entrega un adjunto y conserva modal, filtros y página.
6. Cerrar por botón o Escape restaura el foco al trigger de la misma tarea.

Ejecutar pruebas unitarias/focales CJS sobre el módulo JavaScript y MSBuild sobre el proyecto Web Forms afectado, registrando comandos y resultados. Código, E2E, validación autorizada y evidencia saneada forman una única unidad del cambio; no crear una entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas, tareas y autorizaciones expresamente aprobados. Usar secretos efímeros, verificaciones solo `SELECT` y evidencia saneada; no imprimir ni persistir credenciales, cookies, tokens, cadenas de conexión o XML real.

La cobertura E2E incluye autorización/control de acceso, lectura sin mutación, tarea cruzada, vacío, paginación, detalle, descarga adjunta sin navegación, accesibilidad y regresión visual. Escrituras autorizadas y concurrencia mutante no aplican. Respetar gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

La E2E es parte integral del mismo cambio funcional y de su criterio de cierre. Antes de autenticar se requieren ambiente, cuentas y datos o tareas descartables expresamente autorizados. No cerrar sin validación autorizada; respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente.

## Documentación técnica

Actualizar diagramas, arquitectura UI, secuencia, estados, accesibilidad, matriz de pruebas, evidencia y rollback en el paquete documental canónico.

## Entregable final

Entregar código, pruebas focales CJS, E2E autorizada o bloqueo, MSBuild, documentación y evidencia saneada, y relevo a 03.
