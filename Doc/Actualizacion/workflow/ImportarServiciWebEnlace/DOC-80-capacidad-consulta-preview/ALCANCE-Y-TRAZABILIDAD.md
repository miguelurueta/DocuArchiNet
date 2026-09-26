# Alcance y trazabilidad de la revisión

## Convención

- `CODE:` identifica una clase, interfaz, método, función o DTO resoluble en código.
- `EXT:` identifica un actor o sistema externo y se excluye expresamente de resolución estructural.
- `CONCEPT:` identifica una decisión o resultado conceptual y se excluye expresamente de resolución estructural.
- La notación normativa es UML 2 expresada como fuente PlantUML: componentes, secuencias y actividad. `CODE:`, `EXT:` y `CONCEPT:` son estereotipos de trazabilidad del proyecto, no sustituyen los elementos UML.

## Repositorios y módulos incluidos

La revisión corresponde al repositorio `DocuArchiNet` y al alcance de lectura/preview de DOC-80: frontera ASMX, reconstrucción de contexto, proveedor/cliente/mapper SII, DTOs y descriptor/streaming persistido. Se revisaron `DTOs/Workflow/ImportarServicioWeb`, `Modelo/Workflow/ImportarServicioWeb`, `Infrastructure/Workflow/ImportarServicioWeb/{Sii,Preview}`, `webservice/WebServiceImportarServicioWebModern.asmx.vb`, `workflow/ImportarServicioWebPreview.ashx.vb` y la consulta legacy puntual de `workflow/Class_DAT_ADIC_TAR.vb`.

Quedan fuera: preparación, intención, ejecución y persistencia documental; expedientes; UI completa; asignación `ENLASE`; endpoints legacy; servicios físicos de SII y esquema MySQL desplegado. Se mencionan solo cuando constituyen una frontera del flujo. No se declara cobertura completa sobre esos componentes.

## Inventario obligatorio de diagramas

| ID | Archivo | Operación | Fuentes principales |
|---|---|---|---|
| DOC80-D01 | `Diagramas/01-componentes.puml` | Capacidades y autorización de contexto | ASMX, proveedor |
| DOC80-D02 | `Diagramas/02-consulta-anexos-secuencia.puml` | Consulta y normalización | ASMX, cliente, mapper |
| DOC80-D03 | `Diagramas/03-preview-descriptor-secuencia.puml` | Resolución y descriptor | ASMX, SII, servicio, repositorio |
| DOC80-D04 | `Diagramas/04-streaming-actividad.puml` | HEAD/GET y consumo único | handler, servicio, repositorio |

El contrato ejecutable es `diagram-contract.json`. La prueba falla si falta un diagrama, si faltan los delimitadores PlantUML o están desbalanceados los bloques UML usados (`alt/end`, `if/endif`), si una fuente no existe, si un símbolo no está declarado o si Roslyn no encuentra exactamente su propietario, sobrecarga, parámetros, retorno y archivo. Esta validación estructural no reemplaza el renderizado completo con PlantUML CLI.

## Hallazgos corregidos y no verificables

- Se corrigió la documentación que presentaba `GetPreview` como streaming directo: el ASMX crea un descriptor; el binario se entrega por el handler.
- Se corrigió la expectativa de códigos HTTP funcionales: los métodos ASMX retornan envelopes con `Error.Codigo` y normalmente HTTP 200; los códigos 404/405/503 pertenecen al handler.
- Se corrigió la asimetría de tarea: si `SELECCIONTEMPORAL` declara `ENLASE`, el handler exige `ID_TAREA_SELECCIONDA_ENLACE` y su coincidencia exacta con `selection(0)`; no utiliza la selección estándar como fallback. Para otros contextos conserva `ID_TAREA_SELECCIONDA`.
- No puede verificarse localmente que el despliegue SII siempre informe `idanexo` único, que los hosts/formats productivos coincidan con la allowlist ni que la tabla de descriptores esté migrada en cada ambiente.
