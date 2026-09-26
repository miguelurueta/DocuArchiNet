# Prompt 01 — Capacidad, consulta y preview de anexos SII para ENLASE

## Contexto de ejecución OPSXJ

Este prompt se ejecuta dentro de un ticket OPSXJ previamente creado y revisado. No ejecutar `opsxj:new`, no crear otro ticket, no reiniciar la orquestación y no crear un cambio OpenSpec manual. Continúa exclusivamente sobre el contexto, identificador y artefactos OPSXJ entregados al iniciar la ejecución.

Antes de modificar código productivo, verifica que la revisión del prompt y las compuertas previas del ticket estén satisfechas.

## Rol esperado

Actúa como arquitecto e implementador senior de ASP.NET WebForms, VB.NET, integración HTTP y seguridad. Investiga el contrato real antes de escribir el adaptador; no completes vacíos mediante suposiciones.

## Fuentes normativas y contexto obligatorio

Lee completamente antes de investigar o modificar código:

- `../exploracion/modernizacion-importacion-anexos-sii-enlase.md`.
- `../../ImportarServicioWeb/CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.
- El proveedor, registro, transporte, preview y telemetría modernos de `ImportarServicioWeb`.
- `workflow/Classselecciotarea.vb` para el inicio de actividades `ENLASE`.
- `ServiciosIntegracion/ClassAdjuntaDocumentoServicioIntegracion.vb`.
- `Integracionccv/ClassListaAnexosSII.vb`.
- `webservice/WebService_integracion_sii.asmx.vb`.
- La documentación y pruebas de DOC-50, DOC-51, DOC-69, DOC-72, DOC-73 y DOC-74.

La exploración es la base arquitectónica común de esta entrega. Usa expresamente sus secciones de conclusión arquitectónica, registro del servicio, contexto seguro, riesgos, matriz E2E y matriz de reutilización. Registra en los artefactos OPSXJ qué decisiones fueron aplicadas.

Si el contrato real, el código vigente o evidencia verificable contradicen la exploración, detén el punto afectado, registra el hallazgo mediante OPSXJ y actualiza la exploración antes de adoptar una arquitectura distinta. No sustituyas evidencia con supuestos ni cambies silenciosamente `ProviderId`, capacidad o fronteras de seguridad.

## Diagnóstico confirmado

- `ENLASE` abre un administrador documental antes de asignar la tarea.
- La consulta actual obtiene anexos desde la colección `imagenes` del radicado SII.
- Utiliza el mismo proveedor, credenciales y contrato de seguridad de `INTEGRACIONSII`.
- El servicio web y el contrato funcional difieren de la consulta de constancias e inscripciones.
- El flujo legacy depende de sesión mutable y devuelve estructuras orientadas a Bootstrap y cadenas legacy.

## Objetivo

Registrar y publicar la capacidad `ANEXOS_RADICADO_ENLASE` dentro del proveedor `INTEGRACIONSII`, implementar la consulta normalizada de anexos y ofrecer preview/descarga mediada sin mutación documental.

## Compuerta contractual obligatoria

Antes de implementar integración productiva, documenta con evidencia de código o contrato:

- Operación y endpoint reales de consulta.
- Operación y endpoint reales de obtención del contenido.
- Solicitud, respuesta y errores conocidos.
- Campo o composición que identifica establemente cada anexo.
- Tipo de contenido, nombre, tamaño y metadatos disponibles.
- Comportamiento cuando `imagenes` es nulo, vacío, incompleto o duplicado.

Si no puede demostrarse una identidad externa estable, detén la implementación productiva y registra el bloqueo OPSXJ. No uses índice de fila, URL temporal ni nombre de archivo como identidad sin prueba de estabilidad.

## Diseño obligatorio

```text
RegistroClientesProveedoresImportacion
                |
                v
        SiiImportProvider
        INTEGRACIONSII
                |
        +-------+-------+
        |               |
        v               v
 Constancias       Anexos ENLASE
 cliente actual    cliente específico
