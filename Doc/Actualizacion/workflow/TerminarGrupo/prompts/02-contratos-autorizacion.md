# 02 — Contratos y autorización de envío a grupo

## ROL ESPERADO

Actúa como desarrollador senior VB.NET responsable de los límites Domain, Application e Infrastructure de Workflow.

## OBJETIVO

Crear contratos específicos de envío directo a grupo y exponer de forma segura la autorización efectiva `Cambio_Ruta`, reutilizando el contexto y gate modernos existentes.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- Limitar cambios a `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar`, `webservice/WorkflowPreviewSessionContextGate.vb` y pruebas relacionadas.
- No modificar `SolicitudTransicionWorkflow`, `DestinoTransicionDto`, `ValidadorTransicionTarea`, `ServicioTransicionTarea`, ni contratos de continuar flujo.
- No crear una bandera, gate o configuración adicional; reutilizar `IWorkflowModernFeatureGate` y la evaluación existente.
- Application no lee Session; repositorios no reciben controles Web Forms ni devuelven HTML, `DataSet`, SQL o excepciones.
- No conectar todavía destinos, ejecución, ASMX de grupo ni interfaz.

## REQUISITOS POSITIVOS

1. Definir DTOs serializables de preview y solicitud con `IdTarea`, `IdActividadDestino` y `TokenVersion`.
2. Definir modelo de destino directo `ENVIO_GRUPO_DIRECTO`, con actividad, grupo, notificación y datos mínimos de auditoría.
3. Extender de forma compatible el contexto o resultado de autorización para transportar el permiso efectivo `Cambio_Ruta`, obtenido por servidor durante `AsegurarContextoEjecucion`.
4. Definir códigos públicos estables para sesión/contexto/permiso inválidos sin filtrar detalles internos.

## CRITERIOS DE ACEPTACION

- Los nuevos contratos no requieren ni contienen `IdConector` para el envío a grupo.
- El permiso denegado bloquea de forma fail-closed.
- Los contratos y firmas de continuar flujo permanecen sin cambios.
- Las dependencias respetan Presentation → Application → Domain → Infrastructure.

## REGLAS DE ANTIRREGRESION

- Mantener intactos los contratos y el comportamiento de continuar flujo durante esta etapa.
- No modificar los tipos, miembros públicos, serialización o validación de los DTO actuales de continuar flujo.
- Las nuevas propiedades de contexto deben tener valor seguro por defecto y no cambiar la evaluación existente cuando el envío a grupo no se invoque.
- No cambiar firmas, rutas, request, respuesta, códigos o gate de `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea` ni `WorkflowLegacyExecutorAdapter`.
- No modificar el postback Web Forms `ImageButtonEnviaActividad` durante esta etapa de contratos.
- Ejecutar las pruebas actuales de DTOs, gate y transición junto con las pruebas nuevas; detener la etapa ante diferencias de contrato.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas focales de serialización, validación de solicitud, permiso permitido/denegado y sesión incompleta. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`, registrar código de salida y errores; si el comando no está disponible, documentar la limitación y QA manual reproducible.

## DOCUMENTACION TECNICA

Actualizar exclusivamente `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/`: registrar contratos y DTOs en `02-contrato.md`, autorización y controles en `03-flujo-y-seguridad.md`, y evidencia en `04-pruebas-y-evidencia.md`. No crear una carpeta documental para esta etapa.

## JUSTIFICACION E2E

E2E autenticado no aplica en esta etapa porque no se crea endpoint, interfaz ni mutación ejecutable. Registrar esta justificación en la evidencia y, antes de cualquier E2E futuro, leer `tools/e2e/AGENT-RUNBOOK.md` y obtener autorización explícita de ambiente y cuentas de prueba.

## ENTREGABLE FINAL

Entregar modelos/DTOs/contratos modificados, mapa de dependencias, pruebas y compilación ejecutadas, documentación actualizada y riesgos pendientes. No continuar al preview si el contrato no es compatible con continuar flujo.
