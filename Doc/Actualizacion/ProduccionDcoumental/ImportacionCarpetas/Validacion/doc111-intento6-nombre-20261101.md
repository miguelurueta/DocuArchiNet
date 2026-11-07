# DOC-111: intento 6 almacenado, pendiente de verificación por nombre de gabinete

Actualización posterior: se ajustó la verificación a la configuración visible, conservando el nombre original obligatorio en inventario/índice. El documento real ya pasa la comprobación SELECT del nuevo verificador; la reconciliación y continuación solo del segundo archivo están preparadas. [Pruebas, alcance y SHA actuales](doc111-nombre-visible-reconciliacion-20261101.md). Aún no es reconciliación persistida ni aceptación REC.

La sesión 12 terminó el 2026-11-01 a las 10:38:12 UTC con `AvanzarLote`, HTTP 409, `RESULTADO_INCIERTO`. El cierre de la reserva 5 sí quedó confirmado: evento 15, intento RECHAZADO con `RESERVA_CERRADA_SIN_INICIO`. El nuevo intento 6 registra reserva, inicio y resultado incierto, eventos 16–18. No es admisible aplicar el cierre previo al inicio a este intento ni repetir almacenamiento.

SELECT de las 10:59:36 UTC por IDs persistidos:

| Recurso | Resultado |
|---|---|
| Lote 2 | PENDIENTE_VERIFICACION, revisión 18 |
| Expediente 648 / relación 1073 | Identidad conservada; raíz INCIERTO |
| Archivo 3 / intento 6 | INCIERTO, archivo revisión 8, reclamación conservada |
| Documento / inventario | 16204 / 23293, una fila física y una fila de inventario |
| Incorporación | Una fila, enlazada a inventario 23293 y expediente 648 |
| Expediente 649 / archivo 4 | Conservados; archivo NO_INICIADO; cero documentos/inventario/incorporación |

En el primer documento coinciden IDs de documento/inventario/expediente, gabinete, usuarios/empresa, tipología 79, clase y formato PDF. El nombre original coincide exactamente en inventario e índice electrónico. La única discrepancia de las comprobaciones de fila realizadas es `gabinete.NOMBRE`: está vacío. No es una diferencia de mayúsculas; el catálogo tiene cero campos NOMBRE con VISIBLE=1.

La causa está caracterizada en el código existente: `Docuarchi/Class_DETALLE_GABIENETE.vb`, función `SolicitaValoresCamposDocumentoGabinete`, selecciona exclusivamente campos VISIBLE=1. Aunque `AlmacenamientoDocumentoProduccionDocumental` precarga NOMBRE, ese valor no entra en la matriz final si el campo no es visible. Los nombres de inventario e índice se asignan por separado desde el nombre original de la copia, y quedaron correctos. El verificador DOC-111 exige actualmente coincidencia de las tres columnas sin considerar esta configuración; por eso conserva INCIERTO.

Evidencia saneada: `tools/e2e/artifacts/doc111-lot2-after-attempt6-readonly.json` y `tools/e2e/artifacts/doc111-attempt6-document-readonly.json`. Solo SELECT, sin cambiar datos, reconciliar, limpiar staging o invocar almacenamiento. La DLL vigente sigue siendo `892853f896bf8784c751b73da7ed6c5344d78df8c00c8e84c5bb49dc8a530351`; no hubo otro despliegue en este diagnóstico.

Pendiente: resolver la discrepancia entre el contrato de verificación de DOC-111 y la selección real de campos visibles del flujo reutilizado. El nombre original debe seguir comprobándose en inventario e índice, y en NOMBRE cuando ese campo forme parte de la configuración efectiva. Cualquier ajuste debe probar el caso visible obligatorio, el no visible, nombres alterados y metadatos ambiguos; no basta ignorar la diferencia para declarar PASS. Después corresponde verificar/reconciliar el mismo intento 6 por sus IDs conocidos, sin volver a almacenar el primer PDF, y continuar exclusivamente el segundo archivo. No se modificó la configuración compartida del gabinete ni la función legacy. **121/130; REC LOTE pendiente.**
