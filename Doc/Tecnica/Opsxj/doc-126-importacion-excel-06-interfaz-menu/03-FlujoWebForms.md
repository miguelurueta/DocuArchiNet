# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: implementación local: 40/44; aceptación UI real y cierre abiertos.

## Flujo

GET renderiza host cuando Resolver acredita permiso propio. No emite CSRF desde GET. AbrirCargaExcel POST obtiene el token; la cabecera de namespace se contrasta con el contexto vigente y los consumidores anteriores sin ella mantienen contrato. Menú y montaje idempotentes conservan módulos vecinos. El cliente procesa .d/datos, conserva incidencias de ok=false y usa FormData del navegador. Estado de UI no añade estados DB. GuardarPlantilla/Preparar conservan CAS; Confirmar congela hashes y pide consentimiento; consultar no muta.

## Riesgos

Cerrar/timeout no cancelan ni prueban rollback; el ID se guarda antes de subir/confirmar. Los callbacks obsoletos se descartan y no reintentan. Referencias sin secretos y aviso si storage falla. Reemplazo no inventa filas borradas o aislamiento por prefijo. El renderer opcional permite Content con DLL anterior hasta el despliegue compatible. Build y browser locales no acreditan E2E física ni capacidad de 20000; ver evidencia y SQL manual.

## Corte actual: asistente desde Excel

Ampliación D-16/RQ-16 implementada localmente: 47/52 tareas, 160/160 pruebas y build sin errores. Análisis independiente sin plantilla ni efectos durables; propuesta editable, clave explícita y File sólo en memoria hasta recepción habitual con contraste de hash. No requiere migración SQL y preserva los contratos de carga. Evidencia actual y límites en `Doc/Actualizacion/Importacion/Validacion/doc126-asistente-20261124.md`. 8.8 y las cuatro tareas anteriores de aceptación/gobierno siguen abiertas; sin despliegue ni E2E real. Los números anteriores pertenecen al corte inicial.
