# Inventario legacy

## Referencias conservadas

| Superficie | Referencias observadas | Clasificación DOC-79 |
| --- | --- | --- |
| `btnloadservice` | `Webworkflow.aspx`, `Webworkflow.js`, scripts visuales | No removible; entrada fallback |
| `Panel_list_inscripciones_sii` | markup, modal extender y code-behind | No removible |
| `GridView_list_inscripciones_sii` | carga, paginación y eventos `RowCreated`/`PageIndexChanging` | No removible |
| `ModalPopupExtender_edition_list_inscripciones_sii` | presentación legacy desde code-behind | No removible |
| `Panel_sube_documento_integra_sii` | modal, tipología y guardado | No removible |
| `Button_Activa_guardar_Multiplex_Constancias_sii_` | dispatcher `Webworkflow.js` | No removible |
| `Button_acepta_sube_documento_integra_sii` | postback y handler VB | No removible |
| `WebService_integracion_sii.asmx` y `WebServiceGaExpediente.asmx` | recorrido legacy | No removible |
| `JSProgresBar.js` | progreso legacy sin dependencias modernas | Invariante |
| `ClassAlmacenamiento.vb` | almacenamiento legacy usado por adaptador único | Invariante |

## Criterios para un retiro futuro

El retiro requiere otro ticket autorizado, cero referencias ejecutables, evidencia con gate estable, rollback aprobado y pruebas que demuestren ausencia de postbacks/handlers consumidores. DOC-79 no elimina ninguna superficie.
