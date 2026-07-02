# Modelo Tecnico Frontend

## Alcance

Este documento define la arquitectura objetivo del frontend para `src/modules/radicacion`, incluyendo navegacion contextual, contexto documental, lista de pendientes con `AppTable`, mutaciones de pendiente y limpieza del formulario.

No implementa carga documental real, visor PDF final, digitalizacion ni reemplazo completo del workbench documental.

## Estado Actual Observado

| Archivo | Observacion |
|---|---|
| `src/modules/radicacion/pages/RadicacionRoutePage.tsx` | Carga plantilla con `useCamposPlantilla` y la pasa a `RadicacionPage`. |
| `src/modules/radicacion/pages/RadicacionPage.tsx` | Recibe `plantilla`, pero el componente actual no la usa funcionalmente en el flujo principal. |
| `src/modules/radicacion/hooks/RadicacionTabs.tsx` | Usa tabs con keys numericas y renderiza `CapDocument` sin guard documental. |
| `src/modules/radicacion/components/RadicacionForm.tsx` | Vuelve a llamar `useCamposPlantilla`, concentra demasiada logica y tiene boton `Enviar a Pendientes` sin flujo final. |
| `src/modules/radicacion/components/Modalpendiente.tsx` | Usa tabla y datos mock; debe migrar a `AppTable`. |
| `src/modules/radicacion/components/CapDocument.tsx` | Mantiene UI prototipo; debe quedar detras de guard y no aparentar datos reales. |

## Arquitectura Objetivo

```txt
RadicacionRoutePage
  -> RadicacionStartupGuard
  -> RadicacionDocumentalProvider
  -> RadicacionPage
  -> RadicacionTabs
      -> RadicacionForm
      -> RadicacionPendientesModal
      -> RadicacionDocumentosGuard
          -> CapDocument
```

## Responsabilidades

| Elemento | Responsabilidad |
|---|---|
| `RadicacionRoutePage` | Resolver plantilla inicial, montar provider y delegar inicio del modulo. |
| `RadicacionStartupGuard` | Consultar `estado-activo` y decidir si entra a formulario o documentos. |
| `RadicacionDocumentalProvider` | Mantener contexto unico del tramite activo. |
| `RadicacionPage` | Componer layout de radicacion y pasar dependencias al shell. |
| `RadicacionTabs` | Administrar tabs/rutas semanticas y estado disabled. |
| `RadicacionForm` | Capturar datos entrantes, radicar, limpiar formulario y enviar activo a pendiente. |
| `RadicacionPendientesModal` | Mostrar pendientes con `AppTable` y ejecutar `asignacion-tarea`. |
| `RadicacionDocumentosGuard` | Bloquear `CapDocument` si no hay `estado = 0`. |
| `CapDocument` | Renderizar el panel documental solo con contexto valido. |

## Contexto Documental

Contrato interno sugerido:

```ts
type RadicacionDocumentalContextValue = {
  activo: boolean;
  estado: 0 | 1 | null;
  idEstadoRadicado: number | null;
  idRadicado: number | null;
  consecutivoRadicado: string | null;
  idTareaWorkflow: number | null;
  tramite: string | null;
  requiereGestionDocumental: boolean;
  origen: "registro" | "pendiente" | "startup" | null;
  setActivoDesdeBackend(contexto: RadicacionDocumentalContextDto): void;
  limpiarContextoDocumental(): void;
};
```

Regla tecnica: `activo === true` solo si `estado === 0` y existe `idEstadoRadicado`.

## Services Y Hooks

| Hook/Service | Contrato |
|---|---|
| `useRadicacionEstadoActivo` | Lee `GET /api/radicacion/pendientes/estado-activo`. |
| `useRadicacionPendientesContador` | Lee `GET /api/radicacion/pendientes/contador`. |
| `useRadicacionPendientesTable` | Configura `AppTable` contra listado de pendientes. |
| `useTomarRadicadoPendiente` | Ejecuta `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`. |
| `useEnviarRadicadoPendiente` | Ejecuta `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`. |
| `radicacionPendientes.service` | Aisla endpoints HTTP de pendientes. |
| `radicacionRegistro.service` | Aisla registro entrante y mappers de DTO. |

## Rutas Objetivo

| Ruta | Uso |
|---|---|
| `/dashboard/radicacion` | Entrada base al modulo. |
| `/dashboard/radicacion/registro` | Formulario de radicacion entrante. |
| `/dashboard/radicacion/registro/:idEstadoRadicado` | Shell contextual del tramite. |
| `/dashboard/radicacion/registro/:idEstadoRadicado/documentos` | Panel documental, solo con `estado = 0`. |

## Integracion Con AppTable

La lista de pendientes debe usar `AppTable` porque el repo ya tiene:

- adaptadores `dynamicUiToAgGridRows` y `dynamicUiToAgGridColumns`;
- hooks de query y acciones;
- resolutores de accion dinamica;
- pruebas existentes del comportamiento general.

La accion requerida es:

```txt
asignacion-tarea
```

Payload minimo de fila:

```ts
type RadicacionPendienteRow = {
  id_estado_radicado: number;
  id_tarea_workflow: number;
  consecutivo_radicado: string;
  tramite: string;
  fecha_radicado?: string;
  remitente?: string;
};
```

## Riesgos Tecnicos

| Riesgo | Mitigacion |
|---|---|
| Doble carga de plantilla | Ejecutar `PROMPT-TD-FE-01` antes de conectar registro y pendientes. |
| `Documentos` habilitado sin activo | Crear `RadicacionDocumentosGuard` antes de integrar mutaciones. |
| Pendientes mock mezclados con datos reales | Reemplazar `Modalpendiente` con `AppTable` y eliminar datos hardcoded. |
| Limpieza borra contexto activo | Separar estado de formulario y estado documental. |
| Toma simultanea de varios pendientes | Backend debe bloquear si ya existe activo `estado = 0`; frontend debe reflejar el error. |

