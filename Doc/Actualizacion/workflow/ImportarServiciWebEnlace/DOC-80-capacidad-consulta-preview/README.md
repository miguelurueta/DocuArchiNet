# DOC-80 — Capacidad, consulta y preview de anexos SII ENLASE

## Alcance

DOC-80 agrega lectura y vista previa no mutadoras de anexos SII durante la preasignación `ENLASE`. Conserva `INTEGRACIONSII` como único proveedor y publica la capacidad `ANEXOS_RADICADO_ENLASE`. No prepara tipologías, persiste documentos, asigna tareas ni modifica el recorrido legacy.

## Diagramas UML

La arquitectura se documenta en fuentes PlantUML de texto conformes con notación UML 2:

- `Diagramas/01-componentes.puml`: componentes, interfaces y dependencias.
- `Diagramas/02-consulta-anexos-secuencia.puml`: secuencia de consulta y normalización.
- `Diagramas/03-preview-descriptor-secuencia.puml`: secuencia de preview y creación del descriptor.
- `Diagramas/04-streaming-actividad.puml`: actividad de autorización, HEAD/GET y consumo único.

El detalle narrativo y la trazabilidad están en [FLUJO-TECNICO-DETALLADO.md](FLUJO-TECNICO-DETALLADO.md) y [ALCANCE-Y-TRAZABILIDAD.md](ALCANCE-Y-TRAZABILIDAD.md).
## Contrato externo confirmado

- Autenticación: `solicitarToken`, con las credenciales existentes de SII.
- Consulta: `consultarRadicado`, `POST` JSON con `codigoempresa`, `usuariows`, `token` y `radicado`.
- Colección: `imagenes` del radicado.
- Identidad: `idanexo`; vacío o repetido invalida toda la respuesta.
- Contenido: la URL se usa únicamente después de reconsultar el radicado; jamás se acepta desde el navegador.
- Lista nula o ausente: lista vacía válida.
- Error SII o contrato inconsistente: código público saneado y telemetría técnica, sin cuerpo crudo.

## Mapping

| SII | Contrato moderno | Regla |
|---|---|---|
| `idanexo` | `ExternalKey` | Obligatorio y único |
| `observaciones` | `DisplayName` | Fallback `Anexo {idanexo}` |
| `formato` | `ContentType` | Allowlist compartida |
| `tipo`, `tipoanexo` | `Metadata` | Texto informativo |
| `fechadocumento`, `origen` | `Metadata` | Texto informativo |
| `url` | No se publica | Solo resolución interna reconsultada |

## Seguridad y estados

- El servidor selecciona `ID_TAREA_SELECCIONDA_ENLACE` y exige que `SELECCIONTEMPORAL` indique `ENLASE` y la misma tarea.
- Recibo y código de barras se reconstruyen mediante `Class_DAT_ADIC_TAR`; los valores del cliente se sobrescriben.
- Preview reutiliza límite de bytes, formatos, descriptor temporal, pertenencia usuario/tarea, consumo único y allowlist de host existentes.
- `QueryItems` y preview no escriben tarea, documentos, expedientes, índices ni auditoría funcional.
- La telemetría diferencia `CONSULTAR_ANEXOS_RADICADO_ENLASE`; descarga conserva `DESCARGAR_ANEXO` y el `OperationId` del preview.

## Inventario

- `SolicitudImportacionServicioDto.Capability`
- `SiiImportProvider.AnnexesEnlaseCapability`
- `SiiExternalImportProviderClient.QueryAnnexesAsync`
- `SiiExternalImportProviderClient.GetAnnexPreviewContentAsync`
- `SiiEnlaseAnnexContractMapper.MapQuery/Resolve`
- `WebServiceImportarServicioWebModern.TryBuildImportContext`
- escenario E2E `import-sii-enlase-read`

## Evidencia

- Compilación `MSBuild /t:Compile`: correcta; permanecen advertencias históricas del proyecto.
- Suite local `tests/importar-servicio-web-*.test.cjs`: correcta.
- Suite focal DOC-80: 6/6 correcta.
- Registro/perfil E2E DOC-80: 2/2 correcta.
- E2E real `import-sii-enlase-read`: PASS el 2026-09-25 con tarea ENLASE autorizada; 7 controles verificados, todos sin cambios; evidencia saneada disponible; gate restaurado a `false` con usuarios y grupos vacíos.
- No se modificaron `ClassAlmacenamiento`, endpoints legacy ni páginas Workflow.

## Ejecución E2E autorizada futura

Copiar el perfil de ejemplo con datos no sensibles y ejecutar únicamente después de autorización:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-read --profile <perfil-runtime.json> --authorize environment,gate
```

Al finalizar, el gate debe quedar en `false`, con usuarios y grupos vacíos, y la evidencia debe permanecer saneada.

## Paquete documental verificable

- [Flujo técnico textual](FLUJO-TECNICO-DETALLADO.md)
- [Alcance, convención y trazabilidad](ALCANCE-Y-TRAZABILIDAD.md)
- [Casos de uso](CASOS-DE-USO.md)
- [Inventario técnico](INVENTARIO-TECNICO.md)
- `Diagramas/*.txt` y `diagram-contract.json`: inventario ejecutable de los cuatro diagramas obligatorios.

Validación local y CI:

```powershell
node --test tests/doc80-technical-documentation.test.cjs
dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-80-capacidad-consulta-preview/diagram-contract.json .
```

Resultado local 2026-09-25: Node 4/4 PASS; Roslyn DOC-80 23 firmas y 6 tipos PASS; regresión Roslyn DOC-72 13 firmas y 17 tipos PASS.

La prueba valida existencia, sintaxis textual y correspondencia estructural de archivos, propietarios, sobrecargas, parámetros, retornos y DTOs. No demuestra por sí sola la fidelidad completa del comportamiento ni que estén cubiertos todos los casos de uso; esa conclusión requiere revisión del código y pruebas funcionales/E2E autorizadas.

## Corrección de autoridad ENLASE

El handler `ImportarServicioWebPreview` usa `TryResolveTrustedTaskId`. Cuando `SELECCIONTEMPORAL` declara `ENLASE`, exige `ID_TAREA_SELECCIONDA_ENLACE`, valida que sea positiva y que coincida con `selection(0)`; no usa `ID_TAREA_SELECCIONDA` como fallback. Los contextos no ENLASE conservan la resolución histórica. Ausencia o discrepancia produce el mismo HTTP 404 opaco que un descriptor inválido.

Evidencia focal: 16/16 pruebas PASS; suite completa `tests/importar-servicio-web-*.test.cjs` PASS; compilación MSBuild PASS con advertencias históricas.
