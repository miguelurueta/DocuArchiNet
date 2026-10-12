# DOBLE-FACTOR-DESPLIEGUE

- Ticket: DOC-98
- Cambio OpenSpec: doc-98-doble-factor-despliegue
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

- Endpoint/UI: sin contratos nuevos; se preserva `WebServiceLoginSegundoFactor.asmx` y el flujo Web Forms existente.
- Catalogo central: `empresa_gestion_documental` + `gestor_modulos`, resuelto por razon social, nombre de modulo y `ESTADO_MODULO=1`.
- Activacion valida: `RequiereSegundoFactor=1`, `SecondFactorProviderType=1` (`EMAIL`) y `SegundoFactorTiempoExpira` entre 1 y 10.
- Principales existentes: `usuarios_da`, `remit_dest_interno`, `usuario_radicador` y `usuario_workflow`. No contienen configuracion 2FA por usuario.
- SMTP existente: `Config_Smpt_Side`, unica fila `ESTADO_ENVIO=1`; no se reemplaza el servicio legacy ni su configuracion.
- Persistencia existente: `ra_auth_second_factor_challenge`; DOC-98 no crea ni altera tablas, columnas o indices.
- Scripts mutables: requieren IDs, valores esperados, snapshot, cardinalidad exacta, transaccion y autorizacion cerrada por defecto.
