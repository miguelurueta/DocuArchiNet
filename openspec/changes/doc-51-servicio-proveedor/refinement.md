<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-51-servicio-proveedor

## Fuente y alcance

- Ticket: `DOC-51` — clientes HTTP asíncronos y seguridad.
- Dependencia verificada: contratos v1 de DOC-50 y `CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.
- Alcance: puerto asíncrono, transporte HTTP común, validación, errores seguros, fixtures, pruebas y documentación.
- Fuera de alcance: cliente SII concreto, endpoints ASMX, almacenamiento, ejecución por elementos, base de datos, PDF y sustitución del transporte legacy.

## Contexto inspeccionado

- `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`: puertos del núcleo; aún no existe el puerto HTTP asíncrono.
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`: ocho operaciones públicas v1 y preview mediado publicados por DOC-50.
- `Integracionccv/Class_ClassResfull.vb`: referencia legacy con `WebRequest`, formatos JSON/formulario y callback global de certificado; queda sin cambios.
- `GestionDocumental-Docuarchi.net.vbproj`: proyecto VB.NET Framework 4.6.1; los nuevos `.vb` requieren entradas `Compile` únicas.
- `Tests/importar-servicio-web-*.test.cjs`: pruebas focales CommonJS con `node:test` y fixtures locales.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/Exploracion/03-exploracion-backend-importar-servicio-web.md`: hallazgos B-04, B-05 y B-06 y preguntas 1 y 9.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Declarar `IExternalImportProviderClient` asíncrono y cancelable en el archivo de puertos existente, sin duplicarlo en infraestructura ni cambiar DTO públicos. | `ImportarServicioWebInterfaces.vb`; DTO v1 de DOC-50 | D-01 | RQ-01 | 1.1, 1.2 |
| D-02 | Representar cada solicitud HTTP mediante configuración inmutable por operación, conservando verbo, encabezados, JSON/formulario y serialización aportados por el adaptador futuro. | formatos de `Class_ClassResfull.vb`; B-04/B-05 | D-02 | RQ-02 | 2.1, 2.2 |
| D-03 | Crear clientes con configuración estable y validación TLS del sistema; se prohíben mutaciones por solicitud y callbacks globales permisivos. | `Class_ClassResfull.vb`; B-05/B-06 | D-03 | RQ-03 | 2.3, 5.2 |
| D-04 | Aplicar token del llamador y timeout por operación mediante tokens enlazados, distinguiendo cancelación solicitada de expiración. | contrato del ticket; pregunta 9 | D-04 | RQ-04 | 3.1, 5.3 |
| D-05 | Validar estado, tipo y tamaño declarado y realmente leído antes de deserializar; la descarga se lee con límite y cancelación. | preview mediado compartido | D-05 | RQ-05 | 3.2, 3.3, 5.1 |
| D-06 | Mapear acceso denegado, timeout, cancelación, indisponibilidad y respuesta inválida a resultados tipados con mensajes saneados y `correlationId`. | aceptación DOC-51 | D-06 | RQ-06 | 3.4, 5.2 |
| D-07 | Mantener `GetPreview` mediado: ningún resultado público contiene URL externa, token, ruta física, payload ni respuesta cruda. | contrato compartido, sección 6 | D-07 | RQ-07 | 1.2, 3.3, 5.2 |
| D-08 | Validar sin SII mediante fixtures saneados y servidor loopback; documentar límites configurables y diferir ASMX/cliente SII. | patrón `node:test`; restricciones DOC-51 | D-08 | RQ-08 | 4.1-4.5, 6.1-6.4 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Puerto con capacidades, consulta, preview y descarga que retorna `Task` y recibe `CancellationToken`. | Firmas asíncronas, tipadas y declaradas una sola vez en Modelo. | El puerto síncrono vigente permanece intacto. |
| RQ-02 | El transporte envía verbo HTTP, contenido y encabezados de la solicitud preparada. | JSON continúa JSON y formulario continúa formulario. | Evita romper proveedores por normalización implícita. |
| RQ-03 | La configuración compartida no cambia durante una solicitud y TLS usa validación predeterminada. | No hay escritura a defaults ni callback global. | Evita carreras y certificados aceptados indiscriminadamente. |
| RQ-04 | Cancelación del llamador y timeout producen códigos distintos. | Cancelar retorna cancelación; expirar sin cancelación retorna timeout. | Reduce ambigüedad operativa. |
| RQ-05 | Estado, MIME, JSON inválido y exceso de tamaño se rechazan antes de uso. | El límite aplica sin `Content-Length` o con longitud falsa. | Previene consumo no acotado y datos corruptos. |
| RQ-06 | Errores y trazas solo exponen código seguro y `correlationId`. | No aparecen token, URL sensible, ruta física o cuerpo crudo. | Evita fuga de secretos y datos personales. |
| RQ-07 | Preview conserva el descriptor mediado v1. | Se reciben metadatos autorizados, nunca autoridad externa. | Preserva DOC-50. |
| RQ-08 | Pruebas focales usan loopback y fixtures del repo. | Los siete escenarios no acceden a red externa. | No requiere cuentas ni gate. |

## Resultado del refinamiento

- Estado: aprobado para implementación.
- Ruta documental: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-51-clientes-http-asincronos-seguridad/`.
- Timeout, tamaño y MIME serán opciones explícitas por operación; no se inventan valores productivos SII.
