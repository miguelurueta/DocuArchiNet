# Arquitectura

- Ticket: DOC-79
- Cambio OpenSpec: doc-79-pruebas-retiro-gate
- Clasificacion: cross_cutting

## Objetivo

Validar la experiencia moderna sin retirar el fallback legacy y sin crear infraestructura paralela.

## Componentes

- `ImportarServicioWebFeatureGate`: exige bandera activa y una sesión Workflow válida; no limita por listas adicionales.
- `WebServiceImportarServicioWebModern`: aplica el gate antes de validar solicitudes o resolver dependencias.
- `Webworkflow.aspx(.vb)`: registra assets/bootstrap modernos para cualquier sesión Workflow válida cuando el gate está activo.
- `importar-servicio-web-ui.js`: enlaza una sola entrada moderna y oculta reversiblemente las raíces legacy declaradas.
- `Verify-ImportarServicioWebFrontend.ps1`: compone pruebas Node locales, sin autenticación ni red.

El modo oficial de otras operaciones Workflow no se modifica.
