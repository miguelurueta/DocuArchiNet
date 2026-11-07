# DOC-117 — Preparación local ALM-02, 2026-11-06

Resultado: candidata desactivada compilada y soporte local verificado. **No acredita ALM-02 real ni autoriza su instalación.**

`build-doc117-alm02-dormant.cjs` compila con `Compile` a un directorio nuevo bajo `.tmp`, reemplazando únicamente la composición de autoridad/checkpoint del adaptador documental en una copia. El generador conserva la invocación compartida original y falla si los puntos de sustitución cambiaron. El helper de laboratorio permanece fuera del proyecto productivo y no consume banderas, headers, endpoints ni autorización de DOC-116.

La candidata tiene IDs de lote/raíz/archivo/intento inválidos: su guard rechaza toda llamada antes del escritor. **No instalarla.** Una futura variante con recursos reales requiere revisión propia; este generador solo construye la variante desactivada.

Compilación ejecutada:

```powershell
node tools/e2e/scripts/build-doc117-alm02-dormant.cjs .tmp/doc117-alm02-dormant-20261106
```

Salida: `COMPILED_DORMANT_CANDIDATE`, cero errores. La DLL del sitio y los 912 archivos de entrada `Compile`/`EmbeddedResource` y proyecto conservaron sus hashes. Huellas de entradas en `.tmp/doc117-alm02-dormant-20261106/source-inputs.json`; resultado en `build-result.json`. Los hashes se preservan también en el JSON de este reporte.

Validación local:

| Corrida | Resultado | Alcance |
| --- | --- | --- |
| `node --test tests/doc117-alm02-candidate.test.cjs` | 2/2 PASS | Generador y helper VB compilado. Copias desechables con IDs sintéticos y repositorio doble. Sin escritor real. |
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc117-almacenamiento.test.cjs tests/doc117-alm01-driver.test.cjs` | 24/24 PASS | Harness .NET y soporte DOC-117 existentes; PDF local, SQL/HTTP/persistencia/escritor dobles. |
| `openspec.cmd validate doc-117-produccion-importacion-carga-carpeta-almacenamiento --strict` | PASS | Estructura documental; no acredita ejecución real ni aprobación técnica OPSXJ. |

En los dobles del helper se comprueban: rechazo desactivado sin checkpoint/marcadores; rechazo de actor/empresa/nivel/versión/gabinete/raíz/archivo/intento/fase ajenos; rechazo de IDs incompletos o retorno discordante; A delega una vez al checkpoint confirmado y falla antes de retornar; B falla después de la verificación sin delegar al checkpoint; ambos conservan los IDs observados y rechazan otra activación del mismo marcador. Los tests del coordinador existentes cubren consulta durable/replay sin otro escritor. Nada de ello convierte la inyección real en ejecutada.

La tarea 16.1 sigue pendiente. Antes de instalar una futura variante faltan recursos reales autorizados, preflight SELECT, cierre del alcance y correlación exacta del intento, candidata específica revisada y prueba DOC-117 de respaldo/restauración con guardián independiente. No se solicitó ni usó una cuenta ni se abrió sesión real. No se modificó el runtime para conseguir identidades.

El siguiente paso revisable está en `doc117-alm02-preflight-propuesto-20261106.json`: máximo ocho SELECT por los DSN RA/DA existentes, sin login de aplicación, efectos ni instalación. Ambiente propuesto `https://localhost/GestionDocumental-Docuarchi.net/`, ámbito de datos actor 141/empresa 2/nivel 7/versión 38/gabinete 10. Una cuenta MySQL exclusivamente SELECT debe ingresarse por consola enmascarada tras autorización expresa. El puente `queryRows` aceptó offline las ocho consultas y sus parámetros con ejecutor doble (cero acceso ODBC). La receta se revisará antes de aceptar nombres/códigos; cualquier incompatibilidad obliga a revisar el manifiesto, sin preparar planes para descubrirla. La lectura previa no autoriza posteriores cargas ni fallos.

ALM-03 sigue NO_EJECUTADO: aún no existe evidencia segura de entrada/salida de ambos escritores. La tarea 18.1 no se completa con este reporte. El lote 18/intento 41 y todos los efectos existentes permanecen fuera de toda mutación.
