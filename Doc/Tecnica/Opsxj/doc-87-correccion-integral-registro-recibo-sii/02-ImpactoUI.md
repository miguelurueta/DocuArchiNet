# CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII

- Ticket: DOC-87
- Cambio OpenSpec: doc-87-correccion-integral-registro-recibo-sii
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- Solo el bloque `conten_registro_ruta` recibe el contrato faltante en sus seis controles.
- El recibo se canoniza como S/R + nueve dígitos; editar número o prefijo invalida y limpia datos derivados.
- El trámite debe coincidir una sola vez usando el subtipo del radicado o, si está vacío, el tipo del recibo; la actividad debe ser positiva.
- Aceptar bloquea doble envío, no navega, no recarga y muestra éxito o relación pendiente.
- Los demás formularios de la página conservan eventos, serialización y estilos.

## Validacion visual

Recorrido: consultar un recibo desechable autorizado, seleccionar actividad, aceptar y comprobar aviso sin navegación. La E2E automatizada DOC-87 reproduce este recorrido cuando se entregan prerrequisitos.
