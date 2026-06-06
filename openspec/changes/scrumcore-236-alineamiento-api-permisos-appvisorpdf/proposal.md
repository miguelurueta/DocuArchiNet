## Why

ALINEAMIENTO-API-PERMISOS-APPVISORPDF. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-236.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> PROMPT ENTERPRISE - Alineamiento Frontend con Contrato API Permisos Visor PDF
> Contexto
>   El proyecto DocuArchiCore.react ya consume permisos del visor PDF desde:
>   GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos
>   Actualmente el frontend sí está enviando:
>   codigoImpl = gestion_correspondencia
>   Pero la implementación no está completamente alineada con el contrato real de la API porque:
> El servicio espera Permissions en la raíz de la respuesta.
> 
> La API documentada retorna permisos dentro de data.Permissions.
> 
> El mapping frontend usa claves antiguas/no documentadas como pdf.export, pdf.signature.add, pdf.annotation.edit.
> 
> El contrato backend documenta claves como pdf.download, pdf.annotate.signature.place, pdf.annotate.signature.delete, pdf.zoom, etc.
> 
> Objetivo
>   Alinear AppVisorEmbedPdf con el contrato API oficial de permisos del visor PDF.
>   El cambio debe garantizar que:
> El frontend lea correctamente response.data.data.Permissions.
> 
> El frontend conserve metadata relevante de CodigoImplementacion, IdUsuario, Sources, GeneratedAt, success, message, meta, errors.
> 
> El mapping de permisos use las claves reales del contrato backend.
> 
> DocumentosWorkbench siga enviando nombre_modulo: "gestioncorrespondencia".
> 
> resolveCodigoImplementacion("gestioncorrespondencia") siga resolviendo gestion_correspondencia.
> 
> El idUsuario siga resolviéndose desde el JWT en backend, no desde frontend.
> 
> El visor mantenga fail-safe/fail-closed para acciones sensibles si la API falla o retorna contrato inválido.
> 
> No se amplíe el alcance visual del visor salvo que ya exista soporte en el componente.
> 
> Carpeta de documentación a crear
>   Crear documentación enterprise en:
>   docs/Architecture/AlineamientoContratoApiPermisosVisor/
>   Archivos requeridos:
> SCRUM-[ID]-Metadata.md
> 
> SCRUM-[ID]-Arquitectura.md
> 
> SCRUM-[ID]-Contrato-API.md
> 
> SCRUM-[ID]-Implementacion-Detallada.md
> 
> SCRUM-[ID]-Pruebas.md
> 
> PROMPT-SCRUM-[ID]-AlineamientoContratoApiPermisosVisor.md
> 
>   Usar el ID real del ticket cuando esté disponible. Si no existe todavía, usar SCRUM-TBD.
> Contrato API oficial
> Endpoint usuario autenticado
> GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos
> Authorization: Bearer {jwt}
> 
> ### Claims mínimos requeridos en JWT
> 
> defaulalias
> usuarioid
> 
> ### Valores válidos conocidos de codigoImpl
> 
> workflow
> gestion_correspondencia
> 
> ### Response OK esperado
> 
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "CodigoImplementacion": "gestion_correspondencia",
>     "IdUsuario": 205,
>     "Permissions": {
>       "pdf.view": true,
>       "pdf.print": false,
>       "pdf.download": false,
>       "pdf.annotate.open_signature_modal": false,
>       "pdf.annotate.signature.draw": false,
>       "pdf.annotate.signature.upload": false,
>       "pdf.annotate.signature.personal": false,
>       "pdf.annotate.signature.place": false,
>       "pdf.annotate.signature.delete": false,
>       "pdf.annotate.signature.lock": false,
>       "pdf.annotate.signature.unlock": false,
>       "pdf.rotate": false,
>       "pdf.zoom": true
>     },
>     "Sources": {
>       "pdf.view": "perfil_activo",
>       "pdf.zoom": "perfil_activo",
>       "pdf.print": "perfil_activo",
>       "pdf.download": "perfil_activo"
>     },
>     "GeneratedAt": "2026-05-20T14:40:00Z"
>   },
>   "meta": {
>     "Status": "success",
>     "Total": 13
>   },
>   "errors": []
> }
> 
> ### Response no autorizado esperado
> 
> {
>   "success": false,
>   "message": "No cuenta con permisos administrativos",
>   "data": {},
>   "meta": {
>     "Status": "validation"
>   },
>   "errors": [
>     {
>       "Type": "Validation",
>       "Field": "authorization",
>       "Message": "No cuenta con permisos administrativos"
>     }
>   ]
> }
> 
> ## Archivos de código a modificar
> 
> ### 1. Service de permisos
> 
> Modificar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts
> 
> Actualmente el service asume que Permissions viene en raíz:
> 
> const res = await clienteApi.get<VisorPdfPermissionsResponse>(url);
> return res.data;
> 
> Debe alinearse al envelope real:
> 
> type ApiEnvelope<T> = {
>   success: boolean;
>   message: string;
>   data: T;
>   meta?: {
>     Status?: string;
>     Total?: number;
>   };
>   errors?: Array<{
>     Type?: string;
>     Field?: string;
>     Message?: string;
>   }>;
> };
> 
> Implementación esperada:
> 
> const res = await clienteApi.get<ApiEnvelope<VisorPdfPermissionsResponse>>(
>   `/api/gestor-documental/permisos-visorpdf/implementaciones/${encodeURIComponent(codigoImpl)}/mis-permisos`,
>   { signal },
> );
> 
> const envelope = res.data;
> 
> if (!envelope?.success || !envelope?.data?.Permissions) {
>   throw new Error(envelope?.message || "Permisos visor PDF: contrato inválido.");
> }
> 
> return envelope.data;
> 
> Reglas:
> 
> - Mantener AbortSignal.
> - No enviar idUsuario.
> - No usar endpoint admin en este flujo.
> - No hacer fallback silencioso a respuesta antigua salvo que el ticket lo pida explícitamente.
> - Si se decide compatibilidad temporal con contrato antiguo, documentarlo como fallback transitorio y agregar pruebas.
> 
> ### 2. Tipos de respuesta
> 
> Modificar o ampliar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts
> 
> Mantener el payload interno:
> 
> export type VisorPdfPermissionsResponse = {
>   CodigoImplementacion: string;
>   IdUsuario: number;
>   Permissions: Record<string, boolean>;
>   Sources?: Record<string, string>;
>   GeneratedAt: string;
> };
> 
> Agregar tipo de permisos documentados:
> 
> export type VisorPdfPermissionCode =
>   | "pdf.view"
>   | "pdf.print"
>   | "pdf.download"
>   | "pdf.annotate.open_signature_modal"
>   | "pdf.annotate.signature.draw"
>   | "pdf.annotate.signature.upload"
>   | "pdf.annotate.signature.personal"
>   | "pdf.annotate.signature.place"
>   | "pdf.annotate.signature.delete"
>   | "pdf.annotate.signature.lock"
>   | "pdf.annotate.signature.unlock"
>   | "pdf.rotate"
>   | "pdf.zoom";
> 
> Opcional:
> 
> Permissions: Partial<Record<VisorPdfPermissionCode, boolean>> & Record<string, boolean>;
> 
> ## 3. Mapping de permisos
> 
> Modificar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts
> 
> Actualmente el mapping usa claves antiguas:
> 
> pdf.signature.add
> pdf.signature.delete
> pdf.signature.lock
> pdf.annotation.edit
> pdf.export
> pdf.print
> 
> Debe usar las claves reales del contrato:
> 
> export function mapPermisosVisorPdfToEffectivePermissions(
>   permissionsRaw: Record<string, boolean>,
> ): ViewerEffectivePermissions {
>   const raw = permissionsRaw ?? {};
> 
>   const allowSignaturePlacement = Boolean(raw["pdf.annotate.signature.place"]);
>   const allowSignatureDelete = Boolean(raw["pdf.annotate.signature.delete"]);
>   const allowSignatureLockToggle = Boolean(
>     raw["pdf.annotate.signature.lock"] || raw["pdf.annotate.signature.unlock"]
>   );
> 
>   const allowAnnotationEdit = Boolean(
>     raw["pdf.annotate.open_signature_modal"] ||
>     raw["pdf.annotate.signature.draw"] ||
>     raw["pdf.annotate.signature.upload"] ||
>     raw["pdf.annotate.signature.personal"] ||
>     raw["pdf.annotate.signature.place"]
>   );
> 
>   const allowExport = Boolean(raw["pdf.download"]);
>   const allowPrint = Boolean(raw["pdf.print"]);
> 
>   return {
>     allowSignaturePlacement,
>     allowSignatureDelete,
>     allowSignatureLockToggle,
>     allowAnnotationEdit,
>     allowExport,
>     allowPrint,
>   };
> }
> 
> ### Sobre pdf.view, pdf.zoom y pdf.rotate
> 
> No ampliar ViewerEffectivePermissions para pdf.view, pdf.zoom o pdf.rotate en esta iteración, salvo que el ticket lo exija explícitamente.
> 
> Documentarlos como permisos backend disponibles pero actualmente no conectados a la UI si el componente no tiene soporte previo.
> 
> No bloquear visualización del PDF por pdf.view=false en esta iteración, salvo decisión explícita de producto/seguridad.
> 
> ## 4. codigoImpl
> 
> Validar que el mapping actual permanezca así:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts
> 
> gestioncorrespondencia: "gestion_correspondencia"
> 
> No volver a:
> 
> GESTION_CORRESPONDENCIA
> 
> ## 5. DocumentosWorkbench
> 
> Revisar, pero no cambiar salvo necesidad:
> 
> src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
> 
> Debe seguir enviando:
> 
> nombre_modulo: "gestioncorrespondencia"
> 
> No debe enviar idUsuario.
> 
> No debe consultar permisos.
> 
> No debe conocer endpoint de permisos.
> 
> No debe resolver codigoImpl directamente si el resolver sigue centralizado en AppVisorEmbedPdf.permissions.ts.
> 
> ## 6. AppVisorEmbedPdf
> 
> Revisar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx
> 
> Debe mantenerse el flujo:
> 
> const codigoImpl = resolveCodigoImplementacion(input.nombre_modulo);
> const perms = await fetchMisPermisosVisorPdf({ codigoImpl, signal });
> const mapped = mapPermisosVisorPdfToEffectivePermissions(perms.Permissions ?? {});
> 
> Después del cambio de service, perms ya debe ser response.data.data.
> 
> Agregar o mantener log debug solo detrás del flag existente:
> 
> dvLog("[DV][visor][permissions]", {
>   codigoImpl,
>   response: perms,
>   raw: perms.Permissions ?? {},
>   effective: mapped,
> });
> 
> El log no debe ejecutarse salvo que esté activo:
> 
> window.__DV_DEBUG__ = true
> 
> Si el equipo no quiere logs en runtime, retirarlo al finalizar la validación.
> 
> ## Diagrama de flujo esperado
> 
> sequenceDiagram
>   autonumber
>   participant U as Usuario
>   participant T as AppTreeTable
>   participant W as DocumentosWorkbench
>   participant O as AppDocumentViewerOrchestrator
>   participant V as AppVisorEmbedPdf
>   participant S as AppVisorEmbedPdf.service
>   participant API as Backend permisos visor
> 
>   U->>T: Click en documento
>   T->>W: onSelectRow(rowId)
>   W->>O: visualizarDocumento(documentId, nombreGabinete, context)
>   O-->>W: documentoActivo con fileUrl
>   W->>V: load({ url, nombre_modulo: gestioncorrespondencia, contexto })
>   V->>V: resolveCodigoImplementacion(nombre_modulo)
>   V->>S: fetchMisPermisosVisorPdf({ codigoImpl: gestion_correspondencia })
>   S->>API: GET /implementaciones/gestion_correspondencia/mis-permisos
>   API-->>S: Envelope { success, data: { Permissions } }
>   S-->>V: data
>   V->>V: mapPermisosVisorPdfToEffectivePermissions(data.Permissions)
>   V-->>U: PDF visible + acciones segun permisos
> 
> ## Diagrama de responsabilidades
> 
> flowchart TD
>   A[Modulo funcional] --> B[Define nombre_modulo]
>   B --> C[DocumentosWorkbench]
>   C --> D[AppTreeTable emite click]
>   C --> E[Orquestador resuelve documento]
>   C --> F[AppVisorEmbedPdf.load]
>   F --> G[Resolver codigoImpl]
>   G --> H[Service permisos]
>   H --> I[Backend mis-permisos]
>   I --> J[Permisos efectivos]
>   J --> K[Toolbar/acciones del visor]
> 
>   D -. no consulta permisos .-> D
>   C -. no decide policy .-> C
>   F -. aplica policy .-> K
> 
> ## Reglas de seguridad
> 
> - No enviar idUsuario desde frontend para mis-permisos.
> - No usar endpoint admin desde el visor normal.
> - idUsuario se resuelve en backend desde claim usuarioid.
> - Mantener Authorization: Bearer {jwt} vía clienteApi.
> - Si success=false, contrato inválido o error HTTP, aplicar fail-closed para acciones sensibles.
> - No bloquear visualización del PDF si url es válida, salvo decisión explícita sobre pdf.view.
> - No enviar codiperfil desde frontend.
> - No duplicar lógica de permisos en DocumentosWorkbench ni AppTreeTable.
> 
> ## Casos de prueba requeridos
> 
> ### Service
> 
> Crear o actualizar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.test.ts
> 
> Casos:
> 
> - Hace GET al endpoint correcto con codigoImpl = gestion_correspondencia.
> - Retorna data del envelope.
> - Lanza error si success=false.
> - Lanza error si data.Permissions falta.
> - Respeta AbortSignal.
> 
> ### Permissions mapping
> 
> Crear o actualizar:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.test.ts
> 
> Casos:
> 
> - gestioncorrespondencia resuelve gestion_correspondencia.
> - pdf.print=true habilita allowPrint.
> - pdf.download=true habilita allowExport.
> - pdf.annotate.signature.place=true habilita allowSignaturePlacement.
> - pdf.annotate.signature.delete=true habilita allowSignatureDelete.
> - pdf.annotate.signature.lock=true habilita allowSignatureLockToggle.
> - pdf.annotate.signature.unlock=true habilita allowSignatureLockToggle.
> - Permisos vacíos producen todo false.
> - Permisos desconocidos no rompen mapping.
> - applySignedOverride bloquea edición/firma cuando el PDF está firmado.
> 
> ### Integración visor
> 
> Actualizar si aplica:
> 
> src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
> 
> Casos:
> 
> - load() consulta permisos usando gestion_correspondencia.
> - load() aplica mapping desde data.Permissions.
> - Si el service falla, acciones sensibles quedan bloqueadas.
> - Si no hay mapping de módulo, estado de permisos queda failed.
> 
> ## Validación manual esperada
> 
> Activar debug:
> 
> window.__DV_DEBUG__ = true
> 
> Abrir documento desde Gestión Correspondencia.
> 
> Debe verse en consola:
> 
> [DV][visor][permissions] {
>   codigoImpl: "gestion_correspondencia",
>   response: {
>     CodigoImplementacion: "gestion_correspondencia",
>     IdUsuario: 205,
>     Permissions: {
>       "pdf.view": true,
>       "pdf.print": false,
>       "pdf.download": false,
>       "pdf.zoom": true
>     }
>   },
>   raw: {
>     "pdf.view": true,
>     "pdf.print": false,
>     "pdf.download": false,
>     "pdf.zoom": true
>   },
>   effective: {
>     allowPrint: false,
>     allowExport: false,
>     allowSignaturePlacement: false,
>     allowSignatureDelete: false,
>     allowSignatureLockToggle: false,
>     allowAnnotationEdit: false
>   }
> }
> 
> Si backend devuelve:
> 
> "pdf.download": true
> 
> Debe resultar:
> 
> effective.allowExport === true
> 
> Si backend devuelve:
> 
> "pdf.print": true
> 
> Debe resultar:
> 
> effective.allowPrint === true
> 
> ## Documentación enterprise a generar
> 
> ### SCRUM-[ID]-Metadata.md
> 
> Debe incluir:
> 
> - Ticket.
> - Fecha.
> - Tipo de cambio.
> - Archivos impactados.
> - Endpoint.
> - Estado de implementación.
> - Estado de pruebas.
> - Riesgos.
> - Dependencias backend.
> 
> ### SCRUM-[ID]-Contrato-API.md
> 
> Debe incluir:
> 
> - Endpoint usuario autenticado.
> - Endpoints admin.
> - Claims JWT.
> - Valores codigoImpl.
> - Response OK.
> - Response error.
> - Tabla de permisos.
> - Interpretación FE.
> - Checklist depuración.
> 
> ### SCRUM-[ID]-Arquitectura.md
> 
> Debe incluir:
> 
> - Objetivo arquitectónico.
> - Responsabilidades por capa.
> - Diagramas Mermaid.
> - Decisiones ADR.
> - Riesgos.
> - Non-goals.
> 
> ### SCRUM-[ID]-Implementacion-Detallada.md
> 
> Debe incluir:
> 
> - Cambios por archivo.
> - Antes/después del contrato.
> - Mapping anterior vs mapping nuevo.
> - Fail-closed.
> - Debug logging.
> - Compatibilidad con módulos futuros.
> 
> ### SCRUM-[ID]-Pruebas.md
> 
> Debe incluir:
> 
> - Unit tests requeridos.
> - Integration tests requeridos.
> - Validación manual.
> - Casos negativos.
> - Evidencia esperada.
> - Pendientes si no hay backend/QA.
> 
> ## Criterios de aceptación
> 
> - codigoImpl enviado para Gestión Correspondencia es gestion_correspondencia.
> - El frontend lee response.data.data.Permissions.
> - El frontend no depende de Permissions en raíz.
> - pdf.download se traduce a allowExport.
> - pdf.print se traduce a allowPrint.
> - Permisos de firma se traducen desde pdf.annotate.signature.*.
> - Si Permissions está vacío, acciones quedan bloqueadas.
> - Si API falla, acciones quedan bloqueadas.
> - DocumentosWorkbench no consulta permisos.
> - AppTreeTable no conoce permisos.
> - idUsuario no se envía desde frontend en mis-permisos.
> - No se amplía ViewerEffectivePermissions para pdf.view, pdf.zoom, pdf.rotate sin requerimiento explícito.
> - Tests unitarios pasan.
> - Lint focalizado pasa.
> 
> ## Comandos de validación sugeridos
> 
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.test.tsx
> 
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.test.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.test.ts
> 
> npx.cmd eslint src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ALINEAMIENTO, API, APPVISOREMBEDPDF

## Capabilities

### New Capabilities
- `alineamiento-api-permisos-appvisorpdf`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
