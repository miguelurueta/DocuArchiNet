# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado:58/62; agregar/reemplazar y rollback/concurrencia reales aprobados; aceptación restante y cierre abiertos.

## Flujo

GET renderiza host cuando Resolver acredita permiso propio. No emite CSRF desde GET. AbrirCargaExcel POST obtiene el token; la cabecera de namespace se contrasta con el contexto vigente y los consumidores anteriores sin ella mantienen contrato. Menú y montaje idempotentes conservan módulos vecinos. El cliente procesa .d/datos, conserva incidencias de ok=false y usa FormData del navegador. Estado de UI no añade estados DB. GuardarPlantilla/Preparar conservan CAS; Confirmar congela hashes y pide consentimiento; consultar no muta.

## Riesgos

Cerrar/timeout no cancelan ni prueban rollback; el ID se guarda antes de subir/confirmar. Los callbacks obsoletos se descartan y no reintentan. Referencias sin secretos y aviso si storage falla. Reemplazo no inventa filas borradas o aislamiento por prefijo. El renderer opcional permite Content con DLL anterior hasta el despliegue compatible. Build y browser locales no acreditan E2E física ni capacidad de 20000; ver evidencia y SQL manual.

## Corte actual: asistente desde Excel

Ampliación D-16/RQ-16 implementada localmente: 47/52 tareas, 160/160 pruebas y build sin errores. Análisis independiente sin plantilla ni efectos durables; propuesta editable, clave explícita y File sólo en memoria hasta recepción habitual con contraste de hash. No requiere migración SQL y preserva los contratos de carga. Evidencia actual y límites en `Doc/Actualizacion/Importacion/Validacion/doc126-asistente-20261124.md`. 8.8 y las cuatro tareas anteriores de aceptación/gobierno siguen abiertas; sin despliegue ni E2E real. Los números anteriores pertenecen al corte inicial.

## Diagnóstico explicativo posterior — D-17/RQ-17

Metadatos SELECT acreditan opciones obligatorias; campos nuevos usan ceros explícitos, históricos conservan NULL. Validación por campo/fila/regla/límite/tipo y panel de causa/corrección con foco; hasta25 y total, sin valores de celda o textos del proveedor. Respuesta aditiva, códigos/solicitudes conservados y matriz verificada antes de escribir. Sin regla acreditada, no se inventa causa o resultado y no se repite el alta. 171/171 regresión y 4603 checks sintéticos; DLL/assets v4 publicados con respaldo y salud sin sesión. 52/57 tareas; aceptación/gobierno pendientes. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-diagnostico-configuracion-20261124.md`.

## Guía por semáforo — 2026-11-24, video 191328

56/61 tareas completas; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Regresión 178/178 y comprobación final de UI 39/39; 4603 checks nativos sintéticos. Assets doc126-5 contrastados por HTTP anónimo, sin cambiar DLL, SQL ni ejecutar E2E autenticada. La misma plantilla se reutiliza: Nueva importación consulta CREAR/AGREGAR vigente; Cambiar operación edita el mismo ID con CAS y guardado explícito. Reemplazo conserva consentimiento. Colores con texto y siguiente acción, sin confirmación automática. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-semaforo-reutilizacion-20261124.md`.

## Aceptación funcional real con la cuenta existente — 2026-11-24

E2E Playwright real PASS: un test, dos casos. Agregar1000:1→1001 filas; reemplazar1000:2→1000 filas, con consentimiento. Usuario141 en IIS local HTTPS/GESTOR, plantillas58/59 y hashes/propiedad/locks verificados. Huellas íntegras de27 tablas protegidas y configuración de plantillas sin cambios. Perfil local separado; perfil compartido, permisos, DLL y Web.config conservados. La creación/importación manual del usuario se reconoce como realizada. No acredita exclusividad/restricción, rollback/carrera o asistente automatizado. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-funcional-real-20261124.md`.

## Rollback y concurrencia reales — 2026-11-24

6.11 completada:58/62 tareas. E2E real PASS, dos casos: rollback conserva íntegramente1000 filas; concurrencia conserva las1001 anteriores y sólo una fila externa adicional (total1002), con cero filas propias confirmadas. Actor observó INSERT bloqueado y locks activos antes de confirmar.27 tablas protegidas, configuración y dos resultados positivos previos intactos; locks liberados.41/41 pruebas Node y4607 checks sintéticos. Sin instalación/limpieza/permisos nuevos. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-rollback-concurrencia-20261124.md`.
