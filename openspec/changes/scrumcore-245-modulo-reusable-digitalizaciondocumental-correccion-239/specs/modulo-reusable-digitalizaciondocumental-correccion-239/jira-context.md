# Jira Context - SCRUMCORE-245

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL - CORRECCION SCRUMCORE-239

## Description

Corregir `SCRUMCORE-239` para que la migracion de `DigitalizacionDocumental` tenga trazabilidad real contra el legacy y un componente React corporativo reutilizable.

La correccion debe:

- analizar explicitamente los archivos legacy nombrados;
- extraer reglas reales de negocio y flujo;
- documentar equivalencias legacy -> React/API;
- identificar brechas entre `SCRUMCORE-239`, `SCRUMCORE-240`, `SCRUMCORE-241` y `SCRUMCORE-242`;
- implementar brechas frontend viables en este repo;
- dejar como pendiente backend lo que dependa de APIs no implementadas.

## Archivos legacy validados

Todos los archivos obligatorios fueron encontrados en:

```txt
C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner
```

Archivos:

```txt
online_demo_initpage.js
online_demo_operation.js
WebFormEscan.js
WebFormEscan.aspx
WebFormEscan.aspx.designer.vb
WebFormEscan.aspx.vb
Webform_save_digital_image.aspx
Webform_save_digital_image.aspx.designer.vb
Webform_save_digital_image.aspx.vb
```

## Entregables

- Matriz de trazabilidad legacy en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-239-legacy-traceability.md`.
- Componente inline reusable `DigitalizacionDocumentalWorkspace`.
- Wrapper modal compatible `DigitalizacionDocumentalModal`.
- Pruebas que confirmen render inline sin `AppModal` y compatibilidad del modal.

## Restricciones

- No reutilizar WebForms.
- No usar jQuery.
- No acceder directamente a `DWObject` desde componentes React.
- No usar `Session` como contrato funcional.
- No mantener TIF/JPG/BMP como salida final.
- La salida final debe ser PDF.
- No asumir reglas legacy sin trazarlas al archivo/funcion donde se originan.
