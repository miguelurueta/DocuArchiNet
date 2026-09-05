# Prompt backend 06 — Adaptador SII y compatibilidad ASMX

Implementa `INTEGRACIONSII` como primer adaptador del núcleo backend, preservando el recorrido legacy mientras esté vigente.

Publica las implementaciones SII de `ResolveCapabilities`, `QueryItems` y `GetPreview`, y respeta la propiedad contractual de compatibilidad legacy.

## Objetivo

Encapsular reglas, identidad y traducciones SII sin contaminar los contratos comunes ni el almacenamiento documental.

## Rutas canónicas de implementación

```txt
Infrastructure/Workflow/ImportarServicioWeb/Sii/
├── SiiImportProvider.vb
├── SiiExternalImportProviderClient.vb
├── SiiImportContractMapper.vb
├── SiiLegacyResultAdapter.vb
└── SiiPreviewResponseFactory.vb

webservice/
├── WebServiceImportarServicioWebModern.asmx
└── WebServiceImportarServicioWebModern.asmx.vb

Tests/
├── importar-servicio-web-sii-provider.test.cjs
├── importar-servicio-web-sii-contract-mapping.test.cjs
├── importar-servicio-web-sii-legacy-compatibility.test.cjs
└── importar-servicio-web-preview-mediation.test.cjs

Tests/Fixtures/Workflow/ImportarServicioWeb/sii-v1/
├── query-provider-response.json
├── normalized-query-response.json
├── preview-metadata.json
├── legacy-yes.json
├── legacy-ctrl.json
└── legacy-ctrlreturn.json
```

- Toda regla SII reside en `Infrastructure/Workflow/ImportarServicioWeb/Sii/`; no se agrega conocimiento SII a `Modelo`, DTO comunes, orquestador o almacenamiento.
- `SiiExternalImportProviderClient.vb` consume el transporte común de Backend 02.
- `SiiLegacyResultAdapter.vb` es el único traductor nuevo de `YES`, `CTRL`, `CTRLRETURN` y `dato_lista`.
- El ASMX nuevo es una frontera paralela delgada: contexto, gate, validación, invocación de servicios y serialización. No contiene negocio ni llama directamente `ClassAlmacenamiento`.
- Registrar los `.vb` y `.asmx` nuevos en el `.vbproj` sin cambiar entradas o endpoints existentes.

No modificar `webservice/WebService_integracion_sii.asmx*`, `WebServiceGaExpediente.asmx*`, `Integracionccv/`, `ServiciosIntegracion/`, `workflow/ClassAlmacenamiento.vb` ni `js/java_general/JSProgresBar.js`.

## Ruta documental obligatoria

```txt
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-adaptador-sii-compatibilidad-asmx/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear `00-Indice.md` a `07-Metadata.md` y `Diagramas/`; documentar endpoints modernos, contratos SII saneados, traducción legacy, preview mediado, gate, compatibilidad y archivos legacy comprobados sin cambios.

## Investigación obligatoria

- Resolver y documentar la clave externa canónica de cada inscripción.
- Caracterizar los contratos reales de token, consulta, recurso y constancia con fixtures saneados.
- Inventariar consumidores de `WebService_integracion_sii`, `WebServiceGaExpediente`, códigos `YES`/`CTRL`/`CTRLRETURN` y `dato_lista`.
- Confirmar qué controles y firmas ASMX deben conservarse durante la transición.

## Implementa

- `SiiImportProvider` y su registro exclusivo para la identidad configurada `INTEGRACIONSII`.
- Traducción de libro, registro, matrícula, acto, noticia, código de barras y cachés hacia contratos comunes en la frontera del adaptador.
- Consulta, preview/descarga, preparación y generación del comando documental normalizado.
- Uso del orquestador para expediente, documento, índices y caché, sin devolver la coordinación al navegador.
- Adaptadores ASMX delgados que validen entrada, construyan contexto explícito, invoquen casos de uso y traduzcan respuestas legacy.
- Traducción localizada entre resultados estructurados y `YES`, `CTRL`, `CTRLRETURN`/`dato_lista` mientras existan consumidores legacy.
- El adaptador backend ASMX es el único traductor de códigos legacy para la ruta moderna; nunca delega esa interpretación al frontend.
- Endpoint `GetPreview` mediado con autorización, expiración, tipo/tamaño, disposición y encabezados seguros conforme al contrato compartido.
- Gate reversible con comportamiento anterior intacto cuando esté desactivado.
- Evaluación servidor del gate `WorkflowCentroTrabajoModernActive`; apagado responde `FEATURE_DISABLED` sin efectos en las rutas modernas.
- Adaptación de los datos SII al contrato requerido por `AlmacenaDocumentoTareaWorkflow(...)`, invocándola sin modificar su implementación.

## Restricciones

- Construir endpoints y adaptadores modernos en paralelo; no reemplazar, reescribir ni redirigir los ASMX existentes.
- No modificar `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni sus consumidores vigentes; toda traducción se implementa fuera de ellos.
- SII nunca es fallback para proveedor desconocido.
- El almacenamiento común no interpreta conceptos registrales SII.
- No duplicar lógica moderna dentro de ASMX ni bloquear tareas asíncronas.
- No retirar endpoints o controles legacy sin inventario, regresión y evidencia autorizada.
- No cambiar formatos del proveedor basándose solo en variantes asíncronas existentes.
- No ejecutar llamadas reales a SII en pruebas automatizadas.

## Aceptación

- El núcleo backend no contiene símbolos ni reglas exclusivas de SII.
- El adaptador SII reutiliza la función de almacenamiento existente sin cambios comprobables en su archivo.
- Los consumidores legacy mantienen sus códigos y forma de respuesta bajo gate apagado.
- El recorrido moderno devuelve contratos estructurados y correlacionables.
- Proveedor desconocido y SII deshabilitado fallan de manera explícita.
- Las pruebas contractuales usan fixtures sin red y cubren traducción bidireccional legacy.

## Trazabilidad

Exploración backend: secciones 3, 4, 8, 12, 15 y 16; preguntas abiertas 1, 2, 3 y 10.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer B01–B05, `webservice/WebService_integracion_sii.asmx.vb`, `webservice/WebServiceGaExpediente.asmx.vb`, `Integracionccv/Class_consultarInformacionSello.vb`, `Integracionccv/Class_ClassResfull.vb`, `workflow/ClassAlmacenamiento.vb` y el `.vbproj`. Son referencias de comportamiento/contrato y no se modifican.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con contratos, diagramas, tabla de funciones, matriz legacy/moderno y evidencia de no modificación de almacenamiento y ASMX vigentes.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
