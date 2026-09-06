# Contrato HTTP, serialización y límites

## Solicitud preparada

| Campo | Regla |
|---|---|
| URI | Absoluta; nunca se incluye completa en errores públicos |
| Verbo | Se conserva sin inferencia |
| Cuerpo | Copia inmutable de bytes ya serializados |
| Content-Type | Se conserva; no se convierte JSON/formulario |
| Encabezados | Copia por solicitud; no cambia defaults compartidos |
| Timeout | Positivo y obligatorio por operación/ambiente |
| Máximo | Bytes positivos y obligatorios por operación |
| MIME aceptados | Lista explícita no vacía |
| Correlación | Identificador obligatorio y saneado |

Los valores productivos de timeout, tamaño y MIME pertenecen a configuración del adaptador/ambiente. DOC-51 implementa y prueba el mecanismo sin inventar límites SII.

La descarga entrega bytes validados. Preview usa después los DTO v1 de DOC-50 y nunca expone token, URL externa, ruta física o respuesta cruda.
