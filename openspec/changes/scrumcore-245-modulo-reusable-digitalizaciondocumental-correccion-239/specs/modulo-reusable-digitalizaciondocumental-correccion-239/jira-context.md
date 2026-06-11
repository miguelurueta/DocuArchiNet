# Jira Context - SCRUMCORE-245

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- CORRECCION-239

## Description

> Corrección SCRUMCORE-239 - Migración real de DigitalizacionDocumental desde legacy
> Objetivo
>   Auditar y corregir SCRUMCORE-239, porque la ejecución actual pudo haber quedado como una base React parcial sin trazabilidad suficiente  frente al legacy.
>   La corrección debe:
> analizar explícitamente los archivos legacy nombrados;
> 
> extraer reglas reales de negocio y flujo;
> 
> documentar equivalencias legacy -> React/API;
> 
> identificar brechas entre SCRUMCORE-239, SCRUMCORE-240, SCRUMCORE-241 y SCRUMCORE-242;
> 
> implementar solo las brechas frontend viables en este repo;
> 
> dejar como bloqueo real todo lo que requiera backend inexistente o archivos legacy inaccesibles.
> 
> Archivos legacy obligatorios a analizar
>   Antes de implementar cambios, leer y documentar hallazgos de estos archivos si están disponibles:
> C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_initpage.js
> C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_operation.js
> C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.js
> D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx
> D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx.vb
> D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx
> D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx.vb
> 
> También buscar y analizar archivos relacionados invocados por estos:
> 
> - clases VB;
> - webservices;
> - scripts auxiliares;
> - handlers;
> - endpoints .aspx;
> - funciones globales JS;
> - controles ASP.NET;
> - servicios de metadata.
> 
> Si algún archivo no es accesible desde el workspace o permisos actuales, documentar:
> 
> - ruta;
> - motivo;
> - impacto;
> - qué decisión queda bloqueada.
> 
> ## Entregable 1: matriz de trazabilidad legacy
> 
> Crear:
> 
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-239-legacy-traceability.md
> 
> Debe incluir una tabla con:
> 
> Archivo legacy | Funcion/metodo/evento | Responsabilidad | Regla funcional | Nueva ubicacion React/API | Estado | Evidencia
> 
> Estados permitidos:
> 
> - migrado
> - reemplazado por API
> - descartado por decision
> - pendiente backend
> - pendiente frontend
> - requiere confirmacion
> - bloqueado por archivo inaccesible
> 
> No marcar responsabilidades criticas como completadas sin evidencia concreta.
> 
> ## Entregable 2: decisiones de migracion
> 
> Documentar explicitamente:
> 
> - Que hace online_demo_initpage.js.
> - Que hace online_demo_operation.js.
> - Que hace WebFormEscan.js.
> - Que hace WebFormEscan.aspx.
> - Que hace WebFormEscan.aspx.vb.
> - Que hace Webform_save_digital_image.aspx.
> - Que hace Webform_save_digital_image.aspx.vb.
> - Que partes se migran a React.
> - Que partes se reemplazan por API.
> - Que partes se descartan y por que.
> - Que reglas no pueden implementarse sin backend.
> 
> ## Entregable 3: gap analysis contra implementacion actual
> 
> Comparar contra:
> 
> src/modules/digitalizacion/
> docs/Architecture/DigitalizacionDocumental/
> 
> Determinar que ya existe, que esta incompleto y que falta para cumplir SCRUMCORE-239.
> 
> Revisar especialmente:
> 
> SCRUMCORE-240: adapter Dynamsoft PDF-only
> SCRUMCORE-241: workbench grafico de captura
> SCRUMCORE-242: integracion frontend con APIs modernas
> 
> ## Alcance funcional minimo esperado
> 
> Cubrir o documentar estado de:
> 
> 1. Adapter React para Dynamsoft.
> 2. Carga controlada de scripts/runtime Dynamsoft.
> 3. Inicializacion de licencia/runtime.
> 4. Listado de scanners.
> 5. Seleccion de scanner.
> 6. Captura de paginas.
> 7. Preview/miniaturas.
> 8. Rotacion basica.
> 9. Eliminacion de paginas.
> 10. Generacion exclusiva de PDF.
> 11. Upload temporal moderno.
> 12. Resolucion de metadata/lista de chequeo mediante API.
> 13. Crear documento digitalizado nuevo.
> 14. Adjuntar digitalizacion a PDF existente.
> 15. Validar bloqueo/firma/radicado no modificable antes de adjuntar.
> 16. Manejo de errores equivalente al legacy sin Session, jQuery ni WebForms.
> 17. Pruebas unitarias/integracion focales.
> 
> ## Restricciones
> 
> - No reutilizar WebForms.
> - No usar jQuery.
> - No acceder directamente a DWObject desde componentes React.
> - No usar Session como contrato funcional.
> - No mantener TIF/JPG/BMP como salida final.
> - La salida final debe ser PDF.
> - No asumir reglas legacy sin trazarlas al archivo/funcion donde se originan.
> - No cerrar brechas con placeholders.
> - No modificar backend si este repo es solo frontend; documentar contratos/bloqueos.
> 
> ## Implementacion esperada en frontend
> 
> Respetar la estructura existente bajo:
> 
> src/modules/digitalizacion/
> 
> Si faltan piezas, agregarlas siguiendo el patron actual del modulo.
> 
> ## Validacion requerida
> 
> Ejecutar y documentar resultado:
> 
> npx vitest run src/modules/digitalizacion
> npm run build
> 
> Si falla por ambiente/dependencias, documentar comando, error y causa probable.
> 
> Si npm run build falla por errores fuera del alcance, documentar archivo, línea y motivo.
> 
> Este prompt deja claro que primero hay que leer los legacy, luego mapearlos, y recién después implementar.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CORRECCION, DIGITALIZACIONDOCUMENTAL, MODULOS, REUSABLE
