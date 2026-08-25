# Evidencia y precondiciones

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificación: cross_cutting

## Base técnica

DOC-34 aprobó la verificación transversal de la capacidad: compilación MSBuild sin errores, 83 pruebas CJS focales correctas, 35 pruebas de reconfirmación de políticas, inspección estática y QA no autenticada. También registra antecedentes saneados de E2E autorizada de DOC-32 y DOC-33 sin repetirla.

La referencia de versión es main en el merge del PR #29. La evidencia identifica el preview SELECT, aristas entrantes de Ruta y Flujo, token y lock por tarea, auditoría saneada, búsqueda paginada, ruta moderna oficial e inexistencia de tratamiento de respuestas.

## Precondiciones operativas

Antes de cualquier operación, una solicitud debe contener ambiente exacto, versión o paquete verificable, alcance Devolver a actividad anterior, ventana, criterio de aborto, aprobador de liberación, operador de despliegue y dueño funcional de Workflow. Debe adjuntar referencias saneadas a DOC-34, PR #29 y los controles autorizados.

No se incluyen credenciales, cookies, cadenas de conexión, datos personales, cuerpos de respuesta ni identificadores de tareas. La evidencia técnica o una aprobación de pruebas no autorizan por sí solas un ambiente.
