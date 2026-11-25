# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: implementación local: 40/44; aceptación UI real y cierre abiertos.

## Evidencia requerida

150/150 pruebas locales y4505 comprobaciones nativas sintéticas: DOC122: 259 / DOC123: 318 / DOC124: 492 / DOC125: 3436; regresión heredada PASS. Build net461 aislado0 errores/308 advertencias previas, sin cambio de DLL desplegada/Web.config. Incluye contrato/API, modos UI, CAS, duplicación, incertidumbre, storage, permisos, XSS/precisión, foco/zoom y coexistencia de menú. Comandos/hashes/alcance en Validacion/doc126-implementacion-local-20261124.md/JSON.

## QA/E2E WebForms

Dos tests DOC126 registrados en importacion-excel.spec.cjs mediante flags del runner DOC110, sin ejecución real. Control SELECT DOC32 extendido a RA y huella de DSN opcional; actor DOC125 acepta sólo el scope/autorización DOC126 explícitos para esa fase. Libros sintéticos: 1000/1000/1000/300/10leídos completos con lector real; cero escrituras en DB. SQL 22–25 entregados y no aplicados. Quedan 6.9/6.10/6.11 y 7.5: nueva autorización/cuentas/fixtures, aceptación física y revisión final. Mocks y UI local no son evidencia de IIS/MySQL.

## Corte actual: asistente desde Excel

Ampliación D-16/RQ-16 implementada localmente: 47/52 tareas, 160/160 pruebas y build sin errores. Análisis independiente sin plantilla ni efectos durables; propuesta editable, clave explícita y File sólo en memoria hasta recepción habitual con contraste de hash. No requiere migración SQL y preserva los contratos de carga. Evidencia actual y límites en `Doc/Actualizacion/Importacion/Validacion/doc126-asistente-20261124.md`. 8.8 y las cuatro tareas anteriores de aceptación/gobierno siguen abiertas; sin despliegue ni E2E real. Los números anteriores pertenecen al corte inicial.

## Despliegue y preparación posteriores

DLL validada desplegada en IIS local con respaldo, WSDL200, rechazo anónimo del analizador y Web.config intacto. Se amplió la E2E existente con modo de asistente y autorización propia; 39/39 pruebas locales afectadas, tres casos registrados, ninguno ejecutado realmente. 47/52 tareas; pendientes cuentas, alcances y aceptación/gobierno. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-despliegue-e2e-preparada-20261124.md`.

## Corrección de creación observada en video

Destino sugerido y editable desde el nombre del Excel, mensajes de tabla/clave con foco y acción Quitar legible. Sólo frontend/ASPX, sin SQL o reemplazo de DLL. 55/55 pruebas locales y assets v3 contrastados por HTTP sin sesión. 48/53 tareas; aceptación real pendiente. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-correccion-video-crear-plantilla-20261124.md`.

## Diagnóstico explicativo posterior — D-17/RQ-17

Metadatos SELECT acreditan opciones obligatorias; campos nuevos usan ceros explícitos, históricos conservan NULL. Validación por campo/fila/regla/límite/tipo y panel de causa/corrección con foco; hasta25 y total, sin valores de celda o textos del proveedor. Respuesta aditiva, códigos/solicitudes conservados y matriz verificada antes de escribir. Sin regla acreditada, no se inventa causa o resultado y no se repite el alta. 171/171 regresión y 4603 checks sintéticos; DLL/assets v4 publicados con respaldo y salud sin sesión. 52/57 tareas; aceptación/gobierno pendientes. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-diagnostico-configuracion-20261124.md`.

## Guía por semáforo — 2026-11-24, video 191328

56/61 tareas completas; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Regresión 178/178 y comprobación final de UI 39/39; 4603 checks nativos sintéticos. Assets doc126-5 contrastados por HTTP anónimo, sin cambiar DLL, SQL ni ejecutar E2E autenticada. La misma plantilla se reutiliza: Nueva importación consulta CREAR/AGREGAR vigente; Cambiar operación edita el mismo ID con CAS y guardado explícito. Reemplazo conserva consentimiento. Colores con texto y siguiente acción, sin confirmación automática. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-semaforo-reutilizacion-20261124.md`.

## Recorrido funcional con cuenta existente — 2026-11-24

57/62 tareas: completada la adaptación 6.12; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Usuario 141 activo y autorizado con sus cinco permisos vigentes, conservados. Modo funcional de agregar/reemplazar separado de exclusividad/restricción; 88/88 pruebas locales y cuatro E2E registradas, sin ejecutar la nueva ventana real. Preflight SELECT: faltan tablas, políticas y plantillas DOC126. Copias SQL23–25 parametrizadas en .tmp/doc126-functional-preparacion, sin aplicar. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-cuenta-existente-funcional-20261124.md`.
