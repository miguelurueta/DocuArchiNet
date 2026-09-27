# DOC-82 — Interfaz moderna ENLASE e integración con asignación

Esta documentación refleja el código de DOC-82. La interfaz abre desde `a_adj_service_web`, consume el proveedor `INTEGRACIONSII` con capacidad `ANEXOS_RADICADO_ENLASE`, permite consultar, previsualizar y preparar uno o varios anexos, y conserva la asignación como acción humana posterior.

Decisión arquitectónica comprobada: el backend moderno no publica `ValidateAssignment`. Por ello la UI no afirma preventivamente que una tarea sea asignable ni ejecuta `Buttonaceptar`. Al pulsar **Asignar**, `Webworkflow.Buttonaceptar_Click` vuelve a consultar los documentos obligatorios mediante `ClassWorkflowDigitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado`; cualquier resultado distinto de `YES` detiene el postback.

Contenido:

- `01-ARQUITECTURA-Y-FLUJO.md`: recorrido Cliente → Controller → Service → Repository → Respuesta.
- `02-ESTADOS-Y-CASOS-DE-USO.md`: estados visibles, decisiones y casos implementados.
- `03-INVENTARIO-Y-TRAZABILIDAD.md`: archivos, símbolos, endpoints y DTO consumidos.
- `04-PRUEBAS-SEGURIDAD-Y-OPERACION.md`: seguridad, accesibilidad, E2E y límites.
- `Diagramas/*.puml`: cuatro diagramas UML 2 en PlantUML.
- `diagram-contract.json`: inventario ejecutable de diagramas y referencias al código.

Convención: `CODE:` identifica un símbolo resoluble del repositorio; `EXT:` un actor/sistema externo; `CONCEPT:` un estado o concepto sin símbolo de código. Solo `EXT:` y `CONCEPT:` se excluyen de resolución estructural.

La prueba documental valida existencia, sintaxis básica y correspondencia estructural de símbolos y firmas. No demuestra por sí sola la fidelidad completa del comportamiento ni la cobertura de todos los casos de uso.
