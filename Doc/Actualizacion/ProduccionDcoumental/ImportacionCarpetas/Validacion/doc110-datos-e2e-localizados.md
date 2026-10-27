# DOC-110 — Datos E2E localizados

Inspección: 2026-10-20. El usuario autorizó el ambiente y solicitó buscar la configuración en la infraestructura E2E existente. No se ejecutó login ni escritura de perfiles.

| Dato | Evidencia en el repositorio | Resultado |
|---|---|---|
| Ambiente de Producción | `tools/e2e/profiles/doc88-production-document-upload.local.runtime.json` y variante `execution` | `http://localhost/GestionDocumental-Docuarchi.net/`; ambiente declarado `pruebas-locales-autorizadas`. |
| Módulo de login | Mismos perfiles | `GESTOR`. |
| Disponibilidad | GET anónimo a `gestor.aspx` | HTTP 200; presentes selector de módulo y entrada de usuario. No acredita autenticación ni permiso DOC-110. |
| DSN de control existente | Mismos perfiles | `MembershipUsers`. El nombre no acredita ubicación de tablas RA ni permisos de solo lectura; verificar antes de usarlo como control DOC-110. |
| Captura de cuentas | `scripts/support/interactive-e2e-console.cjs`, `collectValue` y `promptSecret`; consumidor `run-doc105-pdf-cross-module-interactive.cjs` | Cuenta y contraseña se capturan por consola. No se persisten en el perfil. |
| Login reutilizable | `tests/support/authenticated-workflow-session.cjs` | Formulario real `gestor.aspx`; credenciales efímeras suministradas por el llamador. |
| Perfiles inspeccionados | 97 JSON en `tools/e2e/profiles`, incluidos archivos locales ignorados | Sin claves pobladas de contraseña/credencial/secreto/username/authorizedUser/mysqlUrl detectadas. No equivale a auditoría exhaustiva de secretos del repositorio. |
| Configuración efímera | Nombres de variables E2E del proceso; DOC110 también revisado en usuario/máquina | No disponible en esta sesión. No se imprimieron valores. |
| Datos DOC-88 | Perfil de carga individual | Referencias de expediente, tipología y archivo de prueba; no constituyen configuración y reglas completas del perfil DOC-110. |
| Fixture DOC-110 | Perfiles existentes y contrato de la suite | No se encontró perfil DOC-110. Se necesitan configuración y reglas válidas para la cuenta, más dos ejemplos con resultados esperados. |

## Diferencia pendiente de integración del harness

La suite DOC-110 reutiliza Playwright y login, pero hoy espera `DOC110_E2E_MYSQL_URL`; la plataforma existente captura acceso de control mediante DSN y consola. La URL MySQL no puede deducirse de un DSN ni sustituirse automáticamente por credenciales operativas del sitio. Falta conectar el mecanismo de captura/control existente a esta suite para evitar una configuración paralela.

El ambiente queda identificado y accesible. Para ejecutar los casos reales faltan las cuentas administrativa y restringida suministradas por el canal efímero existente, verificar su capacidad vigente y preparar el fixture con catálogos autorizados. No extraer contraseñas de usuarios desde la base de datos ni conceder permisos automáticamente.
