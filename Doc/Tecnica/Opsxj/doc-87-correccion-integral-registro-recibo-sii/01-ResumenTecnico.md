# CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII

- Ticket: DOC-87
- Cambio OpenSpec: doc-87-correccion-integral-registro-recibo-sii
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Corregir el registro de recibos SII en **Registrar tarea para ruta** y completar el mismo atributo HTML obligatorio en los formularios de flujo, RUE y virtuales, sin cambiar su lógica. La solución introduce un estado cliente exclusivo para ruta, comando mínimo autorizado, persistencia Workflow parametrizada y outbox hacia Docuarchi; importación SII, Radicación Simplificada y adjuntos permanecen intactos.

## Alcance y compatibilidad

- Afectados: `WebFormGestionFlujoTrabajoCamaras.aspx`, su JavaScript, el método ASMX de ruta y clases nuevas DOC-87.
- Preservados: validador general, `zeroFillFReciboSII`, `FileUploadHandler_.ashx`, `ClassGestionTareasFlujoTrabajo`, relación legacy y restantes endpoints.
- Reversa: retirar primero la versión aplicativa; eliminar la tabla outbox solo si está vacía mediante el script oficial.
