# DOC-116 — 05-PruebasEvidencia

- Ticket: DOC-116
- Cambio OpenSpec: doc-116-produccion-importacion-carga-carpeta-expediente
- Clasificacion: cross_cutting

## Evidencia requerida

Matriz canónica: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-expedientes-matriz.md. Manifiesto y verificación: doc116-verificacion-final-20261105.json y doc116-verificacion-final-20261105.md en el mismo directorio. Cada resultado real tiene código/recursos/backend/checkpoint por SHA; el manifiesto agrega SHA de los JSON y compara la línea base local protegida.

## QA/E2E WebForms

EXP01 PASS (plan 32/lote 16, 657/658), EXP02 cliente PASS parcial (33/17, 659) y EXP03 PASS (34/18, cero creaciones; fila 629 restaurada). Todos conservaron huellas de recursos previos; seis PDF de 677 bytes quedan en staging, sin documentos definitivos nuevos. Diario EXP03 RESTORED, originalPermission=1; cero cambios permanentes al permiso.

95/95 pruebas Node seleccionadas PASS previas, Framework 2/2 PASS previo reutilizado. No se repiten pruebas reales por consolidar documentación. No se almacenan credenciales, cookies ni cadenas de conexión en evidencia.

Hallazgo DOC116-VERIFY-01 resuelto por ejecución autenticada de las dos ventanas internas de laboratorio. Conservadas la incertidumbre del intento 41 y la distinción entre candidata y runtime original restaurado. Cierre formal, publicación y Jira pendientes de integración.

## Estado vigente — EXP02 interno aceptado

Ambas ventanas internas de laboratorio autorizadas PASS: raíz 29/intento 40/expediente 660/relación 1085 con checkpoint y replay; raíz 30/intento 41/expediente 661/relación 1086 con incertidumbre conservada sin recreación. DLL original restaurada y dos copias iguales byte por byte, diario RESTORED. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp02-interno-resultados.md`. Brecha funcional resuelta; pendientes entrega/publicación y revisión formal. El intento 41 no se reconcilió. No repetir.


## Estado vigente para entrega

Escenarios aceptados: EXP01 destino fijo y replay, EXP02 cliente y dos ventanas internas autenticadas de laboratorio, EXP03 autoridad y restauración. DLL original restaurada; dos respaldos verificados byte por byte. El intento 41 del expediente 661 permanece deliberadamente EN_CURSO/public INCIERTO sin recreación ni reconciliación manual.

20/20 tareas completas; suite de entrega aislada 130/130 PASS y prueba principal de restauración 9/9 PASS tras ajuste de portabilidad. Los conteos de las dos copias no se suman. Los informes previos son históricos con sus SHA propios. Quedan publicación/revisión del PR, sincronización/archivo OpenSpec y transición de Jira tras integración. Informe canónico: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-entrega-20261105.md.
