# DOC-89 - Servicios y reglas preservados

- Ticket: DOC-89
- Cambio OpenSpec: doc-89-correcion-guardar-documento-escaner-enlace
- Clasificacion: cross_cutting

## Servicios y reglas

Los servicios y reglas de almacenamiento no cambian. La modificación se detiene en la presentación cliente de la página padre.

## Recorrido servidor sin cambios

```text
ButtonAlmacenar_Click
  -> ClassAlmacenamiento.UploadSaveFileScan(Session("DG_TIPODIGITALIZACION"), datos)
     -> resultado distinto de YES: mensaje existente y Exit Sub
     -> YES: Hidden_result_load_ = "YES" y Hidden_date_row_ = datos del documento
  -> CheckStatus/endRequest
  -> insert_row_documento_relacionado(...), exactamente una vez
```

La corrección no agrega consultas, reintentos, temporizadores, `DataBind`, recarga ni segunda escritura. El tratamiento de error de `ButtonAlmacenar_Click` permanece intacto.

## Deuda técnica excluida

- `OnHttpUploadSuccess()` mantiene comentada su continuación.
- `OnHttpUploadFailure()` mantiene la continuación específica para `-2003`.
- El typo legacy `heigth` en `posicion_update_pogres_modal()` no se corrige.

Estos contratos pertenecen al componente compartido y requieren caracterización/aprobación separada si se modifican.
