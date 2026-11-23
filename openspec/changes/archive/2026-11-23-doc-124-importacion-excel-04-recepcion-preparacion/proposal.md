## Why

DOC122 entrega autorización/plantillas y DOC123 un lector comprobado, pero todavía no existe una recepción web privada ni una preparación durable que valide el archivo completo y la unicidad con la semántica de MySQL antes de escribir datos.

## What Changes

- Añadir emisión de referencia de recepción, upload ASHX privado y consulta propia de cargas.
- Implementar Preparar sobre lector DOC123, metadatos reales, aislamiento empresarial e índices completos.
- Persistir RECIBIDA/PREPARADA, huellas y conteos en imp01_carga_ejecucion; comparar claves con temporales de la conexión.
- Reutilizar harness y E2E existentes, incluyendo prueba real autorizada del recorrido de servicios sin mutación del destino.

## Capabilities

### New Capabilities

- `importacion-excel-04-recepcion-preparacion`: recepción identificada, archivo privado inmutable, validación integral y preparación persistida para confirmar posteriormente.

### Modified Capabilities

Ninguna. Se amplían servicios/DTO existentes sin redefinir autorización DOC122, hash de plantilla v1 ni resultados del lector DOC123.

## Impact

Modelos/DTO Excel, guarda multipart, almacenamiento privado, repositorio de ejecuciones/metadatos, servicio de recepción/preparación y ASHX/ASMX; registro en vbproj, pruebas y documentación. No confirma, crea o reemplaza tablas de negocio, activa fuentes/proveedores ni añade UI. Permanece net461, MySQL5.1.37 y el conector vigente; no se introduce otra cuenta o conexión en Web.config.

## Fuente y continuidad

Ticket DOC124 y prompt04 en Doc/Actualizacion/Importacion/Exploracion/04-recepcion-y-preparacion.md. Contexto original conservado en specs/importacion-excel-04-recepcion-preparacion/jira-context.md. DOC123 cerrado87d155b7 es base; se conserva su lector con fórmulas sin recalcular y límites10000/10MB decimal.

La cuenta dedicada del antecedente fue sustituida por decisión del usuario en DOC122. SQL permanente siempre se entrega para ejecución manual. Autorizaciones E2E anteriores cerradas; esta planificación no autoriza pruebas reales ni despliegue.
