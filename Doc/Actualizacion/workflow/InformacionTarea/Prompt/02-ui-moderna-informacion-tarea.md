# 02 — Interfaz moderna de Información de la tarea

## Rol esperado

Desarrollador senior de ASP.NET Web Forms y JavaScript accesible, especializado en modales, estado asíncrono y regresión visual.

## Objetivo

Sustituir el acceso visual de `Información de la tarea` por una experiencia moderna oficial conectada al contrato seguro de 01.

## Contexto obligatorio

- Requiere 01 aprobado.
- Leer `00-contexto-obligatorio.md`, evidencia de 01 y ambos documentos de Exploración.
- Reutilizar componentes, estilos y bootstrap modernos existentes; el HTML de Exploración no se copia como framework paralelo.
- Limitar cambios productivos a `workflow/Webworkflow.aspx`, sus archivos JavaScript/CSS existentes y registros mínimos de presentación requeridos.

## Requisitos positivos

- Conservar `Detalle → Información de la tarea` como trigger reconocible, con selector y listener exclusivos.
- Abrir modal o panel según la decisión aprobada, de tamaño estable, scroll interno y comportamiento responsive.
- Consumir `IdTarea` explícito y descartar respuestas obsoletas al cambiar de tarea o cerrar la vista.
- Representar resumen, Workflow e información adicional mediante texto seguro; no interpretar HTML recibido.
- Implementar carga, vacío, error/reintento, éxito y ausencia de campos publicables.
- Ocultar secciones sin contenido y permitir expandir texto largo sin cambiar el tamaño general.
- Implementar foco inicial, trampa/restauración, Tab, Shift+Tab, Escape y nombres accesibles.
- Evitar solicitudes duplicadas y bloquear solo los controles incompatibles durante carga.

## Secuencia funcional

1. El usuario selecciona una tarea y abre `Detalle`.
2. Activa `Información de la tarea`.
3. La UI abre la superficie y consulta con la tarea explícita.
4. Presenta carga y luego resumen/campos, vacío o error saneado.
5. El usuario explora secciones o texto extenso sin mutar información.
6. Al cerrar, se cancela o ignora cualquier respuesta tardía y el foco vuelve al trigger.

## Restricciones críticas

- No autorizar, mapear columnas físicas ni desenmascarar datos en JavaScript.
- No usar postback, `Hidden_menucab`, `Button_tool_menucab`, `ModalPopupExtender`, `UpdatePanel` o `asp:Table` desde el nuevo trigger.
- No agregar edición, descarga, copiado ni persistencia.
- No retirar todavía controles legacy compartidos o no inventariados.

## Reglas de antirregresión

- Preservar tabla de tareas, colores, iconos, índice, scroll, visor y acciones vecinas.
- No compartir estado o listeners con Detalle del trámite, Transacciones o Trazabilidad.
- No mantener dos recorridos alcanzables desde el mismo trigger.

## Criterios de aceptación

- La UI muestra únicamente DTOs autorizados y etiquetas funcionales.
- Cero, uno y muchos campos conservan tamaño y scroll.
- Cambio de tarea o respuesta tardía no mezcla información.
- Teclado, foco, Escape y responsive funcionan de forma verificable.

## Pruebas obligatorias

Agregar pruebas unitarias/focales CJS para bootstrap, trigger, request/response, carga, vacío, error/reintento, secciones, texto extenso, caracteres seguros, respuesta obsoleta, doble clic, cambio de tarea, cierre, foco, teclado, responsive y ausencia de postback. Ejecutar MSBuild y registrar comandos, resultados y evidencia.

La E2E es parte integral del mismo cambio funcional y de su cierre. Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usar secretos efímeros, verificaciones solo `SELECT` y evidencia saneada; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión.

Cubrir autorización/control de acceso, lectura sin mutación, tarea propia/ajena/inactiva, campos variables, vacío, texto extenso, accesibilidad y regresión visual. Escrituras autorizadas y concurrencia mutante no aplican. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente; no cerrar sin validación autorizada. Registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

## Documentación técnica

Actualizar arquitectura UI, secuencia, estados, accesibilidad, matriz, diagramas, evidencia y rollback en el paquete del DOC.

## Entregable final

Entregar código UI, pruebas focales CJS, MSBuild, E2E autorizada o bloqueo, documentación, evidencia saneada y relevo a 03.

