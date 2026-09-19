<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05 -->

## Context

DOC-67 dejó operativo el backend moderno. `QueryItems` aplana cada imagen SII a un item básico y `ResolveCapabilities` no publica tipologías. DOC-68 evita que el frontend interprete claves opacas o consulte superficies legacy.

## Goals / Non-Goals

**Goals:** metadatos saneados; estado/acciones por item; catálogo por trámite; una consulta SII; compatibilidad DOC-67.

**Non-Goals:** importar, descargar, crear intención/expediente, modificar legacy, exponer datos sensibles o implementar frontend.

## Architecture

```text
WebServiceImportarServicioWebModern
  ├─ ResolveCapabilities → ImportItemPresentationService
  │                        └─ MySqlImportDocumentTypeCatalogRepository (Radicación)
  └─ QueryItems → SiiExternalImportProviderClient.QueryItemsAsync (una vez)
                  → SiiImportContractMapper.MapQuery
                  → ImportItemPresentationService
                    └─ MySqlImportItemStatusRepository (Workflow)
```

## Decisions

### D-01 — Contrato aditivo con subcontratos 1.1

Agregar DTOs para metadatos, tipologías y acciones. Capabilities recibe catálogo y cada item presentación/estado. El envelope 1.0 y todos sus campos permanecen; los objetos añadidos declaran 1.1 siguiendo `ImportItemExpedientEffectsDto`. Se descarta otro endpoint y cambiar `DisplayName`.

### D-02 — Metadatos desde el único mapping SII

`MapQuery` crea `Code/Label/Value` limitado y saneado desde la inscripción/imagen que originó `ExternalKey`: libro, registro, fecha, acto/naturaleza, noticia y referencia cuando existan. No devuelve JSON ni atributos sensibles. Se descarta consultar por fila o contaminar el núcleo con propiedades SII.

### D-03 — Catálogo autoritativo desde Radicación

`MySqlImportDocumentTypeCatalogRepository` filtra por `ContextoImportacionServicio.IdTramite` y une checklist con `tipo_doc_series`. Publica ID TRD, descripción, `OBLIGATORIO` y `ORDEN_LISTA`; el ID checklist queda interno. Orden estable y ambigüedad fail-closed. `ResolveCapabilities` conserva la sesión resuelta para usar conexión Radicación.

### D-04 — Estado y acciones desde persistencia moderna

`MySqlImportItemStatusRepository` consulta tarea+proveedor+clave. El servicio presenta `Disponible`, `Importado` o `ConNovedad`. Solo documento confirmado y reconciliado es `Importado`; fallo/parcial/conflicto/incierto es `ConNovedad`; ausencia es `Disponible`. Las acciones son derivadas, no aceptadas del cliente.

### D-05 — Enriquecimiento local y llamada externa única

`QueryItems` llama una vez al proveedor y después aplica estado local. Catálogo, filtros y paginación no llaman SII. Las pruebas cuentan `CONSULTAR_SELLO`, cubren cardinalidades y reutilizan `tools/e2e`.

## Risks / Trade-offs

- Campos opcionales se omiten; no se inventan.
- Duplicados de checklist bloquean en vez de escoger la primera fila.
- El estado puede cambiar después del listado; preflight/ejecución revalidan.
- Sin autorización, la E2E queda bloqueada y no se simula.

## Migration Plan

1. DTOs/fixtures. 2. Repositorios catálogo/estado. 3. Mapper/servicio. 4. Composición ASMX. 5. Pruebas, compilación, documentación y E2E autorizada. Gate apagado por defecto.
