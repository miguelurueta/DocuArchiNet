# Casos de uso implementados

| Caso | Actor | Precondiciones | Flujo principal | Alternativas/errores | Resultado | Trazabilidad |
|---|---|---|---|---|---|---|
| Consultar importables | Usuario Workflow | Sesión, tarea activa, gate y proveedor SII | `QueryItems` consulta y normaliza candidatos | `FEATURE_DISABLED`, `INVALID_REQUEST`, fallo seguro del proveedor | `QueryItemsResponseDto` | ASMX `QueryItems` → `SiiImportProvider.QueryItemsAsync` |
| Previsualizar documento | Usuario Workflow | Identidad externa autorizada | `GetPreview` valida allowlist, tamaño y disposición | ausente/no autorizado devuelve rechazo opaco | `GetPreviewResponseDto` | ASMX `GetPreview` → proveedor SII |
| Validar importación | Usuario Workflow | Tipología válida y selección no vacía | `PreflightImport` resuelve contexto y configuración | tarea/ruta/tipología/configuración inválida | `PreflightImportResponseDto` | `ServicioPreflightImportacion` |
| Crear intención | Usuario Workflow | Preflight válido y radicado | reconstruye inscripciones SII y persiste idempotentemente | clave equivalente reutiliza; conflicto bloquea | `CreateImportIntentResponseDto` | `ServicioIntencionImportacion.Crear` → `MySqlImportIntentRepository.CrearOReutilizar` |
| Ejecutar saga | Usuario Workflow | intención/version autorizadas | expediente → almacenamiento → relacionados → reconciliación | detención, conflicto de versión, efecto parcial o incierto | `ExecuteImportIntentResponseDto` | `ImportServiceOrchestrator.Execute` |
| Consultar/recuperar | Usuario Workflow | intención perteneciente a tarea/contexto | carga snapshot sin mutar | identidad/contexto discrepante | `GetImportIntentResponseDto` | repositorio de intención + mapper |
| Reconciliar | Usuario Workflow | intención existente | relee efectos físicos y proyecta estados | divergencia conserva parcial/incierto | `ReconcileImportIntentResponseDto` | `ServicioReconciliacionImportacion` |

Los ASMX usan `POST /webservice/WebServiceImportarServicioWebModern.asmx/{Método}` y normalmente devuelven errores funcionales en `Error.Codigo` con HTTP 200; un HTTP 500 corresponde a fallos no manejados de transporte/serialización.
