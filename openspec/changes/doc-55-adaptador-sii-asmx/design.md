<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Diseño: adaptador SII y compatibilidad ASMX

## Contexto

Los contratos B01-B05 ya separan proveedor, transporte, orquestación, almacenamiento y reconciliación. DOC-55 conecta SII a esas fronteras sin trasladar conceptos registrales al núcleo ni alterar rutas históricas.

## Objetivos y no objetivos

### Objetivos

- Implementar `ResolveCapabilities`, `QueryItems` y `GetPreview` para `INTEGRACIONSII`.
- Mantener compatibilidad legacy localizada y reversible.
- Publicar una frontera ASMX moderna delgada protegida por gate.

### No objetivos

- Reescribir o redirigir ASMX existentes.
- Modificar `ClassAlmacenamiento`, `AlmacenaDocumentoTareaWorkflow(...)` o consumidores legacy.
- Activar el gate, llamar SII real o ejecutar E2E autenticado.

## Decisiones

### D-01 — Registro exacto del proveedor

`RegistroProveedoresImportacion` resolverá SII solo para `INTEGRACIONSII`, con comparación normalizada explícita. Una identidad no registrada retorna error contractual; nunca cae en SII.

### D-02 — Transporte común

`SiiExternalImportProviderClient` compondrá solicitudes SII sobre las abstracciones HTTP de B02. Tokens, timeouts, límites y errores se gestionan en esa infraestructura y no atraviesan el contrato público.

### D-03 — Mapper fronterizo

`SiiImportContractMapper` convertirá libro, registro, matrícula, acto, noticia, código de barras y metadatos mínimos a DTO comunes. La clave externa será estable, opaca para el núcleo y documentada con fixtures.

### D-04 — Compatibilidad localizada

`SiiLegacyResultAdapter` será el único código nuevo que reconoce `YES`, `CTRL`, `CTRLRETURN` y `dato_lista`. La traducción será total, determinista y no se delegará al navegador.

### D-05 — ASMX moderno delgado

`WebServiceImportarServicioWebModern.asmx.vb` solo obtendrá contexto autenticado, evaluará gate, validará entrada, llamará servicios modernos y serializará. No incorporará reglas SII, SQL, filesystem ni llamadas directas a `ClassAlmacenamiento`.

### D-06 — Preview mediado

`SiiPreviewResponseFactory` proyectará preview autorizado con expiración, tipo/tamaño permitido, disposición segura y encabezados defensivos. Ninguna ruta física, token o excepción será serializada.

### D-07 — Gate reversible

El servidor evaluará `WorkflowCentroTrabajoModernActive` antes de cualquier llamada downstream. Apagado retorna `FEATURE_DISABLED`; la configuración debe permanecer en false al finalizar cualquier validación.

### D-08 — Persistencia por puerto moderno

El proveedor generará el comando normalizado y el orquestador coordinará persistencia mediante `IImportDocumentStorage`/`LegacyImportDocumentStorageAdapter`. DOC-55 no modificará la implementación legacy invocada.

### D-09 — Compatibilidad demostrable

Fixtures saneados cubrirán contratos SII y traducciones legacy sin red. Se registrarán pruebas focales, build VB.NET, validación OpenSpec y comparación de archivos protegidos.

## Flujo

1. El ASMX moderno valida contexto y gate.
2. El servicio resuelve exactamente `INTEGRACIONSII`.
3. El cliente usa el transporte HTTP compartido.
4. El mapper normaliza la respuesta SII.
5. Query/preview regresan DTO estructurado; ejecución entrega comando al orquestador.
6. Solo si un consumidor legacy lo requiere, el adaptador traduce la salida estructurada.

## Riesgos y mitigaciones

- Variantes SII no confirmadas: capturarlas como fixtures saneados antes de codificar mapping.
- Regresión legacy: archivos históricos quedan fuera del diff y se verifican por hash/diff.
- Exposición de contenido: preview valida autorización, expiración, tipo y tamaño.
- Activación accidental: gate evaluado en servidor y documentado en false.

## Rollback

Desactivar `WorkflowCentroTrabajoModernActive` retira la ruta moderna sin cambiar consumidores legacy. Los archivos nuevos pueden revertirse de forma aditiva porque ningún endpoint vigente será redirigido.
