# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: implementación local: 40/44; aceptación UI real y cierre abiertos.

## Objetivo

Menú y diálogo Excel conectados con los servicios DOC122–125, con permiso propio, editor de plantillas, archivo/hoja, validación completa, consentimiento y recuperación por ID.15 decisiones/15 requisitos/45 escenarios trazados.

## Alcance y compatibilidad

Se preservan plataforma, servicios, transacciones,20k/10MB y consumidores existentes. Tres assets Content y helper contextual en guarda actual; sin nuevas conexiones.150/150 pruebas locales,4505 comprobaciones sintéticos y build0 errores. Evidencia y límites en Doc/Actualizacion/Importacion/Validacion/doc126-implementacion-local-20261124.md/JSON. E2E real, aplicación SQL 22–25 y despliegue no ejecutados; no se presentan como terminados.

## Corte actual: asistente desde Excel

Ampliación D-16/RQ-16 implementada localmente: 47/52 tareas, 160/160 pruebas y build sin errores. Análisis independiente sin plantilla ni efectos durables; propuesta editable, clave explícita y File sólo en memoria hasta recepción habitual con contraste de hash. No requiere migración SQL y preserva los contratos de carga. Evidencia actual y límites en `Doc/Actualizacion/Importacion/Validacion/doc126-asistente-20261124.md`. 8.8 y las cuatro tareas anteriores de aceptación/gobierno siguen abiertas; sin despliegue ni E2E real. Los números anteriores pertenecen al corte inicial.

## Despliegue y preparación posteriores

DLL validada desplegada en IIS local con respaldo, WSDL200, rechazo anónimo del analizador y Web.config intacto. Se amplió la E2E existente con modo de asistente y autorización propia; 39/39 pruebas locales afectadas, tres casos registrados, ninguno ejecutado realmente. 47/52 tareas; pendientes cuentas, alcances y aceptación/gobierno. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-despliegue-e2e-preparada-20261124.md`.

## Diagnóstico explicativo posterior — D-17/RQ-17

Metadatos SELECT acreditan opciones obligatorias; campos nuevos usan ceros explícitos, históricos conservan NULL. Validación por campo/fila/regla/límite/tipo y panel de causa/corrección con foco; hasta25 y total, sin valores de celda o textos del proveedor. Respuesta aditiva, códigos/solicitudes conservados y matriz verificada antes de escribir. Sin regla acreditada, no se inventa causa o resultado y no se repite el alta. 171/171 regresión y 4603 checks sintéticos; DLL/assets v4 publicados con respaldo y salud sin sesión. 52/57 tareas; aceptación/gobierno pendientes. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-diagnostico-configuracion-20261124.md`.