```

- Un solo `ProviderId`: `INTEGRACIONSII`.
- Capacidad nueva: `ANEXOS_RADICADO_ENLASE`.
- Autenticación, token, transporte y política de seguridad compartidos.
- DTO, mapper, validador y operaciones externas específicos para anexos.
- Telemetría diferenciada mediante nombres de operación, no mediante otro proveedor.

## Implementación obligatoria

- Extender el contrato de capacidades sin romper solicitudes existentes.
- Construir un cliente específico de anexos que no conozca sesión HTTP, SQL ni almacenamiento.
- Normalizar cada anexo como elemento externo estructurado.
- Reconstruir en servidor la tarea, actividad, ruta, trámite, recibo, código de barras y gabinete.
- Verificar que la tarea sea operable y que la actividad real sea `ENLASE`.
- Validar la configuración activa de `INTEGRACIONSII` para el trámite.
- Publicar consulta sin mutación mediante el Controller/ASMX moderno.
- Reutilizar el mecanismo moderno de descriptor y streaming para preview/descarga.
- Emitir errores públicos estables y mensajes seguros sin propagar excepciones crudas.
- Registrar telemetría como mínimo para consulta, preview y descarga de anexos.

## Restricciones críticas y antirregresión

- No crear otro proveedor, otra autenticación ni otra fila de servicio salvo evidencia nueva y aprobación expresa.
- No alterar el comportamiento de la capacidad de constancias e inscripciones.
- No aceptar URL externa, recibo, código de barras o gabinete del navegador como autoridad.
- No insertar una URL SII directamente en `iframe` ni transportar archivos como JSON/base64.
- No consultar ni modificar documentos, expedientes, índices o auditoría funcional durante `QueryItems` o preview.
- No modificar `ClassAlmacenamiento`, `AlmacenaDocumentoTareaWorkflow(...)` ni endpoints legacy en este prompt.
- No exponer secretos, tokens, rutas físicas, cadenas de conexión o respuestas SII crudas.
- No ejecutar pruebas reales contra SII sin autorización explícita.

## Matriz mínima de aceptación

| Caso | Resultado esperado |
|---|---|
| Tarea válida `ENLASE` | Capacidad habilitada |
| Actividad diferente | Rechazo seguro de contexto |
| Proveedor no configurado | Capacidad no disponible |
| Cero anexos | Respuesta vacía válida |
| Uno o varios anexos | Elementos normalizados con identidad estable |
| Respuesta SII inválida | Error seguro y trazable |
| Preview permitido | Descriptor temporal y contenido mediado |
| Descriptor vencido | Rechazo y posibilidad de solicitar uno nuevo |
| Consulta repetida | Sin mutación documental |
| Constancias existentes | Sin regresión funcional |

## CRITERIOS DE ACEPTACION

- `INTEGRACIONSII` publica `ANEXOS_RADICADO_ENLASE` sin registrar un proveedor o credenciales adicionales.
- El contexto se reconstruye desde la tarea seleccionada en servidor y rechaza una actividad distinta de `ENLASE` antes de llamar a SII.
- `consultarRadicado` produce cero, uno o varios elementos por `idanexo`; una identidad vacía o duplicada falla cerrada.
- El preview reconsulta por `idanexo`, crea un descriptor temporal y aplica pertenencia, expiración, tamaño, formato y allowlist de host.
- Consulta y preview no modifican tarea, documentos, expedientes, índices ni auditoría funcional.
- Compilación, pruebas focales y regresión completa de `ImportarServicioWeb` finalizan sin errores.
- El escenario E2E se entrega reutilizable y no se ejecuta sin autorización expresa de ambiente y cuenta.

## REGLAS DE ANTIRREGRESION

- Las solicitudes sin `Capability` conservan el despacho actual de constancias y sus claves `SII2`.
- `ResolveInscriptionsAsync`, preparación, intención, ejecución, reconciliación y almacenamiento mantienen su comportamiento previo.
- `ClassAlmacenamiento`, páginas Workflow y endpoints de integración legacy no se modifican.
- La capacidad desconocida falla cerrada; nunca cae implícitamente en constancias o anexos.
- El nuevo recorrido no acepta URL, recibo ni código de barras del navegador como autoridad.
- La regresión debe incluir todas las suites `tests/importar-servicio-web-*.test.cjs` y las pruebas de registro/perfil de `tools/e2e` afectadas.
## Pruebas obligatorias

- Unitarias del registro y despacho por capacidad.
- Unitarias del mapper y validador del contrato SII.
- Unitarias de reconstrucción y rechazo de contexto.
- Integración local del Controller/Service/Provider con fixtures deterministas y sin red.
- Seguridad del preview, expiración, formatos y hosts permitidos.
- Regresión del proveedor de constancias existente.

## EVIDENCIA DE COMPILACION

- Ejecutar MSBuild sobre `GestionDocumental-Docuarchi.net.vbproj` con el target `Compile`.
- Registrar resultado, configuración y advertencias preexistentes sin ocultar errores.
- Ejecutar `git diff --check` y validación OpenSpec estricta.
- No considerar suficiente una prueba textual de fuentes si la compilación VB.NET falla.
## Documentación técnica

Documenta exclusivamente en:

```text
Doc/Actualizacion/workflow/ImportarServiciWebEnlace/<TICKET>-capacidad-consulta-preview/
```

Incluye arquitectura, contrato externo confirmado, mapping, estados, amenazas, pruebas, diagramas, inventario de funciones y metadata OPSXJ.

## Entregable final

Entrega código, pruebas, documentación y evidencia coherentes. El prompt se completa únicamente si la consulta y preview son no mutadores, el proveedor existente no presenta regresión y OPSXJ valida el cambio.
