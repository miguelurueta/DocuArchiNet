# Arquitectura y flujo técnico detallado

## Alcance revisado

Se revisaron `workflow/Webworkflow.aspx(.vb)`, `workflow/ClassWorkflowDigitalizacion.vb`, `js/workflow/importar-servicio-web/`, los contratos ASMX de DOC-80/DOC-81 y la plataforma `tools/e2e`. DOC-82 agrega presentación e integración; no agrega controller, service ni repository backend.

## Consulta y selección

```text
┌──────────────────────────────────────────────────────────────────────┐
│ CLIENTE: Webworkflow.aspx + importar-servicio-web-ui.js             │
│ open(control,event)                                                  │
│ - trigger: #a_adj_service_web                                       │
│ - captura TaskId, Radicado, ProviderId y Capability                 │
│ - Capability debe ser ANEXOS_RADICADO_ENLASE                        │
│ - el guard inmoviliza el contexto original antes de escribir        │
└───────────────────────────────┬──────────────────────────────────────┘
                                │ POST JSON autenticado
                                ▼
┌──────────────────────────────────────────────────────────────────────┐
│ CONTROLLER: WebServiceImportarServicioWebModern.asmx                │
│ ResolveCapabilities(request), QueryItems(request), GetPreview(req.)  │
│ PreflightImport(req.), CreateImportIntent(req.),                    │
│ ExecuteImportIntent(req.), GetImportIntent(req.),                   │
│ ReconcileImportIntent(req.)                                         │
│ - valida sesión, tarea, proveedor, capacidad y DTO                   │
│ - errores funcionales via ErrorImportacionServicioDto               │
└───────────────────────────────┬──────────────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────────────┐
│ SERVICE: servicios DOC-80/DOC-81                                    │
│ - consulta/preview seguro del SII                                   │
│ - preflight y catálogo autoritativos                                │
│ - una intención para Items[1..N] y una ejecución                    │
│ - reconciliación confirma persistencia por elemento                 │
└───────────────────────────────┬──────────────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────────────┐
│ REPOSITORY/ADAPTER (reutilizado, no accedido por la UI)             │
│ - repositorios de intención/reconciliación DOC-81                   │
│ - LegacyEnlaseImportDocumentStorageAdapter.Almacenar                │
│ - proveedor SII de DOC-80                                           │
│ - nunca se confía en una URL externa recibida por la tabla          │
└───────────────────────────────┬──────────────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────────────┐
│ RESPUESTA Y PROYECCIÓN                                               │
│ enlase-adapter.create(options) fuerza la capacidad                   │
│ enlase-list.render(container,data) representa 0/1/N                 │
│ - no importables: selección/preparación deshabilitadas              │
│ - preview: descriptor temporal mediado                              │
│ - éxito total confirmado: refresca lista y cierra                   │
│ - parcial/incierto/error: conserva el modal y detalle saneado       │
└──────────────────────────────────────────────────────────────────────┘
```

## Decisiones

1. `activeAdapter(control)` elige el adaptador ENLASE únicamente cuando la capacidad activa coincide; el disparador de constancias conserva su adaptador.
2. `openPreparation(control, keys, trigger)` trabaja con una colección. El catálogo backend puede predeterminar tipología solo si la resolución es inequívoca; una selección inválida no crea intención.
3. `executeCreatedIntent(control, intent)` mantiene el cierre bloqueado y el guard de tarea durante ejecución y reconciliación.
4. `settleAfterResult(control, execution, authoritative)` solo llama al cierre posterior cuando `shouldCloseAfterResult` recibe éxito total y todos los documentos tienen confirmación autoritativa. Los demás estados permanecen visibles.
5. `refreshDocumentListPartial(control)` reutiliza el refresco Web Forms. La proyección filtra por tarea original, `DocumentId > 0`, confirmación y deduplicación.

## Flujo de asignación posterior

```text
Usuario pulsa Asignar
  -> Webworkflow.Buttonaceptar_Click(sender As Object, e As EventArgs)
  -> valida Session("SELECCIONTEMPORAL")
  -> [si hay trámite/gabinete/radicado]
       ClassWorkflowDigitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado(
         Radicado As String, nombre_gabinete As String, id_tramite As Integer) As String
  -> ¿resultado = "YES"?
       NO: muestra mensaje y Exit Sub; no asigna
       SÍ: continúa el flujo legacy de permisos/estado/asignación
```

No hay llamada DOC-82 a `ValidateAssignment`, porque ese endpoint no existe. Tampoco hay habilitación preventiva garantizada: el botón sigue explícito y la autoridad final está en el postback.

## Errores HTTP

Los métodos ASMX usan POST y normalmente transportan errores de dominio en el DTO aun con HTTP 200. Los fallos de transporte/autenticación siguen la infraestructura ASP.NET. El recurso binario de preview conserva los códigos documentados por DOC-80 (200/404/405/503). DOC-82 no cambia esos contratos.
