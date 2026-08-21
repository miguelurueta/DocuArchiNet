<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
# Diseño técnico — Enviar a usuario (DOC-28)

## Alcance

La etapa implementa solamente el corte servidor de Enviar a usuario. No introduce interfaz, activación por configuración ni cambios en los contratos de transición por conector.

## Decisiones

### D-01 — Límite y contratos exclusivos

Se agregan `SolicitudEnvioUsuarioWorkflow`, `SolicitudPreviewEnvioUsuario`, `DestinoEnvioUsuarioWorkflow`, resultados tipados, puertos y DTOs paralelos a los de grupo. Ninguno define `IdConector`; los tipos compartidos de Continuar flujo permanecen intactos.

### D-02 — Contexto y autorización de servidor

`WorkflowPreviewSessionContextGate` incorpora una operación para Enviar a usuario que reconstruye el contexto autenticado y calcula `PuedeCambioUsuario` desde `SolicitaPermisosUsuarioWorkflow`. El permiso ausente, una sesión inválida o un error en la consulta equivalen a denegar. Esta operación no usa `IWorkflowModernFeatureGate`.

### D-03 — Preview privado y de solo lectura

`MySqlEnvioUsuarioRepository` forma primero el conjunto usuario–actividad de la ruta abierta y de la tarea actual. Después aplica búsqueda parametrizada, orden por nombre y claves, cursor opaco validable y un tamaño máximo. El repositorio expone únicamente datos de selección; sus rutas de preview usan `SELECT` y no reciben objetos Web Forms.

### D-04 — Revalidación y concurrencia

`ServicioEnvioUsuarioTarea` valida forma, adquiere `MySqlTransicionConcurrencyGuard` y, dentro del lease, relee tarea y token. Resuelve otra vez permiso, ruta/flujo, respuesta permitida, usuario activo, actividad relacionada, pertenencia a ruta, `UTIL_ASIGNA_TAREA` y notificación. Un rechazo no alcanza el motor legacy.

### D-05 — Único límite mutante

`WorkflowLegacyEnvioUsuarioExecutorAdapter` recibe el destino reautorizado y llama una vez a `ClassWorkflow.Terminar_Tarea_Workflow`. Pasa `Page = Nothing`, conector cero y valores de actualización de interfaz desactivados. No llama `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario` ni `Cambia_Estado` directamente.

### D-06 — Resultado y auditoría

El servicio traduce fallos internos a códigos públicos, no revela SQL ni Session y preserva advertencias de correo o eventos cuando la transición fue exitosa. La auditoría incluye un mecanismo normalizado `ASMX_ENVIO_USUARIO`; si registrar falla tras un éxito, se agrega advertencia sin revertir la transición.

### D-07 — Compatibilidad y evidencia

Los endpoints existentes y los contratos con `IdConector` quedan sin cambios. Las pruebas son unitarias y de integración local focalizada; no hay E2E autenticado, carga, modificación de gate ni configuración de ambiente. La documentación de esta etapa deja preparado el consumo UI posterior.

## Secuencia de ejecución

1. El ASMX obtiene el contexto de Enviar a usuario y crea dependencias específicas.
2. Preview valida formato, permiso y tarea; consulta una página de destinos sin escribir.
3. Ejecución valida solicitud, toma `GET_LOCK` por tarea y token y relee el estado.
4. El servicio reautoriza el destino y los requisitos de respuesta dentro del lock.
5. El adaptador exclusivo llama el motor legacy una vez.
6. El servicio normaliza, audita y libera el lock en todos los casos.

## Riesgos y mitigaciones

| Riesgo | Mitigación |
| --- | --- |
| El destino cambia entre preview y confirmación. | Relectura y resolución bajo lock con token de versión. |
| El permission array legacy cambia de forma. | Índice 18 validado con longitud mínima y denegación fail-closed. |
| La consulta revela usuarios no autorizados. | Universo restringido por ruta, actividad, estado y capacidad antes de filtrar. |
| Auditoría adicional no disponible. | Advertencia sanitizada; la transición confirmada no se revierte. |
| Regresión de Continuar flujo. | Tipos, puertos, endpoints y adaptadores separados; pruebas existentes sin cambios. |
