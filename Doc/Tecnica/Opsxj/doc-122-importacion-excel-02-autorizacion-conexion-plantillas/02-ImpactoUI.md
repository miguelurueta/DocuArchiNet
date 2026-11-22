# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Superficies UI
No se añade pantalla ni menú en fase02. El indicador GA_ADMINISTRAR_CARGAS_IMPORTACION se reinicia a0 y solo se establece con permiso exacto1. El servidor vuelve a comprobar RA en cada operación, independientemente de la presentación. No se modifica WebFormProducionDocumental ni el editor de perfiles.

## Validacion visual
No aplica a esta fase de servidor; no se afirma QA de una interfaz todavía inexistente. Contrato ASMX probado por reflexión SOAP y serialización tipada local. La interfaz futura debe consumir límites efectivos, errores controlados y revisión/huella. E2E de sesión/API preparado y pendiente de configuración y autorización específicas.
