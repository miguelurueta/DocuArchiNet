# 02 — Contratos y autorización de envío a usuario

## ROL ESPERADO

Actúa como desarrollador senior VB.NET responsable de límites Domain, Application e Infrastructure de Workflow.

## OBJETIVO

Crear contratos específicos de envío directo a usuario y exponer de forma segura la autorización efectiva `CAMBIO_USUARIO`, reutilizando contexto y gate modernos.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- Limita cambios a `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar`, `webservice/WorkflowPreviewSessionContextGate.vb` y pruebas relacionadas.
- No modificar `SolicitudTransicionWorkflow`, `DestinoTransicionDto`, `ValidadorTransicionTarea`, `ServicioTransicionTarea` ni contratos de Continuar flujo.
- No crear configuración, gate ni bandera adicional.
- Application no lee Session; repositorios no reciben controles Web Forms ni devuelven HTML, `DataSet`, SQL o excepciones.
- No conectar aún destinos, ejecución, ASMX de usuario ni interfaz.

## REQUISITOS POSITIVOS

1. Definir DTOs serializables de preview y solicitud con `IdTarea`, `IdUsuarioWorkflowDestino`, `IdActividadDestino` y `TokenVersion`.
2. Definir destino directo `ENVIO_USUARIO_DIRECTO` con usuario, actividad, grupo, notificación y datos mínimos de auditoría.
3. Transportar de forma compatible la autorización efectiva `CAMBIO_USUARIO`, calculada por servidor durante `AsegurarContextoEjecucion` o mediante un puerto de autorización específico.
4. Definir un resultado tipado de requisito de respuesta: permitido, pendiente bloqueante o no disponible; Application no compara textos legacy.
5. Definir códigos públicos estables para contexto, permiso, destino, respuesta y versión inválidos.

## CRITERIOS DE ACEPTACIÓN

- Los nuevos contratos no contienen `IdConector` ni referencias a reasignación.
- El permiso denegado y la respuesta pendiente bloquean fail-closed.
- Los contratos y firmas de Continuar flujo permanecen intactos.
- Las dependencias respetan Presentation → Application → Domain → Infrastructure.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas focales de serialización, solicitud inválida, permiso permitido/denegado, respuesta permitida/bloqueada y sesión incompleta. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`; registrar código y errores o una limitación reproducible.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/02-contratos-autorizacion/` con índice, arquitectura, contrato, seguridad, evidencia y justificación de no E2E.

## ENTREGABLE FINAL

Entregar contratos modificados, mapa de dependencias, pruebas, compilación, documentación y riesgos. No avanzar al preview sin compatibilidad demostrada con Continuar flujo.

