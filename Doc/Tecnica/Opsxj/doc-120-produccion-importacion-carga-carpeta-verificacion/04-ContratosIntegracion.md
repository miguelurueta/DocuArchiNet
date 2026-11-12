# DOC-120 — ContratosIntegracion

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Contratos e integraciones

Sin DTO, endpoint, ASMX/ASHX, esquema o coordinador nuevos. Los índices relacionan requisito/caso/componente/prueba/código/ambiente/evidencia/resultado/vigencia/límite. Los primarios originales permanecen intactos, incluidos STOPPED, NO_ACREDITADO y estados de laboratorio.

El módulo interno E2E de degradación extrae y exporta checkUnavailableUI para regresión aislada; el runner usa la misma función y ahora abre el menú moderno antes del click. No es un endpoint de aplicación ni habilita fallos/producto. Controla únicamente503 y aviso existente; no amplía perfil, presupuesto o efectos autorizados.

INT-03 tiene contrato con campos/decisiones explícitos en resultados.md; faltan objetivo/umbrales/corpus/ambiente/recursos/presupuesto y autorización propia. La solicitud al responsable no equivale a aceptación ni ejecución.
