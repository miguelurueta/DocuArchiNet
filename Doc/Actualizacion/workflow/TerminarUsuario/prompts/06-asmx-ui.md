# 06 — Integración visual de Enviar a usuario

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript legacy accesible, con experiencia en migraciones Strangler Fig.

## OBJETIVO

Conectar el comando `Enviar a usuario` del Centro de trabajo a los endpoints implementados en `WebServiceWorkflowModern.asmx`, reutilizando componentes genéricos y preservando el postback legacy como fallback.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- No crear otro ASMX, framework, bundler, módulo ES, `ConfirmationDialog` ni segunda evaluación de gate.
- No cambiar endpoints, trigger, payload `IdConector` ni flujo visual de Continuar flujo.
- JavaScript no decide permiso, ruta, flujo, requisitos, respuesta o destino; solo representa JSON del servidor.
- No llamar controles ocultos, handlers Web Forms, `Terminar_Tarea_Workflow`, `Cambia_Estado` ni repositorios desde JavaScript.
- Con gate inactivo, el enlace que hoy activa `ImageButtonEnviarUsuario` conserva su postback y modal legacy exactos.
- No incorporar interacción de reasignación, credenciales administrativas ni campos de respuesta.

## REQUISITOS POSITIVOS

1. Reutilizar `ConfirmationDialog`, CSS de confirmación, accesibilidad, foco, Escape, prevención de doble clic y presentación de éxito existentes.
2. Crear un adaptador o instancia JavaScript exclusiva para usuario, con selector del comando, modal, preview y ejecución propios; no compartir listeners ni estado con Continuar flujo.
3. Convertir el destino seleccionado al contexto `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }` y consumir solo `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`.
4. Mostrar bloqueo funcional si la respuesta está pendiente, sin abrir ningún camino legacy de reasignación desde la UI moderna.
5. Tras éxito correlacionado, retirar solo la tarea afectada, limpiar contexto/visor, actualizar contador y mostrar mensaje no intrusivo.
6. Conservar contexto y restaurar acciones ante cancelación, bloqueo o error técnico.

## CONTRATO DETALLADO

- Preview: `POST ../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarUsuario` con `{ idTarea }`.
- Ejecución: `POST ../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioUsuario` con `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }`.
- Evento de selección: solo `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion, destino }`; nunca Session, permiso, SQL ni `IdConector`.
- Compatibilidad: el adaptador actual de Continuar flujo conserva `{ idTarea, idConector, tokenVersion }`, endpoints y selectores sin cambios.

## CRITERIOS DE ACEPTACIÓN

- Enviar a usuario y Continuar flujo coexisten sin listeners, identificadores ni requests cruzados.
- La UI de usuario no publica ni requiere `IdConector` ni reasignación de respuesta.
- Gate inactivo conserva recorrido Web Forms exacto.
- El modal cumple foco inicial, trampa de foco, teclado, Escape, ARIA y representación móvil/escritorio.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas JavaScript de normalización, contratos ASMX, aislamiento de eventos, éxito, respuesta pendiente, bloqueo, error, doble clic, respuesta obsoleta, cancelación, teclado y fallback inactivo. Ejecutar MSBuild y pruebas CJS focales; no E2E autenticado sin autorización.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/06-asmx-ui/` con selectores, endpoints, DTOs, estados UI, correlación, accesibilidad, fallback, pruebas y diagramas de interacción.

## ENTREGABLE FINAL

Entregar archivos UI/adaptadores modificados, pruebas, compilación, evidencia de fallback, documentación y declaración de no regresión para Continuar flujo.

