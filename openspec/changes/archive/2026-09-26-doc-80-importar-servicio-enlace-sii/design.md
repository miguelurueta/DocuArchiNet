<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-80 moderniza exclusivamente la consulta y preview de anexos SII disponibles durante la preasignación de tareas `ENLASE`. El recorrido actual obtiene `imagenes` mediante `consultarRadicado`, depende de sesión mutable y entrega una tabla Bootstrap con URL externa. La plataforma moderna de `ImportarServicioWeb` ya dispone de proveedor SII, transporte, autorización, telemetría y preview mediado.

## Goals / Non-Goals

**Goals**

- Publicar `ANEXOS_RADICADO_ENLASE` dentro de `INTEGRACIONSII`.
- Reconstruir y validar el contexto `ENLASE` en servidor.
- Normalizar anexos usando `idanexo` como identidad externa.
- Consultar y previsualizar sin mutaciones ni exposición de URL técnica.
- Preservar constancias y flujo legacy.

**Non-Goals**

- Preparar tipologías, persistir anexos o reconciliar documentos.
- Asignar la tarea.
- Modificar `ClassAlmacenamiento` o retirar endpoints/controles legacy.
- Ejecutar E2E real sin autorización.

## Decisions

### D-01 — Proveedor único con capacidad explícita

`SiiImportProvider` mantiene `ProviderId = INTEGRACIONSII`. La nueva semántica se expresa como capacidad `ANEXOS_RADICADO_ENLASE`; no se registra otro proveedor ni credenciales duplicadas.

### D-02 — Contexto autoritativo de preasignación

El servicio recibe identidad mínima de tarea y reconstruye usuario, grupo, actividad, ruta, trámite, código de barras, recibo y gabinete. La consulta se rechaza antes de llamar SII si el contexto no coincide o la actividad no es `ENLASE`.

### D-03 — Contrato e identidad del anexo

La fuente es `consultarRadicado` y su colección `imagenes`. El mapper conserva metadatos permitidos y usa `idanexo` como `ExternalKey`. Un valor vacío o duplicado invalida la respuesta; no se sintetizan claves con URL, nombre o índice.

### D-04 — Preview mediado

El navegador solicita preview por proveedor, capacidad e `idanexo`. El backend reconsulta el universo autorizado, localiza el anexo y convierte su URL en descriptor temporal ligado a usuario/tarea. El streaming aplica expiración, tamaño, formato y allowlist de host existentes. El handler resuelve la tarea desde `ID_TAREA_SELECCIONDA_ENLACE` cuando `SELECCIONTEMPORAL` declara `ENLASE`, exige coincidencia exacta con la tarea de la selección y no degrada a la variable estándar.

### D-05 — Lectura sin mutación y telemetría

`ResolveCapabilities`, `QueryItems` y preview no escriben documentos, expedientes, índices, caché o auditoría funcional. Los intentos técnicos se registran con operaciones diferenciadas y datos saneados.

### D-06 — Cambio aditivo y reversible

La implementación extiende puertos modernos. El flujo de constancias y los endpoints/controles legacy permanecen intactos durante DOC-80.

### D-07 — Validación proporcional

Fixtures deterministas prueban mapper, contexto, errores y preview sin red. La especificación E2E reutiliza `tools/e2e` para lectura/preview real; ejecutarla exige autorización de ambiente y cuentas.

## Component Flow

```text
Cliente moderno
  -> ASMX moderno
  -> servicio de contexto ENLASE
  -> SiiImportProvider / ANEXOS_RADICADO_ENLASE
  -> cliente SII compartido: solicitarToken + consultarRadicado
  -> mapper de imagenes por idanexo
  -> respuesta estructurada

Preview
  -> solicitud por idanexo
  -> reconsulta autoritativa
  -> descriptor temporal
  -> handler de streaming seguro
```

## Risks / Trade-offs

- `idanexo` es la única identidad explícita observada; vacío/duplicado obliga a fallo seguro.
- Reconsultar para preview aumenta una llamada SII, pero evita confiar en URL cliente.
- La sesión legacy puede participar como compatibilidad inicial, pero no sustituye la validación de tarea.
- Compartir proveedor reduce duplicación; exige regresión rigurosa de constancias.

## Migration Plan

1. Agregar contratos/capacidad de forma compatible.
2. Implementar contexto, cliente/mapeo y preview detrás de endpoints modernos.
3. Ejecutar pruebas locales y regresión de constancias.
4. Preparar E2E de lectura sin ejecutarla sin autorización.
5. Conservar legacy como rollback; su retiro pertenece a otra entrega.

## Open Questions

- Confirmar en E2E autorizada que `idanexo` siempre está informado y es único dentro del radicado.
- Confirmar formatos y hosts reales antes de ampliar allowlists productivas.
