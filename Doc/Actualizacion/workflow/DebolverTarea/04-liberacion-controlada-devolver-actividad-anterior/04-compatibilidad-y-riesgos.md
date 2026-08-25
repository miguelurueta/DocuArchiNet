# Compatibilidad y riesgos residuales

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificación: cross_cutting

## Invariantes de operación

Devolver a actividad anterior conserva la ruta moderna oficial para todo contexto Workflow válido. PreviewDevolverActividad permanece en solo lectura y EjecutarDevolverActividad conserva permiso, relectura dentro del lock por tarea, token, conector entrante y auditoría saneada.

Ruta conserva la validación de actividad origen, actividad siguiente, Ruta y configuración de conector; Flujo conserva su identificador y contexto propios. Ningún navegador autoriza actividad, usuario, grupo, Ruta, Flujo ni destino.

La capacidad no consulta, valida, bloquea, crea, actualiza, reasigna ni audita respuestas. Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior preservan sus contratos y recorridos propios.

## Riesgos residuales

- No existe autorización de ambiente, responsables nominales ni ventana; la operación queda fuera de alcance hasta recibirlos.
- La advertencia heredada MSB3247 no impidió la compilación DOC-34, pero sigue siendo una señal de mantenimiento general.
- La QA visual no autenticada solo cubre la carcasa sin tarea seleccionada; los comportamientos dinámicos se sustentan en pruebas CJS y evidencia E2E previa saneada.
- Una reversión de paquete no cambia datos ni transiciones confirmadas; incidentes sobre esos datos siguen el proceso funcional y de soporte vigente.

## Decisión de compatibilidad

La liberación futura debe abortar ante una discrepancia de versión o contrato, un intento de cambio de configuración o una reactivación de UI legacy. DOC-35 no introduce una modificación funcional; deja el paquete listo para solicitar autorización operativa por ambiente.
