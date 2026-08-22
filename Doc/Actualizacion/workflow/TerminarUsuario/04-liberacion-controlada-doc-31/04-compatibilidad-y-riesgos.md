# Compatibilidad y riesgos residuales

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificación: cross_cutting

## Invariantes de operación

Enviar a usuario se conserva como la ruta moderna oficial para contexto Workflow válido. Su contrato directo usa tarea, usuario destino, actividad destino y token; no acepta ni necesita `IdConector`. El preview permanece de solo lectura y la ejecución conserva autorización, revalidación bajo lock, política de respuesta y auditoría sanitizada.

Continuar flujo conserva endpoints, validaciones, pruebas y payload con `IdConector`. Grupo conserva su operación propia. Una liberación no habilita un postback, modal o fallback Web Forms de Enviar a usuario ni mezcla listeners o estado de los tres comandos.

## Riesgos residuales

- No hay autorización de ambiente, responsables nominales ni ventana; la operación permanece fuera de alcance hasta recibirlos.
- La evidencia de calidad no reemplaza controles de despliegue ni soporte del ambiente.
- Una reversión de paquete no puede cambiar tareas, respuestas, auditoría o transiciones ya confirmadas; cualquier incidente sobre esos datos sigue el proceso funcional y de soporte vigente.
- Los secretos permanecen fuera de tickets, documentación, evidencias y logs de esta actividad.

## Decisión de compatibilidad

La liberación futura debe abortar si detecta una discrepancia de contrato, una solicitud que incluya cambios de configuración o un intento de reactivar una ruta legacy. No se identificó una modificación funcional atribuible a DOC-31.
