# Servicios y reglas

- Ticket: DOC-55
- Cambio OpenSpec: doc-55-adaptador-sii-asmx
- Clasificación: cross_cutting

## Servicios y reglas

| Superficie | Responsabilidad vigente | Regla DOC-55 |
| --- | --- | --- |
| `webservice/WebService_integracion_sii.asmx*` | Entrada histórica, `YES` y `dato_lista`. | Solo lectura. |
| `webservice/WebServiceGaExpediente.asmx*` | Expediente y respuestas delimitadas. | Preservar firmas. |
| `Integracionccv/Class_consultarInformacionSello.vb` | Consulta registral SII. | Referencia para fixtures. |
| `Integracionccv/Class_ClassResfull.vb` | Transporte REST histórico. | El adaptador nuevo usa B02. |
| `workflow/ClassAlmacenamiento.vb` | `AlmacenaDocumentoTareaWorkflow`. | Invocar solo mediante el puerto moderno. |
| `js/java_general/JSProgresBar.js` | Consumidor browser histórico. | No agregar interpretación moderna. |

## Matriz de compatibilidad

| Resultado | Código | `dato_lista` |
| --- | --- | --- |
| Completado | `YES` | Datos mínimos para el consumidor existente. |
| Requiere decisión | `CTRL` | Vacío. |
| Detenido | `CTRLRETURN` | Vacío. |

`SiiLegacyResultAdapter` será el único componente nuevo autorizado para conocer esta matriz.
