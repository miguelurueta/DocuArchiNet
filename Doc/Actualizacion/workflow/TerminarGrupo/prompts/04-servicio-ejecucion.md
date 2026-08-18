# 04 — Ejecución segura de envío a grupo

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de casos de uso Workflow, concurrencia MySQL y encapsulación de motores legacy.

## OBJETIVO

Implementar `ServicioEnvioGrupoTarea` y `EjecutarEnvioGrupo` en el ASMX moderno existente, manteniendo la operación como reenvío directo y reutilizando los controles de seguridad transversales ya implementados.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- No modificar la lógica ni validación de `ServicioTransicionTarea` o `EjecutarEnvioTarea`.
- No usar `IdConector` ni crear un conector artificial; el request usa `IdActividadDestino`.
- Application no usa `Page`, `Session`, `GridView`, `UpdatePanel` ni `ModalPopupExtender`.
- No duplicar `Terminar_Tarea_Workflow`, `Cambia_Estado`, firma, expediente, balanceo, correo ni eventos dinámicos.
- No introducir la regla de respuesta radicada sin aprobación funcional explícita.
- Reutilizar el mismo gate y ASMX; no crear ni cambiar configuración de habilitación.

## REQUISITOS POSITIVOS

1. Validar contexto, gate, `Cambio_Ruta`, `IdTarea`, `IdActividadDestino` y token.
2. Adquirir el `GET_LOCK` existente por tarea y versión.
3. Dentro del lock, releer tarea, comparar token y revalidar permiso, apertura de ruta/flujo/actividad y pertenencia del destino a la ruta.
4. Evaluar aprobación pendiente y requisitos exclusivos del envío a grupo; recuperar notificación desde la actividad destino.
5. Delegar al adaptador legacy directo de la etapa 05 y mapear éxito, bloqueo, error reintentable y advertencias a DTO público.
6. Registrar auditoría sanitizada con mecanismo `ASMX_ENVIO_GRUPO`, sin SQL, Session, token, documentos ni credenciales.
7. Exponer `EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)` sin alterar endpoints existentes.

## CRITERIOS DE ACEPTACION

- Dos solicitudes simultáneas no producen dos transiciones.
- Token vencido, permiso cambiado o destino retirado bloquean antes del motor legacy.
- Una falla de auditoría genera advertencia segura y no revierte una transición ya confirmada.
- `EjecutarEnvioTarea` y la transición por conector continúan sin cambios.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de solicitud inválida, gate/permiso bloqueados, token vencido, destino fuera de ruta, aprobación pendiente, concurrencia, error reintentable, advertencia de auditoría y éxito. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`, registrar código de salida y los comandos de prueba focales; si falla por dependencia de ambiente, documentar causa y QA manual reproducible. No ejecutar E2E mutante sin autorización y tarea descartable.

## DOCUMENTACION TECNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarGrupo/04-servicio-ejecucion/` con componentes, secuencia bajo lock, contrato de ejecución, códigos, auditoría, riesgos y evidencia de pruebas.

## ENTREGABLE FINAL

Entregar servicio, composición ASMX, contratos usados, pruebas y compilación, documentación y listado explícito de reglas legacy preservadas. No implementar la UI hasta que la ejecución pase sus pruebas.
