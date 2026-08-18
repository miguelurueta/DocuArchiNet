# 06 — Integración visual de Enviar a grupo

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript legacy accesible, con experiencia en migraciones Strangler Fig.

## OBJETIVO

Conectar el botón `Enviar a grupo` a los endpoints ya implementados en `WebServiceWorkflowModern.asmx`, reutilizando los componentes genéricos y preservando íntegramente el postback legacy como fallback.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- No crear otro ASMX, framework, bundler, módulo ES, `ConfirmationDialog` ni segunda evaluación de gate.
- No cambiar los endpoints, trigger, payload `IdConector` ni flujo visual de continuar flujo.
- JavaScript no toma decisiones de permiso, ruta, flujo, requisitos o destino; solo representa el JSON del servidor.
- No llamar controles ocultos, handlers Web Forms, `Terminar_Tarea_Workflow`, `Cambia_Estado` ni repositorios desde JavaScript.
- Con gate inactivo, el botón debe conservar el postback legacy existente; no modificar configuración de gate.

## REQUISITOS POSITIVOS

1. Reutilizar `ConfirmationDialog`, CSS de confirmación, estilos de modal, accesibilidad, foco, Escape, prevención de doble clic y presentación de éxito ya existentes.
2. Crear un adaptador o instancia Workflow específica para grupo, con trigger, modal, evento, preview y ejecución propios.
3. Convertir el destino seleccionado al contexto `{ idTarea, idActividadDestino, tokenVersion }` y consumir solo `PreviewEnviarGrupo` y `EjecutarEnvioGrupo`.
4. Tras éxito correlacionado, retirar solo la tarea afectada, limpiar contexto/visor, actualizar contador y mostrar mensaje no intrusivo.
5. Mantener contexto y restaurar acciones ante bloqueo funcional o fallo técnico.

## CONTRATO DETALLADO

- Preview: `POST ../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarGrupo` con `{ idTarea }`; respuesta ASMX con tarea, token, contexto sanitizado, destinos de actividad y error público.
- Ejecución: `POST ../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioGrupo` con `{ idTarea, idActividadDestino, tokenVersion }`; respuesta con éxito, estado final, código/mensaje, advertencias y referencia de auditoría.
- Evento de selección: debe transportar solo `{ idTarea, idActividadDestino, tokenVersion, destino }`; nunca Session, permiso, SQL o `IdConector`.
- Compatibilidad: el adaptador actual conserva `{ idTarea, idConector, tokenVersion }` y sus endpoints sin cambios.

## CRITERIOS DE ACEPTACION

- Los dos comandos modernos pueden coexistir sin compartir identificadores, listeners o requests incorrectos.
- La UI de grupo no publica ni requiere `IdConector`.
- Gate inactivo conserva el recorrido Web Forms exacto.
- El modal cumple foco inicial, trampa de foco, teclado, Escape, ARIA y representación móvil/escritorio.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas JavaScript de normalización, contratos ASMX, aislamiento de eventos, éxito, bloqueo, error, doble clic, respuesta obsoleta, cancelación, teclado y fallback inactivo. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` y el comando de pruebas CJS focales configurado por el repositorio; registrar código de salida, archivos y resultado. Si no están disponibles, registrar limitación/QA responsive reproducible. No E2E autenticado sin autorización.

## DOCUMENTACION TECNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarGrupo/06-asmx-ui/` con selectores, endpoints, DTOs, estados UI, correlación, accesibilidad, fallback, pruebas y diagramas de interacción.

## ENTREGABLE FINAL

Entregar archivos UI/adaptadores modificados, pruebas y compilación, evidencia de fallback, documentación y declaración de no regresión para continuar flujo.
